SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ============================================================
-- Migration: AddContratTable
-- Base: AppDev_KCCMaterialFlow_DB_Dev sur CDKTGNBK01127
-- ============================================================

-- ÉTAPE 1: Créer la table ref.Contrats
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'ref' AND TABLE_NAME = 'Contrats')
BEGIN
    CREATE TABLE [ref].[Contrats] (
        [Id] int NOT NULL IDENTITY,
        [PoNumber] nvarchar(50) NOT NULL,
        [ContratDescription] nvarchar(500) NULL,
        [CompagnieId] int NOT NULL,
        [EstActif] bit NOT NULL DEFAULT 1,
        [DateCreation] datetime2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Contrats] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Contrats_Compagnies_CompagnieId] FOREIGN KEY ([CompagnieId]) 
            REFERENCES [ref].[Compagnies] ([Id]) ON DELETE NO ACTION
    );
    CREATE INDEX [IX_Contrats_CompagnieId] ON [ref].[Contrats] ([CompagnieId]);
    CREATE INDEX [IX_Contrats_PoNumber] ON [ref].[Contrats] ([PoNumber]);
    PRINT 'Table ref.Contrats creee.';
END
GO

-- ÉTAPE 2: Migrer NumeroContrat existants vers Contrats
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_SCHEMA = 'ref' AND TABLE_NAME = 'Compagnies' AND COLUMN_NAME = 'NumeroContrat')
BEGIN
    INSERT INTO [ref].[Contrats] ([PoNumber], [CompagnieId], [EstActif], [DateCreation])
    SELECT DISTINCT c.[NumeroContrat], c.[Id], 1, GETDATE()
    FROM [ref].[Compagnies] c
    WHERE c.[NumeroContrat] IS NOT NULL AND c.[NumeroContrat] <> '';
    PRINT 'Donnees NumeroContrat migrees vers Contrats.';
END
GO

-- ÉTAPE 3: Ajouter ContratId à bem.Bons
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'bem' AND TABLE_NAME = 'Bons' AND COLUMN_NAME = 'ContratId')
BEGIN
    ALTER TABLE [bem].[Bons] ADD [ContratId] int NULL;
    PRINT 'Colonne ContratId ajoutee a bem.Bons.';
END
GO

-- ÉTAPE 4: Mettre à jour ContratId dans Bons depuis NumeroContrat
UPDATE b
SET b.[ContratId] = ct.[Id]
FROM [bem].[Bons] b
INNER JOIN [ref].[Contrats] ct ON b.[NumeroContrat] = ct.[PoNumber]
WHERE b.[NumeroContrat] IS NOT NULL AND b.[NumeroContrat] <> ''
AND b.[ContratId] IS NULL;
PRINT 'ContratId mis a jour dans bem.Bons.';
GO

-- ÉTAPE 5: Supprimer NumeroContrat de ref.Compagnies
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_SCHEMA = 'ref' AND TABLE_NAME = 'Compagnies' AND COLUMN_NAME = 'NumeroContrat')
BEGIN
    DECLARE @constraint nvarchar(200);
    SELECT @constraint = d.[name]
    FROM [sys].[default_constraints] d
    INNER JOIN [sys].[columns] c ON d.[parent_column_id] = c.[column_id] 
        AND d.[parent_object_id] = c.[object_id]
    WHERE d.[parent_object_id] = OBJECT_ID(N'[ref].[Compagnies]') AND c.[name] = N'NumeroContrat';
    
    IF @constraint IS NOT NULL
        EXEC(N'ALTER TABLE [ref].[Compagnies] DROP CONSTRAINT [' + @constraint + ']');

    ALTER TABLE [ref].[Compagnies] DROP COLUMN [NumeroContrat];
    PRINT 'Colonne NumeroContrat supprimee de ref.Compagnies.';
END
GO

-- ÉTAPE 6: Enregistrer dans EF Migrations History
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260304055402_AddContratTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260304055402_AddContratTable', N'10.0.2');
    PRINT 'Migration enregistree dans __EFMigrationsHistory.';
END
GO

PRINT '====== Migration AddContratTable terminee avec succes ! ======';
GO
