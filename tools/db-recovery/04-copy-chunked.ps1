<#
.SYNOPSIS
  Kopiert alle Tabellen einer beschaedigten Quell-DB in eine leere Ziel-DB mit gleichem Schema,
  in Schluesselbereichen (Chunks). Trifft ein Chunk auf eine unlesbare Seite (Fehler 824; der Server
  beendet dann die Verbindung), wird neu verbunden und der Bereich halbiert, bis die betroffenen
  Zeilen einzeln uebersprungen sind. IDs bleiben erhalten, Fremdschluessel im Ziel sind waehrend der
  Kopie aus und werden danach mit Pruefung wieder aktiviert.

.DESCRIPTION
  Braucht keine Reparatur und liest keine PFS/GAM-Seiten (siehe 02-prepare-source.sql): Zeilen werden
  ueber den Primaerschluessel gelesen. Zeilen auf unlesbaren Datenseiten gehen verloren (lost-rows.csv).
  Zeilen, deren BLOB (Content) unlesbar ist, werden ohne BLOB kopiert (blob-lost.csv), ausser
  -DropRowOnBlobError ist gesetzt.

  Am besten direkt auf dem Server ausfuehren (Loopback, keine 2 x 22 GB uebers Netz):

    powershell -ExecutionPolicy Bypass -File .\04-copy-chunked.ps1 `
      -SourceConnectionString "Data Source=localhost,1433;Initial Catalog=iSmileAlignerTS_db;User ID=<LOGIN>;Password=<PASSWORT>;" `
      -TargetConnectionString "Data Source=localhost,1433;Initial Catalog=iSmileAlignerTS_db_new;User ID=<LOGIN>;Password=<PASSWORT>;"

  Voraussetzung: Ziel-DB mit identischem Schema (03-create-target.sql + ef6.exe), RECOVERY SIMPLE.
  Ausgabe in -LogDir: copy-chunked.log, lost-rows.csv, blob-lost.csv, summary.csv

.PARAMETER Tables
  Nur diese Tabellen kopieren (Standard: alle Benutzertabellen der Quelle).
.PARAMETER SkipTables
  Tabellen auslassen. __MigrationHistory legt ef6.exe selbst an, sysdiagrams ist SSMS-intern.
.PARAMETER ChunkRows / BlobChunkRows
  Schluesselbereich je Chunk (normale Tabellen / Tabellen mit varbinary(max) o.ae.).
.PARAMETER NoClearTarget
  Zieltabellen vorher nicht leeren (Standard: leeren, damit der Lauf wiederholbar ist).
.PARAMETER DropRowOnBlobError
  Zeile mit unlesbarem BLOB ganz weglassen statt ohne BLOB zu kopieren.
.PARAMETER SimulateFailIds
  Nur zum Testen: diese Ids gelten in jeder Tabelle als unlesbar (BLOB-Lesefehler simuliert).
#>
param(
  [Parameter(Mandatory = $true)][string]$SourceConnectionString,
  [Parameter(Mandatory = $true)][string]$TargetConnectionString,
  [string[]]$Tables = @(),
  [string[]]$SkipTables = @('__MigrationHistory', 'sysdiagrams'),
  [int]$ChunkRows = 2000,
  [int]$BlobChunkRows = 100,
  [switch]$NoClearTarget,
  [switch]$DropRowOnBlobError,
  [string]$LogDir = '',
  [long[]]$SimulateFailIds = @()
)

$ErrorActionPreference = 'Stop'
# $PSScriptRoot ist in aelteren PowerShell-Versionen innerhalb von param() leer, daher hier
if ([string]::IsNullOrEmpty($LogDir)) { $LogDir = Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) 'out' }
if (-not (Test-Path $LogDir)) { New-Item -ItemType Directory -Path $LogDir | Out-Null }
$logFile = Join-Path $LogDir 'copy-chunked.log'
$lostRows = New-Object System.Collections.Generic.List[object]
$blobLost = New-Object System.Collections.Generic.List[object]
$summary  = New-Object System.Collections.Generic.List[object]

function Log($s) {
  $line = "[{0:HH:mm:ss}] {1}" -f (Get-Date), $s
  Add-Content -Path $logFile -Value $line -Encoding UTF8
  Write-Host $line
}

function Open-Conn($cs) {
  $c = New-Object System.Data.SqlClient.SqlConnection $cs
  $c.Open()
  return $c
}

function Add-Params($cmd, $params) {
  if ($params -ne $null -and $params.Count -gt 0) { foreach ($k in $params.Keys) { [void]$cmd.Parameters.AddWithValue($k, $params[$k]) } }
}

function Invoke-Scalar($cs, $sql, $params) {
  $c = Open-Conn $cs
  try { $cmd = $c.CreateCommand(); $cmd.CommandTimeout = 600; $cmd.CommandText = $sql; Add-Params $cmd $params; return $cmd.ExecuteScalar() }
  finally { $c.Dispose() }
}

function Invoke-NonQuery($cs, $sql, $params) {
  $c = Open-Conn $cs
  try { $cmd = $c.CreateCommand(); $cmd.CommandTimeout = 0; $cmd.CommandText = $sql; Add-Params $cmd $params; return $cmd.ExecuteNonQuery() }
  finally { $c.Dispose() }
}

function Invoke-Table($cs, $sql, $params) {
  $c = Open-Conn $cs
  try {
    $cmd = $c.CreateCommand(); $cmd.CommandTimeout = 600; $cmd.CommandText = $sql; Add-Params $cmd $params
    $da = New-Object System.Data.SqlClient.SqlDataAdapter $cmd
    $dt = New-Object System.Data.DataTable
    [void]$da.Fill($dt)
    return , $dt
  } finally { $c.Dispose() }
}

# Alle Meldungen einer Ausnahmekette (PowerShell -> .NET -> InnerException ...) als ein Text.
# SqlBulkCopy verpackt einen Lesefehler der Quelle in "Fehler beim Lesen." / "An error occurred
# while reading."; der eigentliche 824- oder Transportfehler steht erst in der InnerException.
function Get-ErrorText($ex) {
  $parts = New-Object System.Collections.Generic.List[string]
  $e = $ex
  while ($e -ne $null) {
    $t = [string]$e.Message
    if ($e -is [System.Data.SqlClient.SqlException]) { $t = ('SqlException {0}: {1}' -f $e.Number, $t) }
    $parts.Add($t)
    $e = $e.InnerException
  }
  return ($parts -join ' <- ')
}

# Lesefehler durch Korruption (Quelle) von echten Fehlern (Ziel, Schema) unterscheiden.
# Nach einem 824 beendet der Server die Verbindung; der Client sieht dann oft nur noch einen
# Transportfehler (SqlException 64 / 10054 / 233, "Netzwerkname nicht mehr verfuegbar").
# Immer mit dem Text von Get-ErrorText aufrufen, nicht nur mit $_.Exception.Message.
function Test-CorruptionError($msg) {
  return ($msg -match '824|823|829|checksum|Pr.fsumme|consisten|konsisten|transport|bertragungsebene|torn page|zerrissen|BUF_IOERR|SIMULATED|Fehler beim Lesen|while reading|Netzwerkname|network name|SqlException (64|10054|233):')
}

function Get-NullCast($col) {
  switch ($col.type_name) {
    'varbinary' { return 'CAST(NULL AS varbinary(max))' }
    'image'     { return 'CAST(NULL AS varbinary(max))' }
    'nvarchar'  { return 'CAST(NULL AS nvarchar(max))' }
    'ntext'     { return 'CAST(NULL AS nvarchar(max))' }
    'varchar'   { return 'CAST(NULL AS varchar(max))' }
    'text'      { return 'CAST(NULL AS varchar(max))' }
    'xml'       { return 'CAST(NULL AS xml)' }
    default     { return 'NULL' }
  }
}

# Einen Schluesselbereich (lo, hi] kopieren; wirft bei Fehlern. Gibt die Zahl der kopierten Zeilen zurueck.
# lo/hi = $null: ganze Tabelle in einem Rutsch.
function Copy-Range($tbl, $lo, $hi, $withBlobs) {
  if ($withBlobs -and $lo -ne $null -and $SimulateFailIds.Count -gt 0) {
    foreach ($id in $SimulateFailIds) { if ($id -gt $lo -and $id -le $hi) { throw "SIMULATED 824 bei Id $id" } }
  }
  $src = $null; $tgt = $null; $reader = $null; $bc = $null
  try {
    $src = Open-Conn $SourceConnectionString
    $tgt = Open-Conn $TargetConnectionString

    $where = ''
    $params = @{}
    if ($lo -ne $null) {
      $where = " WHERE $($tbl.pkq) > @lo AND $($tbl.pkq) <= @hi"
      $params = @{ '@lo' = [long]$lo; '@hi' = [long]$hi }
      # Zielbereich leeren, damit ein Neuversuch keine Duplikate erzeugt
      $del = $tgt.CreateCommand(); $del.CommandTimeout = 0
      $del.CommandText = "DELETE FROM $($tbl.qname)$where"
      Add-Params $del $params
      [void]$del.ExecuteNonQuery()
    }

    $selectCols = @()
    foreach ($c in $tbl.cols) {
      if (-not $withBlobs -and $c.is_blob) { $selectCols += ((Get-NullCast $c) + ' AS ' + $c.q) }
      else { $selectCols += $c.q }
    }

    $cmd = $src.CreateCommand(); $cmd.CommandTimeout = 0
    $cmd.CommandText = "SELECT " + ($selectCols -join ', ') + " FROM $($tbl.qname)$where"
    Add-Params $cmd $params
    $reader = $cmd.ExecuteReader([System.Data.CommandBehavior]::SequentialAccess)

    $opts = [System.Data.SqlClient.SqlBulkCopyOptions]::KeepIdentity -bor `
            [System.Data.SqlClient.SqlBulkCopyOptions]::KeepNulls -bor `
            [System.Data.SqlClient.SqlBulkCopyOptions]::UseInternalTransaction -bor `
            [System.Data.SqlClient.SqlBulkCopyOptions]::TableLock
    $bc = New-Object System.Data.SqlClient.SqlBulkCopy($tgt, [System.Data.SqlClient.SqlBulkCopyOptions]$opts, $null)
    $bc.DestinationTableName = $tbl.qname
    $bc.BulkCopyTimeout = 0
    $bc.BatchSize = 0
    $bc.EnableStreaming = $true
    foreach ($c in $tbl.cols) { [void]$bc.ColumnMappings.Add($c.name, $c.name) }
    $bc.WriteToServer($reader)
    $reader.Close()

    $cnt = $tgt.CreateCommand(); $cnt.CommandTimeout = 600
    $cnt.CommandText = "SELECT COUNT_BIG(*) FROM $($tbl.qname)$where"
    Add-Params $cnt $params
    return [long]$cnt.ExecuteScalar()
  } finally {
    if ($bc) { $bc.Close() }
    if ($reader) { try { $reader.Dispose() } catch { } }
    if ($src) { try { $src.Dispose() } catch { } }
    if ($tgt) { try { $tgt.Dispose() } catch { } }
  }
}

# ---------------------------------------------------------------------------
Log "Start. Quelle -> Ziel, Chunk $ChunkRows / Blob-Chunk $BlobChunkRows"
if ($SimulateFailIds.Count -gt 0) { Log ("TESTMODUS: simulierte Lesefehler bei Ids " + ($SimulateFailIds -join ',')) }

# 1. Tabellen der Quelle (nur Metadaten)
$metaSql = @"
SELECT s.name AS table_schema, t.name AS table_name, t.object_id,
  pk.col_name AS pk_column,
  CASE WHEN pk.col_count = 1 AND pk.type_name IN ('int','bigint','smallint','tinyint') THEN 1 ELSE 0 END AS pk_is_int,
  CASE WHEN EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = t.object_id AND c.is_identity = 1) THEN 1 ELSE 0 END AS has_identity,
  ISNULL((SELECT SUM(p.rows) FROM sys.partitions p WHERE p.object_id = t.object_id AND p.index_id IN (0,1)), 0) AS src_rows
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
OUTER APPLY (
  SELECT MIN(c.name) AS col_name, COUNT(*) AS col_count, MIN(ty.name) AS type_name
  FROM sys.indexes i
  JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
  JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
  JOIN sys.types ty ON ty.user_type_id = c.user_type_id
  WHERE i.object_id = t.object_id AND i.is_primary_key = 1
) pk
WHERE t.is_ms_shipped = 0
ORDER BY t.name;
"@
$srcTables = Invoke-Table $SourceConnectionString $metaSql $null

$colSql = @"
SELECT c.name, ty.name AS type_name, c.max_length, c.is_computed
FROM sys.columns c JOIN sys.types ty ON ty.user_type_id = c.user_type_id
WHERE c.object_id = @objid ORDER BY c.column_id;
"@
$tgtColSql = "SELECT c.name FROM sys.columns c JOIN sys.tables t ON t.object_id = c.object_id JOIN sys.schemas s ON s.schema_id = t.schema_id WHERE t.name = @t AND s.name = @s;"

# 2. Fremdschluessel im Ziel aus
[void](Invoke-NonQuery $TargetConnectionString "EXEC sp_MSforeachtable N'ALTER TABLE ? NOCHECK CONSTRAINT ALL';" $null)
Log "Fremdschluessel im Ziel deaktiviert"

# 3. Tabelle fuer Tabelle
foreach ($row in $srcTables.Rows) {
  $name = [string]$row.table_name
  if ($SkipTables -contains $name) { continue }
  if ($Tables.Count -gt 0 -and -not ($Tables -contains $name)) { continue }

  $tbl = @{
    schema = [string]$row.table_schema; name = $name
    qname  = '[' + $row.table_schema + '].[' + $name + ']'
    pk     = $null; pkq = $null
    pk_is_int = ([int]$row.pk_is_int -eq 1); has_identity = ([int]$row.has_identity -eq 1)
    src_rows = [long]$row.src_rows; cols = @(); has_blob = $false
  }
  if ($row.pk_column -isnot [System.DBNull]) { $tbl.pk = [string]$row.pk_column; $tbl.pkq = '[' + $tbl.pk + ']' }

  $tgtCols = @()
  foreach ($r in (Invoke-Table $TargetConnectionString $tgtColSql @{ '@t' = $name; '@s' = $tbl.schema }).Rows) { $tgtCols += [string]$r.name }
  if ($tgtCols.Count -eq 0) { Log "$name : im Ziel nicht vorhanden, uebersprungen"; continue }

  foreach ($r in (Invoke-Table $SourceConnectionString $colSql @{ '@objid' = [int]$row.object_id }).Rows) {
    if ([int]$r.is_computed -eq 1) { continue }
    if (-not ($tgtCols -contains [string]$r.name)) { continue }
    $isBlob = ([string]$r.type_name -in @('image', 'text', 'ntext', 'xml')) -or (([string]$r.type_name -in @('varbinary', 'varchar', 'nvarchar')) -and [int]$r.max_length -eq -1)
    if ($isBlob) { $tbl.has_blob = $true }
    $tbl.cols += @{ name = [string]$r.name; q = '[' + $r.name + ']'; type_name = [string]$r.type_name; is_blob = $isBlob }
  }

  $copied = [long]0; $lost = 0; $blobs = 0; $status = 'OK'
  Log ("{0}: {1} Zeilen laut Quelle, PK={2}, Blob={3}" -f $name, $tbl.src_rows, $tbl.pk, $tbl.has_blob)

  try {
    if (-not $NoClearTarget) { [void](Invoke-NonQuery $TargetConnectionString "DELETE FROM $($tbl.qname);" $null) }

    if ($tbl.pk_is_int) {
      $initial = $ChunkRows; if ($tbl.has_blob) { $initial = $BlobChunkRows }
      # Grenzen: MIN liest die erste Seite (kann scheitern -> 0); MAX ueber IDENT_CURRENT (Metadaten) oder MAX()
      $min = 1
      try { $v = Invoke-Scalar $SourceConnectionString "SELECT MIN($($tbl.pkq)) FROM $($tbl.qname)" $null; if ($v -ne $null -and $v -isnot [System.DBNull]) { $min = [long]$v } } catch { Log "$name : MIN nicht lesbar, starte bei 1" }
      $max = $null
      if ($tbl.has_identity) { $v = Invoke-Scalar $SourceConnectionString "SELECT IDENT_CURRENT(@n)" @{ '@n' = $tbl.schema + '.' + $name }; if ($v -ne $null -and $v -isnot [System.DBNull]) { $max = [long]$v } }
      if ($max -eq $null) { try { $v = Invoke-Scalar $SourceConnectionString "SELECT MAX($($tbl.pkq)) FROM $($tbl.qname)" $null; if ($v -ne $null -and $v -isnot [System.DBNull]) { $max = [long]$v } } catch { } }
      if ($max -eq $null) { throw "Obergrenze des Schluessels nicht ermittelbar (weder IDENT_CURRENT noch MAX)" }

      $lo = $min - 1; $step = $initial; $retried = $false
      while ($lo -lt $max) {
        $hi = $lo + $step; if ($hi -gt $max) { $hi = $max }
        try {
          $n = Copy-Range $tbl $lo $hi $true
          $copied += $n; $lo = $hi; $retried = $false
          if ($n -eq 0) { $step = [Math]::Min([long]($step * 2), [long]1000000) } else { $step = $initial }
          if ($tbl.has_blob -or ($copied % 20000) -lt $n) { Write-Host ("   {0}: {1} Zeilen, Id <= {2}" -f $name, $copied, $lo) }
        } catch {
          $msg = Get-ErrorText $_.Exception
          if (-not (Test-CorruptionError $msg)) { throw }
          if ($step -gt 1) {
            $step = [long][Math]::Ceiling($step / 2)
            Log ("{0}: Lesefehler im Bereich ({1}, {2}], halbiere auf {3}" -f $name, $lo, $hi, $step)
          } elseif (-not $retried) {
            $retried = $true     # einmal wiederholen, falls es nur ein Netzwerkproblem war
          } else {
            $id = $lo + 1; $ok = $false
            if ($tbl.has_blob -and -not $DropRowOnBlobError) {
              try {
                $n = Copy-Range $tbl $lo $hi $false
                if ($n -gt 0) { $copied += $n; $blobs++; $blobLost.Add([pscustomobject]@{ table = $name; id = $id }); Log "$name : Id $id ohne BLOB kopiert (BLOB unlesbar)" }
                $ok = $true
              } catch { }
            }
            if (-not $ok) { $lost++; $lostRows.Add([pscustomobject]@{ table = $name; id = $id; error = $msg }); Log "$name : Id $id verloren ($msg)" }
            $lo = $hi; $step = $initial; $retried = $false
          }
        }
      }
    } else {
      # kein einspaltiger Integer-PK: in einem Rutsch
      $copied = Copy-Range $tbl $null $null $true
    }
  } catch {
    $status = 'FEHLER: ' + (Get-ErrorText $_.Exception)
    Log "$name : $status"
  }

  Log ("{0}: {1} kopiert, {2} verloren, {3} ohne BLOB - {4}" -f $name, $copied, $lost, $blobs, $status)
  $summary.Add([pscustomobject]@{ table = $name; src_rows = $tbl.src_rows; copied = $copied; lost = $lost; blob_lost = $blobs; status = $status })
}

# 4. Fremdschluessel wieder aktivieren, Waisen melden
Log "Fremdschluessel im Ziel aktivieren (WITH CHECK)"
$fkSql = @"
SELECT fk.name AS fk_name, QUOTENAME(ps.name) + '.' + QUOTENAME(pt.name) AS parent_q, pc.name AS parent_column,
       QUOTENAME(rs.name) + '.' + QUOTENAME(rt.name) AS ref_q, rc.name AS ref_column
FROM sys.foreign_keys fk
JOIN sys.tables pt ON pt.object_id = fk.parent_object_id
JOIN sys.schemas ps ON ps.schema_id = pt.schema_id
JOIN sys.tables rt ON rt.object_id = fk.referenced_object_id
JOIN sys.schemas rs ON rs.schema_id = rt.schema_id
JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
JOIN sys.columns pc ON pc.object_id = fkc.parent_object_id AND pc.column_id = fkc.parent_column_id
JOIN sys.columns rc ON rc.object_id = fkc.referenced_object_id AND rc.column_id = fkc.referenced_column_id
ORDER BY parent_q, fk.name;
"@
foreach ($fk in (Invoke-Table $TargetConnectionString $fkSql $null).Rows) {
  try {
    [void](Invoke-NonQuery $TargetConnectionString ("ALTER TABLE {0} WITH CHECK CHECK CONSTRAINT [{1}];" -f $fk.parent_q, $fk.fk_name) $null)
  } catch {
    $orphanSql = "SELECT COUNT_BIG(*) FROM {0} p WHERE p.[{1}] IS NOT NULL AND NOT EXISTS (SELECT 1 FROM {2} r WHERE r.[{3}] = p.[{1}])" -f $fk.parent_q, $fk.parent_column, $fk.ref_q, $fk.ref_column
    $cnt = -1
    try { $cnt = [long](Invoke-Scalar $TargetConnectionString $orphanSql $null) } catch { }
    Log ("{0}: nicht aktivierbar - {1} Waisen in {2}.{3} ohne Treffer in {4}.{5}" -f $fk.fk_name, $cnt, $fk.parent_q, $fk.parent_column, $fk.ref_q, $fk.ref_column)
    Log ("   anzeigen: " + ($orphanSql -replace 'COUNT_BIG\(\*\)', '*'))
    Log ("   danach:   ALTER TABLE {0} WITH CHECK CHECK CONSTRAINT [{1}];" -f $fk.parent_q, $fk.fk_name)
  }
}

# 5. Zusammenfassung
function Save-Csv($list, $path, $header) {
  if ($list.Count -gt 0) { $list | Export-Csv -Path $path -NoTypeInformation -Encoding UTF8 }
  else { Set-Content -Path $path -Value $header -Encoding UTF8 }
}
Save-Csv $summary  (Join-Path $LogDir 'summary.csv')   '"table","src_rows","copied","lost","blob_lost","status"'
Save-Csv $lostRows (Join-Path $LogDir 'lost-rows.csv') '"table","id","error"'
Save-Csv $blobLost (Join-Path $LogDir 'blob-lost.csv') '"table","id"'
Log "FERTIG"
$summary | Format-Table -AutoSize | Out-String -Width 200 | Write-Host
