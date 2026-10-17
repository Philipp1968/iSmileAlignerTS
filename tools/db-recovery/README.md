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

## Stand 17.10.2026, 21:40

- Schritt 1 (Diagnose): erledigt, Ergebnisse in `out\`.
- Schritt 2 (Quelle vorbereiten): erledigt. iSmileAlignerTS_db steht auf SIMPLE, Log 1 GB, MULTI_USER. IIS, Websites und App Pools auf DOCTORS sind gestoppt.
- Schritt 3 (Ziel anlegen): erledigt. iSmileAlignerTS_db_new existiert, Schema per ef6.exe aufgebaut und gegen die Quelle verglichen: Spalten, Indizes und Fremdschluessel identisch, 38 Migrationen, keine Datenzeilen.
- **Naechster Schritt: 4, die Kopie mit `04-copy-chunked.ps1` auf DOCTORS** (Connection Strings mit `Data Source=localhost,1433`, Login und Passwort aus Web.config). Danach Schritt 5.
- Auf DOCTORS laeuft eine aeltere PowerShell: die Skripte sind darauf angepasst (kein `$PSScriptRoot` in param-Defaults). Aeltere Kopien der .ps1-Dateien dort durch diese ersetzen.

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

## Alternative: lokal statt auf dem Server

Nur sinnvoll, wenn der Server nicht benutzt werden soll. Dazu muss auf diesem PC SQL Server 2025
Developer Edition installiert werden (kostenlos, keine Groessenbegrenzung, gleiche Version wie der Server).
Dann `D:\ismile20261017.bak` lokal einspielen und die Schritte 3-6 lokal ausfuehren, zuletzt die saubere
.bak auf den Server kopieren. Kostet zusaetzlich Installation plus 22 GB Upload.
