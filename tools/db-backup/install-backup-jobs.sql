/* ============================================================================
   install-backup-jobs.sql  -  taegliche Vollsicherung von iSmileAlignerTS_db per SQL Server Agent

   Ausfuehren ueber Install-BackupJobs.ps1 (setzt die Variablen und startet den Agent) oder in SSMS
   im SQLCMD-Modus (Menue Abfrage > SQLCMD-Modus); dann die :setvar-Zeilen unten einkommentieren.
   Idempotent: Job, Zeitplan und Alerts gleichen Namens werden ersetzt.

   Job "iSmile DB - Sicherung", taeglich um $(StartTime):
     1. DBCC CHECKDB und Blick in msdb.dbo.suspect_pages. Schlaegt das fehl, endet der Job hier:
        es wird NICHT gesichert und NICHT rotiert, die letzten guten Sicherungen bleiben liegen.
     2. BACKUP DATABASE ... WITH CHECKSUM, COMPRESSION in $(BackupDir), danach RESTORE VERIFYONLY WITH CHECKSUM.
     3. Rotation: die neuesten $(KeepCount) eigenen Vollsicherungen bleiben, aeltere .bak-Dateien im Ordner
        werden geloescht. Laeuft nur, wenn Schritt 2 gelungen ist; waehrend des Jobs liegt also kurz eine
        Datei mehr im Ordner als $(KeepCount).
     4. msdb-Sicherungshistorie aelter als 90 Tage aufraeumen.
   Jeder Fehlschlag landet im Windows-Ereignisprotokoll (Anwendung, Quelle SQLSERVERAGENT) und in der
   Jobhistorie in SSMS (SQL Server-Agent > Aufträge > Verlauf anzeigen).

   Warum taeglich voll und keine differenziellen Sicherungen: der Hoster zieht jede Nacht gegen 22:30 eine
   VSS-Momentaufnahme (msdb.dbo.backupset: is_snapshot = 1, is_copy_only = 0). Die setzt die Basis fuer
   differenzielle Sicherungen zurueck; eigene differenzielle Sicherungen waeren ohne diese Momentaufnahme
   nicht wiederherstellbar.
   ============================================================================ */

-- Fuer SSMS im SQLCMD-Modus einkommentieren (Install-BackupJobs.ps1 uebergibt sie per -v):
-- :setvar DbName        iSmileAlignerTS_db
-- :setvar BackupDir     "C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\Backup\iSmileAlignerTS_db"
-- :setvar KeepCount     1
-- :setvar JobEnabled    0
-- :setvar StartTime     020000

USE master;
GO
SET NOCOUNT ON;
PRINT N'Datenbank: $(DbName)   Ordner: $(BackupDir)   Behalten: $(KeepCount) Vollsicherung(en)   Job aktiv: $(JobEnabled)   Start: $(StartTime)';
GO

-- 1. Serverweite Vorgaben: jede Sicherung mit Pruefsumme und komprimiert, auch die von Hand
EXEC sp_configure N'show advanced options', 1; RECONFIGURE;
EXEC sp_configure N'backup checksum default', 1;
EXEC sp_configure N'backup compression default', 1;
RECONFIGURE;
EXEC sp_configure N'show advanced options', 0; RECONFIGURE;
GO

-- 2. Zielordner (legt der SQL Server-Dienst an, der dort auch schreibt)
EXEC master.dbo.xp_create_subdir N'$(BackupDir)';
GO

-- 3. Job neu anlegen
IF EXISTS (SELECT 1 FROM msdb.dbo.sysjobs WHERE name = N'iSmile DB - Sicherung')
    EXEC msdb.dbo.sp_delete_job @job_name = N'iSmile DB - Sicherung', @delete_unused_schedule = 1;
GO

EXEC msdb.dbo.sp_add_job
    @job_name = N'iSmile DB - Sicherung',
    @enabled = $(JobEnabled),
    @description = N'Taeglich: CHECKDB, Vollsicherung WITH CHECKSUM + VERIFYONLY, Rotation. Eingerichtet mit tools\db-backup\install-backup-jobs.sql.',
    @owner_login_name = N'sa',
    @notify_level_eventlog = 2,        -- 2 = bei Fehlschlag ins Windows-Ereignisprotokoll
    @delete_level = 0;
GO

EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'iSmile DB - Sicherung',
    @step_id = 1,
    @step_name = N'1 CHECKDB',
    @subsystem = N'TSQL',
    @database_name = N'master',
    @on_success_action = 3,            -- 3 = naechster Schritt
    @on_fail_action = 2,               -- 2 = Job mit Fehler beenden (keine Sicherung, keine Rotation)
    @retry_attempts = 0,
    @command = N'SET NOCOUNT ON;
DECLARE @n int = (SELECT COUNT(*) FROM msdb.dbo.suspect_pages WHERE database_id = DB_ID(N''$(DbName)''));
IF @n > 0 RAISERROR(N''%d verdaechtige Seiten fuer $(DbName) in msdb.dbo.suspect_pages. Keine Sicherung, keine Rotation.'', 16, 1, @n);
DBCC CHECKDB (N''$(DbName)'') WITH NO_INFOMSGS, ALL_ERRORMSGS;
PRINT N''CHECKDB ohne Fehler.'';';
GO

EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'iSmile DB - Sicherung',
    @step_id = 2,
    @step_name = N'2 Vollsicherung und Pruefung',
    @subsystem = N'TSQL',
    @database_name = N'master',
    @on_success_action = 3,
    @on_fail_action = 2,
    @retry_attempts = 0,
    @command = N'SET NOCOUNT ON;
DECLARE @file nvarchar(400) = N''$(BackupDir)\$(DbName)_FULL_''
    + CONVERT(nvarchar(8), GETDATE(), 112) + N''_'' + REPLACE(CONVERT(nvarchar(5), GETDATE(), 108), N'':'', N'''') + N''.bak'';
BACKUP DATABASE [$(DbName)] TO DISK = @file
    WITH CHECKSUM, COMPRESSION, INIT, FORMAT, NAME = N''$(DbName) Vollsicherung'', STATS = 25;
RESTORE VERIFYONLY FROM DISK = @file WITH CHECKSUM;
PRINT N''Sicherung geprueft: '' + @file;';
GO

EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'iSmile DB - Sicherung',
    @step_id = 3,
    @step_name = N'3 Rotation',
    @subsystem = N'TSQL',
    @database_name = N'master',
    @on_success_action = 3,
    @on_fail_action = 2,
    @retry_attempts = 0,
    @command = N'SET NOCOUNT ON;
-- Die neuesten $(KeepCount) eigenen Vollsicherungen (laut msdb) bleiben, alles Aeltere im Ordner wird geloescht.
DECLARE @cutoff datetime = (
    SELECT MIN(t.backup_start_date) FROM (
        SELECT TOP ($(KeepCount)) b.backup_start_date
        FROM msdb.dbo.backupset b JOIN msdb.dbo.backupmediafamily m ON m.media_set_id = b.media_set_id
        WHERE b.database_name = N''$(DbName)'' AND b.type = ''D'' AND b.is_copy_only = 0 AND b.is_snapshot = 0
          AND m.physical_device_name LIKE N''$(BackupDir)\%''
        ORDER BY b.backup_start_date DESC) t);
IF @cutoff IS NULL BEGIN PRINT N''Keine eigene Vollsicherung protokolliert, nichts geloescht.''; RETURN; END
SET @cutoff = DATEADD(minute, -1, @cutoff);
DECLARE @cutoffStr nvarchar(30) = LEFT(CONVERT(nvarchar(30), @cutoff, 126), 19);
EXEC master.dbo.xp_delete_file 0, N''$(BackupDir)'', N''bak'', @cutoffStr, 0;   -- loescht nur echte SQL-Sicherungsdateien
PRINT N''Behalten: die neuesten $(KeepCount) Vollsicherung(en); geloescht: Dateien aelter als '' + @cutoffStr + N''.'';';
GO

EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'iSmile DB - Sicherung',
    @step_id = 4,
    @step_name = N'4 msdb-Historie aufraeumen',
    @subsystem = N'TSQL',
    @database_name = N'msdb',
    @on_success_action = 1,            -- 1 = Job erfolgreich beenden
    @on_fail_action = 2,
    @retry_attempts = 0,
    @command = N'DECLARE @d datetime = DATEADD(day, -90, GETDATE()); EXEC msdb.dbo.sp_delete_backuphistory @oldest_date = @d;';
GO

EXEC msdb.dbo.sp_add_jobschedule
    @job_name = N'iSmile DB - Sicherung',
    @name = N'Taeglich $(StartTime)',
    @enabled = 1,
    @freq_type = 4,                    -- taeglich
    @freq_interval = 1,
    @freq_subday_type = 1,             -- einmal am Tag
    @active_start_time = $(StartTime);
GO

EXEC msdb.dbo.sp_add_jobserver @job_name = N'iSmile DB - Sicherung', @server_name = N'(local)';
GO

-- 4. Alerts: I/O-Fehler 823/824/825 und Schweregrad 20-25 werden in SSMS unter Warnungen gezaehlt
--    (letztes Auftreten sichtbar). Ohne Datenbank-E-Mail gibt es keine Benachrichtigung, nur die Zaehlung;
--    SQL Server schreibt diese Fehler ausserdem selbst ins Windows-Ereignisprotokoll.
DECLARE @alerts TABLE (name sysname, message_id int, severity int);
INSERT @alerts VALUES
    (N'iSmile - Fehler 823 (I/O)', 823, 0), (N'iSmile - Fehler 824 (Pruefsumme)', 824, 0), (N'iSmile - Fehler 825 (Lesewiederholung)', 825, 0),
    (N'iSmile - Schweregrad 20', 0, 20), (N'iSmile - Schweregrad 21', 0, 21), (N'iSmile - Schweregrad 22', 0, 22),
    (N'iSmile - Schweregrad 23', 0, 23), (N'iSmile - Schweregrad 24', 0, 24), (N'iSmile - Schweregrad 25', 0, 25);
DECLARE @name sysname, @mid int, @sev int;
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT name, message_id, severity FROM @alerts;
OPEN c; FETCH NEXT FROM c INTO @name, @mid, @sev;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT 1 FROM msdb.dbo.sysalerts WHERE name = @name) EXEC msdb.dbo.sp_delete_alert @name = @name;
    EXEC msdb.dbo.sp_add_alert @name = @name, @message_id = @mid, @severity = @sev, @enabled = 1,
        @delay_between_responses = 300, @include_event_description_in = 0;
    FETCH NEXT FROM c INTO @name, @mid, @sev;
END
CLOSE c; DEALLOCATE c;
GO

-- 5. Kontrolle
SELECT j.name AS job, j.enabled AS job_enabled, s.name AS schedule, s.enabled AS schedule_enabled,
       s.active_start_time AS start_time, SUSER_SNAME(j.owner_sid) AS owner_
FROM msdb.dbo.sysjobs j
JOIN msdb.dbo.sysjobschedules js ON js.job_id = j.job_id
JOIN msdb.dbo.sysschedules s ON s.schedule_id = js.schedule_id
WHERE j.name = N'iSmile DB - Sicherung';
SELECT step_id, step_name, on_success_action, on_fail_action FROM msdb.dbo.sysjobsteps
WHERE job_id = (SELECT job_id FROM msdb.dbo.sysjobs WHERE name = N'iSmile DB - Sicherung') ORDER BY step_id;
SELECT name AS alert_, enabled FROM msdb.dbo.sysalerts WHERE name LIKE N'iSmile - %' ORDER BY name;
GO
