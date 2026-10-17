# Wiederherstellung iSmileAlignerTS_db nach Seitenkorruption

Stand 17.10.2026, nach Lauf von `01-diagnose.ps1`. Skripte in Reihenfolge der Nummern ausfuehren.

## Befund

| | |
|---|---|
| Server | DOCTORS, SQL Server 2025 Developer Edition (17.0.1135), sysadmin-Login aus Web.config, 37 GB frei auf C: |
| Datenbank | iSmileAlignerTS_db, 21,9 GB Daten, Log 29 GB (nur 1,2 GB belegt), PAGE_VERIFY CHECKSUM |
| Kernschaden | Seite 1:638952 ist eine **PFS-Zuordnungsseite** (638952 = 79 x 8088) mit Pruefsummenfehler. Sie wird bei jedem Schreibzugriff in ihrem Bereich gelesen, daher >2 Mio. Fehler und ein 824-Fehler pro Minute, solange die Website lief. |
| Folge | `DBCC CHECKDB` bricht nach 14 Sekunden ab (Fehler 8998, 8921). `REPAIR_ALLOW_DATA_LOSS` kann PFS/GAM/SGAM-Seiten nicht neu aufbauen. **Reparatur ist keine Option.** |
| Weitere Schaeden | ca. 320 Datenseiten (ca. 2,5 MB) in Clustern bei Seite 319660, 368119, 391064, 431614, 438752, 479518, 505248. Zuordnung zu Tabellen war wegen der PFS-Seite nicht moeglich; die Kopie zeigt es je Tabelle. |
| Zeitlinie | erste defekte Seite protokolliert 28.09.2026. Backup-Historie auf DOCTORS beginnt 09.09.2026 (Restore dort); alle Sicherungen auf DOCTORS enthalten den Schaden. |
| Sicherungen | D:\ismile20261017.bak (23 GB, enthaelt den Schaden 1:1); X:\Ablage\ISmileAlignerTS_db.bak (18 GB, 21.12.2023, SQL 2014, sauber) |
| Entwickler-PC | nur LocalDB und Express 2016 mit 10-GB-Grenze: koennen die 2025er Sicherung weder einspielen noch halten |

Folgerung: die Daten werden **ueber den Primaerschluessel in Bereichen gelesen** (beruehrt keine
PFS-Seiten) und in eine frische Datenbank auf dem Server geschrieben. Unlesbare Bereiche werden
halbiert, bis einzelne Zeilen uebersprungen sind. Das macht `04-copy-chunked.ps1`; es wurde lokal
mit simulierten Lesefehlern, Waisen, Identity-Spalten und einer berechneten Spalte getestet.

Noch lohnend zu klaeren: hat der Hoster eine Sicherung der **alten** VM von vor dem 28.09.2026?
Die waere vollstaendig und ohne Verlust.

## Stand 17.10.2026, 23:35 - Umschalten erledigt

- Schritt 1 (Diagnose): erledigt, Ergebnisse in `out\`.
- Schritt 2 (Quelle vorbereiten): erledigt. Quelle auf SIMPLE, Log 1 GB. IIS, Websites und App Pools auf DOCTORS gestoppt.
- Schritt 3 (Ziel anlegen): erledigt. Schema per ef6.exe aufgebaut und gegen die Quelle verglichen: identisch, 38 Migrationen.
- Schritt 4 (Kopie): erledigt, direkt auf DOCTORS mit Windows-Anmeldung (`Data Source=localhost,1433;Integrated Security=True`; DOCTORS\Administrator ist sysadmin), zwei Laeufe:
  - Lauf 1, 22:17: alle Tabellen ausser CaseFiles vollstaendig, 0 Zeilen verloren (`out\summary-run1-all-tables.csv`). CaseFiles brach nach 610 Zeilen ab: nach einem 824 beendet der Server die Verbindung, SqlBulkCopy verpackt den Transportfehler in "Fehler beim Lesen.", und das Skript pruefte nur die aeussere Meldung. Behoben: `Get-ErrorText` wertet die ganze InnerException-Kette aus, `Test-CorruptionError` kennt auch diese Huelle und SqlException 64/10054/233.
  - Lauf 2, 22:25 bis 23:19, nur `-Tables CaseFiles`: 36.339 von 36.339 Zeilen kopiert, 0 verloren, 107 ohne BLOB (`out\blob-lost.csv`; Details mit Dateiname, Fall und Datum in `out\blob-lost-details.csv`). Jede defekte Zeile kostet ca. 15 s Halbierung.
  - Die 107 Dateien ohne Inhalt: 55 Faelle, 22 davon Thumbnails, zusammen 321 MB, FileDate 07/2016 bis 05/2024. Am staerksten betroffen: Fall 50380 (9 von 23 Dateien), 60457 (7/22), 60493 (6/23), 70911 (6/23). 102 der 107 sind aelter als die saubere Sicherung vom 21.12.2023 und koennten von dort nachgeladen werden (CaseFiles.Id ist stabil).
- Schritt 5 (Pruefen, sichern, umschalten): **erledigt**, 23:23 bis 23:31.
  - `DBCC CHECKDB` auf der neuen DB: 0 Fehler (1 min 15 s). Zeilenzahlen alt = neu in allen 18 Tabellen, alle 4 Fremdschluessel aktiv und trusted.
  - Optionen an die alte DB angeglichen: READ_COMMITTED_SNAPSHOT ON, ALLOW_SNAPSHOT_ISOLATION ON, Service Broker aus. Datenbankbenutzer `ismiletsdb` angelegt (der Login existiert seit 09.09.2026; in der alten DB war der Benutzer verwaist), Rolle db_owner.
  - Sicherung: `C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\Backup\iSmileAlignerTS_db_new.bak`, 16,8 GB, WITH CHECKSUM, COMPRESSION, 71 s; RESTORE VERIFYONLY: gueltig. **Auf den Entwickler-PC kopieren und aufheben.**
  - Umschalten: alte DB -> `iSmileAlignerTS_db_damaged`, OFFLINE (SINGLE_USER); neue DB -> `iSmileAlignerTS_db`, MULTI_USER. Web.config unveraendert. Die Dateien der neuen DB heissen weiterhin `iSmileAlignerTS_db_new.mdf` / `iSmileAlignerTS_db_new_log.ldf` (auch die logischen Dateinamen); nur die Datenbank wurde umbenannt. Die minuetlichen 824-Meldungen des VersionCleaner im Errorlog hoerten mit OFFLINE auf.
- Protokolle dieses Laufs liegen versioniert in `runs\2026-10-17\` (Kopier-Log und Konsolenmitschrift, Zusammenfassungen beider Laeufe, verlorene BLOB-Ids, CHECKDB- und Sicherungsausgabe, Probe-Skript). `out\blob-lost-details.csv` enthaelt Patientennamen in den Dateinamen und bleibt lokal (gitignored); versioniert ist nur die Fassung ohne Dateinamen.
- **Offen (Owner):** W3SVC und die Application Pools `doctors.thinortho.com` und `patients.thinortho.com` starten, Stichprobe (Login, Fall mit Bildern und PDFs oeffnen, Bild speichern). Danach Punkt 6 aus `05-verify-and-switch.sql`: regelmaessige Sicherung WITH CHECKSUM einrichten, `iSmileAlignerTS_db_damaged` nach einigen fehlerfreien Tagen loeschen (23 GB), Anwendung auf den Login `ismiletsdb` umstellen und das sa-Passwort aendern. Fuer die 107 Dateien ohne Inhalt: die 102 aelteren aus der Sicherung vom 21.12.2023 nachladen (dazu diese Sicherung auf DOCTORS unter anderem Namen einspielen).

## Ablauf

1. **Website anhalten.** IIS auf DOCTORS: Application Pool der Seite stoppen und gestoppt lassen,
   bis Schritt 6 fertig ist. Sonst schreibt die Anwendung waehrend der Kopie weiter in die alte DB.
2. **Schadensbild** ist mit `01-diagnose.ps1` erledigt (`out\diagnose.txt`, `out\checkdb.csv`).
3. **Quelle vorbereiten** mit `02-prepare-source.sql` in SSMS auf dem Server: Log 29 GB -> 1 GB,
   danach MULTI_USER (der gestoppte Application Pool ist der Schutz, nicht SINGLE_USER).
4. **Zieldatenbank** mit `03-create-target.sql` anlegen, Schema per `ef6.exe` aus den Projekt-Migrationen
   vom Entwickler-PC aus (Befehl steht im Skript). Ergebnis: 18 Tabellen, 38 Eintraege in `__MigrationHistory`.
5. **Kopieren** mit `04-copy-chunked.ps1` **auf dem Server** (Ordner `tools\db-recovery` per RDP auf
   DOCTORS kopieren; von hier aus muessten 2 x 22 GB uebers Internet):

   ```
   powershell -ExecutionPolicy Bypass -File .\04-copy-chunked.ps1 `
     -SourceConnectionString "Data Source=localhost,1433;Initial Catalog=iSmileAlignerTS_db;User ID=<LOGIN>;Password=<PASSWORT>;" `
     -TargetConnectionString "Data Source=localhost,1433;Initial Catalog=iSmileAlignerTS_db_new;User ID=<LOGIN>;Password=<PASSWORT>;"
   ```

   Laeuft fuer CaseFiles (36.000 Zeilen, 21 GB) grob 10-40 Minuten. Ausgabe in `out\`:
   `copy-chunked.log`, `summary.csv` (je Tabelle: Quelle, kopiert, verloren, ohne BLOB),
   `lost-rows.csv` (verlorene Ids), `blob-lost.csv` (Zeilen ohne Bild/PDF). Waisen (z.B. CaseFiles zu
   einem verlorenen Case) werden am Ende gemeldet, mit fertigem SELECT zum Ansehen; den Fremdschluessel
   danach von Hand aktivieren (Befehl steht im Log) oder die Waisen loeschen.
   Der Lauf ist wiederholbar: Zieltabellen werden vorher geleert.
6. **Pruefen, sichern, umschalten** mit `05-verify-and-switch.sql`: CHECKDB auf der neuen DB, Zeilen alt/neu,
   Stichprobe mit der Anwendung, Sicherung WITH CHECKSUM, dann Umbenennen alt -> `_damaged`, neu -> Produktivname.
   Web.config bleibt unveraendert.

## Was verloren geht

Zeilen auf den ca. 320 unlesbaren Datenseiten und BLOBs auf unlesbaren LOB-Seiten. Grob einige Dutzend bis
wenige Hundert Zeilen, vor allem in CaseFiles. `summary.csv` zeigt es je Tabelle, `lost-rows.csv` je Id.
Zeilen mit unlesbarem BLOB werden ohne BLOB kopiert (Content = NULL), damit Dateiname und Fallzuordnung
erhalten bleiben; `OpenPicture` wuerde fuer so eine Zeile einen Fehler liefern. Wer das nicht will:
`-DropRowOnBlobError`, oder hinterher `DELETE FROM CaseFiles WHERE Id IN (...)` aus `blob-lost.csv`.

Fuer Faelle, deren Bilder fehlen, kann die Sicherung vom 21.12.2023 als Quelle dienen, sofern die Dateien
damals schon existierten (CaseFiles.Id ist stabil). Dazu braucht es eine Instanz, die 18 GB halten kann,
z.B. DOCTORS selbst unter anderem DB-Namen.

## Dateien

| Datei | Zweck |
|---|---|
| `01-diagnose.ps1` | Schadensbild, nur lesend (erledigt) |
| `02-prepare-source.sql` | Log verkleinern, Zugriff setzen |
| `03-create-target.sql` | Ziel-DB anlegen, ef6-Befehl fuer das Schema |
| `04-copy-chunked.ps1` | die Kopie (Chunks, Halbierung, BLOB-Fallback, FK-Pruefung) |
| `05-verify-and-switch.sql` | Kontrolle, Sicherung, Umschalten |
| `alt-copy-tables.sql` | T-SQL-Kopie fuer eine **konsistente** Quelle; hier nicht anwendbar |
| `runs\2026-10-17\` | Protokolle der Wiederherstellung vom 17.10.2026 (ohne Patientendaten) |
| `out\` | Arbeitsausgaben der Skripte, gitignored (enthaelt `blob-lost-details.csv` mit Dateinamen) |

## Alternative: lokal statt auf dem Server

Nur sinnvoll, wenn der Server nicht benutzt werden soll. Dazu muss auf diesem PC SQL Server 2025
Developer Edition installiert werden (kostenlos, keine Groessenbegrenzung, gleiche Version wie der Server).
Dann `D:\ismile20261017.bak` lokal einspielen und die Schritte 3-6 lokal ausfuehren, zuletzt die saubere
.bak auf den Server kopieren. Kostet zusaetzlich Installation plus 22 GB Upload.
