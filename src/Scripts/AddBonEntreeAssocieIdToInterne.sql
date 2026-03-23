-- Script pour ajouter la colonne BonSortieInterne_BonEntreeAssocieId si elle n'existe pas
-- D'abord vérifier si les schémas et tables existent

-- Vérifier quelle base de données nous utilisons
SELECT DB_NAME() AS CurrentDatabase;

-- Lister les tables existantes dans les schémas bsm et bem
SELECT TABLE_SCHEMA, TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA IN ('bsm', 'bem', 'dbo')
ORDER BY TABLE_SCHEMA, TABLE_NAME;

-- Créer le schéma bsm s'il n'existe pas
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'bsm')
BEGIN
    EXEC('CREATE SCHEMA bsm');
    PRINT 'Schema bsm created.';
END

-- Si la table BonsSortie existe, ajouter la colonne
IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'bsm' 
    AND TABLE_NAME = 'BonsSortie'
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_SCHEMA = 'bsm' 
        AND TABLE_NAME = 'BonsSortie' 
        AND COLUMN_NAME = 'BonSortieInterne_BonEntreeAssocieId'
    )
    BEGIN
        ALTER TABLE [bsm].[BonsSortie] 
        ADD [BonSortieInterne_BonEntreeAssocieId] INT NULL;
        
        PRINT 'Column BonSortieInterne_BonEntreeAssocieId added successfully.';
    END
    ELSE
    BEGIN
        PRINT 'Column BonSortieInterne_BonEntreeAssocieId already exists.';
    END
END
ELSE
BEGIN
    PRINT 'Table bsm.BonsSortie does not exist! Run EF migrations first.';
END

-- Vérifier également les autres colonnes de la migration
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'bsm' 
    AND TABLE_NAME = 'MaterielsSortie' 
    AND COLUMN_NAME = 'QuantiteInitialeBem'
)
BEGIN
    ALTER TABLE [bsm].[MaterielsSortie] 
    ADD [QuantiteInitialeBem] DECIMAL(18,4) NULL;
    
    PRINT 'Column QuantiteInitialeBem added successfully.';
END

IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'bem' 
    AND TABLE_NAME = 'Materiels' 
    AND COLUMN_NAME = 'QuantiteDisponible'
)
BEGIN
    ALTER TABLE [bem].[Materiels] 
    ADD [QuantiteDisponible] DECIMAL(18,2) NOT NULL DEFAULT 0;
    
    PRINT 'Column QuantiteDisponible added successfully.';
END

-- Créer l'index si nécessaire
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_MaterielsSortie_MaterielEntreeId' 
    AND object_id = OBJECT_ID('bsm.MaterielsSortie')
)
BEGIN
    CREATE INDEX [IX_MaterielsSortie_MaterielEntreeId] 
    ON [bsm].[MaterielsSortie] ([MaterielEntreeId]);
    
    PRINT 'Index IX_MaterielsSortie_MaterielEntreeId created successfully.';
END
