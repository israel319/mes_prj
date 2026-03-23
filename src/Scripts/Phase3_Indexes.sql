-- =====================================================
-- PHASE 3 : Ajout des index de performance
-- Base : AppDev_KCCMaterialFlow_DB_Dev
-- Date : 2026-03-10
-- =====================================================
--
-- Ce script ajoute les 16 index recommandés par l'audit.
-- Tous les index sont NON-CLUSTERED et n'affectent pas
-- la logique métier. Seule la performance est impactée.
--
-- PRÉREQUIS : Phase1_PK_Rename.sql doit être exécuté d'abord
-- =====================================================

USE [AppDev_KCCMaterialFlow_DB_Dev];
GO

-- =====================================================
-- DIAGNOSTIC : Index existants
-- =====================================================
PRINT '=== DIAGNOSTIC : Index existants ===';
SELECT 
    t.name AS Table_Name,
    i.name AS Index_Name,
    i.type_desc AS Index_Type,
    STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS Columns
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.schema_id = SCHEMA_ID('dbo')
  AND t.name LIKE 'T_%'
  AND i.type > 0  -- Exclure heap
GROUP BY t.name, i.name, i.type_desc
ORDER BY t.name, i.name;
GO

-- =====================================================
-- INDEX 1-3 : Table T_Bons (BonEntree)
-- =====================================================

-- Filtrage par statut (page liste des bons)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Bons_StatutActuel')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Bons_StatutActuel]
    ON [dbo].[T_Bons] ([StatutActuel])
    INCLUDE ([NumeroReference], [DateCreation], [Provenance], [Destination]);
    PRINT '✅ IX_Bons_StatutActuel créé';
END
ELSE PRINT '⏭️ IX_Bons_StatutActuel existe déjà';
GO

-- Recherche par contrat (FK lookup)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Bons_ContratId' AND object_id = OBJECT_ID('dbo.T_Bons'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Bons_ContratId]
    ON [dbo].[T_Bons] ([ContratId])
    WHERE [ContratId] IS NOT NULL;
    PRINT '✅ IX_Bons_ContratId créé';
END
ELSE PRINT '⏭️ IX_Bons_ContratId existe déjà';
GO

-- Tri chronologique (page liste)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Bons_DateCreation')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Bons_DateCreation]
    ON [dbo].[T_Bons] ([DateCreation] DESC)
    INCLUDE ([NumeroReference], [StatutActuel]);
    PRINT '✅ IX_Bons_DateCreation créé';
END
ELSE PRINT '⏭️ IX_Bons_DateCreation existe déjà';
GO

-- =====================================================
-- INDEX 4-6 : Table T_BonsSortie
-- =====================================================

-- Filtrage par statut
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_BonsSortie_StatutActuel')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_BonsSortie_StatutActuel]
    ON [dbo].[T_BonsSortie] ([StatutActuel])
    INCLUDE ([NumeroReference], [DateCreation], [TypeSortie], [Provenance], [Destination]);
    PRINT '✅ IX_BonsSortie_StatutActuel créé';
END
ELSE PRINT '⏭️ IX_BonsSortie_StatutActuel existe déjà';
GO

-- "Mes bons" — requête très fréquente
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_BonsSortie_CreatedByLogin')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_BonsSortie_CreatedByLogin]
    ON [dbo].[T_BonsSortie] ([CreatedByLogin])
    INCLUDE ([NumeroReference], [StatutActuel], [DateCreation], [TypeSortie]);
    PRINT '✅ IX_BonsSortie_CreatedByLogin créé';
END
ELSE PRINT '⏭️ IX_BonsSortie_CreatedByLogin existe déjà';
GO

-- Tri chronologique
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_BonsSortie_DateCreation')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_BonsSortie_DateCreation]
    ON [dbo].[T_BonsSortie] ([DateCreation] DESC)
    INCLUDE ([NumeroReference], [StatutActuel], [TypeSortie]);
    PRINT '✅ IX_BonsSortie_DateCreation créé';
END
ELSE PRINT '⏭️ IX_BonsSortie_DateCreation existe déjà';
GO

-- =====================================================
-- INDEX 7-8 : Table T_MaterielsSortie
-- =====================================================

-- Traçabilité entrée → sortie
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MaterielsSortie_MaterielEntreeId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MaterielsSortie_MaterielEntreeId]
    ON [dbo].[T_MaterielsSortie] ([MaterielEntreeId])
    WHERE [MaterielEntreeId] IS NOT NULL;
    PRINT '✅ IX_MaterielsSortie_MaterielEntreeId créé';
END
ELSE PRINT '⏭️ IX_MaterielsSortie_MaterielEntreeId existe déjà';
GO

-- Recherche par BEM source
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MaterielsSortie_BonEntreeId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MaterielsSortie_BonEntreeId]
    ON [dbo].[T_MaterielsSortie] ([BonEntreeId])
    WHERE [BonEntreeId] IS NOT NULL;
    PRINT '✅ IX_MaterielsSortie_BonEntreeId créé';
END
ELSE PRINT '⏭️ IX_MaterielsSortie_BonEntreeId existe déjà';
GO

-- =====================================================
-- INDEX 9-11 : Table T_ScanEvenements
-- =====================================================

-- Historique scans d'un bon (composite BonId + TypeBon)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ScanEvenements_BonId_TypeBon')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ScanEvenements_BonId_TypeBon]
    ON [dbo].[T_ScanEvenements] ([BonId], [TypeBon])
    INCLUDE ([DateHeureScan], [StatutScan], [BarriereId]);
    PRINT '✅ IX_ScanEvenements_BonId_TypeBon créé';
END
ELSE PRINT '⏭️ IX_ScanEvenements_BonId_TypeBon existe déjà';
GO

-- Recherche chronologique
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ScanEvenements_DateHeureScan')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ScanEvenements_DateHeureScan]
    ON [dbo].[T_ScanEvenements] ([DateHeureScan] DESC)
    INCLUDE ([BonId], [TypeBon], [StatutScan]);
    PRINT '✅ IX_ScanEvenements_DateHeureScan créé';
END
ELSE PRINT '⏭️ IX_ScanEvenements_DateHeureScan existe déjà';
GO

-- Activité par barrière
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ScanEvenements_BarriereId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ScanEvenements_BarriereId]
    ON [dbo].[T_ScanEvenements] ([BarriereId])
    INCLUDE ([DateHeureScan], [StatutScan], [BonId]);
    PRINT '✅ IX_ScanEvenements_BarriereId créé';
END
ELSE PRINT '⏭️ IX_ScanEvenements_BarriereId existe déjà';
GO

-- =====================================================
-- INDEX 12-13 : Table T_Anomalies
-- =====================================================

-- Anomalies d'un bon (composite)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Anomalies_BonId_TypeBon')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Anomalies_BonId_TypeBon]
    ON [dbo].[T_Anomalies] ([BonId], [TypeBon])
    INCLUDE ([TypeAnomalie], [NiveauGravite], [EstTraitee]);
    PRINT '✅ IX_Anomalies_BonId_TypeBon créé';
END
ELSE PRINT '⏭️ IX_Anomalies_BonId_TypeBon existe déjà';
GO

-- Anomalies non traitées (filtre rapide)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Anomalies_EstTraitee')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Anomalies_EstTraitee]
    ON [dbo].[T_Anomalies] ([EstTraitee])
    WHERE [EstTraitee] = 0;
    PRINT '✅ IX_Anomalies_EstTraitee créé (filtré)';
END
ELSE PRINT '⏭️ IX_Anomalies_EstTraitee existe déjà';
GO

-- =====================================================
-- INDEX 14-16 : Table T_AuditLogs
-- =====================================================

-- Recherche chronologique
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AuditLogs_DateAction')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_DateAction]
    ON [dbo].[T_AuditLogs] ([DateAction] DESC)
    INCLUDE ([Action], [UtilisateurLogin], [Categorie]);
    PRINT '✅ IX_AuditLogs_DateAction créé';
END
ELSE PRINT '⏭️ IX_AuditLogs_DateAction existe déjà';
GO

-- Audit par utilisateur
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AuditLogs_UtilisateurLogin')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_UtilisateurLogin]
    ON [dbo].[T_AuditLogs] ([UtilisateurLogin])
    INCLUDE ([DateAction], [Action], [Categorie]);
    PRINT '✅ IX_AuditLogs_UtilisateurLogin créé';
END
ELSE PRINT '⏭️ IX_AuditLogs_UtilisateurLogin existe déjà';
GO

-- Audit par module/catégorie
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AuditLogs_Categorie_EntiteType')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_Categorie_EntiteType]
    ON [dbo].[T_AuditLogs] ([Categorie], [EntiteType])
    INCLUDE ([DateAction], [Action]);
    PRINT '✅ IX_AuditLogs_Categorie_EntiteType créé';
END
ELSE PRINT '⏭️ IX_AuditLogs_Categorie_EntiteType existe déjà';
GO

-- =====================================================
-- VÉRIFICATION FINALE
-- =====================================================
PRINT '';
PRINT '=== VÉRIFICATION : Tous les index après modifications ===';
SELECT 
    t.name AS Table_Name,
    i.name AS Index_Name,
    i.type_desc AS Index_Type,
    STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS Columns
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.schema_id = SCHEMA_ID('dbo')
  AND t.name LIKE 'T_%'
  AND i.type > 0
  AND i.name LIKE 'IX_%'
GROUP BY t.name, i.name, i.type_desc
ORDER BY t.name, i.name;
GO

PRINT '';
PRINT '=== Phase 3 terminée : 16 index ajoutés ===';
GO
