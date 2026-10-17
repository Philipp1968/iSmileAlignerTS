# Sicherung iSmileAlignerTS_db auf DOCTORS

Eingerichtet am 18.10.2026 nach der Wiederherstellung vom 17.10.2026 (`tools\db-recovery\README.md`).
Ziel: eine Korruption wie im September faellt in der naechsten Nacht auf, und es liegt immer eine
geprueft lesbare Vollsicherung bereit.

## Was laeuft

SQL Server-Agent-Job **"iSmile DB - Sicherung"**, taeglich 02:00, angelegt mit `Install-BackupJobs.ps1`:

| Schritt | Was | Bei Fehler |
|---|---|---|
| 1 | `DBCC CHECKDB` auf iSmileAlignerTS_db und Blick in `msdb.dbo.suspect_pages` | Job endet. **Keine** Sicherung, **keine** Rotation: die letzten guten Sicherungen bleiben liegen. |
| 2 | `BACKUP DATABASE ... WITH CHECKSUM, COMPRESSION` nach `...\MSSQL\Backup\iSmileAlignerTS_db\iSmileAlignerTS_db_FULL_<Datum>_<Zeit>.bak`, danach `RESTORE VERIFYONLY WITH CHECKSUM` | Job endet, keine Rotation. |
| 3 | Rotation: die neuesten *KeepCount* eigenen Vollsicherungen (laut msdb) bleiben, aeltere `.bak` im Ordner werden geloescht (`xp_delete_file`, loescht nur echte SQL-Sicherungen) | Job endet mit Fehler. |
| 4 | msdb-Sicherungshistorie aelter als 90 Tage aufraeumen | - |

Jeder Fehlschlag steht im Windows-Ereignisprotokoll (Anwendung, Quelle SQLSERVERAGENT) und in SSMS unter
SQL Server-Agent > Aufträge > "iSmile DB - Sicherung" > Verlauf anzeigen. Dazu kommen Agent-Warnungen fuer
die Fehler 823/824/825 und Schweregrad 20-25 (SSMS: SQL Server-Agent > Warnungen, Spalte "Letztes Auftreten").
Servervorgaben `backup checksum default` und `backup compression default` stehen auf 1, damit auch
Sicherungen von Hand Pruefsummen tragen.

Die erste geprueft saubere Sicherung nach der Wiederherstellung liegt **ausserhalb** des Rotationsordners und
bleibt als Anker: `...\MSSQL\Backup\iSmileAlignerTS_db_new.bak` (15,7 GB, 17.10.2026 23:28).

## Warum taeglich voll, keine differenziellen Sicherungen

Jede Nacht gegen 22:30 zieht etwas eine VSS-Momentaufnahme aller Datenbanken (vermutlich die VM-Sicherung
des Hosters; `msdb.dbo.backupset` zeigt seit 06.10.2026 Eintraege mit `is_snapshot = 1`, `is_copy_only = 0`,
Geraetename eine GUID). Eine solche Momentaufnahme gilt als normale Vollsicherung und setzt die Basis fuer
differenzielle Sicherungen zurueck. Eigene differenzielle Sicherungen waeren also nur zusammen mit der
Momentaufnahme des Hosters wiederherstellbar, die wir nicht haben. Darum taeglich eine volle Sicherung.

Beim Hoster nachfragen lohnt sich: wie lange halten sie die VM-Sicherungen vor, und koennen sie den SQL-Teil
auf "copy only" stellen? Dann waeren spaeter wochenweise Voll- plus taegliche differenzielle Sicherungen
moeglich (deutlich weniger Platz je Wiederherstellungspunkt).

## Platz

Eine Vollsicherung ist komprimiert ca. **15,7 GB** (Datenbank 22 GB, fast nur JPEG/PDF/STL). Waehrend des
Jobs liegt kurz eine Datei mehr im Ordner, weil erst gesichert und geprueft, dann rotiert wird.

| KeepCount | Dateien dauerhaft | Spitze waehrend des Jobs |
|---|---|---|
| 1 | 1 (ca. 16 GB) | 2 (ca. 32 GB) |
| 2 | 2 (ca. 32 GB) | 3 (ca. 47 GB) |
| 3 | 3 (ca. 47 GB) | 4 (ca. 63 GB) |

Stand 18.10.2026 auf C: (175 GB): 46 GB frei vor der ersten Sicherung, nachdem `...\Backup\ismile.bak`
(21,4 GB, Vollsicherung vom 09.09.2026 **mit** dem Schaden, durch `iSmileAlignerTS_db_new.bak` ueberholt) auf den
Entwickler-PC kopiert und hier geloescht wurde. Grosse Posten weiterhin: `iSmileAlignerTS_db_damaged` (alte,
kaputte DB, offline, 23 GB) und `iSmileAlignerTS_db_new.bak` (15,7 GB, behalten).

- Jetzt: KeepCount 1, Spitze 32 GB bei 46 GB frei, 14 GB Luft.
- Nach `DROP DATABASE iSmileAlignerTS_db_damaged` (nach einigen fehlerfreien Tagen): 69 GB frei, reicht fuer
  KeepCount 2 mit 22 GB Luft.
- Mehr Aufbewahrung braucht einen zweiten Datentraeger oder ein Netzlaufwerk als Ziel (`-BackupDir`).

Die Aufbewahrung aendern: `Install-BackupJobs.ps1 -KeepCount <n> -Enable` erneut ausfuehren (ersetzt den Job,
die Jobhistorie in SSMS beginnt von vorn).

## Erster Lauf

18.10.2026 01:22, von Hand gestartet (`-RunNow`): CHECKDB 1 min 10 s ohne Fehler, Sicherung 60 s (360 MB/s),
`iSmileAlignerTS_db_FULL_20261018_0122.bak` 15,7 GB, RESTORE VERIFYONLY gueltig, Rotation nichts zu loeschen,
Job insgesamt 2 min 41 s. Danach Zeitplan eingeschaltet (taeglich 02:00).

## Ausser Haus

Lokale Sicherungen auf derselben Platte schuetzen nicht vor Plattenausfall oder Malware. Mindestens die
woechentliche Kopie einer geprueften Vollsicherung gehoert woanders hin, z.B. auf den Entwickler-PC:

```
robocopy "C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\Backup\iSmileAlignerTS_db" "\\<ziel>\<freigabe>" *.bak /MAXAGE:1 /R:2 /W:10
```

Sobald ein Ziel feststeht, kann das als Schritt 5 in den Job (Subsystem CmdExec) oder als geplante Aufgabe.

## Bedienen

```
# einrichten, Zeitplan noch aus (laeuft nur auf Anforderung)
powershell -ExecutionPolicy Bypass -File .\Install-BackupJobs.ps1 -KeepCount 1
# einrichten und sofort einmal laufen lassen (3-5 Minuten), Ergebnis wird ausgegeben
powershell -ExecutionPolicy Bypass -File .\Install-BackupJobs.ps1 -KeepCount 1 -RunNow
# Zeitplan einschalten (legt den Job neu an) ...
powershell -ExecutionPolicy Bypass -File .\Install-BackupJobs.ps1 -KeepCount 1 -Enable
# ... oder nur den Schalter umlegen, ohne den Job neu anzulegen:
sqlcmd -S localhost,1433 -E -C -Q "EXEC msdb.dbo.sp_update_job @job_name = N'iSmile DB - Sicherung', @enabled = 1;"
```

In SSMS: SQL Server-Agent > Aufträge > "iSmile DB - Sicherung" > Rechtsklick > Auftrag starten bei Schritt /
Verlauf anzeigen. Letzte Sicherungen abfragen:

```sql
SELECT TOP 10 b.backup_start_date, b.type, b.is_snapshot, CAST(b.compressed_backup_size/1073741824.0 AS decimal(6,1)) AS gb,
       b.has_backup_checksums, m.physical_device_name
FROM msdb.dbo.backupset b JOIN msdb.dbo.backupmediafamily m ON m.media_set_id = b.media_set_id
WHERE b.database_name = N'iSmileAlignerTS_db' ORDER BY b.backup_start_date DESC;
```

## Wiederherstellen

Unter anderem Namen neben die Produktion (zum Nachsehen oder um einzelne Zeilen zu holen):

```sql
RESTORE DATABASE iSmileAlignerTS_db_restore
  FROM DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\Backup\iSmileAlignerTS_db\iSmileAlignerTS_db_FULL_<Datum>_<Zeit>.bak'
  WITH CHECKSUM,
       MOVE N'iSmileAlignerTS_db_new'     TO N'C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\DATA\iSmileAlignerTS_db_restore.mdf',
       MOVE N'iSmileAlignerTS_db_new_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\DATA\iSmileAlignerTS_db_restore_log.ldf',
       STATS = 10;
```

Die logischen Dateinamen heissen weiterhin `iSmileAlignerTS_db_new` / `iSmileAlignerTS_db_new_log` (siehe
db-recovery README). Produktion ersetzen: Application Pools stoppen, dann `RESTORE DATABASE iSmileAlignerTS_db
FROM DISK = ... WITH CHECKSUM, REPLACE`.

## Dateien

| Datei | Zweck |
|---|---|
| `Install-BackupJobs.ps1` | Agent-Dienst einschalten, SQL-Skript mit Parametern ausfuehren, optional Job sofort laufen lassen |
| `install-backup-jobs.sql` | Job, Zeitplan, Alerts, Servervorgaben (SQLCMD-Variablen, idempotent) |
