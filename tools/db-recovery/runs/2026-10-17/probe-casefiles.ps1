param([long]$StartAfter = 19370, [int]$Chunk = 100)
$ErrorActionPreference = 'Stop'
$cs = "Data Source=localhost,1433;Initial Catalog=iSmileAlignerTS_db;Integrated Security=True;"
$c = New-Object System.Data.SqlClient.SqlConnection $cs; $c.Open()
$cmd = $c.CreateCommand(); $cmd.CommandText = "SELECT IDENT_CURRENT('dbo.CaseFiles')"
$maxId = [long]$cmd.ExecuteScalar(); $c.Dispose()
Write-Host "maxId=$maxId, probing from Id > $StartAfter in chunks of $Chunk (streaming Content like the copy does)"
$buf = New-Object byte[] (1048576)
$lo = $StartAfter
$sw = [Diagnostics.Stopwatch]::StartNew()
while ($lo -lt $maxId) {
  $hi = $lo + $Chunk; if ($hi -gt $maxId) { $hi = $maxId }
  $c = New-Object System.Data.SqlClient.SqlConnection $cs; $c.Open()
  $cmd = $c.CreateCommand(); $cmd.CommandTimeout = 0
  $cmd.CommandText = "SELECT Id, Content FROM dbo.CaseFiles WHERE Id > @lo AND Id <= @hi ORDER BY Id"
  [void]$cmd.Parameters.AddWithValue('@lo', $lo); [void]$cmd.Parameters.AddWithValue('@hi', $hi)
  $curId = $null; $rows = 0; $bytes = 0L
  try {
    $r = $cmd.ExecuteReader([System.Data.CommandBehavior]::SequentialAccess)
    while ($r.Read()) {
      $curId = $r.GetValue(0); $rows++
      if (-not $r.IsDBNull(1)) { $off = 0L; while (($n = $r.GetBytes(1, $off, $buf, 0, $buf.Length)) -gt 0) { $off += $n }; $bytes += $off }
    }
    $r.Close()
    if ($rows -gt 0) { Write-Host ("ok  ({0}, {1}]  {2} rows  {3:N0} MB  t={4:N0}s" -f $lo, $hi, $rows, ($bytes/1MB), $sw.Elapsed.TotalSeconds) }
    $lo = $hi
  } catch {
    Write-Host ("FAIL in ({0}, {1}] while reading Id {2} (after {3} rows in this chunk)" -f $lo, $hi, $curId, $rows)
    $e = $_.Exception; $d = 0
    while ($e) {
      Write-Host ("  [{0}] {1}: {2}" -f $d, $e.GetType().FullName, $e.Message)
      if ($e -is [System.Data.SqlClient.SqlException]) { Write-Host ("      SqlException Number={0} Class={1} State={2}" -f $e.Number, $e.Class, $e.State) }
      $e = $e.InnerException; $d++
    }
    break
  } finally { $c.Dispose() }
}
