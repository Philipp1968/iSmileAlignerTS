/* ============================================================================
   alt-copy-tables.sql  -  ALTERNATIVE zu 04-copy-chunked.ps1, nur fuer eine konsistente Quelle.
   Bei iSmileAlignerTS_db (Stand 17.10.2026) NICHT anwendbar: PFS-Seite 1:638952 ist defekt,
   jeder 824-Fehler beendet die Verbindung und damit dieses Skript. Siehe README.
   Kopiert alle Benutzertabellen von @Source nach @Target (gleiche SQL-Instanz),
   batchweise ueber den Primaerschluessel, mit IDENTITY_INSERT (IDs bleiben erhalten),
   Fremdschluessel im Ziel waehrend des Kopierens deaktiviert, danach wieder geprueft.

   Voraussetzungen
     - DBCC CHECKDB auf @Source laeuft fehlerfrei durch.
       (Ein 824-Fehler beendet die Verbindung, TRY/CATCH kann das nicht abfangen.)
     - @Target existiert mit identischem Schema (03-create-target.sql + ef6.exe) und steht auf RECOVERY SIMPLE.

   Ausfuehren in SSMS auf dem Server als sysadmin, Kontext master.
   Tipp: Strg+T (Ergebnisse als Text), dann sind Fortschritt und Zusammenfassung in einem Fenster.
   Dauer: CaseFiles (36.000 Zeilen, ~21 GB) ist der Loewenanteil, grob 10-40 Minuten je nach Platte.
   ============================================================================ */
SET NOCOUNT ON;
SET XACT_ABORT OFF;

DECLARE @Source sysname = N'iSmileAlignerTS_db';
DECLARE @Target sysname = N'iSmileAlignerTS_db_new';
DECLARE @BatchRows int = 5000;      -- Zeilen pro Batch (normale Tabellen)
DECLARE @BlobBatchRows int = 200;   -- Zeilen pro Batch bei Tabellen mit varbinary(max)/nvarchar(max)/image/text
DECLARE @ClearTarget bit = 1;       -- 1 = Zieltabelle vor dem Kopieren leeren (Wiederholung moeglich)
DECLARE @SkipTables nvarchar(max) = N'__MigrationHistory,sysdiagrams';   -- ef6.exe fuellt __MigrationHistory selbst

DECLARE @sql nvarchar(max), @msg nvarchar(max);

/* ---- 1. Tabellenliste aus der Quelle ------------------------------------- */
IF OBJECT_ID('tempdb..#tables') IS NOT NULL DROP TABLE #tables;
CREATE TABLE #tables (
  table_schema sysname, table_name sysname, object_id int,
  pk_column sysname NULL, pk_is_int bit, has_identity bit, has_blob bit,
  src_rows bigint, copied bigint NULL, tgt_rows bigint NULL, status nvarchar(400) NULL
);

SET @sql = N'
INSERT #tables (table_schema, table_name, object_id, pk_column, pk_is_int, has_identity, has_blob, src_rows)
SELECT s.name, t.name, t.object_id,
  pk.col_name,
  CASE WHEN pk.col_count = 1 AND pk.type_name IN (''int'',''bigint'',''smallint'',''tinyint'') THEN 1 ELSE 0 END,
  CASE WHEN EXISTS (SELECT 1 FROM ' + QUOTENAME(@Source) + N'.sys.columns c WHERE c.object_id = t.object_id AND c.is_identity = 1) THEN 1 ELSE 0 END,
  CASE WHEN EXISTS (SELECT 1 FROM ' + QUOTENAME(@Source) + N'.sys.columns c
                    JOIN ' + QUOTENAME(@Source) + N'.sys.types ty ON ty.user_type_id = c.user_type_id
                    WHERE c.object_id = t.object_id
                      AND (ty.name IN (''image'',''text'',''ntext'',''xml'')
                           OR (ty.name IN (''varbinary'',''varchar'',''nvarchar'') AND c.max_length = -1))) THEN 1 ELSE 0 END,
  ISNULL((SELECT SUM(p.rows) FROM ' + QUOTENAME(@Source) + N'.sys.partitions p WHERE p.object_id = t.object_id AND p.index_id IN (0,1)), 0)
FROM ' + QUOTENAME(@Source) + N'.sys.tables t
JOIN ' + QUOTENAME(@Source) + N'.sys.schemas s ON s.schema_id = t.schema_id
OUTER APPLY (
  SELECT MIN(c.name) AS col_name, COUNT(*) AS col_count, MIN(ty.name) AS type_name
  FROM ' + QUOTENAME(@Source) + N'.sys.indexes i
  JOIN ' + QUOTENAME(@Source) + N'.sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
  JOIN ' + QUOTENAME(@Source) + N'.sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
  JOIN ' + QUOTENAME(@Source) + N'.sys.types ty ON ty.user_type_id = c.user_type_id
  WHERE i.object_id = t.object_id AND i.is_primary_key = 1
) pk
WHERE t.is_ms_shipped = 0;';
EXEC sp_executesql @sql;

DELETE FROM #tables WHERE table_name IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@SkipTables, N','));

DECLARE @tableCount int;
SELECT @tableCount = COUNT(*) FROM #tables;
PRINT N'Quelle ' + @Source + N' -> Ziel ' + @Target + N': ' + CAST(@tableCount AS nvarchar(10)) + N' Tabellen';

/* ---- 2. Fremdschluessel im Ziel deaktivieren ------------------------------ */
SET @sql = N'USE ' + QUOTENAME(@Target) + N'; EXEC sp_MSforeachtable N''ALTER TABLE ? NOCHECK CONSTRAINT ALL'';';
EXEC (@sql);

/* ---- 3. Tabelle fuer Tabelle kopieren ------------------------------------ */
DECLARE @schema sysname, @table sysname, @objid int, @pk sysname, @pkint bit, @ident bit, @blob bit, @srcrows bigint;
DECLARE @cols nvarchar(max), @qsrc nvarchar(max), @qtgt nvarchar(max);
DECLARE @batch int, @lo bigint, @hi bigint, @maxid bigint, @n bigint, @copied bigint;

DECLARE tcur CURSOR LOCAL FAST_FORWARD FOR
  SELECT table_schema, table_name, object_id, pk_column, pk_is_int, has_identity, has_blob, src_rows
  FROM #tables ORDER BY table_name;
OPEN tcur;
FETCH NEXT FROM tcur INTO @schema, @table, @objid, @pk, @pkint, @ident, @blob, @srcrows;
WHILE @@FETCH_STATUS = 0
BEGIN
  SET @qsrc = QUOTENAME(@Source) + N'.' + QUOTENAME(@schema) + N'.' + QUOTENAME(@table);
  SET @qtgt = QUOTENAME(@Target) + N'.' + QUOTENAME(@schema) + N'.' + QUOTENAME(@table);
  SET @copied = 0;

  -- Spaltenliste: nicht berechnete Spalten, die es im Ziel auch gibt
  SET @cols = NULL;
  SET @sql = N'SELECT @cols = STRING_AGG(CAST(QUOTENAME(c.name) AS nvarchar(max)), N'', '') WITHIN GROUP (ORDER BY c.column_id)
    FROM ' + QUOTENAME(@Source) + N'.sys.columns c
    WHERE c.object_id = @objid AND c.is_computed = 0
      AND EXISTS (SELECT 1 FROM ' + QUOTENAME(@Target) + N'.sys.columns tc
                  JOIN ' + QUOTENAME(@Target) + N'.sys.tables tt ON tt.object_id = tc.object_id
                  JOIN ' + QUOTENAME(@Target) + N'.sys.schemas ts ON ts.schema_id = tt.schema_id
                  WHERE tt.name = @table AND ts.name = @schema AND tc.name = c.name);';
  EXEC sp_executesql @sql, N'@objid int, @table sysname, @schema sysname, @cols nvarchar(max) OUTPUT', @objid, @table, @schema, @cols OUTPUT;

  IF @cols IS NULL
  BEGIN
    UPDATE #tables SET status = N'im Ziel nicht vorhanden - uebersprungen' WHERE object_id = @objid;
    PRINT @table + N': im Ziel nicht vorhanden, uebersprungen';
  END
  ELSE
  BEGIN
    BEGIN TRY
      IF @ClearTarget = 1
      BEGIN
        SET @sql = N'DELETE FROM ' + @qtgt + N';';
        EXEC (@sql);
      END

      IF @pkint = 1
      BEGIN
        SET @batch = CASE WHEN @blob = 1 THEN @BlobBatchRows ELSE @BatchRows END;
        SET @sql = N'SELECT @lo = ISNULL(MIN(' + QUOTENAME(@pk) + N'), 0) - 1, @maxid = ISNULL(MAX(' + QUOTENAME(@pk) + N'), 0) FROM ' + @qsrc + N';';
        EXEC sp_executesql @sql, N'@lo bigint OUTPUT, @maxid bigint OUTPUT', @lo OUTPUT, @maxid OUTPUT;

        WHILE @lo < @maxid
        BEGIN
          -- obere Grenze = der @batch-te Schluessel nach @lo (Luecken in den IDs sind damit egal)
          SET @hi = NULL;
          SET @sql = N'SELECT @hi = MAX(k) FROM (SELECT TOP (@batch) ' + QUOTENAME(@pk) + N' AS k FROM ' + @qsrc + N' WHERE ' + QUOTENAME(@pk) + N' > @lo ORDER BY k) x;';
          EXEC sp_executesql @sql, N'@batch int, @lo bigint, @hi bigint OUTPUT', @batch, @lo, @hi OUTPUT;
          IF @hi IS NULL BREAK;

          SET @sql = N'';
          IF @ident = 1 SET @sql = N'SET IDENTITY_INSERT ' + @qtgt + N' ON; ';
          SET @sql = @sql + N'INSERT INTO ' + @qtgt + N' (' + @cols + N') SELECT ' + @cols + N' FROM ' + @qsrc
                   + N' WHERE ' + QUOTENAME(@pk) + N' > @lo AND ' + QUOTENAME(@pk) + N' <= @hi; SET @n = @@ROWCOUNT;';
          IF @ident = 1 SET @sql = @sql + N' SET IDENTITY_INSERT ' + @qtgt + N' OFF;';
          EXEC sp_executesql @sql, N'@lo bigint, @hi bigint, @n bigint OUTPUT', @lo, @hi, @n OUTPUT;

          SET @copied = @copied + @n;
          SET @lo = @hi;

          -- Log der Ziel-DB im SIMPLE-Modus klein halten
          SET @sql = N'USE ' + QUOTENAME(@Target) + N'; CHECKPOINT;';
          EXEC (@sql);

          IF @blob = 1 RAISERROR(N'%s: %I64d Zeilen ...', 0, 1, @table, @copied) WITH NOWAIT;
        END
      END
      ELSE
      BEGIN
        -- kein einspaltiger Integer-PK (z.B. AspNetUserRoles, AspNetUserLogins): in einem Rutsch
        SET @sql = N'';
        IF @ident = 1 SET @sql = N'SET IDENTITY_INSERT ' + @qtgt + N' ON; ';
        SET @sql = @sql + N'INSERT INTO ' + @qtgt + N' (' + @cols + N') SELECT ' + @cols + N' FROM ' + @qsrc + N'; SET @n = @@ROWCOUNT;';
        IF @ident = 1 SET @sql = @sql + N' SET IDENTITY_INSERT ' + @qtgt + N' OFF;';
        EXEC sp_executesql @sql, N'@n bigint OUTPUT', @n OUTPUT;
        SET @copied = @n;
      END

      UPDATE #tables SET copied = @copied, status = N'OK' WHERE object_id = @objid;
      PRINT @table + N': ' + CAST(@copied AS nvarchar(20)) + N' von ' + CAST(@srcrows AS nvarchar(20)) + N' Zeilen kopiert';
    END TRY
    BEGIN CATCH
      SET @msg = N'Fehler ' + CAST(ERROR_NUMBER() AS nvarchar(10)) + N': ' + ERROR_MESSAGE();
      UPDATE #tables SET copied = @copied, status = @msg WHERE object_id = @objid;
      PRINT @table + N': ' + @msg + N' (bis dahin ' + CAST(@copied AS nvarchar(20)) + N' Zeilen)';
      IF @ident = 1
      BEGIN
        SET @sql = N'SET IDENTITY_INSERT ' + @qtgt + N' OFF;';
        BEGIN TRY EXEC (@sql); END TRY BEGIN CATCH SET @n = 0; END CATCH;
      END
    END CATCH
  END

  FETCH NEXT FROM tcur INTO @schema, @table, @objid, @pk, @pkint, @ident, @blob, @srcrows;
END
CLOSE tcur; DEALLOCATE tcur;

/* ---- 4. Fremdschluessel wieder aktivieren (WITH CHECK), Waisen melden ---- */
PRINT N'';
PRINT N'--- Fremdschluessel im Ziel aktivieren ---';
IF OBJECT_ID('tempdb..#fks') IS NOT NULL DROP TABLE #fks;
CREATE TABLE #fks (fk_name sysname, parent_q nvarchar(300), parent_column sysname, ref_q nvarchar(300), ref_column sysname);
SET @sql = N'
SELECT fk.name, QUOTENAME(ps.name) + N''.'' + QUOTENAME(pt.name), pc.name, QUOTENAME(rs.name) + N''.'' + QUOTENAME(rt.name), rc.name
FROM ' + QUOTENAME(@Target) + N'.sys.foreign_keys fk
JOIN ' + QUOTENAME(@Target) + N'.sys.tables pt ON pt.object_id = fk.parent_object_id
JOIN ' + QUOTENAME(@Target) + N'.sys.schemas ps ON ps.schema_id = pt.schema_id
JOIN ' + QUOTENAME(@Target) + N'.sys.tables rt ON rt.object_id = fk.referenced_object_id
JOIN ' + QUOTENAME(@Target) + N'.sys.schemas rs ON rs.schema_id = rt.schema_id
JOIN ' + QUOTENAME(@Target) + N'.sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
JOIN ' + QUOTENAME(@Target) + N'.sys.columns pc ON pc.object_id = fkc.parent_object_id AND pc.column_id = fkc.parent_column_id
JOIN ' + QUOTENAME(@Target) + N'.sys.columns rc ON rc.object_id = fkc.referenced_object_id AND rc.column_id = fkc.referenced_column_id;';
INSERT #fks EXEC sp_executesql @sql;

DECLARE @fk sysname, @pq nvarchar(300), @pcol sysname, @rq nvarchar(300), @rcol sysname, @cnt bigint;
DECLARE fkcur CURSOR LOCAL FAST_FORWARD FOR
  SELECT fk_name, parent_q, parent_column, ref_q, ref_column FROM #fks ORDER BY parent_q, fk_name;
OPEN fkcur;
FETCH NEXT FROM fkcur INTO @fk, @pq, @pcol, @rq, @rcol;
WHILE @@FETCH_STATUS = 0
BEGIN
  BEGIN TRY
    SET @sql = N'ALTER TABLE ' + QUOTENAME(@Target) + N'.' + @pq + N' WITH CHECK CHECK CONSTRAINT ' + QUOTENAME(@fk) + N';';
    EXEC (@sql);
  END TRY
  BEGIN CATCH
    -- Waisen: Zeilen, deren Verweis ins Leere geht (z.B. CaseFiles zu einem verlorenen Case)
    SET @cnt = NULL;
    SET @sql = N'SELECT @cnt = COUNT(*) FROM ' + QUOTENAME(@Target) + N'.' + @pq + N' p WHERE p.' + QUOTENAME(@pcol) + N' IS NOT NULL'
             + N' AND NOT EXISTS (SELECT 1 FROM ' + QUOTENAME(@Target) + N'.' + @rq + N' r WHERE r.' + QUOTENAME(@rcol) + N' = p.' + QUOTENAME(@pcol) + N');';
    EXEC sp_executesql @sql, N'@cnt bigint OUTPUT', @cnt OUTPUT;
    PRINT @fk + N': nicht aktivierbar - ' + CAST(ISNULL(@cnt, -1) AS nvarchar(20)) + N' Waisen in ' + @pq + N'.' + @pcol + N' ohne Treffer in ' + @rq + N'.' + @rcol;
    PRINT N'   anzeigen: SELECT * FROM ' + QUOTENAME(@Target) + N'.' + @pq + N' p WHERE p.' + QUOTENAME(@pcol) + N' IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ' + QUOTENAME(@Target) + N'.' + @rq + N' r WHERE r.' + QUOTENAME(@rcol) + N' = p.' + QUOTENAME(@pcol) + N');';
    PRINT N'   danach:   ALTER TABLE ' + QUOTENAME(@Target) + N'.' + @pq + N' WITH CHECK CHECK CONSTRAINT ' + QUOTENAME(@fk) + N';';
  END CATCH
  FETCH NEXT FROM fkcur INTO @fk, @pq, @pcol, @rq, @rcol;
END
CLOSE fkcur; DEALLOCATE fkcur;

/* ---- 5. Zusammenfassung --------------------------------------------------- */
SET @sql = N'UPDATE t SET tgt_rows = x.rows_ FROM #tables t JOIN (
  SELECT tt.name AS table_name, SUM(p.rows) AS rows_
  FROM ' + QUOTENAME(@Target) + N'.sys.tables tt
  JOIN ' + QUOTENAME(@Target) + N'.sys.partitions p ON p.object_id = tt.object_id AND p.index_id IN (0,1)
  GROUP BY tt.name) x ON x.table_name = t.table_name;';
EXEC (@sql);

PRINT N'';
PRINT N'--- Zusammenfassung (fehlend > 0 = Zeilen, die durch die Reparatur verloren sind) ---';
SELECT table_name, src_rows, copied, tgt_rows, src_rows - ISNULL(tgt_rows, 0) AS fehlend, status
FROM #tables ORDER BY table_name;
