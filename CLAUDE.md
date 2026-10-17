# iSmileAlignerTS

ASP.NET MVC 5 web app (.NET Framework 4.7.2) for dental aligner cases. Entity Framework 6.5.2 Code First
with explicit migrations (38, latest `201802121011166_m20180212`, `Seed()` is empty), ASP.NET Identity.
Code comments are German, chat with the owner is English. Source files are UTF-8 with BOM and CRLF.

## Build

NuGet restore must go through the solution (packages.config project):

```
MSBuild.exe iSmileAlignerTS.sln -t:Restore -p:RestorePackagesConfig=true
MSBuild.exe iSmileAlignerTS\iSmileAlignerTS.csproj -t:Build -p:Configuration=Debug
```

MSBuild lives under Visual Studio (`vswhere -find MSBuild\**\Bin\MSBuild.exe`). From Git Bash use `-t:` not `/t:`.

## Database

- Production: SQL Server 2025 Developer Edition on host DOCTORS, instance `ISMILE`.
  Connection string `DefaultConnection` in `iSmileAlignerTS/Web.config` (a sysadmin login).
  On DOCTORS itself use `Data Source=localhost,1433` so bulk traffic stays on loopback.
- Database `iSmileAlignerTS_db`, about 22 GB, almost all of it `CaseFiles.Content` blobs.
- The dev PC only has LocalDB and SQL Express 2016 (10 GB cap): they can neither restore nor hold the
  production database. Do not plan work that needs the production data locally.

## Database recovery, October 2026 (READ BEFORE TOUCHING THE DATABASE)

`iSmileAlignerTS_db` carries page corruption from a disk-level malware incident. Page 1:638952 is a PFS
allocation page with a checksum error, so `DBCC CHECKDB` aborts and `REPAIR_ALLOW_DATA_LOSS` cannot help.
About 320 further data pages are unreadable. Every read of a bad page raises error 824 and kills the
connection, which the app saw as SqlClient error 19.

**Rules**

- Never run `DBCC CHECKDB` with a repair option on `iSmileAlignerTS_db`. It cannot fix a PFS page.
- Treat `iSmileAlignerTS_db` as read-only. All writes go to the fresh copy `iSmileAlignerTS_db_new`.
- IIS, the websites and the app pools on DOCTORS stay stopped until the switch-over in step 5 is done.
- Do not open `iSmileAlignerTS_db` in SSMS while a copy is running. In SINGLE_USER mode even the SSMS
  IntelliSense connection takes the only slot (error 5064).

**Runbook and status**: `tools/db-recovery/README.md`. Steps 1 to 3 are done (diagnosis, source on SIMPLE
with the log shrunk, `iSmileAlignerTS_db_new` created with the schema built by `ef6.exe` and verified
identical). Next is step 4, `04-copy-chunked.ps1`, run on DOCTORS; then step 5, verify and switch.

The copy tool reads rows by primary-key ranges (never touching PFS pages), reconnects after each 824,
halves the range down to single rows, keeps rows whose blob is unreadable with `Content = NULL`, and
re-enables foreign keys with checks, reporting orphans. It was tested on LocalDB with simulated failures.
Outputs land in `tools/db-recovery/out/` (gitignored): `summary.csv`, `lost-rows.csv`, `blob-lost.csv`.

DOCTORS runs an older Windows PowerShell where `$PSScriptRoot` is empty inside `param()` defaults; the
scripts already work around this. Keep new scripts PowerShell 3 compatible (no `??`, no ternary).

## Claude Code notes

- The auto-mode classifier blocks bulk reads of the production database unless `PowerShell(*)` and
  `Bash(*)` are allowed in `.claude/settings.json` (committed) or `.claude/settings.local.json` (ignored).
  Claude cannot create or edit these files itself; the owner does.
- `tools/db-recovery/alt-copy-tables.sql` is a pure T-SQL copy for a *consistent* source. Not applicable here.
- After the recovery: rotate the admin password and give the app its own login with `db_owner` only.
