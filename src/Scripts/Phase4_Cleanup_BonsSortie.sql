-- =====================================================
-- PHASE 4 : OBSOLÈTE — NE PAS EXÉCUTER
-- Base : AppDev_KCCMaterialFlow_DB_Dev
-- Date : 2026-03-10
-- =====================================================
--
-- ⚠️ CE SCRIPT EST OBSOLÈTE
-- Les colonnes DepartementDestination et NomReceveur
-- N'EXISTENT PAS dans la base de données réelle.
-- Elles avaient été identifiées par erreur depuis le code.
--
-- Remplacé par : Phase5_Cleanup_Orphans.sql
-- =====================================================
--
-- ANCIEN CONTENU (pour référence) :
-- Ce script supprime les colonnes qui ne sont plus mappées
-- par aucune entité/configuration EF Core.
--
-- ⚠️ ATTENTION : Cette opération est IRRÉVERSIBLE.
-- Faites une sauvegarde AVANT d'exécuter ce script.
--
-- PRÉREQUIS : Phase1_PK_Rename.sql doit être exécuté d'abord
-- =====================================================

USE [AppDev_KCCMaterialFlow_DB_Dev];
GO

-- =====================================================
-- DIAGNOSTIC : Colonnes orphelines dans T_BonsSortie
-- (colonnes en DB mais PAS dans les entités C#)
-- =====================================================
PRINT '=== DIAGNOSTIC : Colonnes de T_BonsSortie ===';
SELECT 
    c.name AS Colonne,
    t.name AS Type,
    c.max_length AS MaxLength,
    c.is_nullable AS Nullable,
    CASE 
        WHEN c.name IN ('DepartementDestination', 'NomReceveur') 
        THEN '❌ ORPHELINE — pas mappée par EF'
        ELSE '✅ Utilisée par EF/C#'
    END AS Statut
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.T_BonsSortie')
ORDER BY c.column_id;
GO

-- =====================================================
-- DIAGNOSTIC : Données existantes dans les colonnes orphelines
-- =====================================================
PRINT '=== Données dans DepartementDestination ===';
SELECT COUNT(*) AS Total,
       COUNT(DepartementDestination) AS NonNull,
       COUNT(DISTINCT DepartementDestination) AS Distincts
FROM [dbo].[T_BonsSortie]
WHERE DepartementDestination IS NOT NULL;

PRINT '=== Données dans NomReceveur ===';
SELECT COUNT(*) AS Total,
       COUNT(NomReceveur) AS NonNull,
       COUNT(DISTINCT NomReceveur) AS Distincts
FROM [dbo].[T_BonsSortie]
WHERE NomReceveur IS NOT NULL;
GO

-- =====================================================
-- SUPPRESSION : Colonnes orphelines 
-- (plus mappées par aucune entité C# / config EF)
-- =====================================================

-- 1. DepartementDestination (anciennement BonSortieInterne)
--    Supprimée des entités C# et du ModelSnapshot EF.
--    Les DTOs l'ignorent via opt.Ignore().
PRINT 'Suppression colonne orpheline : DepartementDestination';
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_BonsSortie') AND name = 'DepartementDestination')
BEGIN
    ALTER TABLE [dbo].[T_BonsSortie] DROP COLUMN [DepartementDestination];
    PRINT '  ✅ Colonne DepartementDestination supprimée';
END
ELSE
    PRINT '  ⏭️ Colonne déjà absente';
GO

-- 2. NomReceveur (anciennement BonSortieInterne)
--    Supprimée des entités C# et du ModelSnapshot EF.
--    Les DTOs l'ignorent via opt.Ignore().
PRINT 'Suppression colonne orpheline : NomReceveur';
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_BonsSortie') AND name = 'NomReceveur')
BEGIN
    ALTER TABLE [dbo].[T_BonsSortie] DROP COLUMN [NomReceveur];
    PRINT '  ✅ Colonne NomReceveur supprimée';
END
ELSE
    PRINT '  ⏭️ Colonne déjà absente';
GO

-- =====================================================
-- NOTE : Colonnes NON supprimées (encore utilisées)
-- =====================================================
-- Les colonnes suivantes ont été identifiées comme 
-- potentiellement redondantes dans l'audit, MAIS elles
-- sont encore ACTIVEMENT UTILISÉES par le code C# :
--
-- ┌──────────────────────────┬──────────────────────────────┐
-- │ Colonne                  │ Utilisée par                 │
-- ├──────────────────────────┼──────────────────────────────┤
-- │ Description              │ BonSortie entity, Service    │
-- │ NomDemandeur             │ BonSortie, search, alerts    │
-- │ FonctionDemandeur        │ BonSortie, validators        │
-- │ DepartementDemandeur     │ BonSortie, filters, alerts   │
-- │ NomDestinataire          │ BonSortieExterne, validators │
-- │ AdresseDestination       │ BonSortieExterne, service    │
-- │ TelephoneChauffeur       │ BonSortieExterne, validators │
-- │ TypeMaterielInterne      │ BonSortieInterne.TypeMateriel│
-- │ DepartementOrigine       │ BonSortieInterne, service    │
-- │ FonctionReceveur         │ BonSortieInterne, validators │
-- │ EmailReceveur            │ BonSortieInterne, service    │
-- │ LocalisationDestination  │ BonSortieInterne, service    │
-- │ DateTransfertPrevue      │ BonSortieInterne, validators │
-- │ DateTransfertEffective   │ BonSortieInterne, service    │
-- └──────────────────────────┴──────────────────────────────┘
--
-- Pour normaliser ces colonnes (Phase future) :
-- 1. Modifier les entités C# pour supprimer les propriétés
-- 2. Adapter les services pour charger via JOIN/navigation
-- 3. Mettre à jour les DTOs et mapping profiles
-- 4. PUIS seulement supprimer les colonnes en DB

-- =====================================================
-- VÉRIFICATION FINALE
-- =====================================================
PRINT '';
PRINT '=== Colonnes T_BonsSortie après nettoyage ===';
SELECT 
    c.name AS Colonne,
    t.name AS Type,
    c.max_length AS MaxLength,
    c.is_nullable AS Nullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.T_BonsSortie')
ORDER BY c.column_id;
GO

PRINT '';
PRINT '=== Phase 4 terminée : 2 colonnes orphelines supprimées ===';
GO
