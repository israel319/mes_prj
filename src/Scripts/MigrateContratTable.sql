-- ============================================================
-- Migration: AddContratTable (20260304055402)
-- Description: Sépare Contrat de Compagnie (1:N)
--   1. Migre les données NumeroContrat existantes vers la nouvelle table Contrats
--   2. Supprime NumeroContrat de ref.Compagnies
--   3. Ajoute ContratId à bem.Bons
--   4. Crée la table ref.Contrats
-- ============================================================

BEGIN TRANSACTION;
BEGIN TRY

    -- ============================================================
    -- ÉTAPE 1: Créer la table ref.Contrats
    -- ============================================================
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

        PRINT 'Table ref.Contrats créée avec succès.';
    END
    ELSE
    BEGIN
        PRINT 'Table ref.Contrats existe déjà - ignorée.';
    END

    -- ============================================================
    -- ÉTAPE 2: Migrer les données NumeroContrat existantes
    -- (Utilise du SQL dynamique pour éviter l'erreur de compilation
    --  si la colonne NumeroContrat n'existe plus)
    -- ============================================================
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'ref' AND TABLE_NAME = 'Compagnies' AND COLUMN_NAME = 'NumeroContrat')
    BEGIN
        EXEC(N'
            INSERT INTO [ref].[Contrats] ([PoNumber], [CompagnieId], [EstActif], [DateCreation])
            SELECT DISTINCT c.[NumeroContrat], c.[Id], 1, GETDATE()
            FROM [ref].[Compagnies] c
            WHERE c.[NumeroContrat] IS NOT NULL AND c.[NumeroContrat] <> ''''
        ');
        PRINT 'Données migrées: contrats créés à partir de NumeroContrat existants.';
    END
    ELSE
    BEGIN
        PRINT 'Colonne NumeroContrat déjà supprimée de ref.Compagnies - migration de données ignorée.';
    END

    -- ============================================================
    -- ÉTAPE 3: Ajouter la colonne ContratId à bem.Bons
    -- ============================================================
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = 'bem' AND TABLE_NAME = 'Bons' AND COLUMN_NAME = 'ContratId')
    BEGIN
        ALTER TABLE [bem].[Bons] ADD [ContratId] int NULL;
        PRINT 'Colonne ContratId ajoutée à bem.Bons.';
    END
    ELSE
    BEGIN
        PRINT 'Colonne ContratId existe déjà dans bem.Bons - ignorée.';
    END

    -- Mettre à jour ContratId dans bem.Bons à partir des NumeroContrat existants
    -- (Utilise du SQL dynamique pour éviter l'erreur si NumeroContrat n'existe pas dans Bons)
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'bem' AND TABLE_NAME = 'Bons' AND COLUMN_NAME = 'NumeroContrat')
    AND EXISTS (SELECT 1 FROM [ref].[Contrats])
    BEGIN
        EXEC(N'
            UPDATE b
            SET b.[ContratId] = ct.[Id]
            FROM [bem].[Bons] b
            INNER JOIN [ref].[Contrats] ct ON b.[NumeroContrat] = ct.[PoNumber]
            WHERE b.[NumeroContrat] IS NOT NULL AND b.[NumeroContrat] <> ''''
            AND b.[ContratId] IS NULL
        ');
        PRINT 'Bons mis à jour avec ContratId.';
    END

    -- ============================================================
    -- ÉTAPE 4: Supprimer NumeroContrat de ref.Compagnies
    -- ============================================================
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_SCHEMA = 'ref' AND TABLE_NAME = 'Compagnies' AND COLUMN_NAME = 'NumeroContrat')
    BEGIN
        -- Supprimer d'abord les contraintes par défaut s'il y en a
        DECLARE @constraintName nvarchar(max);
        SELECT @constraintName = QUOTENAME([d].[name])
        FROM [sys].[default_constraints] [d]
        INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] 
            AND [d].[parent_object_id] = [c].[object_id]
        WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ref].[Compagnies]') AND [c].[name] = N'NumeroContrat');
        
        IF @constraintName IS NOT NULL 
            EXEC(N'ALTER TABLE [ref].[Compagnies] DROP CONSTRAINT ' + @constraintName + ';');

        ALTER TABLE [ref].[Compagnies] DROP COLUMN [NumeroContrat];
        PRINT 'Colonne NumeroContrat supprimée de ref.Compagnies.';
    END
    ELSE
    BEGIN
        PRINT 'Colonne NumeroContrat déjà absente de ref.Compagnies - ignorée.';
    END

    -- ============================================================
    -- ÉTAPE 5: Enregistrer la migration dans EF Migrations History
    -- ============================================================
    IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260304055402_AddContratTable')
    BEGIN
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (N'20260304055402_AddContratTable', N'10.0.2');
        PRINT 'Migration enregistrée dans __EFMigrationsHistory.';
    END
    ELSE
    BEGIN
        PRINT 'Migration déjà enregistrée dans __EFMigrationsHistory - ignorée.';
    END

    COMMIT TRANSACTION;
    PRINT '';
    PRINT '====================================================';
    PRINT 'Migration AddContratTable appliquée avec succès !';
    PRINT '====================================================';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT '';
    PRINT '====================================================';
    PRINT 'ERREUR lors de la migration !';
    PRINT CONCAT('Message: ', ERROR_MESSAGE());
    PRINT CONCAT('Ligne: ', ERROR_LINE());
    PRINT '====================================================';
END CATCH
GO
