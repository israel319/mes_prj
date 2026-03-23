-- =====================================================
-- DIAGNOSTIC COMPLET DE LA BASE DE DONNÉES
-- Base : AppDev_KCCMaterialFlow_DB_Dev
-- Date : 2026-03-10
-- =====================================================
-- Exécutez ce script AVANT et APRÈS les phases de correction
-- pour comparer l'état de la base.
-- =====================================================

USE [AppDev_KCCMaterialFlow_DB_Dev];
GO

PRINT '╔══════════════════════════════════════════════════╗';
PRINT '║     DIAGNOSTIC COMPLET DE LA BASE DE DONNÉES    ║';
PRINT '╚══════════════════════════════════════════════════╝';
PRINT '';

-- =====================================================
-- 1. INVENTAIRE DES TABLES
-- =====================================================
PRINT '┌──────────────────────────────────────────────────┐';
PRINT '│ 1. INVENTAIRE DES TABLES                         │';
PRINT '└──────────────────────────────────────────────────┘';

SELECT 
    s.name AS [Schema],
    t.name AS [Table],
    p.rows AS [Lignes],
    (SELECT COUNT(*) FROM sys.columns c WHERE c.object_id = t.object_id) AS [Colonnes],
    (SELECT COUNT(*) FROM sys.indexes i WHERE i.object_id = t.object_id AND i.type > 0) AS [Index],
    (SELECT COUNT(*) FROM sys.foreign_keys fk WHERE fk.parent_object_id = t.object_id) AS [FK_Sortantes],
    (SELECT COUNT(*) FROM sys.foreign_keys fk WHERE fk.referenced_object_id = t.object_id) AS [FK_Entrantes]
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
INNER JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
WHERE t.name LIKE 'T_%' OR t.name = '__EFMigrationsHistory'
ORDER BY s.name, t.name;
GO

-- =====================================================
-- 2. COLONNES PK : Vérification du nommage
-- =====================================================
PRINT '';
PRINT '┌──────────────────────────────────────────────────┐';
PRINT '│ 2. CLÉS PRIMAIRES — Convention IdXxx             │';
PRINT '└──────────────────────────────────────────────────┘';

SELECT 
    t.name AS [Table],
    c.name AS [Colonne_PK],
    CASE 
        WHEN c.name = 'Id' THEN '⚠️ Id générique'
        WHEN c.name LIKE 'Id%' THEN '✅ IdXxx'
        ELSE '❓ Autre'
    END AS [Convention],
    ty.name AS [Type],
    c.is_identity AS [Identity]
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.is_primary_key = 1
  AND t.name LIKE 'T_%'
ORDER BY t.name;
GO

-- =====================================================
-- 3. CONTRAINTES FK
-- =====================================================
PRINT '';
PRINT '┌──────────────────────────────────────────────────┐';
PRINT '│ 3. CONTRAINTES FK EXISTANTES                     │';
PRINT '└──────────────────────────────────────────────────┘';

SELECT 
    fk.name AS [FK_Name],
    OBJECT_NAME(fk.parent_object_id) AS [Table_Enfant],
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS [Colonne_FK],
    OBJECT_NAME(fk.referenced_object_id) AS [Table_Parent],
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS [Colonne_PK],
    fk.delete_referential_action_desc AS [OnDelete]
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) LIKE 'T_%'
ORDER BY [Table_Enfant], [FK_Name];
GO

-- =====================================================
-- 4. FK MANQUANTES (colonnes *Id sans FK)
-- =====================================================
PRINT '';
PRINT '┌──────────────────────────────────────────────────┐';
PRINT '│ 4. COLONNES *Id SANS CONTRAINTE FK               │';
PRINT '└──────────────────────────────────────────────────┘';

SELECT 
    t.name AS [Table],
    c.name AS [Colonne_Id],
    '⚠️ Pas de FK' AS [Statut]
FROM sys.columns c
INNER JOIN sys.tables t ON c.object_id = t.object_id
WHERE t.name LIKE 'T_%'
  AND (c.name LIKE '%Id' OR c.name LIKE '%_id')
  AND c.name NOT IN (
      SELECT COL_NAME(fkc.parent_object_id, fkc.parent_column_id)
      FROM sys.foreign_key_columns fkc
      WHERE fkc.parent_object_id = t.object_id
  )
  -- Exclure les PK
  AND NOT EXISTS (
      SELECT 1 FROM sys.index_columns ic
      INNER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
      WHERE i.is_primary_key = 1 AND ic.object_id = t.object_id AND ic.column_id = c.column_id
  )
ORDER BY t.name, c.name;
GO

-- =====================================================
-- 5. INDEX
-- =====================================================
PRINT '';
PRINT '┌──────────────────────────────────────────────────┐';
PRINT '│ 5. INDEX EXISTANTS                               │';
PRINT '└──────────────────────────────────────────────────┘';

SELECT 
    t.name AS [Table],
    i.name AS [Index],
    i.type_desc AS [Type],
    i.is_unique AS [Unique],
    STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS [Colonnes]
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name LIKE 'T_%'
  AND i.type > 0
GROUP BY t.name, i.name, i.type_desc, i.is_unique
ORDER BY t.name, i.name;
GO

-- =====================================================
-- 6. TABLES SANS FK (isolées)
-- =====================================================
PRINT '';
PRINT '┌──────────────────────────────────────────────────┐';
PRINT '│ 6. TABLES ISOLÉES (aucune FK entrante/sortante)  │';
PRINT '└──────────────────────────────────────────────────┘';

SELECT t.name AS [Table_Isolee]
FROM sys.tables t
WHERE t.name LIKE 'T_%'
  AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.parent_object_id = t.object_id)
  AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.referenced_object_id = t.object_id)
ORDER BY t.name;
GO

-- =====================================================
-- 7. MIGRATIONS EF APPLIQUÉES
-- =====================================================
PRINT '';
PRINT '┌──────────────────────────────────────────────────┐';
PRINT '│ 7. MIGRATIONS EF CORE APPLIQUÉES                 │';
PRINT '└──────────────────────────────────────────────────┘';

SELECT MigrationId, ProductVersion
FROM [__EFMigrationsHistory]
ORDER BY MigrationId;
GO

-- =====================================================
-- 8. RÉSUMÉ
-- =====================================================
PRINT '';
PRINT '┌──────────────────────────────────────────────────┐';
PRINT '│ 8. RÉSUMÉ                                        │';
PRINT '└──────────────────────────────────────────────────┘';

DECLARE @nbTables INT, @nbFK INT, @nbIndex INT, @nbPKId INT;

SELECT @nbTables = COUNT(*) FROM sys.tables WHERE name LIKE 'T_%';
SELECT @nbFK = COUNT(*) FROM sys.foreign_keys WHERE OBJECT_NAME(parent_object_id) LIKE 'T_%';
SELECT @nbIndex = COUNT(*) FROM sys.indexes i INNER JOIN sys.tables t ON i.object_id = t.object_id WHERE t.name LIKE 'T_%' AND i.type > 0 AND i.name LIKE 'IX_%';

SELECT @nbPKId = COUNT(*) 
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.is_primary_key = 1 AND t.name LIKE 'T_%' AND c.name = 'Id';

PRINT 'Tables T_* totales     : ' + CAST(@nbTables AS VARCHAR);
PRINT 'FK totales             : ' + CAST(@nbFK AS VARCHAR);
PRINT 'Index IX_* totaux      : ' + CAST(@nbIndex AS VARCHAR);
PRINT 'PK encore "Id" génér.  : ' + CAST(@nbPKId AS VARCHAR);
GO

PRINT '';
PRINT '=== Diagnostic terminé ===';
GO
