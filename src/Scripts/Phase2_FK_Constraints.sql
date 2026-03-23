-- =====================================================
-- PHASE 2 : Ajout des contraintes FK manquantes
-- Base : AppDev_KCCMaterialFlow_DB_Dev
-- Date : 2026-03-10
-- =====================================================
-- 
-- Ce script ajoute les FK identifiées comme manquantes 
-- dans l'audit de la base de données.
--
-- PRÉREQUIS : Phase1_PK_Rename.sql doit être exécuté d'abord
-- =====================================================

USE [AppDev_KCCMaterialFlow_DB_Dev];
GO

-- =====================================================
-- DIAGNOSTIC : Vérifier l'état actuel des FK
-- =====================================================
PRINT '=== DIAGNOSTIC : FK existantes ==='
SELECT 
    fk.name AS FK_Name,
    OBJECT_NAME(fk.parent_object_id) AS Table_Enfant,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS Colonne_FK,
    OBJECT_NAME(fk.referenced_object_id) AS Table_Parent,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS Colonne_PK
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
ORDER BY Table_Enfant, FK_Name;
GO

-- =====================================================
-- DIAGNOSTIC : Vérifier l'intégrité des données AVANT
-- d'ajouter les FK (données orphelines ?)
-- =====================================================
PRINT '=== DIAGNOSTIC : Données orphelines ===';

-- 1. T_Bons.ContratId → T_Contrats (PK renommée en IdContrat par Phase1)
PRINT 'Bons avec ContratId inexistant dans T_Contrats:';
SELECT b.IdBon, b.ContratId 
FROM [dbo].[T_Bons] b 
WHERE b.ContratId IS NOT NULL 
  AND b.ContratId NOT IN (SELECT IdContrat FROM [dbo].[T_Contrats]);

-- 2. T_ItinerairesPrevu.BarriereId → T_Barrieres
PRINT 'ItinerairesPrevu avec BarriereId inexistant dans T_Barrieres:';
SELECT ip.IdItineraire, ip.BarriereId 
FROM [dbo].[T_ItinerairesPrevu] ip 
WHERE ip.BarriereId IS NOT NULL 
  AND ip.BarriereId NOT IN (SELECT IdBarriere FROM [dbo].[T_Barrieres]);

-- 3. T_ItinerairesSortie.BarriereId → T_Barrieres
PRINT 'ItinerairesSortie avec BarriereId inexistant dans T_Barrieres:';
SELECT isr.IdItineraire, isr.BarriereId 
FROM [dbo].[T_ItinerairesSortie] isr 
WHERE isr.BarriereId IS NOT NULL 
  AND isr.BarriereId NOT IN (SELECT IdBarriere FROM [dbo].[T_Barrieres]);

-- 4. T_MaterielsSortie.MaterielEntreeId → T_Materiels
PRINT 'MaterielsSortie avec MaterielEntreeId inexistant dans T_Materiels:';
SELECT ms.IdMateriel, ms.MaterielEntreeId 
FROM [dbo].[T_MaterielsSortie] ms 
WHERE ms.MaterielEntreeId IS NOT NULL 
  AND ms.MaterielEntreeId NOT IN (SELECT IdMateriel FROM [dbo].[T_Materiels]);

-- 5. T_MaterielsSortie.BonEntreeId → T_Bons
PRINT 'MaterielsSortie avec BonEntreeId inexistant dans T_Bons:';
SELECT ms.IdMateriel, ms.BonEntreeId 
FROM [dbo].[T_MaterielsSortie] ms 
WHERE ms.BonEntreeId IS NOT NULL 
  AND ms.BonEntreeId NOT IN (SELECT IdBon FROM [dbo].[T_Bons]);

GO

PRINT '';
PRINT '=== Si des données orphelines apparaissent ci-dessus, ===';
PRINT '=== corrigez-les AVANT de continuer !                 ===';
PRINT '';
GO

-- =====================================================
-- FK 1 : T_Bons.ContratId → T_Contrats.IdContrat
-- (BonEntree référence un contrat)
-- =====================================================
PRINT 'Ajout FK : T_Bons.ContratId → T_Contrats.IdContrat';
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Bons_Contrats_ContratId')
BEGIN
    ALTER TABLE [dbo].[T_Bons]
    ADD CONSTRAINT [FK_Bons_Contrats_ContratId]
    FOREIGN KEY ([ContratId]) REFERENCES [dbo].[T_Contrats]([IdContrat])
    ON DELETE NO ACTION;
    PRINT '  ✅ FK créée';
END
ELSE
    PRINT '  ⏭️ FK existe déjà';
GO

-- =====================================================
-- FK 2 : T_ItinerairesPrevu.BarriereId → T_Barrieres.IdBarriere
-- (Chaque étape d'itinéraire passe par une barrière)
-- =====================================================
PRINT 'Ajout FK : T_ItinerairesPrevu.BarriereId → T_Barrieres.IdBarriere';
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ItinerairesPrevu_Barrieres_BarriereId')
BEGIN
    ALTER TABLE [dbo].[T_ItinerairesPrevu]
    ADD CONSTRAINT [FK_ItinerairesPrevu_Barrieres_BarriereId]
    FOREIGN KEY ([BarriereId]) REFERENCES [dbo].[T_Barrieres]([IdBarriere])
    ON DELETE NO ACTION;
    PRINT '  ✅ FK créée';
END
ELSE
    PRINT '  ⏭️ FK existe déjà';
GO

-- =====================================================
-- FK 3 : T_ItinerairesSortie.BarriereId → T_Barrieres.IdBarriere
-- =====================================================
PRINT 'Ajout FK : T_ItinerairesSortie.BarriereId → T_Barrieres.IdBarriere';
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ItinerairesSortie_Barrieres_BarriereId')
BEGIN
    ALTER TABLE [dbo].[T_ItinerairesSortie]
    ADD CONSTRAINT [FK_ItinerairesSortie_Barrieres_BarriereId]
    FOREIGN KEY ([BarriereId]) REFERENCES [dbo].[T_Barrieres]([IdBarriere])
    ON DELETE NO ACTION;
    PRINT '  ✅ FK créée';
END
ELSE
    PRINT '  ⏭️ FK existe déjà';
GO

-- =====================================================
-- FK 4 : T_MaterielsSortie.MaterielEntreeId → T_Materiels.IdMateriel
-- (Traçabilité : matériel sortie → matériel entrée source)
-- =====================================================
PRINT 'Ajout FK : T_MaterielsSortie.MaterielEntreeId → T_Materiels.IdMateriel';
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MaterielsSortie_Materiels_MaterielEntreeId')
BEGIN
    ALTER TABLE [dbo].[T_MaterielsSortie]
    ADD CONSTRAINT [FK_MaterielsSortie_Materiels_MaterielEntreeId]
    FOREIGN KEY ([MaterielEntreeId]) REFERENCES [dbo].[T_Materiels]([IdMateriel])
    ON DELETE NO ACTION;
    PRINT '  ✅ FK créée';
END
ELSE
    PRINT '  ⏭️ FK existe déjà';
GO

-- =====================================================
-- FK 5 : T_MaterielsSortie.BonEntreeId → T_Bons.IdBon
-- (Lien matériel sortie → bon d'entrée source)
-- =====================================================
PRINT 'Ajout FK : T_MaterielsSortie.BonEntreeId → T_Bons.IdBon';
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MaterielsSortie_Bons_BonEntreeId')
BEGIN
    ALTER TABLE [dbo].[T_MaterielsSortie]
    ADD CONSTRAINT [FK_MaterielsSortie_Bons_BonEntreeId]
    FOREIGN KEY ([BonEntreeId]) REFERENCES [dbo].[T_Bons]([IdBon])
    ON DELETE NO ACTION;
    PRINT '  ✅ FK créée';
END
ELSE
    PRINT '  ⏭️ FK existe déjà';
GO

-- =====================================================
-- NOTE : T_Anomalies.BarriereId → T_Barrieres.IdBarriere
-- DÉJÀ EXISTANTE en DB (FK_T_Anomalies_T_Barrieres_BarriereId)
-- Vérifiée le 2026-03-10 — rien à faire
-- =====================================================

-- =====================================================
-- FK 7 : T_Bons.BonSortieAssocieId → T_BonsSortie.IdBon
-- (Liaison bidirectionnelle BEM ↔ BSM)
-- =====================================================
PRINT 'Ajout FK : T_Bons.BonSortieAssocieId → T_BonsSortie.IdBon';
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.T_Bons') AND name = 'BonSortieAssocieId')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Bons_BonsSortie_BonSortieAssocieId')
    BEGIN
        ALTER TABLE [dbo].[T_Bons]
        ADD CONSTRAINT [FK_Bons_BonsSortie_BonSortieAssocieId]
        FOREIGN KEY ([BonSortieAssocieId]) REFERENCES [dbo].[T_BonsSortie]([IdBon])
        ON DELETE SET NULL;
        PRINT '  ✅ FK créée';
    END
    ELSE
        PRINT '  ⏭️ FK existe déjà';
END
ELSE
    PRINT '  ⏭️ Colonne BonSortieAssocieId absente — ignorée';
GO

-- =====================================================
-- VÉRIFICATION FINALE
-- =====================================================
PRINT '';
PRINT '=== VÉRIFICATION : FK après modifications ===';
SELECT 
    fk.name AS FK_Name,
    OBJECT_NAME(fk.parent_object_id) AS Table_Enfant,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS Colonne_FK,
    OBJECT_NAME(fk.referenced_object_id) AS Table_Parent,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS Colonne_PK
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
ORDER BY Table_Enfant, FK_Name;
GO

PRINT '';
PRINT '=== Phase 2 terminée : FK ajoutées ===';
GO
