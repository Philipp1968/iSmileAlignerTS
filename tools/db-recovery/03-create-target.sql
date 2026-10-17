/* ============================================================================
   03-create-target.sql  -  leere Zieldatenbank auf DOCTORS anlegen
   Ausfuehren in SSMS auf dem Server als sysadmin, Kontext master.
   Danach das Schema mit den EF6-Migrationen des Projekts erzeugen (Befehl unten, auf dem Entwickler-PC).
   Platzbedarf: am Ende ca. 20-22 GB Daten. Vorher 02-prepare-source.sql (Log-Shrink) laufen lassen.
   ============================================================================ */
USE master;
GO
CREATE DATABASE iSmileAlignerTS_db_new
ON PRIMARY (
  NAME = N'iSmileAlignerTS_db_new',
  FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\DATA\iSmileAlignerTS_db_new.mdf',
  SIZE = 8192MB, FILEGROWTH = 2048MB)
LOG ON (
  NAME = N'iSmileAlignerTS_db_new_log',
  FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\DATA\iSmileAlignerTS_db_new_log.ldf',
  SIZE = 2048MB, FILEGROWTH = 1024MB)
COLLATE SQL_Latin1_General_CP1_CI_AS;     -- wie die bisherige DB
GO
ALTER DATABASE iSmileAlignerTS_db_new SET RECOVERY SIMPLE;        -- waehrend der Kopie; spaeter ggf. FULL + Log-Sicherungen
ALTER DATABASE iSmileAlignerTS_db_new SET PAGE_VERIFY CHECKSUM;
ALTER DATABASE iSmileAlignerTS_db_new SET COMPATIBILITY_LEVEL = 120;  -- wie die bisherige DB
GO

/* ----------------------------------------------------------------------------
   Schema anlegen - auf dem ENTWICKLER-PC in einer Eingabeaufforderung.
   Das Projekt muss vorher gebaut sein (bin\iSmileAlignerTS.dll). <SERVER>, <LOGIN>, <PASSWORT> wie in Web.config (DefaultConnection).

     cd C:\Users\Philipp\Source\Repos\iSmileAlignerTS\iSmileAlignerTS
     ..\packages\EntityFramework.6.5.2\tools\net45\any\ef6.exe database update ^
        --assembly bin\iSmileAlignerTS.dll --config Web.config --project-dir . ^
        --connection-provider System.Data.SqlClient ^
        --connection-string "Data Source=<SERVER>,1433;Initial Catalog=iSmileAlignerTS_db_new;User ID=<LOGIN>;Password=<PASSWORT>;MultipleActiveResultSets=True" ^
        --verbose

   Das legt alle 18 Tabellen, Indizes, Fremdschluessel und __MigrationHistory (38 Eintraege) an.
   Seed() im Projekt ist leer, es werden keine Datenzeilen eingefuegt.

   Kontrolle in SSMS:
     SELECT COUNT(*) FROM iSmileAlignerTS_db_new.sys.tables;              -- 18
     SELECT COUNT(*) FROM iSmileAlignerTS_db_new.dbo.__MigrationHistory;  -- 38

   Alternative ohne ef6.exe: SSMS > Rechtsklick iSmileAlignerTS_db > Tasks > Generate Scripts,
   "Schema only", alle Tabellen, in den Optionen Indizes + Fremdschluessel einschalten, Skript gegen
   iSmileAlignerTS_db_new ausfuehren. Dann 04-copy-chunked.ps1 mit  -SkipTables sysdiagrams  aufrufen,
   damit __MigrationHistory mitkopiert wird (sonst meldet EF beim Start ein Modell-Mismatch).
   ---------------------------------------------------------------------------- */
