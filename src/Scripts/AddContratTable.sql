IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'bem') IS NULL EXEC(N'CREATE SCHEMA [bem];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'shared') IS NULL EXEC(N'CREATE SCHEMA [shared];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE TABLE [shared].[Barrieres] (
        [IdBarriere] int NOT NULL IDENTITY,
        [CodeBarriere] nvarchar(20) NOT NULL,
        [NomBarriere] nvarchar(100) NOT NULL,
        [Localisation] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [TypeBarriere] nvarchar(50) NOT NULL DEFAULT N'Mixte',
        [EstActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [HorairesOuverture] nvarchar(100) NULL,
        [Telephone] nvarchar(50) NULL,
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DateModification] datetime2 NULL,
        CONSTRAINT [PK_Barrieres] PRIMARY KEY ([IdBarriere])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE TABLE [bem].[Bons] (
        [IdBon] int NOT NULL IDENTITY,
        [NumeroReference] nvarchar(20) NOT NULL,
        [TypeBon] nvarchar(20) NOT NULL,
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DateExpiration] datetime2 NOT NULL,
        [StatutActuel] nvarchar(30) NOT NULL DEFAULT N'Draft',
        [Provenance] nvarchar(200) NOT NULL,
        [Destination] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [Quantite] int NOT NULL DEFAULT 0,
        [CreateurLogin] nvarchar(100) NOT NULL,
        [CreateurNom] nvarchar(200) NULL,
        [CreateurDepartement] nvarchar(100) NULL,
        [DateModification] datetime2 NULL,
        [ModificateurLogin] nvarchar(100) NULL,
        [QRCodePath] nvarchar(500) NULL,
        [QRCodeData] nvarchar(500) NULL,
        [DateGenerationQR] datetime2 NULL,
        [Commentaires] nvarchar(2000) NULL,
        [EstAnnule] bit NOT NULL DEFAULT CAST(0 AS bit),
        [MotifAnnulation] nvarchar(500) NULL,
        [DateAnnulation] datetime2 NULL,
        [TypeDiscriminator] nvarchar(500) NOT NULL,
        [NumeroContrat] nvarchar(50) NULL,
        [NomCompagnie] nvarchar(200) NULL,
        [EmailContractant] nvarchar(200) NULL,
        [TelephoneContractant] nvarchar(50) NULL,
        [SiteManager] nvarchar(200) NULL,
        [SiteManagerLogin] nvarchar(100) NULL,
        [HostDepartment] nvarchar(100) NULL,
        [ReasonOnSite] nvarchar(500) NULL,
        [NomEscorteur] nvarchar(200) NULL,
        [FonctionEscorteur] nvarchar(150) NULL,
        [EscorteurLogin] nvarchar(100) NULL,
        [DateEntreePrevue] datetime2 NULL,
        [DateEntreeEffective] datetime2 NULL,
        [BarriereEntreeId] int NULL,
        [BarriereEntreeNom] nvarchar(100) NULL,
        [AgentEntreeLogin] nvarchar(100) NULL,
        [DoitRessortir] bit NULL DEFAULT CAST(1 AS bit),
        [BonSortieLieId] int NULL,
        [ToutRessorti] bit NULL DEFAULT CAST(0 AS bit),
        [DateToutRessorti] datetime2 NULL,
        [ZoneTravail] nvarchar(200) NULL,
        [ObservationsEntree] nvarchar(1000) NULL,
        CONSTRAINT [PK_Bons] PRIMARY KEY ([IdBon])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE TABLE [shared].[Departements] (
        [IdDepartement] int NOT NULL IDENTITY,
        [CodeDepartement] nvarchar(20) NOT NULL,
        [NomDepartement] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [ResponsableLogin] nvarchar(100) NOT NULL,
        [ResponsableNom] nvarchar(200) NULL,
        [ResponsableEmail] nvarchar(200) NULL,
        [EstActif] bit NOT NULL DEFAULT CAST(1 AS bit),
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DateModification] datetime2 NULL,
        CONSTRAINT [PK_Departements] PRIMARY KEY ([IdDepartement])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE TABLE [shared].[Utilisateurs] (
        [IdUtilisateur] int NOT NULL IDENTITY,
        [Login] nvarchar(100) NOT NULL,
        [NomComplet] nvarchar(200) NOT NULL,
        [Fonction] nvarchar(150) NULL,
        [Departement] nvarchar(100) NULL,
        [Role] nvarchar(50) NOT NULL DEFAULT N'Utilisateur',
        [Email] nvarchar(200) NULL,
        [Telephone] nvarchar(50) NULL,
        [EstActif] bit NOT NULL DEFAULT CAST(1 AS bit),
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DateModification] datetime2 NULL,
        [DerniereConnexion] datetime2 NULL,
        CONSTRAINT [PK_Utilisateurs] PRIMARY KEY ([IdUtilisateur])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE TABLE [bem].[Approbations] (
        [IdApprobation] int NOT NULL IDENTITY,
        [BonId] int NOT NULL,
        [OrdreEtape] int NOT NULL,
        [NomEtape] nvarchar(100) NOT NULL,
        [StatutAttendu] nvarchar(30) NOT NULL,
        [Decision] nvarchar(30) NOT NULL DEFAULT N'EnAttente',
        [UtilisateurLogin] nvarchar(100) NULL,
        [UtilisateurNom] nvarchar(200) NULL,
        [UtilisateurFonction] nvarchar(150) NULL,
        [DateAction] datetime2 NULL,
        [ReservesEventuelles] nvarchar(1000) NULL,
        [Commentaire] nvarchar(1000) NULL,
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        [EstEtapeCourante] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_Approbations] PRIMARY KEY ([IdApprobation]),
        CONSTRAINT [FK_Approbations_Bons_BonId] FOREIGN KEY ([BonId]) REFERENCES [bem].[Bons] ([IdBon]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE TABLE [bem].[BonEntreeHistory] (
        [IdHistory] int NOT NULL IDENTITY,
        [BonId] int NOT NULL,
        [Action] nvarchar(30) NOT NULL,
        [ActionDescription] nvarchar(500) NOT NULL,
        [ActionBy] nvarchar(100) NOT NULL,
        [ActionByNom] nvarchar(200) NULL,
        [ActionDate] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Comment] nvarchar(1000) NULL,
        [StatutAvant] nvarchar(30) NULL,
        [StatutApres] nvarchar(30) NULL,
        [ChangementsJson] nvarchar(500) NULL,
        [AdresseIP] nvarchar(50) NULL,
        CONSTRAINT [PK_BonEntreeHistory] PRIMARY KEY ([IdHistory]),
        CONSTRAINT [FK_BonEntreeHistory_Bons_BonId] FOREIGN KEY ([BonId]) REFERENCES [bem].[Bons] ([IdBon]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE TABLE [bem].[ItinerairesPrevu] (
        [IdItineraire] int NOT NULL IDENTITY,
        [BonId] int NOT NULL,
        [OrdrePassage] int NOT NULL,
        [BarriereId] int NOT NULL,
        [BarriereNom] nvarchar(100) NULL,
        [BarriereLocalisation] nvarchar(200) NULL,
        [EstObligatoire] bit NOT NULL DEFAULT CAST(1 AS bit),
        [DatePassage] datetime2 NULL,
        [AgentLogin] nvarchar(100) NULL,
        [AgentNom] nvarchar(200) NULL,
        [Observations] nvarchar(500) NULL,
        [ScanEffectue] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_ItinerairesPrevu] PRIMARY KEY ([IdItineraire]),
        CONSTRAINT [FK_ItinerairesPrevu_Bons_BonId] FOREIGN KEY ([BonId]) REFERENCES [bem].[Bons] ([IdBon]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE TABLE [bem].[Materiels] (
        [IdMateriel] int NOT NULL IDENTITY,
        [BonEntreeId] int NOT NULL,
        [CodeProduitSerial] nvarchar(100) NOT NULL,
        [Designation] nvarchar(300) NOT NULL,
        [TypeMateriel] nvarchar(30) NOT NULL DEFAULT N'Autre',
        [Quantite] int NOT NULL DEFAULT 1,
        [Unite] nvarchar(20) NOT NULL DEFAULT N'pcs',
        [Provenance] nvarchar(200) NULL,
        [Destination] nvarchar(200) NULL,
        [Marque] nvarchar(100) NULL,
        [Modele] nvarchar(100) NULL,
        [ValeurEstimee] decimal(18,2) NULL,
        [Devise] nvarchar(3) NOT NULL DEFAULT N'USD',
        [EtatEntree] nvarchar(50) NULL,
        [ObservationsEntree] nvarchar(500) NULL,
        [EstRessorti] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DateSortie] datetime2 NULL,
        [EtatSortie] nvarchar(50) NULL,
        [ObservationsSortie] nvarchar(500) NULL,
        [BonSortieId] int NULL,
        [PhotoEntree] nvarchar(500) NULL,
        [PhotoSortie] nvarchar(500) NULL,
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DateModification] datetime2 NULL,
        CONSTRAINT [PK_Materiels] PRIMARY KEY ([IdMateriel]),
        CONSTRAINT [FK_Materiels_Bons_BonEntreeId] FOREIGN KEY ([BonEntreeId]) REFERENCES [bem].[Bons] ([IdBon]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Approbations_Bon] ON [bem].[Approbations] ([BonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Approbations_Bon_Etape] ON [bem].[Approbations] ([BonId], [OrdreEtape]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Approbations_Decision] ON [bem].[Approbations] ([Decision]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Approbations_EtapeCourante] ON [bem].[Approbations] ([EstEtapeCourante]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Approbations_Utilisateur] ON [bem].[Approbations] ([UtilisateurLogin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Approbations_Utilisateur_Decision_Courante] ON [bem].[Approbations] ([UtilisateurLogin], [Decision], [EstEtapeCourante]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Barrieres_Code] ON [shared].[Barrieres] ([CodeBarriere]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Barrieres_EstActive] ON [shared].[Barrieres] ([EstActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Barrieres_Localisation] ON [shared].[Barrieres] ([Localisation]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Barrieres_Type] ON [shared].[Barrieres] ([TypeBarriere]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BonEntreeHistory_Action] ON [bem].[BonEntreeHistory] ([Action]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BonEntreeHistory_ActionBy] ON [bem].[BonEntreeHistory] ([ActionBy]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BonEntreeHistory_ActionDate] ON [bem].[BonEntreeHistory] ([ActionDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BonEntreeHistory_Bon] ON [bem].[BonEntreeHistory] ([BonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BonEntreeHistory_Bon_Date] ON [bem].[BonEntreeHistory] ([BonId], [ActionDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_BonSortieLie] ON [bem].[Bons] ([BonSortieLieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_Createur] ON [bem].[Bons] ([CreateurLogin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_DateCreation] ON [bem].[Bons] ([DateCreation]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_HostDepartment] ON [bem].[Bons] ([HostDepartment]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_NomCompagnie] ON [bem].[Bons] ([NomCompagnie]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Bons_NumeroReference] ON [bem].[Bons] ([NumeroReference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_Ressortie] ON [bem].[Bons] ([DoitRessortir], [ToutRessorti]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_SiteManager] ON [bem].[Bons] ([SiteManagerLogin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_Statut] ON [bem].[Bons] ([StatutActuel]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_Statut_Type_Date] ON [bem].[Bons] ([StatutActuel], [TypeBon], [DateCreation]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Bons_Type] ON [bem].[Bons] ([TypeBon]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Departements_Code] ON [shared].[Departements] ([CodeDepartement]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Departements_EstActif] ON [shared].[Departements] ([EstActif]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Departements_Responsable] ON [shared].[Departements] ([ResponsableLogin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ItinerairesPrevu_Barriere] ON [bem].[ItinerairesPrevu] ([BarriereId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ItinerairesPrevu_Bon] ON [bem].[ItinerairesPrevu] ([BonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ItinerairesPrevu_Bon_Ordre] ON [bem].[ItinerairesPrevu] ([BonId], [OrdrePassage]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ItinerairesPrevu_Bon_Scan_Ordre] ON [bem].[ItinerairesPrevu] ([BonId], [ScanEffectue], [OrdrePassage]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ItinerairesPrevu_Scan] ON [bem].[ItinerairesPrevu] ([ScanEffectue]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Materiels_BonEntree] ON [bem].[Materiels] ([BonEntreeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Materiels_BonEntree_Ressorti] ON [bem].[Materiels] ([BonEntreeId], [EstRessorti]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Materiels_BonSortie] ON [bem].[Materiels] ([BonSortieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Materiels_CodeSerial] ON [bem].[Materiels] ([CodeProduitSerial]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Materiels_Ressorti] ON [bem].[Materiels] ([EstRessorti]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Materiels_Type] ON [bem].[Materiels] ([TypeMateriel]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Utilisateurs_Departement] ON [shared].[Utilisateurs] ([Departement]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Utilisateurs_EstActif] ON [shared].[Utilisateurs] ([EstActif]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Utilisateurs_Login] ON [shared].[Utilisateurs] ([Login]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Utilisateurs_Role] ON [shared].[Utilisateurs] ([Role]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128055644_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260128055644_InitialCreate', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    ALTER TABLE [bem].[Materiels] DROP CONSTRAINT [FK_Materiels_Bons_BonEntreeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Materiels_BonEntree_Ressorti] ON [bem].[Materiels];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Materiels_BonSortie] ON [bem].[Materiels];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Materiels_Ressorti] ON [bem].[Materiels];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Materiels_Type] ON [bem].[Materiels];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_ItinerairesPrevu_Bon_Scan_Ordre] ON [bem].[ItinerairesPrevu];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_ItinerairesPrevu_Scan] ON [bem].[ItinerairesPrevu];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Bons_BonSortieLie] ON [bem].[Bons];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Bons_Createur] ON [bem].[Bons];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Bons_Ressortie] ON [bem].[Bons];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Bons_SiteManager] ON [bem].[Bons];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Bons_Statut_Type_Date] ON [bem].[Bons];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Bons_Type] ON [bem].[Bons];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Approbations_EtapeCourante] ON [bem].[Approbations];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Approbations_Utilisateur] ON [bem].[Approbations];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Approbations_Utilisateur_Decision_Courante] ON [bem].[Approbations];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'BonSortieId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [BonSortieId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'DateCreation');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [DateCreation];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'DateModification');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [DateModification];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'DateSortie');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [DateSortie];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'Devise');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [Devise];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'EstRessorti');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [EstRessorti];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'EtatEntree');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [EtatEntree];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'EtatSortie');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [EtatSortie];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'Marque');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var8 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [Marque];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'Modele');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var9 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [Modele];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'ObservationsEntree');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var10 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [ObservationsEntree];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'ObservationsSortie');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var11 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [ObservationsSortie];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'PhotoEntree');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var12 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [PhotoEntree];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var13 nvarchar(max);
    SELECT @var13 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'PhotoSortie');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var13 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [PhotoSortie];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var14 nvarchar(max);
    SELECT @var14 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'TypeMateriel');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var14 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [TypeMateriel];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var15 nvarchar(max);
    SELECT @var15 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'Unite');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var15 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [Unite];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var16 nvarchar(max);
    SELECT @var16 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'ValeurEstimee');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var16 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [ValeurEstimee];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var17 nvarchar(max);
    SELECT @var17 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[ItinerairesPrevu]') AND [c].[name] = N'AgentLogin');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT ' + @var17 + ';');
    ALTER TABLE [bem].[ItinerairesPrevu] DROP COLUMN [AgentLogin];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var18 nvarchar(max);
    SELECT @var18 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[ItinerairesPrevu]') AND [c].[name] = N'AgentNom');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT ' + @var18 + ';');
    ALTER TABLE [bem].[ItinerairesPrevu] DROP COLUMN [AgentNom];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var19 nvarchar(max);
    SELECT @var19 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[ItinerairesPrevu]') AND [c].[name] = N'BarriereLocalisation');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT ' + @var19 + ';');
    ALTER TABLE [bem].[ItinerairesPrevu] DROP COLUMN [BarriereLocalisation];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var20 nvarchar(max);
    SELECT @var20 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[ItinerairesPrevu]') AND [c].[name] = N'BarriereNom');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT ' + @var20 + ';');
    ALTER TABLE [bem].[ItinerairesPrevu] DROP COLUMN [BarriereNom];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var21 nvarchar(max);
    SELECT @var21 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[ItinerairesPrevu]') AND [c].[name] = N'DateCreation');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT ' + @var21 + ';');
    ALTER TABLE [bem].[ItinerairesPrevu] DROP COLUMN [DateCreation];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var22 nvarchar(max);
    SELECT @var22 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[ItinerairesPrevu]') AND [c].[name] = N'DatePassage');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT ' + @var22 + ';');
    ALTER TABLE [bem].[ItinerairesPrevu] DROP COLUMN [DatePassage];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var23 nvarchar(max);
    SELECT @var23 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[ItinerairesPrevu]') AND [c].[name] = N'EstObligatoire');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT ' + @var23 + ';');
    ALTER TABLE [bem].[ItinerairesPrevu] DROP COLUMN [EstObligatoire];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var24 nvarchar(max);
    SELECT @var24 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[ItinerairesPrevu]') AND [c].[name] = N'Observations');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT ' + @var24 + ';');
    ALTER TABLE [bem].[ItinerairesPrevu] DROP COLUMN [Observations];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var25 nvarchar(max);
    SELECT @var25 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[ItinerairesPrevu]') AND [c].[name] = N'ScanEffectue');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT ' + @var25 + ';');
    ALTER TABLE [bem].[ItinerairesPrevu] DROP COLUMN [ScanEffectue];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var26 nvarchar(max);
    SELECT @var26 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'AgentEntreeLogin');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var26 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [AgentEntreeLogin];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var27 nvarchar(max);
    SELECT @var27 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'BarriereEntreeId');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var27 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [BarriereEntreeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var28 nvarchar(max);
    SELECT @var28 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'BarriereEntreeNom');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var28 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [BarriereEntreeNom];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var29 nvarchar(max);
    SELECT @var29 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'BonSortieLieId');
    IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var29 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [BonSortieLieId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var30 nvarchar(max);
    SELECT @var30 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'Commentaires');
    IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var30 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [Commentaires];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var31 nvarchar(max);
    SELECT @var31 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'CreateurDepartement');
    IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var31 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [CreateurDepartement];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var32 nvarchar(max);
    SELECT @var32 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'CreateurLogin');
    IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var32 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [CreateurLogin];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var33 nvarchar(max);
    SELECT @var33 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'CreateurNom');
    IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var33 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [CreateurNom];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var34 nvarchar(max);
    SELECT @var34 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'DateAnnulation');
    IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var34 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [DateAnnulation];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var35 nvarchar(max);
    SELECT @var35 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'DateEntreeEffective');
    IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var35 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [DateEntreeEffective];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var36 nvarchar(max);
    SELECT @var36 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'DateEntreePrevue');
    IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var36 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [DateEntreePrevue];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var37 nvarchar(max);
    SELECT @var37 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'DateGenerationQR');
    IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var37 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [DateGenerationQR];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var38 nvarchar(max);
    SELECT @var38 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'DateModification');
    IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var38 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [DateModification];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var39 nvarchar(max);
    SELECT @var39 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'DateToutRessorti');
    IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var39 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [DateToutRessorti];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var40 nvarchar(max);
    SELECT @var40 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'DoitRessortir');
    IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var40 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [DoitRessortir];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var41 nvarchar(max);
    SELECT @var41 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'EscorteurLogin');
    IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var41 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [EscorteurLogin];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var42 nvarchar(max);
    SELECT @var42 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'EstAnnule');
    IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var42 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [EstAnnule];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var43 nvarchar(max);
    SELECT @var43 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'ModificateurLogin');
    IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var43 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [ModificateurLogin];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var44 nvarchar(max);
    SELECT @var44 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'MotifAnnulation');
    IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var44 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [MotifAnnulation];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var45 nvarchar(max);
    SELECT @var45 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'ObservationsEntree');
    IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var45 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [ObservationsEntree];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var46 nvarchar(max);
    SELECT @var46 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'QRCodeData');
    IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var46 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [QRCodeData];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var47 nvarchar(max);
    SELECT @var47 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'QRCodePath');
    IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var47 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [QRCodePath];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var48 nvarchar(max);
    SELECT @var48 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'SiteManagerLogin');
    IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var48 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [SiteManagerLogin];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var49 nvarchar(max);
    SELECT @var49 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'TelephoneContractant');
    IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var49 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [TelephoneContractant];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var50 nvarchar(max);
    SELECT @var50 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'ToutRessorti');
    IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var50 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [ToutRessorti];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var51 nvarchar(max);
    SELECT @var51 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'TypeBon');
    IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var51 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [TypeBon];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var52 nvarchar(max);
    SELECT @var52 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Bons]') AND [c].[name] = N'ZoneTravail');
    IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Bons] DROP CONSTRAINT ' + @var52 + ';');
    ALTER TABLE [bem].[Bons] DROP COLUMN [ZoneTravail];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var53 nvarchar(max);
    SELECT @var53 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Approbations]') AND [c].[name] = N'Commentaire');
    IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Approbations] DROP CONSTRAINT ' + @var53 + ';');
    ALTER TABLE [bem].[Approbations] DROP COLUMN [Commentaire];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var54 nvarchar(max);
    SELECT @var54 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Approbations]') AND [c].[name] = N'DateCreation');
    IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Approbations] DROP CONSTRAINT ' + @var54 + ';');
    ALTER TABLE [bem].[Approbations] DROP COLUMN [DateCreation];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var55 nvarchar(max);
    SELECT @var55 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Approbations]') AND [c].[name] = N'EstEtapeCourante');
    IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Approbations] DROP CONSTRAINT ' + @var55 + ';');
    ALTER TABLE [bem].[Approbations] DROP COLUMN [EstEtapeCourante];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var56 nvarchar(max);
    SELECT @var56 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Approbations]') AND [c].[name] = N'NomEtape');
    IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Approbations] DROP CONSTRAINT ' + @var56 + ';');
    ALTER TABLE [bem].[Approbations] DROP COLUMN [NomEtape];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var57 nvarchar(max);
    SELECT @var57 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Approbations]') AND [c].[name] = N'StatutAttendu');
    IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Approbations] DROP CONSTRAINT ' + @var57 + ';');
    ALTER TABLE [bem].[Approbations] DROP COLUMN [StatutAttendu];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var58 nvarchar(max);
    SELECT @var58 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Approbations]') AND [c].[name] = N'UtilisateurFonction');
    IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Approbations] DROP CONSTRAINT ' + @var58 + ';');
    ALTER TABLE [bem].[Approbations] DROP COLUMN [UtilisateurFonction];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var59 nvarchar(max);
    SELECT @var59 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Approbations]') AND [c].[name] = N'UtilisateurLogin');
    IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Approbations] DROP CONSTRAINT ' + @var59 + ';');
    ALTER TABLE [bem].[Approbations] DROP COLUMN [UtilisateurLogin];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var60 nvarchar(max);
    SELECT @var60 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Approbations]') AND [c].[name] = N'UtilisateurNom');
    IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Approbations] DROP CONSTRAINT ' + @var60 + ';');
    ALTER TABLE [bem].[Approbations] DROP COLUMN [UtilisateurNom];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    EXEC sp_rename N'[bem].[Materiels].[BonEntreeId]', N'BonId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    EXEC sp_rename N'[bem].[Materiels].[IX_Materiels_BonEntree]', N'IX_Materiels_Bon', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DECLARE @var61 nvarchar(max);
    SELECT @var61 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'Quantite');
    IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var61 + ';');
    ALTER TABLE [bem].[Materiels] ALTER COLUMN [Quantite] decimal(18,2) NOT NULL;
    ALTER TABLE [bem].[Materiels] ADD DEFAULT 1.0 FOR [Quantite];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    DROP INDEX [IX_Approbations_Decision] ON [bem].[Approbations];
    DECLARE @var62 nvarchar(max);
    SELECT @var62 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Approbations]') AND [c].[name] = N'Decision');
    IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Approbations] DROP CONSTRAINT ' + @var62 + ';');
    ALTER TABLE [bem].[Approbations] ALTER COLUMN [Decision] nvarchar(50) NOT NULL;
    ALTER TABLE [bem].[Approbations] ADD DEFAULT N'En attente' FOR [Decision];
    CREATE INDEX [IX_Approbations_Decision] ON [bem].[Approbations] ([Decision]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    ALTER TABLE [bem].[Materiels] ADD CONSTRAINT [FK_Materiels_Bons_BonId] FOREIGN KEY ([BonId]) REFERENCES [bem].[Bons] ([IdBon]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128170426_SimplifyEntitiesPerDiagram'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260128170426_SimplifyEntitiesPerDiagram', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    IF SCHEMA_ID(N'bsm') IS NULL EXEC(N'CREATE SCHEMA [bsm];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE TABLE [bsm].[BonsSortie] (
        [IdBon] int NOT NULL IDENTITY,
        [NumeroReference] nvarchar(20) NOT NULL,
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DateExpiration] datetime2 NOT NULL,
        [StatutActuel] nvarchar(30) NOT NULL DEFAULT N'Draft',
        [Destination] nvarchar(200) NOT NULL,
        [Provenance] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [Quantite] int NOT NULL DEFAULT 0,
        [NomDemandeur] nvarchar(200) NOT NULL,
        [FonctionDemandeur] nvarchar(150) NOT NULL,
        [DepartementDemandeur] nvarchar(100) NOT NULL,
        [MotifSortie] nvarchar(1000) NOT NULL,
        [EstDefinitif] bit NOT NULL DEFAULT CAST(1 AS bit),
        [TypeSortie] nvarchar(500) NOT NULL,
        [BonEntreeAssocieId] int NULL,
        [TypeMateriel] nvarchar(50) NULL,
        [NomDestinataire] nvarchar(200) NULL,
        [AdresseDestination] nvarchar(500) NULL,
        [NumeroVehicule] nvarchar(50) NULL,
        [NomChauffeur] nvarchar(200) NULL,
        [TelephoneChauffeur] nvarchar(50) NULL,
        [DateRetourPrevue] datetime2 NULL,
        [DateRetourEffective] datetime2 NULL,
        [EstRetourne] bit NULL DEFAULT CAST(0 AS bit),
        [EtatRetour] nvarchar(1000) NULL,
        [ReceptionnePar] nvarchar(200) NULL,
        [TypeMaterielInterne] nvarchar(50) NULL,
        [DepartementOrigine] nvarchar(100) NULL,
        [DepartementDestination] nvarchar(100) NULL,
        [NomReceveur] nvarchar(200) NULL,
        [FonctionReceveur] nvarchar(150) NULL,
        [EmailReceveur] nvarchar(200) NULL,
        [LocalisationDestination] nvarchar(200) NULL,
        [DateTransfertPrevue] datetime2 NULL,
        [DateTransfertEffective] datetime2 NULL,
        CONSTRAINT [PK_BonsSortie] PRIMARY KEY ([IdBon]),
        CONSTRAINT [FK_BonsSortie_Bons_BonEntreeAssocieId] FOREIGN KEY ([BonEntreeAssocieId]) REFERENCES [bem].[Bons] ([IdBon]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE TABLE [bsm].[ApprobationsSortie] (
        [IdApprobation] int NOT NULL IDENTITY,
        [BonSortieId] int NOT NULL,
        [OrdreEtape] int NOT NULL,
        [NomEtape] nvarchar(100) NOT NULL,
        [Decision] nvarchar(50) NOT NULL DEFAULT N'En attente',
        [DateAction] datetime2 NULL,
        [ApprobateurLogin] nvarchar(100) NULL,
        [ApprobateurNom] nvarchar(200) NULL,
        [Commentaire] nvarchar(1000) NULL,
        CONSTRAINT [PK_ApprobationsSortie] PRIMARY KEY ([IdApprobation]),
        CONSTRAINT [FK_ApprobationsSortie_BonsSortie_BonSortieId] FOREIGN KEY ([BonSortieId]) REFERENCES [bsm].[BonsSortie] ([IdBon]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE TABLE [bsm].[BonSortieHistories] (
        [IdHistory] int NOT NULL IDENTITY,
        [BonSortieId] int NOT NULL,
        [TypeAction] nvarchar(50) NOT NULL,
        [StatutAvant] nvarchar(50) NULL,
        [StatutApres] nvarchar(50) NULL,
        [Description] nvarchar(1000) NULL,
        [UtilisateurLogin] nvarchar(100) NOT NULL,
        [UtilisateurNom] nvarchar(200) NULL,
        [DateAction] datetime2 NOT NULL DEFAULT (GETDATE()),
        [AdresseIP] nvarchar(50) NULL,
        CONSTRAINT [PK_BonSortieHistories] PRIMARY KEY ([IdHistory]),
        CONSTRAINT [FK_BonSortieHistories_BonsSortie_BonSortieId] FOREIGN KEY ([BonSortieId]) REFERENCES [bsm].[BonsSortie] ([IdBon]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE TABLE [bsm].[ItinerairesSortie] (
        [IdItineraire] int NOT NULL IDENTITY,
        [BonSortieId] int NOT NULL,
        [BarriereId] int NOT NULL,
        [OrdrePassage] int NOT NULL,
        [DatePassagePrevue] datetime2 NULL,
        [DatePassageEffective] datetime2 NULL,
        [StatutPassage] nvarchar(50) NOT NULL DEFAULT N'Prévu',
        [Observations] nvarchar(500) NULL,
        CONSTRAINT [PK_ItinerairesSortie] PRIMARY KEY ([IdItineraire]),
        CONSTRAINT [FK_ItinerairesSortie_BonsSortie_BonSortieId] FOREIGN KEY ([BonSortieId]) REFERENCES [bsm].[BonsSortie] ([IdBon]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE TABLE [bsm].[MaterielsSortie] (
        [IdMateriel] int NOT NULL IDENTITY,
        [BonSortieId] int NOT NULL,
        [CodeProduitSerial] nvarchar(100) NOT NULL,
        [Designation] nvarchar(300) NOT NULL,
        [Quantite] decimal(18,2) NOT NULL DEFAULT 1.0,
        [Provenance] nvarchar(200) NULL,
        [Destination] nvarchar(200) NULL,
        CONSTRAINT [PK_MaterielsSortie] PRIMARY KEY ([IdMateriel]),
        CONSTRAINT [FK_MaterielsSortie_BonsSortie_BonSortieId] FOREIGN KEY ([BonSortieId]) REFERENCES [bsm].[BonsSortie] ([IdBon]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_ApprobationsSortie_BonSortieId] ON [bsm].[ApprobationsSortie] ([BonSortieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_ApprobationsSortie_Etape] ON [bsm].[ApprobationsSortie] ([BonSortieId], [OrdreEtape]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonSortieHistories_BonSortieId] ON [bsm].[BonSortieHistories] ([BonSortieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonSortieHistories_DateAction] ON [bsm].[BonSortieHistories] ([DateAction]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonSortieHistories_Utilisateur] ON [bsm].[BonSortieHistories] ([UtilisateurLogin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_BonEntreeAssocie] ON [bsm].[BonsSortie] ([BonEntreeAssocieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_DateCreation] ON [bsm].[BonsSortie] ([DateCreation]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_DateRetourPrevue] ON [bsm].[BonsSortie] ([DateRetourPrevue]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_Departement] ON [bsm].[BonsSortie] ([DepartementDemandeur]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_DeptDestination] ON [bsm].[BonsSortie] ([DepartementDestination]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_DeptOrigine] ON [bsm].[BonsSortie] ([DepartementOrigine]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_Destinataire] ON [bsm].[BonsSortie] ([NomDestinataire]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_EstRetourne] ON [bsm].[BonsSortie] ([EstRetourne]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_BonsSortie_NumeroReference] ON [bsm].[BonsSortie] ([NumeroReference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_PretsEnRetard] ON [bsm].[BonsSortie] ([EstRetourne], [DateRetourPrevue]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_Statut] ON [bsm].[BonsSortie] ([StatutActuel]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_Transfert] ON [bsm].[BonsSortie] ([DepartementOrigine], [DepartementDestination]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_BonsSortie_TypeMateriel] ON [bsm].[BonsSortie] ([TypeMateriel]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_ItinerairesSortie_BarriereId] ON [bsm].[ItinerairesSortie] ([BarriereId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_ItinerairesSortie_BonSortieId] ON [bsm].[ItinerairesSortie] ([BonSortieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_ItinerairesSortie_Ordre] ON [bsm].[ItinerairesSortie] ([BonSortieId], [OrdrePassage]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_MaterielsSortie_BonSortieId] ON [bsm].[MaterielsSortie] ([BonSortieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    CREATE INDEX [IX_MaterielsSortie_CodeProduit] ON [bsm].[MaterielsSortie] ([CodeProduitSerial]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129125918_AddBonSortieModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260129125918_AddBonSortieModule', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129134055_AddQRCodeColumns'
)
BEGIN
    ALTER TABLE [bsm].[BonsSortie] ADD [DateGenerationQR] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129134055_AddQRCodeColumns'
)
BEGIN
    ALTER TABLE [bsm].[BonsSortie] ADD [QRCodeBase64] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129134055_AddQRCodeColumns'
)
BEGIN
    ALTER TABLE [bsm].[BonsSortie] ADD [QRCodeData] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129134055_AddQRCodeColumns'
)
BEGIN
    ALTER TABLE [bsm].[BonsSortie] ADD [QRCodeHash] nvarchar(128) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129134055_AddQRCodeColumns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260129134055_AddQRCodeColumns', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129141447_AddBonEntreeSortieLiaison'
)
BEGIN
    ALTER TABLE [bem].[Bons] ADD [BonSortieAssocieId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129141447_AddBonEntreeSortieLiaison'
)
BEGIN
    ALTER TABLE [bem].[Bons] ADD [BonSortieAssocieNumero] nvarchar(20) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129141447_AddBonEntreeSortieLiaison'
)
BEGIN
    ALTER TABLE [bem].[Bons] ADD [DateVerrouillage] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129141447_AddBonEntreeSortieLiaison'
)
BEGIN
    ALTER TABLE [bem].[Bons] ADD [EstVerrouillePourSortie] bit NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129141447_AddBonEntreeSortieLiaison'
)
BEGIN
    EXEC(N'CREATE INDEX [IX_Bons_BonSortieAssocieId] ON [bem].[Bons] ([BonSortieAssocieId]) WHERE [BonSortieAssocieId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129141447_AddBonEntreeSortieLiaison'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260129141447_AddBonEntreeSortieLiaison', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131082718_RemoveProvenanceDestinationFromMateriels'
)
BEGIN
    DECLARE @var63 nvarchar(max);
    SELECT @var63 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bsm].[MaterielsSortie]') AND [c].[name] = N'Destination');
    IF @var63 IS NOT NULL EXEC(N'ALTER TABLE [bsm].[MaterielsSortie] DROP CONSTRAINT ' + @var63 + ';');
    ALTER TABLE [bsm].[MaterielsSortie] DROP COLUMN [Destination];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131082718_RemoveProvenanceDestinationFromMateriels'
)
BEGIN
    DECLARE @var64 nvarchar(max);
    SELECT @var64 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bsm].[MaterielsSortie]') AND [c].[name] = N'Provenance');
    IF @var64 IS NOT NULL EXEC(N'ALTER TABLE [bsm].[MaterielsSortie] DROP CONSTRAINT ' + @var64 + ';');
    ALTER TABLE [bsm].[MaterielsSortie] DROP COLUMN [Provenance];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131082718_RemoveProvenanceDestinationFromMateriels'
)
BEGIN
    DECLARE @var65 nvarchar(max);
    SELECT @var65 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'Destination');
    IF @var65 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var65 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [Destination];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131082718_RemoveProvenanceDestinationFromMateriels'
)
BEGIN
    DECLARE @var66 nvarchar(max);
    SELECT @var66 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[Materiels]') AND [c].[name] = N'Provenance');
    IF @var66 IS NOT NULL EXEC(N'ALTER TABLE [bem].[Materiels] DROP CONSTRAINT ' + @var66 + ';');
    ALTER TABLE [bem].[Materiels] DROP COLUMN [Provenance];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131082718_RemoveProvenanceDestinationFromMateriels'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260131082718_RemoveProvenanceDestinationFromMateriels', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    IF SCHEMA_ID(N'sec') IS NULL EXEC(N'CREATE SCHEMA [sec];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [IdAuditLog] bigint NOT NULL IDENTITY,
        [DateAction] datetime2 NOT NULL,
        [UtilisateurLogin] nvarchar(100) NOT NULL,
        [UtilisateurNom] nvarchar(200) NULL,
        [TypeAction] nvarchar(50) NOT NULL,
        [Categorie] nvarchar(50) NOT NULL,
        [EntiteId] nvarchar(100) NULL,
        [EntiteType] nvarchar(100) NULL,
        [Description] nvarchar(500) NOT NULL,
        [Details] nvarchar(500) NULL,
        [AncienneValeur] nvarchar(500) NULL,
        [NouvelleValeur] nvarchar(500) NULL,
        [AdresseIP] nvarchar(50) NULL,
        [UserAgent] nvarchar(500) NULL,
        [Resultat] nvarchar(20) NOT NULL,
        [MessageErreur] nvarchar(2000) NULL,
        [Niveau] nvarchar(20) NOT NULL,
        [DureeMs] int NULL,
        [CorrelationId] nvarchar(100) NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([IdAuditLog])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE TABLE [ParametresSysteme] (
        [IdParametre] int NOT NULL IDENTITY,
        [Cle] nvarchar(100) NOT NULL,
        [Valeur] nvarchar(2000) NOT NULL,
        [TypeDonnee] nvarchar(50) NOT NULL,
        [Categorie] nvarchar(50) NOT NULL,
        [Libelle] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [ValeurDefaut] nvarchar(2000) NULL,
        [ValeursPossibles] nvarchar(2000) NULL,
        [ValeurMin] int NULL,
        [ValeurMax] int NULL,
        [Unite] nvarchar(20) NULL,
        [Ordre] int NOT NULL,
        [NecessiteRedemarrage] bit NOT NULL,
        [EstVisible] bit NOT NULL,
        [EstModifiable] bit NOT NULL,
        [EstSysteme] bit NOT NULL,
        [ModifieParLogin] nvarchar(100) NULL,
        [DateModification] datetime2 NULL,
        CONSTRAINT [PK_ParametresSysteme] PRIMARY KEY ([IdParametre])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE TABLE [Roles] (
        [IdRole] int NOT NULL IDENTITY,
        [CodeRole] nvarchar(50) NOT NULL,
        [NomRole] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Permissions] nvarchar(2000) NULL,
        [NiveauPriorite] int NOT NULL,
        [EstActif] bit NOT NULL,
        [EstSysteme] bit NOT NULL,
        [DateCreation] datetime2 NOT NULL,
        [DateModification] datetime2 NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([IdRole])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE TABLE [Statuts] (
        [IdStatut] int NOT NULL IDENTITY,
        [CodeStatut] nvarchar(50) NOT NULL,
        [LibelleStatut] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [TypeBon] nvarchar(50) NOT NULL,
        [CouleurFond] nvarchar(20) NOT NULL,
        [CouleurTexte] nvarchar(20) NOT NULL,
        [Icone] nvarchar(50) NULL,
        [Ordre] int NOT NULL,
        [EstFinal] bit NOT NULL,
        [RequiertAction] bit NOT NULL,
        [StatutsSuivants] nvarchar(200) NULL,
        [EstActif] bit NOT NULL,
        [EstSysteme] bit NOT NULL,
        [DateCreation] datetime2 NOT NULL,
        [DateModification] datetime2 NULL,
        CONSTRAINT [PK_Statuts] PRIMARY KEY ([IdStatut])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE TABLE [TypesMateriels] (
        [IdTypeMateriel] int NOT NULL IDENTITY,
        [CodeType] nvarchar(50) NOT NULL,
        [NomType] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Categorie] nvarchar(100) NULL,
        [Icone] nvarchar(50) NULL,
        [Couleur] nvarchar(20) NULL,
        [RequiertApprobationDepartement] bit NOT NULL,
        [RequiertApprobationDirection] bit NOT NULL,
        [NiveauxApprobation] int NOT NULL,
        [DureeValiditeDefautJours] int NOT NULL,
        [DureeMaximumJours] int NOT NULL,
        [NumeroSerieObligatoire] bit NOT NULL,
        [PhotoObligatoire] bit NOT NULL,
        [ChampsPersonnalises] nvarchar(2000) NULL,
        [WorkflowConfig] nvarchar(4000) NULL,
        [Ordre] int NOT NULL,
        [EstActif] bit NOT NULL,
        [DateCreation] datetime2 NOT NULL,
        [DateModification] datetime2 NULL,
        CONSTRAINT [PK_TypesMateriels] PRIMARY KEY ([IdTypeMateriel])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE TABLE [UtilisateurRoles] (
        [IdUtilisateurRole] int NOT NULL IDENTITY,
        [IdUtilisateur] int NOT NULL,
        [IdRole] int NOT NULL,
        [DateAttribution] datetime2 NOT NULL,
        [AttribueParLogin] nvarchar(100) NULL,
        CONSTRAINT [PK_UtilisateurRoles] PRIMARY KEY ([IdUtilisateurRole]),
        CONSTRAINT [FK_UtilisateurRoles_Roles_IdRole] FOREIGN KEY ([IdRole]) REFERENCES [Roles] ([IdRole]) ON DELETE CASCADE,
        CONSTRAINT [FK_UtilisateurRoles_Utilisateurs_IdUtilisateur] FOREIGN KEY ([IdUtilisateur]) REFERENCES [shared].[Utilisateurs] ([IdUtilisateur]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE TABLE [sec].[Anomalies] (
        [IdAnomalie] int NOT NULL IDENTITY,
        [TypeAnomalie] nvarchar(50) NOT NULL,
        [NiveauGravite] nvarchar(20) NOT NULL DEFAULT N'Moyen',
        [DateSignalement] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Description] nvarchar(2000) NOT NULL,
        [BonId] int NULL,
        [TypeBon] nvarchar(10) NULL,
        [NumeroReferenceBon] nvarchar(30) NULL,
        [ScanId] int NULL,
        [BarriereId] int NULL,
        [EstTraitee] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DateTraitement] datetime2 NULL,
        [SignalePar] nvarchar(100) NOT NULL,
        [SignaleParNom] nvarchar(200) NULL,
        [TraitePar] nvarchar(100) NULL,
        [Resolution] nvarchar(2000) NULL,
        [ActionsCorrectives] nvarchar(2000) NULL,
        CONSTRAINT [PK_Anomalies] PRIMARY KEY ([IdAnomalie]),
        CONSTRAINT [FK_Anomalies_Barrieres_BarriereId] FOREIGN KEY ([BarriereId]) REFERENCES [shared].[Barrieres] ([IdBarriere]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE TABLE [sec].[ScanEvenements] (
        [IdScan] int NOT NULL IDENTITY,
        [DateHeureScan] datetime2 NOT NULL DEFAULT (GETDATE()),
        [StatutScan] nvarchar(30) NOT NULL DEFAULT N'Valid',
        [TypeMouvement] nvarchar(20) NOT NULL DEFAULT N'Sortie',
        [BonId] int NULL,
        [TypeBon] nvarchar(10) NULL,
        [NumeroReferenceBon] nvarchar(30) NULL,
        [BarriereId] int NOT NULL,
        [AgentLogin] nvarchar(100) NOT NULL,
        [AgentNom] nvarchar(200) NULL,
        [QRCodeData] nvarchar(1000) NULL,
        [QRCodeHash] nvarchar(128) NULL,
        [Message] nvarchar(500) NULL,
        [Observations] nvarchar(1000) NULL,
        [AnomalieSignalee] bit NOT NULL DEFAULT CAST(0 AS bit),
        [NumeroPreuve] nvarchar(50) NULL,
        [ProvenanceLieu] nvarchar(200) NULL,
        [DestinationLieu] nvarchar(200) NULL,
        [AnomalieIdAnomalie] int NULL,
        CONSTRAINT [PK_ScanEvenements] PRIMARY KEY ([IdScan]),
        CONSTRAINT [FK_ScanEvenements_Anomalies_AnomalieIdAnomalie] FOREIGN KEY ([AnomalieIdAnomalie]) REFERENCES [sec].[Anomalies] ([IdAnomalie]),
        CONSTRAINT [FK_ScanEvenements_Barrieres_BarriereId] FOREIGN KEY ([BarriereId]) REFERENCES [shared].[Barrieres] ([IdBarriere]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE TABLE [sec].[HistoriqueScans] (
        [IdHistorique] int NOT NULL IDENTITY,
        [ScanId] int NOT NULL,
        [DateHeureMouvement] datetime2 NOT NULL DEFAULT (GETDATE()),
        [TypeMouvement] nvarchar(20) NOT NULL,
        [TypeBon] nvarchar(10) NOT NULL,
        [NumeroReferenceBon] nvarchar(30) NOT NULL,
        [CodeBarriere] nvarchar(20) NOT NULL,
        [NomBarriere] nvarchar(100) NULL,
        [Direction] nvarchar(30) NULL,
        [Departement] nvarchar(100) NULL,
        [Provenance] nvarchar(200) NULL,
        [Destination] nvarchar(200) NULL,
        [NombreMateriels] int NOT NULL DEFAULT 0,
        [ResumeMateriels] nvarchar(2000) NULL,
        [MaterielsJson] nvarchar(max) NULL,
        [StatutScan] nvarchar(30) NOT NULL,
        [PassageAutorise] bit NOT NULL DEFAULT CAST(1 AS bit),
        [AgentLogin] nvarchar(100) NOT NULL,
        [AgentNom] nvarchar(200) NULL,
        [NomDemandeur] nvarchar(200) NULL,
        [MatriculeVehicule] nvarchar(50) NULL,
        [Observations] nvarchar(1000) NULL,
        [AnomalieSignalee] bit NOT NULL DEFAULT CAST(0 AS bit),
        [NombreAnomalies] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_HistoriqueScans] PRIMARY KEY ([IdHistorique]),
        CONSTRAINT [FK_HistoriqueScans_ScanEvenements_ScanId] FOREIGN KEY ([ScanId]) REFERENCES [sec].[ScanEvenements] ([IdScan]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdParametre', N'Categorie', N'Cle', N'DateModification', N'Description', N'EstModifiable', N'EstSysteme', N'EstVisible', N'Libelle', N'ModifieParLogin', N'NecessiteRedemarrage', N'Ordre', N'TypeDonnee', N'Unite', N'Valeur', N'ValeurDefaut', N'ValeurMax', N'ValeurMin', N'ValeursPossibles') AND [object_id] = OBJECT_ID(N'[ParametresSysteme]'))
        SET IDENTITY_INSERT [ParametresSysteme] ON;
    EXEC(N'INSERT INTO [ParametresSysteme] ([IdParametre], [Categorie], [Cle], [DateModification], [Description], [EstModifiable], [EstSysteme], [EstVisible], [Libelle], [ModifieParLogin], [NecessiteRedemarrage], [Ordre], [TypeDonnee], [Unite], [Valeur], [ValeurDefaut], [ValeurMax], [ValeurMin], [ValeursPossibles])
    VALUES (1, N''General'', N''APP_NOM'', NULL, N''Nom affiché dans l''''en-tête et les emails'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Nom de l''''application'', NULL, CAST(0 AS bit), 1, N''String'', NULL, N''KCC Material Flow'', NULL, NULL, NULL, NULL),
    (2, N''General'', N''APP_VERSION'', NULL, N''Version actuelle du système'', CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), N''Version de l''''application'', NULL, CAST(0 AS bit), 2, N''String'', NULL, N''1.0.0'', NULL, NULL, NULL, NULL),
    (10, N''Workflow'', N''WORKFLOW_DUREE_VALIDITE_DEFAUT'', NULL, N''Durée de validité par défaut pour les bons de sortie'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Durée de validité par défaut (jours)'', NULL, CAST(0 AS bit), 1, N''Integer'', N''jours'', N''30'', N''30'', 365, 1, NULL),
    (11, N''Workflow'', N''WORKFLOW_DELAI_RAPPEL_EXPIRATION'', NULL, N''Nombre de jours avant expiration pour envoyer un rappel'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Délai de rappel avant expiration (jours)'', NULL, CAST(0 AS bit), 2, N''Integer'', N''jours'', N''3'', N''3'', 30, 1, NULL),
    (12, N''Workflow'', N''WORKFLOW_DELAI_APPROBATION_MAX'', NULL, N''Délai maximum pour qu''''un approbateur valide un bon'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Délai maximum d''''approbation (jours)'', NULL, CAST(0 AS bit), 3, N''Integer'', N''jours'', N''7'', N''7'', 30, 1, NULL),
    (20, N''Email'', N''EMAIL_ACTIVER_NOTIFICATIONS'', NULL, N''Active ou désactive l''''envoi des notifications par email'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Activer les notifications email'', NULL, CAST(0 AS bit), 1, N''Boolean'', NULL, N''true'', N''true'', NULL, NULL, N''true|false''),
    (21, N''Email'', N''EMAIL_EXPEDITEUR'', NULL, N''Adresse email utilisée comme expéditeur pour les notifications'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Adresse email expéditeur'', NULL, CAST(0 AS bit), 2, N''String'', NULL, N''noreply@kccmaterialflow.local'', NULL, NULL, NULL, NULL),
    (22, N''Email'', N''EMAIL_ADMIN'', NULL, N''Adresse email pour les notifications administratives'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Email administrateur'', NULL, CAST(0 AS bit), 3, N''String'', NULL, N''admin@kccmaterialflow.local'', NULL, NULL, NULL, NULL),
    (30, N''Securite'', N''SECURITE_QRCODE_DUREE_VALIDITE'', NULL, N''Durée pendant laquelle un QR Code scanné est valide'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Durée de validité QR Code (minutes)'', NULL, CAST(0 AS bit), 1, N''Integer'', N''minutes'', N''60'', N''60'', 1440, 5, NULL),
    (31, N''Securite'', N''SECURITE_MAX_SCANS_JOUR'', NULL, N''Nombre maximum de scans autorisés par bon par jour'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Nombre maximum de scans par jour'', NULL, CAST(0 AS bit), 2, N''Integer'', NULL, N''10'', N''10'', 100, 1, NULL),
    (32, N''Securite'', N''SECURITE_DETECTER_ANOMALIES_AUTO'', NULL, N''Active la détection automatique des anomalies lors des scans'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Détection automatique des anomalies'', NULL, CAST(0 AS bit), 3, N''Boolean'', NULL, N''true'', N''true'', NULL, NULL, N''true|false''),
    (40, N''Interface'', N''UI_ITEMS_PAR_PAGE'', NULL, N''Nombre d''''éléments affichés par page dans les listes'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Éléments par page'', NULL, CAST(0 AS bit), 1, N''Integer'', NULL, N''20'', N''20'', 100, 10, N''10|20|50|100''),
    (41, N''Interface'', N''UI_THEME_DEFAUT'', NULL, N''Thème visuel par défaut de l''''application'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Thème par défaut'', NULL, CAST(0 AS bit), 2, N''String'', NULL, N''light'', N''light'', NULL, NULL, N''light|dark|auto''),
    (42, N''Interface'', N''UI_LANGUE_DEFAUT'', NULL, N''Langue par défaut de l''''interface'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Langue par défaut'', NULL, CAST(0 AS bit), 3, N''String'', NULL, N''fr'', N''fr'', NULL, NULL, N''fr|en'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdParametre', N'Categorie', N'Cle', N'DateModification', N'Description', N'EstModifiable', N'EstSysteme', N'EstVisible', N'Libelle', N'ModifieParLogin', N'NecessiteRedemarrage', N'Ordre', N'TypeDonnee', N'Unite', N'Valeur', N'ValeurDefaut', N'ValeurMax', N'ValeurMin', N'ValeursPossibles') AND [object_id] = OBJECT_ID(N'[ParametresSysteme]'))
        SET IDENTITY_INSERT [ParametresSysteme] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdRole', N'CodeRole', N'DateCreation', N'DateModification', N'Description', N'EstActif', N'EstSysteme', N'NiveauPriorite', N'NomRole', N'Permissions') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([IdRole], [CodeRole], [DateCreation], [DateModification], [Description], [EstActif], [EstSysteme], [NiveauPriorite], [NomRole], [Permissions])
    VALUES (1, N''ADMIN'', ''2024-01-01T00:00:00.0000000'', NULL, N''Accès complet au système'', CAST(1 AS bit), CAST(1 AS bit), 100, N''Administrateur'', N''["ALL"]''),
    (2, N''APPROBATEUR'', ''2024-01-01T00:00:00.0000000'', NULL, N''Peut approuver les bons de son département'', CAST(1 AS bit), CAST(1 AS bit), 50, N''Approbateur'', N''["VIEW_BON","APPROVE_BON","REJECT_BON"]''),
    (3, N''AGENT_SECURITE'', ''2024-01-01T00:00:00.0000000'', NULL, N''Peut scanner et contrôler les entrées/sorties'', CAST(1 AS bit), CAST(1 AS bit), 40, N''Agent de sécurité'', N''["VIEW_BON","SCAN_BON","CREATE_ANOMALIE"]''),
    (4, N''UTILISATEUR'', ''2024-01-01T00:00:00.0000000'', NULL, N''Utilisateur standard - peut créer des bons'', CAST(1 AS bit), CAST(1 AS bit), 10, N''Utilisateur'', N''["CREATE_BON","VIEW_OWN_BON"]'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdRole', N'CodeRole', N'DateCreation', N'DateModification', N'Description', N'EstActif', N'EstSysteme', N'NiveauPriorite', N'NomRole', N'Permissions') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdStatut', N'CodeStatut', N'CouleurFond', N'CouleurTexte', N'DateCreation', N'DateModification', N'Description', N'EstActif', N'EstFinal', N'EstSysteme', N'Icone', N'LibelleStatut', N'Ordre', N'RequiertAction', N'StatutsSuivants', N'TypeBon') AND [object_id] = OBJECT_ID(N'[Statuts]'))
        SET IDENTITY_INSERT [Statuts] ON;
    EXEC(N'INSERT INTO [Statuts] ([IdStatut], [CodeStatut], [CouleurFond], [CouleurTexte], [DateCreation], [DateModification], [Description], [EstActif], [EstFinal], [EstSysteme], [Icone], [LibelleStatut], [Ordre], [RequiertAction], [StatutsSuivants], [TypeBon])
    VALUES (1, N''BROUILLON'', N''#6c757d'', N''#ffffff'', ''2024-01-01T00:00:00.0000000'', NULL, N''Bon en cours de création'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''bi-pencil'', N''Brouillon'', 1, CAST(1 AS bit), N''2'', N''Tous''),
    (2, N''EN_ATTENTE_APPROBATION'', N''#ffc107'', N''#212529'', ''2024-01-01T00:00:00.0000000'', NULL, N''Bon soumis, en attente de validation'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''bi-hourglass-split'', N''En attente d''''approbation'', 2, CAST(1 AS bit), N''3,4'', N''Tous''),
    (3, N''APPROUVE'', N''#28a745'', N''#ffffff'', ''2024-01-01T00:00:00.0000000'', NULL, N''Bon approuvé par le responsable'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''bi-check-circle'', N''Approuvé'', 3, CAST(0 AS bit), N''5,6'', N''Tous''),
    (4, N''REJETE'', N''#dc3545'', N''#ffffff'', ''2024-01-01T00:00:00.0000000'', NULL, N''Bon rejeté par le responsable'', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N''bi-x-circle'', N''Rejeté'', 4, CAST(0 AS bit), NULL, N''Tous''),
    (5, N''EN_COURS'', N''#17a2b8'', N''#ffffff'', ''2024-01-01T00:00:00.0000000'', NULL, N''Matériel en cours d''''utilisation/sortie'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''bi-arrow-repeat'', N''En cours'', 5, CAST(0 AS bit), N''6,7'', N''BonSortie''),
    (6, N''TERMINE'', N''#198754'', N''#ffffff'', ''2024-01-01T00:00:00.0000000'', NULL, N''Processus terminé avec succès'', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N''bi-check2-all'', N''Terminé'', 6, CAST(0 AS bit), NULL, N''Tous''),
    (7, N''EXPIRE'', N''#fd7e14'', N''#ffffff'', ''2024-01-01T00:00:00.0000000'', NULL, N''Bon expiré - matériel non retourné à temps'', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''bi-exclamation-triangle'', N''Expiré'', 7, CAST(1 AS bit), N''6'', N''BonSortie''),
    (8, N''ANNULE'', N''#6c757d'', N''#ffffff'', ''2024-01-01T00:00:00.0000000'', NULL, N''Bon annulé par l''''utilisateur ou le système'', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N''bi-slash-circle'', N''Annulé'', 8, CAST(0 AS bit), NULL, N''Tous'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdStatut', N'CodeStatut', N'CouleurFond', N'CouleurTexte', N'DateCreation', N'DateModification', N'Description', N'EstActif', N'EstFinal', N'EstSysteme', N'Icone', N'LibelleStatut', N'Ordre', N'RequiertAction', N'StatutsSuivants', N'TypeBon') AND [object_id] = OBJECT_ID(N'[Statuts]'))
        SET IDENTITY_INSERT [Statuts] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdTypeMateriel', N'Categorie', N'ChampsPersonnalises', N'CodeType', N'Couleur', N'DateCreation', N'DateModification', N'Description', N'DureeMaximumJours', N'DureeValiditeDefautJours', N'EstActif', N'Icone', N'NiveauxApprobation', N'NomType', N'NumeroSerieObligatoire', N'Ordre', N'PhotoObligatoire', N'RequiertApprobationDepartement', N'RequiertApprobationDirection', N'WorkflowConfig') AND [object_id] = OBJECT_ID(N'[TypesMateriels]'))
        SET IDENTITY_INSERT [TypesMateriels] ON;
    EXEC(N'INSERT INTO [TypesMateriels] ([IdTypeMateriel], [Categorie], [ChampsPersonnalises], [CodeType], [Couleur], [DateCreation], [DateModification], [Description], [DureeMaximumJours], [DureeValiditeDefautJours], [EstActif], [Icone], [NiveauxApprobation], [NomType], [NumeroSerieObligatoire], [Ordre], [PhotoObligatoire], [RequiertApprobationDepartement], [RequiertApprobationDirection], [WorkflowConfig])
    VALUES (1, N''Matériel roulant'', NULL, N''VEHICULE'', N''#0d6efd'', ''2024-01-01T00:00:00.0000000'', NULL, N''Véhicules de société (voitures, camions, engins)'', 30, 1, CAST(1 AS bit), N''bi-truck'', 1, N''Véhicule'', CAST(1 AS bit), 1, CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), NULL),
    (2, N''Informatique'', NULL, N''EQUIPEMENT_IT'', N''#6610f2'', ''2024-01-01T00:00:00.0000000'', NULL, N''Ordinateurs, laptops, serveurs, équipements réseau'', 365, 30, CAST(1 AS bit), N''bi-laptop'', 2, N''Équipement informatique'', CAST(1 AS bit), 2, CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), NULL),
    (3, N''Équipement'', NULL, N''OUTILLAGE'', N''#fd7e14'', ''2024-01-01T00:00:00.0000000'', NULL, N''Outils et équipements de travail'', 90, 7, CAST(1 AS bit), N''bi-tools'', 1, N''Outillage'', CAST(0 AS bit), 3, CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), NULL),
    (4, N''Documentation'', NULL, N''DOCUMENT'', N''#20c997'', ''2024-01-01T00:00:00.0000000'', NULL, N''Documents confidentiels, plans, archives'', 7, 1, CAST(1 AS bit), N''bi-file-earmark-text'', 1, N''Document'', CAST(0 AS bit), 4, CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), NULL),
    (5, N''Divers'', NULL, N''MATERIEL_DIVERS'', N''#6c757d'', ''2024-01-01T00:00:00.0000000'', NULL, N''Autre matériel non catégorisé'', 180, 7, CAST(1 AS bit), N''bi-box-seam'', 1, N''Matériel divers'', CAST(0 AS bit), 99, CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdTypeMateriel', N'Categorie', N'ChampsPersonnalises', N'CodeType', N'Couleur', N'DateCreation', N'DateModification', N'Description', N'DureeMaximumJours', N'DureeValiditeDefautJours', N'EstActif', N'Icone', N'NiveauxApprobation', N'NomType', N'NumeroSerieObligatoire', N'Ordre', N'PhotoObligatoire', N'RequiertApprobationDepartement', N'RequiertApprobationDirection', N'WorkflowConfig') AND [object_id] = OBJECT_ID(N'[TypesMateriels]'))
        SET IDENTITY_INSERT [TypesMateriels] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Anomalies_BarriereId] ON [sec].[Anomalies] ([BarriereId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Anomalies_BonId] ON [sec].[Anomalies] ([BonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Anomalies_DateSignalement] ON [sec].[Anomalies] ([DateSignalement]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Anomalies_EstTraitee] ON [sec].[Anomalies] ([EstTraitee]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Anomalies_EstTraitee_NiveauGravite] ON [sec].[Anomalies] ([EstTraitee], [NiveauGravite]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Anomalies_NiveauGravite] ON [sec].[Anomalies] ([NiveauGravite]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Anomalies_ScanId] ON [sec].[Anomalies] ([ScanId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Anomalies_SignalePar] ON [sec].[Anomalies] ([SignalePar]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Anomalies_TypeAnomalie] ON [sec].[Anomalies] ([TypeAnomalie]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Categorie] ON [AuditLogs] ([Categorie]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_CorrelationId] ON [AuditLogs] ([CorrelationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_DateAction] ON [AuditLogs] ([DateAction]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Entite] ON [AuditLogs] ([EntiteType], [EntiteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Niveau] ON [AuditLogs] ([Niveau]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Reporting] ON [AuditLogs] ([DateAction], [Categorie], [TypeAction]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_TypeAction] ON [AuditLogs] ([TypeAction]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_UtilisateurLogin] ON [AuditLogs] ([UtilisateurLogin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_AgentLogin] ON [sec].[HistoriqueScans] ([AgentLogin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_AnomalieSignalee] ON [sec].[HistoriqueScans] ([AnomalieSignalee]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_Barriere_Date] ON [sec].[HistoriqueScans] ([CodeBarriere], [DateHeureMouvement]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_CodeBarriere] ON [sec].[HistoriqueScans] ([CodeBarriere]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_Date_TypeMouvement] ON [sec].[HistoriqueScans] ([DateHeureMouvement], [TypeMouvement]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_DateHeureMouvement] ON [sec].[HistoriqueScans] ([DateHeureMouvement]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_NumeroReferenceBon] ON [sec].[HistoriqueScans] ([NumeroReferenceBon]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_PassageAutorise] ON [sec].[HistoriqueScans] ([PassageAutorise]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_ScanId] ON [sec].[HistoriqueScans] ([ScanId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_Statut_Passage] ON [sec].[HistoriqueScans] ([StatutScan], [PassageAutorise]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_TypeBon] ON [sec].[HistoriqueScans] ([TypeBon]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_HistoriqueScans_TypeMouvement] ON [sec].[HistoriqueScans] ([TypeMouvement]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_ParametresSysteme_Categorie] ON [ParametresSysteme] ([Categorie]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ParametresSysteme_Cle] ON [ParametresSysteme] ([Cle]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_CodeRole] ON [Roles] ([CodeRole]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Roles_EstActif] ON [Roles] ([EstActif]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_ScanEvenements_AgentLogin] ON [sec].[ScanEvenements] ([AgentLogin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_ScanEvenements_AnomalieIdAnomalie] ON [sec].[ScanEvenements] ([AnomalieIdAnomalie]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_ScanEvenements_BarriereId] ON [sec].[ScanEvenements] ([BarriereId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_ScanEvenements_BonId] ON [sec].[ScanEvenements] ([BonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_ScanEvenements_DateHeureScan] ON [sec].[ScanEvenements] ([DateHeureScan]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_ScanEvenements_StatutScan] ON [sec].[ScanEvenements] ([StatutScan]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_ScanEvenements_TypeBon_BonId] ON [sec].[ScanEvenements] ([TypeBon], [BonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Statuts_CodeStatut] ON [Statuts] ([CodeStatut]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_Statuts_TypeBon_EstActif] ON [Statuts] ([TypeBon], [EstActif]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TypesMateriels_CodeType] ON [TypesMateriels] ([CodeType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_TypesMateriels_EstActif_Ordre] ON [TypesMateriels] ([EstActif], [Ordre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE INDEX [IX_UtilisateurRoles_IdRole] ON [UtilisateurRoles] ([IdRole]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UtilisateurRoles_Utilisateur_Role] ON [UtilisateurRoles] ([IdUtilisateur], [IdRole]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    ALTER TABLE [sec].[Anomalies] ADD CONSTRAINT [FK_Anomalies_ScanEvenements_ScanId] FOREIGN KEY ([ScanId]) REFERENCES [sec].[ScanEvenements] ([IdScan]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260131135557_AddAdminTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260131135557_AddAdminTables', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203063230_AddCategorieRaisonSortieTables'
)
BEGIN
    ALTER TABLE [shared].[Barrieres] ADD [OrdreAffichage] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203063230_AddCategorieRaisonSortieTables'
)
BEGIN
    CREATE TABLE [shared].[CategoriesSortie] (
        [IdCategorie] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Libelle] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [RequiertApprobationIT] bit NOT NULL DEFAULT CAST(0 AS bit),
        [RequiertApprobationEnvironnement] bit NOT NULL DEFAULT CAST(0 AS bit),
        [OrdreAffichage] int NOT NULL DEFAULT 0,
        [EstActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DateModification] datetime2 NULL,
        CONSTRAINT [PK_CategoriesSortie] PRIMARY KEY ([IdCategorie])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203063230_AddCategorieRaisonSortieTables'
)
BEGIN
    CREATE TABLE [shared].[RaisonsSortie] (
        [IdRaison] int NOT NULL IDENTITY,
        [CategorieId] int NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Libelle] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [OrdreAffichage] int NOT NULL DEFAULT 0,
        [EstActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DateModification] datetime2 NULL,
        CONSTRAINT [PK_RaisonsSortie] PRIMARY KEY ([IdRaison]),
        CONSTRAINT [FK_RaisonsSortie_CategoriesSortie_CategorieId] FOREIGN KEY ([CategorieId]) REFERENCES [shared].[CategoriesSortie] ([IdCategorie]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203063230_AddCategorieRaisonSortieTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CategoriesSortie_Code] ON [shared].[CategoriesSortie] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203063230_AddCategorieRaisonSortieTables'
)
BEGIN
    CREATE INDEX [IX_CategoriesSortie_Ordre] ON [shared].[CategoriesSortie] ([OrdreAffichage]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203063230_AddCategorieRaisonSortieTables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RaisonsSortie_Categorie_Code] ON [shared].[RaisonsSortie] ([CategorieId], [Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203063230_AddCategorieRaisonSortieTables'
)
BEGIN
    CREATE INDEX [IX_RaisonsSortie_Categorie_Ordre] ON [shared].[RaisonsSortie] ([CategorieId], [OrdreAffichage]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203063230_AddCategorieRaisonSortieTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260203063230_AddCategorieRaisonSortieTables', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203133357_AddNomDemandeurToBonEntree'
)
BEGIN
    ALTER TABLE [bem].[Bons] ADD [NomDemandeur] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203133357_AddNomDemandeurToBonEntree'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260203133357_AddNomDemandeurToBonEntree', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203180627_AddApprobationDetails'
)
BEGIN
    ALTER TABLE [bem].[Approbations] ADD [NomApprobateur] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203180627_AddApprobationDetails'
)
BEGIN
    ALTER TABLE [bem].[Approbations] ADD [NomEtape] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203180627_AddApprobationDetails'
)
BEGIN
    ALTER TABLE [bem].[Approbations] ADD [RoleApprobateur] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203180627_AddApprobationDetails'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260203180627_AddApprobationDetails', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203204009_AddCreatedByLoginToBonSortie'
)
BEGIN
    ALTER TABLE [bsm].[BonsSortie] ADD [CreatedByLogin] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203204009_AddCreatedByLoginToBonSortie'
)
BEGIN
    ALTER TABLE [bem].[BonEntreeHistory] ADD [BonIdBon] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203204009_AddCreatedByLoginToBonSortie'
)
BEGIN
    CREATE INDEX [IX_BonEntreeHistory_BonIdBon] ON [bem].[BonEntreeHistory] ([BonIdBon]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203204009_AddCreatedByLoginToBonSortie'
)
BEGIN
    ALTER TABLE [bem].[BonEntreeHistory] ADD CONSTRAINT [FK_BonEntreeHistory_Bons_BonIdBon] FOREIGN KEY ([BonIdBon]) REFERENCES [bem].[Bons] ([IdBon]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260203204009_AddCreatedByLoginToBonSortie'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260203204009_AddCreatedByLoginToBonSortie', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204081330_AddReferenceDataTables'
)
BEGIN
    IF SCHEMA_ID(N'ref') IS NULL EXEC(N'CREATE SCHEMA [ref];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204081330_AddReferenceDataTables'
)
BEGIN
    CREATE TABLE [ref].[Compagnies] (
        [Id] int NOT NULL IDENTITY,
        [Nom] nvarchar(200) NOT NULL,
        [Code] nvarchar(20) NULL,
        [NumeroContrat] nvarchar(50) NULL,
        [Email] nvarchar(200) NULL,
        [Telephone] nvarchar(50) NULL,
        [SiteManager] nvarchar(200) NULL,
        [EstActif] bit NOT NULL,
        [DateCreation] datetime2 NOT NULL,
        CONSTRAINT [PK_Compagnies] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204081330_AddReferenceDataTables'
)
BEGIN
    CREATE TABLE [ref].[Departements] (
        [Id] int NOT NULL IDENTITY,
        [Nom] nvarchar(100) NOT NULL,
        [Code] nvarchar(20) NULL,
        [Description] nvarchar(500) NULL,
        [Responsable] nvarchar(200) NULL,
        [EstActif] bit NOT NULL,
        [OrdreAffichage] int NOT NULL,
        CONSTRAINT [PK_Departements] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204081330_AddReferenceDataTables'
)
BEGIN
    CREATE TABLE [dbo].[NotificationsRejet] (
        [Id] int NOT NULL IDENTITY,
        [BonType] nvarchar(20) NOT NULL,
        [NumeroReference] nvarchar(50) NOT NULL,
        [EtapeRejet] nvarchar(50) NOT NULL,
        [ApprobateurNom] nvarchar(200) NOT NULL,
        [ApprobateurLogin] nvarchar(100) NULL,
        [MotifRejet] nvarchar(500) NOT NULL,
        [DemandeurNom] nvarchar(200) NULL,
        [DateRejet] datetime2 NOT NULL,
        [EstLue] bit NOT NULL,
        [DateLecture] datetime2 NULL,
        [EmailEnvoye] bit NOT NULL,
        [DateEnvoiEmail] datetime2 NULL,
        CONSTRAINT [PK_NotificationsRejet] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204081330_AddReferenceDataTables'
)
BEGIN
    CREATE TABLE [ref].[Sites] (
        [Id] int NOT NULL IDENTITY,
        [Nom] nvarchar(200) NOT NULL,
        [Code] nvarchar(20) NULL,
        [Adresse] nvarchar(500) NULL,
        [TypeSite] nvarchar(50) NULL,
        [EstInterne] bit NOT NULL,
        [EstActif] bit NOT NULL,
        [OrdreAffichage] int NOT NULL,
        CONSTRAINT [PK_Sites] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204081330_AddReferenceDataTables'
)
BEGIN
    CREATE TABLE [ref].[Employees] (
        [Id] int NOT NULL IDENTITY,
        [Matricule] nvarchar(20) NULL,
        [NomComplet] nvarchar(200) NOT NULL,
        [Prenom] nvarchar(100) NULL,
        [Nom] nvarchar(100) NULL,
        [Fonction] nvarchar(150) NULL,
        [Email] nvarchar(200) NULL,
        [Telephone] nvarchar(50) NULL,
        [DepartementId] int NULL,
        [CompagnieId] int NULL,
        [EstInterne] bit NOT NULL,
        [PeutEtreEscorteur] bit NOT NULL,
        [EstActif] bit NOT NULL,
        [Login] nvarchar(100) NULL,
        [DateCreation] datetime2 NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_Compagnies_CompagnieId] FOREIGN KEY ([CompagnieId]) REFERENCES [ref].[Compagnies] ([Id]),
        CONSTRAINT [FK_Employees_Departements_DepartementId] FOREIGN KEY ([DepartementId]) REFERENCES [ref].[Departements] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204081330_AddReferenceDataTables'
)
BEGIN
    CREATE INDEX [IX_Employees_CompagnieId] ON [ref].[Employees] ([CompagnieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204081330_AddReferenceDataTables'
)
BEGIN
    CREATE INDEX [IX_Employees_DepartementId] ON [ref].[Employees] ([DepartementId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204081330_AddReferenceDataTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260204081330_AddReferenceDataTables', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204140224_AddCheckpointAndCategoriesTables'
)
BEGIN
    CREATE TABLE [ref].[CategoriesSortie] (
        [Id] int NOT NULL IDENTITY,
        [Nom] nvarchar(50) NOT NULL,
        [Code] nvarchar(10) NULL,
        [Description] nvarchar(200) NULL,
        [EstActif] bit NOT NULL,
        [OrdreAffichage] int NOT NULL,
        CONSTRAINT [PK_CategoriesSortie] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204140224_AddCheckpointAndCategoriesTables'
)
BEGIN
    CREATE TABLE [ref].[Checkpoints] (
        [Id] int NOT NULL IDENTITY,
        [SiteId] int NOT NULL,
        [Nom] nvarchar(100) NOT NULL,
        [Code] nvarchar(20) NULL,
        [Description] nvarchar(500) NULL,
        [EstActif] bit NOT NULL,
        [OrdreDefaut] int NOT NULL,
        CONSTRAINT [PK_Checkpoints] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Checkpoints_Sites_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [ref].[Sites] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204140224_AddCheckpointAndCategoriesTables'
)
BEGIN
    CREATE TABLE [ref].[RaisonsSortie] (
        [Id] int NOT NULL IDENTITY,
        [CategorieId] int NOT NULL,
        [Nom] nvarchar(100) NOT NULL,
        [Code] nvarchar(20) NULL,
        [Description] nvarchar(500) NULL,
        [RequiertBonEntree] bit NOT NULL,
        [RequiertDetails] bit NOT NULL,
        [EstActif] bit NOT NULL,
        [OrdreAffichage] int NOT NULL,
        CONSTRAINT [PK_RaisonsSortie] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RaisonsSortie_CategoriesSortie_CategorieId] FOREIGN KEY ([CategorieId]) REFERENCES [ref].[CategoriesSortie] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204140224_AddCheckpointAndCategoriesTables'
)
BEGIN
    CREATE TABLE [dbo].[PassagesCheckpoint] (
        [Id] int NOT NULL IDENTITY,
        [TypeBon] nvarchar(10) NOT NULL,
        [BonId] int NOT NULL,
        [NumeroReference] nvarchar(20) NOT NULL,
        [CheckpointId] int NOT NULL,
        [OrdrePrevu] int NOT NULL,
        [OrdreEffectif] int NULL,
        [DatePrevue] datetime2 NULL,
        [DateEffective] datetime2 NULL,
        [Statut] int NOT NULL,
        [ScannePar] nvarchar(100) NULL,
        [EstAnomalie] bit NOT NULL,
        [TypeAnomalie] int NULL,
        [DescriptionAnomalie] nvarchar(500) NULL,
        [Observations] nvarchar(500) NULL,
        [CoordonneeGPS] nvarchar(100) NULL,
        CONSTRAINT [PK_PassagesCheckpoint] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PassagesCheckpoint_Checkpoints_CheckpointId] FOREIGN KEY ([CheckpointId]) REFERENCES [ref].[Checkpoints] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204140224_AddCheckpointAndCategoriesTables'
)
BEGIN
    CREATE INDEX [IX_Checkpoints_SiteId] ON [ref].[Checkpoints] ([SiteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204140224_AddCheckpointAndCategoriesTables'
)
BEGIN
    CREATE INDEX [IX_PassagesCheckpoint_CheckpointId] ON [dbo].[PassagesCheckpoint] ([CheckpointId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204140224_AddCheckpointAndCategoriesTables'
)
BEGIN
    CREATE INDEX [IX_RaisonsSortie_CategorieId] ON [ref].[RaisonsSortie] ([CategorieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260204140224_AddCheckpointAndCategoriesTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260204140224_AddCheckpointAndCategoriesTables', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    ALTER TABLE [ref].[Employees] DROP CONSTRAINT [FK_Employees_Departements_DepartementId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    DROP TABLE [ref].[Departements];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    DROP INDEX [IX_Departements_Code] ON [shared].[Departements];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    DROP INDEX [IX_Departements_EstActif] ON [shared].[Departements];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    DROP INDEX [IX_Departements_Responsable] ON [shared].[Departements];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    DECLARE @var67 nvarchar(max);
    SELECT @var67 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[shared].[Departements]') AND [c].[name] = N'EstActif');
    IF @var67 IS NOT NULL EXEC(N'ALTER TABLE [shared].[Departements] DROP CONSTRAINT ' + @var67 + ';');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    DECLARE @var68 nvarchar(max);
    SELECT @var68 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[shared].[Departements]') AND [c].[name] = N'DateCreation');
    IF @var68 IS NOT NULL EXEC(N'ALTER TABLE [shared].[Departements] DROP CONSTRAINT ' + @var68 + ';');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    DECLARE @var69 nvarchar(max);
    SELECT @var69 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bsm].[BonsSortie]') AND [c].[name] = N'TypeSortie');
    IF @var69 IS NOT NULL EXEC(N'ALTER TABLE [bsm].[BonsSortie] DROP CONSTRAINT ' + @var69 + ';');
    ALTER TABLE [bsm].[BonsSortie] ALTER COLUMN [TypeSortie] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    ALTER TABLE [ref].[Employees] ADD CONSTRAINT [FK_Employees_Departements_DepartementId] FOREIGN KEY ([DepartementId]) REFERENCES [shared].[Departements] ([IdDepartement]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206053427_AddTypeSortieColumn'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260206053427_AddTypeSortieColumn', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206055511_RenameTypeSortieToRaisonSortieCode'
)
BEGIN
    DECLARE @var70 nvarchar(max);
    SELECT @var70 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bsm].[BonsSortie]') AND [c].[name] = N'TypeSortie');
    IF @var70 IS NOT NULL EXEC(N'ALTER TABLE [bsm].[BonsSortie] DROP CONSTRAINT ' + @var70 + ';');
    ALTER TABLE [bsm].[BonsSortie] ALTER COLUMN [TypeSortie] nvarchar(500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206055511_RenameTypeSortieToRaisonSortieCode'
)
BEGIN
    ALTER TABLE [bsm].[BonsSortie] ADD [RaisonSortieCode] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206055511_RenameTypeSortieToRaisonSortieCode'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260206055511_RenameTypeSortieToRaisonSortieCode', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    DROP INDEX [IX_BonsSortie_DeptDestination] ON [bsm].[BonsSortie];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    DROP INDEX [IX_BonsSortie_Transfert] ON [bsm].[BonsSortie];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    DECLARE @var71 nvarchar(max);
    SELECT @var71 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bsm].[BonsSortie]') AND [c].[name] = N'DepartementDestination');
    IF @var71 IS NOT NULL EXEC(N'ALTER TABLE [bsm].[BonsSortie] DROP CONSTRAINT ' + @var71 + ';');
    ALTER TABLE [bsm].[BonsSortie] DROP COLUMN [DepartementDestination];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    DECLARE @var72 nvarchar(max);
    SELECT @var72 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bsm].[BonsSortie]') AND [c].[name] = N'NomReceveur');
    IF @var72 IS NOT NULL EXEC(N'ALTER TABLE [bsm].[BonsSortie] DROP CONSTRAINT ' + @var72 + ';');
    ALTER TABLE [bsm].[BonsSortie] DROP COLUMN [NomReceveur];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[RaisonsSortie] ADD [Couleur] nvarchar(20) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[RaisonsSortie] ADD [DureeMaxJours] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[RaisonsSortie] ADD [EstTemporaire] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[RaisonsSortie] ADD [Icone] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[RaisonsSortie] ADD [RequiertBarrieres] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[RaisonsSortie] ADD [TypeApprobateurSpecial] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[RaisonsSortie] ADD [ValidationSpeciale] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [bsm].[MaterielsSortie] ADD [BonEntreeId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [bsm].[MaterielsSortie] ADD [BonEntreeNumero] nvarchar(20) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [bsm].[MaterielsSortie] ADD [MaterielEntreeId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [bsm].[MaterielsSortie] ADD [Observations] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [bsm].[MaterielsSortie] ADD [QuantiteDisponible] decimal(18,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[CategoriesSortie] ADD [RequiertBarrieres] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[CategoriesSortie] ADD [RequiertBonEntree] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    ALTER TABLE [ref].[CategoriesSortie] ADD [TypeEntite] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    CREATE TABLE [bem].[SoldesMateriels] (
        [Id] int NOT NULL IDENTITY,
        [MaterielEntreeId] int NOT NULL,
        [BonEntreeId] int NOT NULL,
        [CodeProduitSerial] nvarchar(100) NOT NULL,
        [Designation] nvarchar(300) NOT NULL,
        [QuantiteInitiale] decimal(18,2) NOT NULL,
        [QuantiteSortie] decimal(18,2) NOT NULL,
        [DateDerniereMaj] datetime2 NOT NULL,
        [DernierBsmNumero] nvarchar(20) NULL,
        CONSTRAINT [PK_SoldesMateriels] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260210073317_AddBemBsmLiaisonColumns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260210073317_AddBemBsmLiaisonColumns', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260219140446_AddBonEntreeAssocieIdToInterne'
)
BEGIN
    DECLARE @var73 nvarchar(max);
    SELECT @var73 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[SoldesMateriels]') AND [c].[name] = N'QuantiteSortie');
    IF @var73 IS NOT NULL EXEC(N'ALTER TABLE [bem].[SoldesMateriels] DROP CONSTRAINT ' + @var73 + ';');
    ALTER TABLE [bem].[SoldesMateriels] ALTER COLUMN [QuantiteSortie] decimal(18,4) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260219140446_AddBonEntreeAssocieIdToInterne'
)
BEGIN
    DECLARE @var74 nvarchar(max);
    SELECT @var74 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bem].[SoldesMateriels]') AND [c].[name] = N'QuantiteInitiale');
    IF @var74 IS NOT NULL EXEC(N'ALTER TABLE [bem].[SoldesMateriels] DROP CONSTRAINT ' + @var74 + ';');
    ALTER TABLE [bem].[SoldesMateriels] ALTER COLUMN [QuantiteInitiale] decimal(18,4) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260219140446_AddBonEntreeAssocieIdToInterne'
)
BEGIN
    DECLARE @var75 nvarchar(max);
    SELECT @var75 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bsm].[MaterielsSortie]') AND [c].[name] = N'QuantiteDisponible');
    IF @var75 IS NOT NULL EXEC(N'ALTER TABLE [bsm].[MaterielsSortie] DROP CONSTRAINT ' + @var75 + ';');
    ALTER TABLE [bsm].[MaterielsSortie] ALTER COLUMN [QuantiteDisponible] decimal(18,4) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260219140446_AddBonEntreeAssocieIdToInterne'
)
BEGIN
    DECLARE @var76 nvarchar(max);
    SELECT @var76 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bsm].[MaterielsSortie]') AND [c].[name] = N'Quantite');
    IF @var76 IS NOT NULL EXEC(N'ALTER TABLE [bsm].[MaterielsSortie] DROP CONSTRAINT ' + @var76 + ';');
    ALTER TABLE [bsm].[MaterielsSortie] ALTER COLUMN [Quantite] decimal(18,4) NOT NULL;
    ALTER TABLE [bsm].[MaterielsSortie] ADD DEFAULT 1.0 FOR [Quantite];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260219140446_AddBonEntreeAssocieIdToInterne'
)
BEGIN
    ALTER TABLE [bsm].[MaterielsSortie] ADD [QuantiteInitialeBem] decimal(18,4) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260219140446_AddBonEntreeAssocieIdToInterne'
)
BEGIN
    ALTER TABLE [bem].[Materiels] ADD [QuantiteDisponible] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260219140446_AddBonEntreeAssocieIdToInterne'
)
BEGIN
    ALTER TABLE [bsm].[BonsSortie] ADD [BonSortieInterne_BonEntreeAssocieId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260219140446_AddBonEntreeAssocieIdToInterne'
)
BEGIN
    CREATE INDEX [IX_MaterielsSortie_MaterielEntreeId] ON [bsm].[MaterielsSortie] ([MaterielEntreeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260219140446_AddBonEntreeAssocieIdToInterne'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260219140446_AddBonEntreeAssocieIdToInterne', N'10.0.2');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304055402_AddContratTable'
)
BEGIN
    DECLARE @var77 nvarchar(max);
    SELECT @var77 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ref].[Compagnies]') AND [c].[name] = N'NumeroContrat');
    IF @var77 IS NOT NULL EXEC(N'ALTER TABLE [ref].[Compagnies] DROP CONSTRAINT ' + @var77 + ';');
    ALTER TABLE [ref].[Compagnies] DROP COLUMN [NumeroContrat];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304055402_AddContratTable'
)
BEGIN
    ALTER TABLE [bem].[Bons] ADD [ContratId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304055402_AddContratTable'
)
BEGIN
    CREATE TABLE [ref].[Contrats] (
        [Id] int NOT NULL IDENTITY,
        [PoNumber] nvarchar(50) NOT NULL,
        [ContratDescription] nvarchar(500) NULL,
        [CompagnieId] int NOT NULL,
        [EstActif] bit NOT NULL,
        [DateCreation] datetime2 NOT NULL,
        CONSTRAINT [PK_Contrats] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Contrats_Compagnies_CompagnieId] FOREIGN KEY ([CompagnieId]) REFERENCES [ref].[Compagnies] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304055402_AddContratTable'
)
BEGIN
    CREATE INDEX [IX_Contrats_CompagnieId] ON [ref].[Contrats] ([CompagnieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304055402_AddContratTable'
)
BEGIN
    CREATE INDEX [IX_Contrats_PoNumber] ON [ref].[Contrats] ([PoNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304055402_AddContratTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260304055402_AddContratTable', N'10.0.2');
END;

COMMIT;
GO

