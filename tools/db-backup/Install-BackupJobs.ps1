<#
.SYNOPSIS
  Richtet auf DOCTORS die taegliche Vollsicherung von iSmileAlignerTS_db per SQL Server Agent ein.

.DESCRIPTION
  1. SQL Server-Agent-Dienst auf Starttyp Automatisch setzen und starten.
  2. install-backup-jobs.sql mit den Parametern ausfuehren (Job, Zeitplan, Alerts, Servervorgaben).
  3. Mit -RunNow den Job sofort starten, auf das Ende warten und die Schrittmeldungen ausgeben.

  Als Administrator auf DOCTORS ausfuehren (Windows-Anmeldung ist sysadmin). Wiederholbar.

    powershell -ExecutionPolicy Bypass -File .\Install-BackupJobs.ps1 -KeepCount 1          # anlegen, Zeitplan noch AUS
    powershell -ExecutionPolicy Bypass -File .\Install-BackupJobs.ps1 -KeepCount 1 -RunNow  # anlegen und einmal laufen lassen
    powershell -ExecutionPolicy Bypass -File .\Install-BackupJobs.ps1 -KeepCount 2 -Enable  # Zeitplan EIN, 2 Vollsicherungen behalten

  Jeder Aufruf legt den Job neu an (die Jobhistorie in SSMS beginnt dann von vorn).

.PARAMETER KeepCount
  Wie viele eigene Vollsicherungen im Zielordner bleiben. Platzbedarf siehe README.md (eine Vollsicherung ca. 16 GB,
  waehrend des Jobs liegt kurz eine Datei mehr im Ordner).
.PARAMETER Enable
  Zeitplan aktiv (Job enabled). Ohne den Schalter wird der Job angelegt, laeuft aber nur auf Anforderung.
.PARAMETER RunNow
  Job nach dem Anlegen sofort starten und auf das Ergebnis warten (dauert einige Minuten: CHECKDB + 22 GB sichern + pruefen).
#>
param(
  [string]$Server = 'localhost,1433',
  [string]$DbName = 'iSmileAlignerTS_db',
  [string]$BackupDir = 'C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\Backup\iSmileAlignerTS_db',
  [int]$KeepCount = 1,
  [string]$StartTime = '020000',
  [switch]$Enable,
  [switch]$RunNow
)
$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$jobName = 'iSmile DB - Sicherung'

function Invoke-Sql($sql) {
  $c = New-Object System.Data.SqlClient.SqlConnection ("Data Source=$Server;Initial Catalog=msdb;Integrated Security=True;")
  $c.Open()
  try {
    $cmd = $c.CreateCommand(); $cmd.CommandTimeout = 60; $cmd.CommandText = $sql
    $da = New-Object System.Data.SqlClient.SqlDataAdapter $cmd
    $dt = New-Object System.Data.DataTable
    [void]$da.Fill($dt)
    return , $dt
  } finally { $c.Dispose() }
}

# 1. Agent-Dienst
$svc = Get-Service -Name 'SQLSERVERAGENT'
if ($svc.StartType -ne 'Automatic') { Set-Service -Name 'SQLSERVERAGENT' -StartupType Automatic; Write-Host 'SQL Server-Agent: Starttyp auf Automatisch gesetzt' }
if ($svc.Status -ne 'Running') {
  Start-Service -Name 'SQLSERVERAGENT'
  (Get-Service -Name 'SQLSERVERAGENT').WaitForStatus('Running', [TimeSpan]::FromMinutes(1))
  Write-Host 'SQL Server-Agent gestartet'
  Start-Sleep -Seconds 5   # Agent XPs werden beim Start freigeschaltet
}

# 2. Job, Zeitplan, Alerts, Servervorgaben
$jobEnabled = 0; if ($Enable) { $jobEnabled = 1 }
$sqlFile = Join-Path $here 'install-backup-jobs.sql'
Write-Host "Fuehre $sqlFile aus ..."
# sqlcmd loest $(Name) im Skript auch aus Umgebungsvariablen auf; das umgeht die Anfuehrungszeichen-Probleme
# von -v bei Pfaden mit Leerzeichen ("C:\Program Files\...").
$env:DbName = $DbName; $env:BackupDir = $BackupDir; $env:KeepCount = "$KeepCount"
$env:JobEnabled = "$jobEnabled"; $env:StartTime = $StartTime
& sqlcmd -S $Server -E -C -b -i $sqlFile
if ($LASTEXITCODE -ne 0) { throw "sqlcmd meldete Fehler $LASTEXITCODE" }

# 3. optional: sofort laufen lassen
if ($RunNow) {
  Write-Host "Starte Job '$jobName' ..."
  [void](Invoke-Sql "EXEC msdb.dbo.sp_start_job @job_name = N'$jobName';")
  $sw = [Diagnostics.Stopwatch]::StartNew()
  do {
    Start-Sleep -Seconds 10
    $act = Invoke-Sql "SELECT TOP 1 a.start_execution_date, a.stop_execution_date FROM msdb.dbo.sysjobactivity a JOIN msdb.dbo.sysjobs j ON j.job_id = a.job_id WHERE j.name = N'$jobName' ORDER BY a.session_id DESC, a.start_execution_date DESC;"
    $running = ($act.Rows.Count -gt 0) -and ($act.Rows[0].start_execution_date -isnot [DBNull]) -and ($act.Rows[0].stop_execution_date -is [DBNull])
    if ($running) { Write-Host ("   laeuft seit {0:N0} s" -f $sw.Elapsed.TotalSeconds) }
  } while ($running -and $sw.Elapsed.TotalMinutes -lt 60)

  $hist = Invoke-Sql @"
SELECT TOP 6 h.step_id, h.step_name, h.run_status, h.run_duration, LEFT(h.message, 1500) AS message
FROM msdb.dbo.sysjobhistory h JOIN msdb.dbo.sysjobs j ON j.job_id = h.job_id
WHERE j.name = N'$jobName' ORDER BY h.instance_id DESC;
"@
  foreach ($r in ($hist.Rows | Sort-Object step_id)) {
    $status = switch ([int]$r.run_status) { 0 { 'FEHLER' } 1 { 'OK' } 2 { 'Wiederholung' } 3 { 'Abgebrochen' } 4 { 'laeuft' } default { "Status $($r.run_status)" } }
    Write-Host ("Schritt {0} {1}: {2} (Dauer {3})" -f $r.step_id, $r.step_name, $status, $r.run_duration)
    Write-Host ("   " + (([string]$r.message) -replace '\s+', ' '))
  }
  Write-Host '--- Dateien im Zielordner ---'
  Get-ChildItem -Path $BackupDir -Filter '*.bak' | Sort-Object LastWriteTime | Select-Object Name, @{n='GB';e={[math]::Round($_.Length/1GB,1)}}, LastWriteTime | Format-Table -AutoSize
}
