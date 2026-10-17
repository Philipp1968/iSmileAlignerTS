/* ============================================================================
   05-verify-and-switch.sql  -  neue DB pruefen, sichern, auf den Produktivnamen umschalten
   Ausfuehren in SSMS auf dem Server als sysadmin, Kontext master. Schritt fuer Schritt, nicht am Stueck.
   ============================================================================ */
USE master;
GO

-- 1. Konsistenz der neuen DB (keine Ausgabe = fehlerfrei)
DBCC CHECKDB (N'iSmileAlignerTS_db_new') WITH NO_INFOMSGS, ALL_ERRORMSGS;
GO

-- 2. Zeilen je Tabelle: alt gegen neu
SELECT s.name AS table_name, s.rows_ AS alt, n.rows_ AS neu, s.rows_ - ISNULL(n.rows_, 0) AS fehlend
FROM (SELECT t.name, SUM(p.rows) AS rows_ FROM iSmileAlignerTS_db.sys.tables t
      JOIN iSmileAlignerTS_db.sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1) GROUP BY t.name) s
LEFT JOIN (SELECT t.name, SUM(p.rows) AS rows_ FROM iSmileAlignerTS_db_new.sys.tables t
      JOIN iSmileAlignerTS_db_new.sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1) GROUP BY t.name) n ON n.name = s.name
ORDER BY s.name;
GO

-- 3. Stichprobe Anwendung: Web.config auf dem Entwickler-PC testweise auf
--    Initial Catalog=iSmileAlignerTS_db_new zeigen lassen, Login pruefen, einen Fall mit Bildern und PDFs
--    oeffnen (EditCase, ViewCase, ViewCaseFiles), ein Bild speichern. Danach Web.config zuruecksetzen.

-- 4. Sicherung der neuen DB MIT Pruefsumme (Platz: komprimiert deutlich unter 22 GB)
BACKUP DATABASE iSmileAlignerTS_db_new
  TO DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\Backup\iSmileAlignerTS_db_new.bak'
  WITH CHECKSUM, COMPRESSION, INIT, STATS = 5;
RESTORE VERIFYONLY
  FROM DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\Backup\iSmileAlignerTS_db_new.bak'
  WITH CHECKSUM;
GO
-- Diese .bak-Datei auf den Entwickler-PC kopieren und dort aufheben (sie ist der erste saubere Stand).

-- 5. Umschalten (Application Pool bleibt bis danach gestoppt). Bewusst auskommentiert.
/*
ALTER DATABASE iSmileAlignerTS_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
ALTER DATABASE iSmileAlignerTS_db MODIFY NAME = iSmileAlignerTS_db_damaged;
ALTER DATABASE iSmileAlignerTS_db_damaged SET OFFLINE;

ALTER DATABASE iSmileAlignerTS_db_new SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
ALTER DATABASE iSmileAlignerTS_db_new MODIFY NAME = iSmileAlignerTS_db;
ALTER DATABASE iSmileAlignerTS_db SET MULTI_USER;
*/

-- 6. Danach
--    - Web.config bleibt unveraendert (Initial Catalog=iSmileAlignerTS_db), Application Pool starten.
--    - RECOVERY SIMPLE beibehalten, solange keine Log-Sicherungen eingerichtet sind.
--      Sonst waechst das Log wieder auf zig GB (so wie bisher auf 29 GB).
--    - Regelmaessige Sicherung einrichten: BACKUP DATABASE ... WITH CHECKSUM, COMPRESSION.
--    - Die alte DB (iSmileAlignerTS_db_damaged, Dateien iSmileAlignerTS_db_Primary.mdf/.ldf) erst loeschen,
--      wenn die neue einige Tage fehlerfrei laeuft:  DROP DATABASE iSmileAlignerTS_db_damaged;
--    - Nach dem Malware-Vorfall: Admin-Passwort aendern und der Anwendung einen eigenen Login mit
--      db_owner auf iSmileAlignerTS_db geben statt sa.
