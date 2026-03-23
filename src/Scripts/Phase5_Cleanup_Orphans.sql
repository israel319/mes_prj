-- =====================================================
-- PHASE 5 : Nettoyage des tables/colonnes orphelines
-- Base : AppDev_KCCMaterialFlow_DB_Dev
-- Date : 2026-03-10
-- =====================================================
--
-- Ce script supprime :
--   1. T_CategoriesSortieShared (table entière, pas de mapping EF)
--   2. T_BonEntreeHistory.BonIdBon (colonne shadow, toujours NULL)
--   3. T_BonsSortie.BonSortieInterne_BonEntreeAssocieId (colonne shadow, toujours NULL)
--   4. T_ScanEvenements.AnomalieIdAnomalie (colonne shadow, non utilisée)
--
-- ⚠️ IRRÉVERSIBLE — Sauvegarde obligatoire avant exécution
-- PRÉREQUIS : Phase1_PK_Rename.sql exécuté en premier
-- =====================================================

USE [AppDev_KCCMaterialFlow_DB_Dev];
GO

SET XACT_ABORT ON;
GO

-- =====================================================
-- DIAGNOSTIC AVANT NETTOYAGE
-- =====================================================
PRINT '============================================';
PRINT '  PHASE 5 : Nettoyage des orphelins';
PRINT '============================================';
PRINT '';

-- Vérifier que les colonnes sont bien vides
PRINT '=== Vérification : colonnes à supprimer ===';

PRINT '1. T_BonEntreeHistory.BonIdBon :';
SELECT COUNT(*) AS Total_Lignes,
       SUM(CASE WHEN BonIdBon IS NOT NULL THEN 1 ELSE 0 END) AS NonNull
FROM [dbo].[T_BonEntreeHistory];

PRINT '2. T_BonsSortie.BonSortieInterne_BonEntreeAssocieId :';
SELECT COUNT(*) AS Total_Lignes,
       SUM(CASE WHEN BonSortieInterne_BonEntreeAssocieId IS NOT NULL THEN 1 ELSE 0 END) AS NonNull
FROM [dbo].[T_BonsSortie];

PRINT '3. T_CategoriesSortieShared :';
SELECT COUNT(*) AS Lignes_A_Supprimer FROM [dbo].[T_CategoriesSortieShared];

PRINT '4. T_ScanEvenements.AnomalieIdAnomalie :';
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_ScanEvenements') AND name = 'AnomalieIdAnomalie')
    SELECT COUNT(*) AS Total_Lignes,
           SUM(CASE WHEN AnomalieIdAnomalie IS NOT NULL THEN 1 ELSE 0 END) AS NonNull
    FROM [dbo].[T_ScanEvenements];
ELSE
    PRINT '  Colonne absente (déjà supprimée)';
GO

PRINT '';
PRINT '=== Vérifiez les résultats ci-dessus ===';
PRINT '=== Les colonnes NonNull doivent être 0 ===';
PRINT '';
PRINT 'Si tout est OK, continuez ci-dessous.';
GO

-- =====================================================
-- ÉTAPE 1 : Supprimer T_CategoriesSortieShared
-- (7 lignes, aucun mapping EF, aucune FK)
-- =====================================================
PRINT '';
PRINT '--- Étape 1 : Supprimer T_CategoriesSortieShared ---';

-- Sauvegarder les données avant suppression (au cas où)
PRINT 'Sauvegarde des données :';
SELECT * FROM [dbo].[T_CategoriesSortieShared] ORDER BY IdCategorie;

-- Supprimer les index
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CategoriesSortie_Code' AND object_id = OBJECT_ID('dbo.T_CategoriesSortieShared'))
BEGIN
    DROP INDEX [IX_CategoriesSortie_Code] ON [dbo].[T_CategoriesSortieShared];
    PRINT '  Index IX_CategoriesSortie_Code supprimé';
END;

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_CategoriesSortie_Ordre' AND object_id = OBJECT_ID('dbo.T_CategoriesSortieShared'))
BEGIN
    DROP INDEX [IX_CategoriesSortie_Ordre] ON [dbo].[T_CategoriesSortieShared];
    PRINT '  Index IX_CategoriesSortie_Ordre supprimé';
END;

-- Supprimer la table
IF OBJECT_ID('dbo.T_CategoriesSortieShared', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[T_CategoriesSortieShared];
    PRINT '  ✅ Table T_CategoriesSortieShared supprimée';
END
ELSE
    PRINT '  ⏭️ Table déjà absente';
GO

-- =====================================================
-- ÉTAPE 2 : Supprimer T_BonEntreeHistory.BonIdBon
-- (FK orpheline + index + colonne, toujours NULL)
-- =====================================================
PRINT '';
PRINT '--- Étape 2 : Supprimer T_BonEntreeHistory.BonIdBon ---';

-- 2a. Supprimer la FK orpheline
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_T_BonEntreeHistory_T_Bons_BonIdBon')
BEGIN
    ALTER TABLE [dbo].[T_BonEntreeHistory]
        DROP CONSTRAINT [FK_T_BonEntreeHistory_T_Bons_BonIdBon];
    PRINT '  FK FK_T_BonEntreeHistory_T_Bons_BonIdBon supprimée';
END;

-- 2b. Supprimer l'index orphelin
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_T_BonEntreeHistory_BonIdBon' AND object_id = OBJECT_ID('dbo.T_BonEntreeHistory'))
BEGIN
    DROP INDEX [IX_T_BonEntreeHistory_BonIdBon] ON [dbo].[T_BonEntreeHistory];
    PRINT '  Index IX_T_BonEntreeHistory_BonIdBon supprimé';
END;

-- 2c. Supprimer la colonne
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_BonEntreeHistory') AND name = 'BonIdBon')
BEGIN
    ALTER TABLE [dbo].[T_BonEntreeHistory]
        DROP COLUMN [BonIdBon];
    PRINT '  ✅ Colonne BonIdBon supprimée de T_BonEntreeHistory';
END
ELSE
    PRINT '  ⏭️ Colonne déjà absente';
GO

-- =====================================================
-- ÉTAPE 3 : Supprimer T_BonsSortie.BonSortieInterne_BonEntreeAssocieId
-- (colonne shadow TPH, toujours NULL, doublon de BonEntreeAssocieId)
-- =====================================================
PRINT '';
PRINT '--- Étape 3 : Supprimer T_BonsSortie.BonSortieInterne_BonEntreeAssocieId ---';

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_BonsSortie') AND name = 'BonSortieInterne_BonEntreeAssocieId')
BEGIN
    ALTER TABLE [dbo].[T_BonsSortie]
        DROP COLUMN [BonSortieInterne_BonEntreeAssocieId];
    PRINT '  ✅ Colonne BonSortieInterne_BonEntreeAssocieId supprimée de T_BonsSortie';
END
ELSE
    PRINT '  ⏭️ Colonne déjà absente';
GO

-- =====================================================
-- ÉTAPE 4 : Supprimer T_ScanEvenements.AnomalieIdAnomalie
-- (shadow FK one-to-one, non configurée, table vide)
-- =====================================================
PRINT '';
PRINT '--- Étape 4 : Supprimer T_ScanEvenements.AnomalieIdAnomalie ---';

-- 4a. Supprimer la FK
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_T_ScanEvenements_T_Anomalies_AnomalieIdAnomalie')
BEGIN
    ALTER TABLE [dbo].[T_ScanEvenements]
        DROP CONSTRAINT [FK_T_ScanEvenements_T_Anomalies_AnomalieIdAnomalie];
    PRINT '  FK FK_T_ScanEvenements_T_Anomalies_AnomalieIdAnomalie supprimée';
END;

-- 4b. Supprimer l'index
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_T_ScanEvenements_AnomalieIdAnomalie' AND object_id = OBJECT_ID('dbo.T_ScanEvenements'))
BEGIN
    DROP INDEX [IX_T_ScanEvenements_AnomalieIdAnomalie] ON [dbo].[T_ScanEvenements];
    PRINT '  Index IX_T_ScanEvenements_AnomalieIdAnomalie supprimé';
END;

-- 4c. Supprimer la colonne
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_ScanEvenements') AND name = 'AnomalieIdAnomalie')
BEGIN
    ALTER TABLE [dbo].[T_ScanEvenements]
        DROP COLUMN [AnomalieIdAnomalie];
    PRINT '  ✅ Colonne AnomalieIdAnomalie supprimée de T_ScanEvenements';
END
ELSE
    PRINT '  ⏭️ Colonne déjà absente';
GO

-- =====================================================
-- VÉRIFICATION FINALE
-- =====================================================
PRINT '';
PRINT '============================================';
PRINT '  VÉRIFICATION FINALE';
PRINT '============================================';

-- La table T_CategoriesSortieShared ne doit plus exister
PRINT '1. T_CategoriesSortieShared :';
IF OBJECT_ID('dbo.T_CategoriesSortieShared', 'U') IS NULL
    PRINT '  ✅ Table supprimée';
ELSE
    PRINT '  ❌ TABLE ENCORE PRÉSENTE !';

-- BonIdBon ne doit plus exister
PRINT '2. T_BonEntreeHistory.BonIdBon :';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_BonEntreeHistory') AND name = 'BonIdBon')
    PRINT '  ✅ Colonne supprimée';
ELSE
    PRINT '  ❌ COLONNE ENCORE PRÉSENTE !';

-- BonSortieInterne_BonEntreeAssocieId ne doit plus exister
PRINT '3. T_BonsSortie.BonSortieInterne_BonEntreeAssocieId :';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_BonsSortie') AND name = 'BonSortieInterne_BonEntreeAssocieId')
    PRINT '  ✅ Colonne supprimée';
ELSE
    PRINT '  ❌ COLONNE ENCORE PRÉSENTE !';

-- AnomalieIdAnomalie ne doit plus exister
PRINT '4. T_ScanEvenements.AnomalieIdAnomalie :';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_ScanEvenements') AND name = 'AnomalieIdAnomalie')
    PRINT '  ✅ Colonne supprimée';
ELSE
    PRINT '  ❌ COLONNE ENCORE PRÉSENTE !';

-- Nombre total de tables après nettoyage
PRINT '';
PRINT '5. Nombre de tables restantes :';
SELECT COUNT(*) AS Nombre_Tables
FROM sys.tables
WHERE name LIKE 'T_%';

PRINT '';
PRINT '============================================';
PRINT '  Phase 5 terminée : Orphelins nettoyés';
PRINT '============================================';
GO
