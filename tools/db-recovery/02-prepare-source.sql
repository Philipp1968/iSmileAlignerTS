/* ============================================================================
   02-prepare-source.sql  -  Quelle vorbereiten. KEINE Reparatur.

   Ergebnis von 01-diagnose.ps1 (17.10.2026): Seite 1:638952 ist eine PFS-Zuordnungsseite
   (638952 = 79 x 8088) mit Pruefsummenfehler. DBCC CHECKDB bricht daran nach 14 Sekunden ab
   (Fehler 8998 "Seitenfehler auf GAM-, SGAM- oder PFS-Seiten", 8921 "Check terminated"), und
   REPAIR_ALLOW_DATA_LOSS kann PFS/GAM/SGAM-Seiten nicht neu aufbauen. Ein Repair-Versuch bringt
   nichts und riskiert, die Quelle weiter zu beschaedigen.

   Die Daten werden deshalb mit 04-copy-chunked.ps1 ueber den Primaerschluessel gelesen. Das
   beruehrt keine PFS-Seiten; die Quelle wird nur noch gelesen.

   Hier: Platz schaffen (Log 29 GB -> 1 GB, davon sind nur 1,2 GB belegt) und den Zugriff so
   setzen, dass das Kopierskript und SSMS gleichzeitig arbeiten koennen. Der Application Pool
   der Website MUSS gestoppt bleiben, sonst schreibt die Anwendung waehrend der Kopie weiter.

   Ausfuehren in SSMS auf dem Server als sysadmin, Kontext master.
   ============================================================================ */
USE master;
GO

-- 1. Exklusiv, damit niemand dazwischenfunkt
ALTER DATABASE iSmileAlignerTS_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- 2. Log verkleinern (ohne Log-Sicherungen ist FULL ohnehin ohne Nutzen)
ALTER DATABASE iSmileAlignerTS_db SET RECOVERY SIMPLE;
GO
USE iSmileAlignerTS_db;
GO
CHECKPOINT;
DBCC SHRINKFILE (N'iSmileAlignerTS_db_log', 1024);
-- Falls das Log nicht auf ~1 GB schrumpft: CHECKPOINT; und SHRINKFILE noch einmal.
GO
USE master;
GO

-- 3. Wieder MULTI_USER: das Kopierskript oeffnet nacheinander mehrere Verbindungen,
--    und ein SSMS-Fenster zum Nachsehen soll parallel moeglich sein.
--    (Schutz vor der Anwendung ist der gestoppte Application Pool, nicht SINGLE_USER.)
ALTER DATABASE iSmileAlignerTS_db SET MULTI_USER;
GO

-- 4. Platz pruefen: fuer die Ziel-DB werden ca. 22 GB gebraucht
EXEC master.sys.xp_fixeddrives;
GO
