<#
.SYNOPSIS
  Schadensbild von iSmileAlignerTS_db auf dem Server ermitteln (nur lesend, keine Reparatur).

.DESCRIPTION
  Verbindet sich mit "DefaultConnection" aus Web.config (oder -ConnectionString) und schreibt nach -OutDir:
    diagnose.txt      Protokoll: Platz, Log, Sicherungshistorie, Indizes, FKs,
                      beschaedigte Seiten je Tabelle/Index, CHECKDB-Zusammenfassung
    schema-*.csv      Indizes, Spalten, Fremdschluessel der Quelle
    checkdb.csv       vollstaendiges Ergebnis von DBCC CHECKDB ... WITH TABLERESULTS

  Die Datenbank steht im SINGLE_USER-Modus: waehrend des Laufs kann sich niemand sonst
  verbinden (SSMS meldet Fehler 924). Vorher den Application Pool der Website stoppen,
  sonst belegt die Anwendung den einzigen Platz.
  DBCC CHECKDB auf 22 GB dauert ca. 10-30 Minuten.

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File .\01-diagnose.ps1
  powershell -ExecutionPolicy Bypass -File .\01-diagnose.ps1 -SkipCheckDb     # nur die schnellen Abfragen
#>
param(
  [string]$ConnectionString = '',
  [string]$WebConfig = '',
  [string]$OutDir = '',
  [switch]$SkipCheckDb
)

$ErrorActionPreference = 'Continue'
# $PSScriptRoot ist in aelteren PowerShell-Versionen innerhalb von param() leer, daher hier
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
if ([string]::IsNullOrEmpty($WebConfig)) { $WebConfig = Join-Path $scriptDir '..\..\iSmileAlignerTS\Web.config' }
if ([string]::IsNullOrEmpty($OutDir)) { $OutDir = Join-Path $scriptDir 'out' }
if (-not (Test-Path $OutDir)) { New-Item -ItemType Directory -Path $OutDir | Out-Null }
$out = Join-Path $OutDir 'diagnose.txt'
if (Test-Path $out) { Remove-Item $out -Force }

if ($ConnectionString -eq '') {
  [xml]$x = Get-Content $WebConfig
  $ConnectionString = ($x.configuration.connectionStrings.add | Where-Object { $_.name -eq 'DefaultConnection' }).connectionString
}
$b = New-Object System.Data.SqlClient.SqlConnectionStringBuilder $ConnectionString
$b['Connect Timeout'] = 30
$b['Application Name'] = 'ismile-diagnose'
$connStr = $b.ConnectionString
$dbName = [string]$b['Initial Catalog']

function Log($s) {
  $line = "[{0:HH:mm:ss}] {1}" -f (Get-Date), $s
  $line | Out-File $out -Append -Encoding utf8
  Write-Host $line
}

# Fuehrt eine Abfrage in einer eigenen Verbindung aus (nach einem 824-Fehler ist die Verbindung tot).
function Q($label, $sql, $timeout, $dump) {
  Log "--- $label ---"
  $c = New-Object System.Data.SqlClient.SqlConnection $connStr
  $ds = New-Object System.Data.DataSet
  try {
    $c.Open()
    $cmd = $c.CreateCommand(); $cmd.CommandTimeout = $timeout; $cmd.CommandText = $sql
    $da = New-Object System.Data.SqlClient.SqlDataAdapter $cmd
    [void]$da.Fill($ds)
    if ($dump) { foreach ($t in $ds.Tables) { ($t | Format-Table -AutoSize | Out-String -Width 250) | Out-File $out -Append -Encoding utf8 } }
    Log ("Zeilen: " + (($ds.Tables | ForEach-Object { $_.Rows.Count }) -join ','))
  } catch {
    Log ("FEHLER: " + $_.Exception.Message)
  } finally { if ($c.State -ne 'Closed') { $c.Close() } }
  return $ds
}

function ToCsv($ds, $path) {
  if ($ds.Tables.Count -gt 0 -and $ds.Tables[0].Rows.Count -gt 0) {
    $t = $ds.Tables[0]
    $cols = @($t.Columns | ForEach-Object { $_.ColumnName })
    $t | Select-Object -Property $cols | Export-Csv -Path $path -NoTypeInformation -Encoding UTF8
  }
}

Log ("Diagnose fuer " + $dbName + " auf " + $b['Data Source'])

# 1. Platz, Log, Server
$sql = @"
SELECT SERVERPROPERTY('MachineName') AS machine, SERVERPROPERTY('Edition') AS edition, SERVERPROPERTY('ProductVersion') AS version, IS_SRVROLEMEMBER('sysadmin') AS is_sysadmin;
SELECT name, state_desc, user_access_desc, recovery_model_desc, log_reuse_wait_desc, page_verify_option_desc, compatibility_level FROM sys.databases WHERE name = DB_NAME();
SELECT name, type_desc, size*8/1024 AS size_mb, FILEPROPERTY(name, 'SpaceUsed')*8/1024 AS used_mb, physical_name FROM sys.database_files;
SELECT total_log_size_in_bytes/1048576 AS log_total_mb, used_log_space_in_bytes/1048576 AS log_used_mb FROM sys.dm_db_log_space_usage;
EXEC master.sys.xp_fixeddrives;
"@
$r = Q 'Server, Datenbank, Dateien, Platz' $sql 120 $true

# 2. Sicherungshistorie
$sql = @"
SELECT TOP 10 bs.backup_finish_date, bs.type, bs.backup_size/1048576 AS backup_mb, bs.has_backup_checksums, bmf.physical_device_name
FROM msdb.dbo.backupset bs JOIN msdb.dbo.backupmediafamily bmf ON bmf.media_set_id = bs.media_set_id
WHERE bs.database_name = DB_NAME() ORDER BY bs.backup_finish_date DESC;
"@
$r = Q 'Sicherungshistorie' $sql 120 $true

# 3. Schema der Quelle (fuer die Kopie und zum Zuordnen der CHECKDB-Meldungen)
$sql = "SELECT t.object_id, t.name AS table_name, i.index_id, i.name AS index_name, i.type_desc, i.is_primary_key, i.is_unique FROM sys.indexes i JOIN sys.tables t ON t.object_id = i.object_id ORDER BY t.name, i.index_id;"
$idx = Q 'Indizes' $sql 120 $true
ToCsv $idx (Join-Path $OutDir 'schema-indexes.csv')
$objNames = @{}
if ($idx.Tables.Count -gt 0) { foreach ($row in $idx.Tables[0].Rows) { $objNames[[string]$row.object_id] = [string]$row.table_name } }

$sql = "SELECT t.name AS table_name, c.column_id, c.name AS column_name, ty.name AS type_name, c.max_length, c.precision, c.scale, c.is_nullable, c.is_identity, c.is_computed FROM sys.tables t JOIN sys.columns c ON c.object_id = t.object_id JOIN sys.types ty ON ty.user_type_id = c.user_type_id ORDER BY t.name, c.column_id;"
$r = Q 'Spalten (nur CSV)' $sql 120 $false
ToCsv $r (Join-Path $OutDir 'schema-columns.csv')

$sql = "SELECT fk.name AS fk_name, OBJECT_NAME(fk.parent_object_id) AS parent_table, COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS parent_column, OBJECT_NAME(fk.referenced_object_id) AS referenced_table, COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS referenced_column, fk.delete_referential_action_desc FROM sys.foreign_keys fk JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id ORDER BY 2, 1;"
$r = Q 'Fremdschluessel' $sql 120 $true
ToCsv $r (Join-Path $OutDir 'schema-fks.csv')

# 4. Bereits bekannte beschaedigte Seiten (msdb.dbo.suspect_pages) den Tabellen/Indizes zuordnen.
#    event_type: 1 = 823 (I/O), 2 = Pruefsummenfehler (824), 3 = torn page. index_id 0/1 = Daten, >1 = Nebenindex.
#    Die IAM-Kette lesen dauert bei 22 GB einige Minuten.
$sql = @"
SELECT OBJECT_NAME(a.object_id) AS table_name, a.object_id, a.index_id, a.page_type_desc,
       COUNT(*) AS pages, MIN(s.page_id) AS min_page, MAX(s.page_id) AS max_page, SUM(s.error_count) AS total_errors
FROM msdb.dbo.suspect_pages s
LEFT JOIN sys.dm_db_database_page_allocations(DB_ID(), NULL, NULL, NULL, 'LIMITED') a
       ON a.allocated_page_file_id = s.file_id AND a.allocated_page_page_id = s.page_id
WHERE s.database_id = DB_ID()
GROUP BY a.object_id, a.index_id, a.page_type_desc
ORDER BY 1, 3;
"@
$r = Q 'suspect_pages je Tabelle/Index' $sql 3600 $true

# Die eine Seite mit >2 Mio. Fehlern: wem gehoert sie? (kann selbst mit 824 scheitern)
$sql = "SELECT object_id, OBJECT_NAME(object_id) AS table_name, index_id, page_type_desc, page_level, slot_count FROM sys.dm_db_page_info(DB_ID(), 1, 638952, 'LIMITED');"
$r = Q 'Seite 1:638952 (2 Mio. Fehler)' $sql 60 $true

# 5. Vollstaendige Pruefung (nur lesend). TABLOCK: kein Snapshot noetig, DB ist ohnehin SINGLE_USER.
if (-not $SkipCheckDb) {
  Log "DBCC CHECKDB startet (nur lesend, keine Reparatur) - bitte warten"
  $sql = "DBCC CHECKDB (N'" + $dbName + "') WITH NO_INFOMSGS, ALL_ERRORMSGS, TABLERESULTS, TABLOCK;"
  $r = Q 'DBCC CHECKDB' $sql 0 $false
  if ($r.Tables.Count -gt 0) {
    $t = $r.Tables[0]
    ToCsv $r (Join-Path $OutDir 'checkdb.csv')
    Log ("CHECKDB-Meldungen: " + $t.Rows.Count + "  (0 = Datenbank ist konsistent)")
    if ($t.Rows.Count -gt 0) {
      "Meldungen je Fehlernummer:" | Out-File $out -Append -Encoding utf8
      ($t | Group-Object Error | Sort-Object Count -Descending | Select-Object Name, Count | Format-Table -AutoSize | Out-String) | Out-File $out -Append -Encoding utf8
      "Meldungen je Tabelle/Index (index_id 0/1 = Daten verloren, >1 = Index neu aufbauen reicht):" | Out-File $out -Append -Encoding utf8
      ($t | Group-Object ObjectId, IndexId | Sort-Object Count -Descending | ForEach-Object {
          $parts = $_.Name -split ', '
          $name = $objNames[[string]$parts[0]]
          if (-not $name) { $name = '(object_id ' + $parts[0] + ')' }
          [pscustomobject]@{ table = $name; index_id = $parts[1]; messages = $_.Count }
        } | Format-Table -AutoSize | Out-String) | Out-File $out -Append -Encoding utf8
      "Erste 40 Meldungen:" | Out-File $out -Append -Encoding utf8
      ($t | Select-Object -First 40 Error, Level, State, ObjectId, IndexId, File, Page, MessageText | Format-Table -Wrap -AutoSize | Out-String -Width 250) | Out-File $out -Append -Encoding utf8
      ($t | Where-Object { $_.MessageText -like 'CHECKDB found*' } | Select-Object -ExpandProperty MessageText | Out-String) | Out-File $out -Append -Encoding utf8
    }
  } else { Log "CHECKDB hat keine Ergebnistabelle geliefert (siehe FEHLER oben)" }

  $sql = "SELECT event_type, COUNT(*) AS pages, SUM(error_count) AS errors FROM msdb.dbo.suspect_pages WHERE database_id = DB_ID() GROUP BY event_type;"
  $r = Q 'suspect_pages nach CHECKDB' $sql 120 $true
} else {
  Log "CHECKDB uebersprungen (-SkipCheckDb)"
}

Log "FERTIG - Ergebnisse in $OutDir"
