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

- Production: SQL Server 2025 Developer Edition (17.0.1135) on host DOCTORS, default instance (instance ID `MSSQL17.ISMILE`), port 1433.
  Connection string `DefaultConnection` in `iSmileAlignerTS/Web.config` (a sysadmin login).
  On DOCTORS itself use `Data Source=localhost,1433` so bulk traffic stays on loopback.
- Database `iSmileAlignerTS_db`, about 22 GB, almost all of it `CaseFiles.Content` blobs.
- The dev PC only has LocalDB and SQL Express 2016 (10 GB cap): they can neither restore nor hold the
  production database. Do not plan work that needs the production data locally.

## Database recovery, October 2026 (READ BEFORE TOUCHING THE DATABASE)

The original `iSmileAlignerTS_db` carried page corruption from a disk-level malware incident: PFS allocation page
1:638952 with a checksum error (so `DBCC CHECKDB` aborted and no repair option could help) plus unreadable LOB pages.
On 17.10.2026 its data was copied by primary-key ranges into a fresh database, which was then renamed to the
production name. Runbook, timings and full status: `tools/db-recovery/README.md`.

**State since 17.10.2026, 23:31**

- `iSmileAlignerTS_db` is the clean copy: all 18 tables, row counts equal to the old database, CHECKDB clean, 0 rows
  lost, 107 `CaseFiles` rows kept with `Content = NULL` (`tools/db-recovery/out/blob-lost.csv`; 102 of them predate
  the clean backup of 21.12.2023 and could be reloaded from it, `CaseFiles.Id` is stable). Its files are still named
  `iSmileAlignerTS_db_new.mdf` / `iSmileAlignerTS_db_new_log.ldf` (logical names likewise); only the database was renamed.
- `iSmileAlignerTS_db_damaged` is the old database, OFFLINE and SINGLE_USER. Never point the app at it and never run
  `DBCC CHECKDB` with a repair option on it (it cannot fix a PFS page). Drop it once the new database has run clean
  for a few days: `DROP DATABASE iSmileAlignerTS_db_damaged;` frees 23 GB.
- First clean backup: `C:\Program Files\Microsoft SQL Server\MSSQL17.ISMILE\MSSQL\Backup\iSmileAlignerTS_db_new.bak`
  (16.8 GB, WITH CHECKSUM, verified). It should be copied to the dev PC and kept.
- Database options match the old database: READ_COMMITTED_SNAPSHOT on, snapshot isolation on, broker off, RECOVERY
  SIMPLE, PAGE_VERIFY CHECKSUM. Database user `ismiletsdb` exists, mapped to the server login of the same name, with
  `db_owner`; the app still logs in as `sa` (`Web.config` unchanged).

- Logs of the recovery run: `tools/db-recovery/runs/2026-10-17/` (copy log and console transcript, per-table
  summaries of both runs, lost-blob Ids, redacted lost-blob details, CHECKDB and backup output, the blob probe).
  The unredacted detail list carries patient names in the filenames and stays local and gitignored:
  `tools/db-recovery/out/blob-lost-details.csv`. Never commit filenames from `CaseFiles`.

**Open after the switch**

- IIS, websites and app pools on DOCTORS were left stopped; the owner starts them and smoke-tests the app.
- Set up regular backups `WITH CHECKSUM, COMPRESSION`; keep RECOVERY SIMPLE unless log backups are scheduled.
- Switch the app to the `ismiletsdb` login (owner sets its password), then rotate the `sa` password.

The copy tool `tools/db-recovery/04-copy-chunked.ps1` reads rows by primary-key ranges (never touching PFS pages),
reconnects after each 824, halves the range down to single rows, keeps rows whose blob is unreadable with
`Content = NULL`, and re-enables foreign keys with checks, reporting orphans. SqlBulkCopy wraps the connection kill
that follows an 824 in a generic "Fehler beim Lesen." exception, so the tool inspects the whole InnerException
chain (`Get-ErrorText`). Outputs land in `tools/db-recovery/out/` (gitignored).

DOCTORS runs Windows PowerShell 5.1. Keep the scripts PowerShell 3 compatible (no `??`, no ternary) and do not rely
on `$PSScriptRoot` inside `param()` defaults.

## Claude Code notes

- The auto-mode classifier blocks bulk reads of the production database unless `PowerShell(*)` and
  `Bash(*)` are allowed in `.claude/settings.json` (committed) or `.claude/settings.local.json` (ignored).
  Claude cannot create or edit these files itself; the owner does.
- `tools/db-recovery/alt-copy-tables.sql` is a pure T-SQL copy for a *consistent* source. Not applicable here.
- On DOCTORS itself `DOCTORS\Administrator` is sysadmin: use `Integrated Security=True` in connection strings and
  `sqlcmd -E -C` (`-C` trusts the self-signed server certificate). No password needs to appear on a command line.
- `powershell.exe -File` cannot open scripts in Claude's scratchpad: the path exceeds 260 characters. Put throwaway
  scripts in `tools/db-recovery/out/` (gitignored) instead.
- After the recovery: rotate the admin password and give the app its own login with `db_owner` only.
