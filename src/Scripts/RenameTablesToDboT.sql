BEGIN TRANSACTION;
ALTER TABLE [sec].[Anomalies] DROP CONSTRAINT [FK_Anomalies_Barrieres_BarriereId];

ALTER TABLE [sec].[Anomalies] DROP CONSTRAINT [FK_Anomalies_ScanEvenements_ScanId];

ALTER TABLE [bem].[Approbations] DROP CONSTRAINT [FK_Approbations_Bons_BonId];

ALTER TABLE [bsm].[ApprobationsSortie] DROP CONSTRAINT [FK_ApprobationsSortie_BonsSortie_BonSortieId];

ALTER TABLE [bem].[BonEntreeHistory] DROP CONSTRAINT [FK_BonEntreeHistory_Bons_BonId];

ALTER TABLE [bem].[BonEntreeHistory] DROP CONSTRAINT [FK_BonEntreeHistory_Bons_BonIdBon];

ALTER TABLE [bsm].[BonSortieHistories] DROP CONSTRAINT [FK_BonSortieHistories_BonsSortie_BonSortieId];

ALTER TABLE [bsm].[BonsSortie] DROP CONSTRAINT [FK_BonsSortie_Bons_BonEntreeAssocieId];

ALTER TABLE [ref].[Checkpoints] DROP CONSTRAINT [FK_Checkpoints_Sites_SiteId];

ALTER TABLE [ref].[Contrats] DROP CONSTRAINT [FK_Contrats_Compagnies_CompagnieId];

ALTER TABLE [ref].[Employees] DROP CONSTRAINT [FK_Employees_Compagnies_CompagnieId];

ALTER TABLE [ref].[Employees] DROP CONSTRAINT [FK_Employees_Departements_DepartementId];

ALTER TABLE [sec].[HistoriqueScans] DROP CONSTRAINT [FK_HistoriqueScans_ScanEvenements_ScanId];

ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT [FK_ItinerairesPrevu_Bons_BonId];

ALTER TABLE [bsm].[ItinerairesSortie] DROP CONSTRAINT [FK_ItinerairesSortie_BonsSortie_BonSortieId];

ALTER TABLE [bem].[Materiels] DROP CONSTRAINT [FK_Materiels_Bons_BonId];

ALTER TABLE [bsm].[MaterielsSortie] DROP CONSTRAINT [FK_MaterielsSortie_BonsSortie_BonSortieId];

ALTER TABLE [dbo].[PassagesCheckpoint] DROP CONSTRAINT [FK_PassagesCheckpoint_Checkpoints_CheckpointId];

ALTER TABLE [ref].[RaisonsSortie] DROP CONSTRAINT [FK_RaisonsSortie_CategoriesSortie_CategorieId];

ALTER TABLE [sec].[ScanEvenements] DROP CONSTRAINT [FK_ScanEvenements_Anomalies_AnomalieIdAnomalie];

ALTER TABLE [sec].[ScanEvenements] DROP CONSTRAINT [FK_ScanEvenements_Barrieres_BarriereId];

ALTER TABLE [UtilisateurRoles] DROP CONSTRAINT [FK_UtilisateurRoles_Roles_IdRole];

ALTER TABLE [UtilisateurRoles] DROP CONSTRAINT [FK_UtilisateurRoles_Utilisateurs_IdUtilisateur];

ALTER TABLE [shared].[Utilisateurs] DROP CONSTRAINT [PK_Utilisateurs];

ALTER TABLE [UtilisateurRoles] DROP CONSTRAINT [PK_UtilisateurRoles];

ALTER TABLE [TypesMateriels] DROP CONSTRAINT [PK_TypesMateriels];

ALTER TABLE [Statuts] DROP CONSTRAINT [PK_Statuts];

ALTER TABLE [bem].[SoldesMateriels] DROP CONSTRAINT [PK_SoldesMateriels];

ALTER TABLE [ref].[Sites] DROP CONSTRAINT [PK_Sites];

ALTER TABLE [sec].[ScanEvenements] DROP CONSTRAINT [PK_ScanEvenements];

ALTER TABLE [Roles] DROP CONSTRAINT [PK_Roles];

ALTER TABLE [ref].[RaisonsSortie] DROP CONSTRAINT [PK_RaisonsSortie];

ALTER TABLE [dbo].[PassagesCheckpoint] DROP CONSTRAINT [PK_PassagesCheckpoint];

ALTER TABLE [ParametresSysteme] DROP CONSTRAINT [PK_ParametresSysteme];

ALTER TABLE [dbo].[NotificationsRejet] DROP CONSTRAINT [PK_NotificationsRejet];

ALTER TABLE [bsm].[MaterielsSortie] DROP CONSTRAINT [PK_MaterielsSortie];

ALTER TABLE [bem].[Materiels] DROP CONSTRAINT [PK_Materiels];

ALTER TABLE [bsm].[ItinerairesSortie] DROP CONSTRAINT [PK_ItinerairesSortie];

ALTER TABLE [bem].[ItinerairesPrevu] DROP CONSTRAINT [PK_ItinerairesPrevu];

ALTER TABLE [sec].[HistoriqueScans] DROP CONSTRAINT [PK_HistoriqueScans];

ALTER TABLE [ref].[Employees] DROP CONSTRAINT [PK_Employees];

ALTER TABLE [shared].[Departements] DROP CONSTRAINT [PK_Departements];

ALTER TABLE [ref].[Contrats] DROP CONSTRAINT [PK_Contrats];

ALTER TABLE [ref].[Compagnies] DROP CONSTRAINT [PK_Compagnies];

ALTER TABLE [ref].[Checkpoints] DROP CONSTRAINT [PK_Checkpoints];

ALTER TABLE [ref].[CategoriesSortie] DROP CONSTRAINT [PK_CategoriesSortie];

ALTER TABLE [bsm].[BonsSortie] DROP CONSTRAINT [PK_BonsSortie];

ALTER TABLE [bsm].[BonSortieHistories] DROP CONSTRAINT [PK_BonSortieHistories];

ALTER TABLE [bem].[Bons] DROP CONSTRAINT [PK_Bons];

ALTER TABLE [bem].[BonEntreeHistory] DROP CONSTRAINT [PK_BonEntreeHistory];

ALTER TABLE [shared].[Barrieres] DROP CONSTRAINT [PK_Barrieres];

ALTER TABLE [AuditLogs] DROP CONSTRAINT [PK_AuditLogs];

ALTER TABLE [bsm].[ApprobationsSortie] DROP CONSTRAINT [PK_ApprobationsSortie];

ALTER TABLE [bem].[Approbations] DROP CONSTRAINT [PK_Approbations];

ALTER TABLE [sec].[Anomalies] DROP CONSTRAINT [PK_Anomalies];

EXEC sp_rename N'[shared].[Utilisateurs]', N'T_Utilisateurs', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [shared].[T_Utilisateurs];

EXEC sp_rename N'[UtilisateurRoles]', N'T_UtilisateurRoles', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [T_UtilisateurRoles];

EXEC sp_rename N'[TypesMateriels]', N'T_TypesMateriels', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [T_TypesMateriels];

EXEC sp_rename N'[Statuts]', N'T_Statuts', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [T_Statuts];

EXEC sp_rename N'[bem].[SoldesMateriels]', N'T_SoldesMateriels', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bem].[T_SoldesMateriels];

EXEC sp_rename N'[ref].[Sites]', N'T_Sites', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [ref].[T_Sites];

EXEC sp_rename N'[sec].[ScanEvenements]', N'T_ScanEvenements', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [sec].[T_ScanEvenements];

EXEC sp_rename N'[Roles]', N'T_Roles', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [T_Roles];

EXEC sp_rename N'[ref].[RaisonsSortie]', N'T_RaisonsSortie', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [ref].[T_RaisonsSortie];

EXEC sp_rename N'[dbo].[PassagesCheckpoint]', N'T_PassagesCheckpoint', 'OBJECT';

EXEC sp_rename N'[ParametresSysteme]', N'T_ParametresSysteme', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [T_ParametresSysteme];

EXEC sp_rename N'[dbo].[NotificationsRejet]', N'T_NotificationsRejet', 'OBJECT';

EXEC sp_rename N'[bsm].[MaterielsSortie]', N'T_MaterielsSortie', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bsm].[T_MaterielsSortie];

EXEC sp_rename N'[bem].[Materiels]', N'T_Materiels', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bem].[T_Materiels];

EXEC sp_rename N'[bsm].[ItinerairesSortie]', N'T_ItinerairesSortie', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bsm].[T_ItinerairesSortie];

EXEC sp_rename N'[bem].[ItinerairesPrevu]', N'T_ItinerairesPrevu', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bem].[T_ItinerairesPrevu];

EXEC sp_rename N'[sec].[HistoriqueScans]', N'T_HistoriqueScans', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [sec].[T_HistoriqueScans];

EXEC sp_rename N'[ref].[Employees]', N'T_Employees', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [ref].[T_Employees];

EXEC sp_rename N'[shared].[Departements]', N'T_Departements', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [shared].[T_Departements];

EXEC sp_rename N'[ref].[Contrats]', N'T_Contrats', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [ref].[T_Contrats];

EXEC sp_rename N'[ref].[Compagnies]', N'T_Compagnies', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [ref].[T_Compagnies];

EXEC sp_rename N'[ref].[Checkpoints]', N'T_Checkpoints', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [ref].[T_Checkpoints];

EXEC sp_rename N'[ref].[CategoriesSortie]', N'T_CategoriesSortie', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [ref].[T_CategoriesSortie];

EXEC sp_rename N'[bsm].[BonsSortie]', N'T_BonsSortie', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bsm].[T_BonsSortie];

EXEC sp_rename N'[bsm].[BonSortieHistories]', N'T_BonSortieHistories', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bsm].[T_BonSortieHistories];

EXEC sp_rename N'[bem].[Bons]', N'T_Bons', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bem].[T_Bons];

EXEC sp_rename N'[bem].[BonEntreeHistory]', N'T_BonEntreeHistory', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bem].[T_BonEntreeHistory];

EXEC sp_rename N'[shared].[Barrieres]', N'T_Barrieres', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [shared].[T_Barrieres];

EXEC sp_rename N'[AuditLogs]', N'T_AuditLogs', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [T_AuditLogs];

EXEC sp_rename N'[bsm].[ApprobationsSortie]', N'T_ApprobationsSortie', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bsm].[T_ApprobationsSortie];

EXEC sp_rename N'[bem].[Approbations]', N'T_Approbations', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [bem].[T_Approbations];

EXEC sp_rename N'[sec].[Anomalies]', N'T_Anomalies', 'OBJECT';
ALTER SCHEMA [dbo] TRANSFER [sec].[T_Anomalies];

EXEC sp_rename N'[dbo].[T_UtilisateurRoles].[IX_UtilisateurRoles_IdRole]', N'IX_T_UtilisateurRoles_IdRole', 'INDEX';

EXEC sp_rename N'[dbo].[T_ScanEvenements].[IX_ScanEvenements_AnomalieIdAnomalie]', N'IX_T_ScanEvenements_AnomalieIdAnomalie', 'INDEX';

EXEC sp_rename N'[dbo].[T_RaisonsSortie].[IX_RaisonsSortie_CategorieId]', N'IX_T_RaisonsSortie_CategorieId', 'INDEX';

EXEC sp_rename N'[dbo].[T_PassagesCheckpoint].[IX_PassagesCheckpoint_CheckpointId]', N'IX_T_PassagesCheckpoint_CheckpointId', 'INDEX';

EXEC sp_rename N'[dbo].[T_HistoriqueScans].[IX_HistoriqueScans_ScanId]', N'IX_T_HistoriqueScans_ScanId', 'INDEX';

EXEC sp_rename N'[dbo].[T_Employees].[IX_Employees_DepartementId]', N'IX_T_Employees_DepartementId', 'INDEX';

EXEC sp_rename N'[dbo].[T_Employees].[IX_Employees_CompagnieId]', N'IX_T_Employees_CompagnieId', 'INDEX';

EXEC sp_rename N'[dbo].[T_Contrats].[IX_Contrats_PoNumber]', N'IX_T_Contrats_PoNumber', 'INDEX';

EXEC sp_rename N'[dbo].[T_Contrats].[IX_Contrats_CompagnieId]', N'IX_T_Contrats_CompagnieId', 'INDEX';

EXEC sp_rename N'[dbo].[T_Checkpoints].[IX_Checkpoints_SiteId]', N'IX_T_Checkpoints_SiteId', 'INDEX';

EXEC sp_rename N'[dbo].[T_BonEntreeHistory].[IX_BonEntreeHistory_BonIdBon]', N'IX_T_BonEntreeHistory_BonIdBon', 'INDEX';

EXEC sp_rename N'[dbo].[T_Anomalies].[IX_Anomalies_BarriereId]', N'IX_T_Anomalies_BarriereId', 'INDEX';

ALTER TABLE [dbo].[T_Utilisateurs] ADD CONSTRAINT [PK_T_Utilisateurs] PRIMARY KEY ([IdUtilisateur]);

ALTER TABLE [dbo].[T_UtilisateurRoles] ADD CONSTRAINT [PK_T_UtilisateurRoles] PRIMARY KEY ([IdUtilisateurRole]);

ALTER TABLE [dbo].[T_TypesMateriels] ADD CONSTRAINT [PK_T_TypesMateriels] PRIMARY KEY ([IdTypeMateriel]);

ALTER TABLE [dbo].[T_Statuts] ADD CONSTRAINT [PK_T_Statuts] PRIMARY KEY ([IdStatut]);

ALTER TABLE [dbo].[T_SoldesMateriels] ADD CONSTRAINT [PK_T_SoldesMateriels] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_Sites] ADD CONSTRAINT [PK_T_Sites] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_ScanEvenements] ADD CONSTRAINT [PK_T_ScanEvenements] PRIMARY KEY ([IdScan]);

ALTER TABLE [dbo].[T_Roles] ADD CONSTRAINT [PK_T_Roles] PRIMARY KEY ([IdRole]);

ALTER TABLE [dbo].[T_RaisonsSortie] ADD CONSTRAINT [PK_T_RaisonsSortie] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_PassagesCheckpoint] ADD CONSTRAINT [PK_T_PassagesCheckpoint] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_ParametresSysteme] ADD CONSTRAINT [PK_T_ParametresSysteme] PRIMARY KEY ([IdParametre]);

ALTER TABLE [dbo].[T_NotificationsRejet] ADD CONSTRAINT [PK_T_NotificationsRejet] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_MaterielsSortie] ADD CONSTRAINT [PK_T_MaterielsSortie] PRIMARY KEY ([IdMateriel]);

ALTER TABLE [dbo].[T_Materiels] ADD CONSTRAINT [PK_T_Materiels] PRIMARY KEY ([IdMateriel]);

ALTER TABLE [dbo].[T_ItinerairesSortie] ADD CONSTRAINT [PK_T_ItinerairesSortie] PRIMARY KEY ([IdItineraire]);

ALTER TABLE [dbo].[T_ItinerairesPrevu] ADD CONSTRAINT [PK_T_ItinerairesPrevu] PRIMARY KEY ([IdItineraire]);

ALTER TABLE [dbo].[T_HistoriqueScans] ADD CONSTRAINT [PK_T_HistoriqueScans] PRIMARY KEY ([IdHistorique]);

ALTER TABLE [dbo].[T_Employees] ADD CONSTRAINT [PK_T_Employees] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_Departements] ADD CONSTRAINT [PK_T_Departements] PRIMARY KEY ([IdDepartement]);

ALTER TABLE [dbo].[T_Contrats] ADD CONSTRAINT [PK_T_Contrats] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_Compagnies] ADD CONSTRAINT [PK_T_Compagnies] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_Checkpoints] ADD CONSTRAINT [PK_T_Checkpoints] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_CategoriesSortie] ADD CONSTRAINT [PK_T_CategoriesSortie] PRIMARY KEY ([Id]);

ALTER TABLE [dbo].[T_BonsSortie] ADD CONSTRAINT [PK_T_BonsSortie] PRIMARY KEY ([IdBon]);

ALTER TABLE [dbo].[T_BonSortieHistories] ADD CONSTRAINT [PK_T_BonSortieHistories] PRIMARY KEY ([IdHistory]);

ALTER TABLE [dbo].[T_Bons] ADD CONSTRAINT [PK_T_Bons] PRIMARY KEY ([IdBon]);

ALTER TABLE [dbo].[T_BonEntreeHistory] ADD CONSTRAINT [PK_T_BonEntreeHistory] PRIMARY KEY ([IdHistory]);

ALTER TABLE [dbo].[T_Barrieres] ADD CONSTRAINT [PK_T_Barrieres] PRIMARY KEY ([IdBarriere]);

ALTER TABLE [dbo].[T_AuditLogs] ADD CONSTRAINT [PK_T_AuditLogs] PRIMARY KEY ([IdAuditLog]);

ALTER TABLE [dbo].[T_ApprobationsSortie] ADD CONSTRAINT [PK_T_ApprobationsSortie] PRIMARY KEY ([IdApprobation]);

ALTER TABLE [dbo].[T_Approbations] ADD CONSTRAINT [PK_T_Approbations] PRIMARY KEY ([IdApprobation]);

ALTER TABLE [dbo].[T_Anomalies] ADD CONSTRAINT [PK_T_Anomalies] PRIMARY KEY ([IdAnomalie]);

ALTER TABLE [dbo].[T_Anomalies] ADD CONSTRAINT [FK_T_Anomalies_T_Barrieres_BarriereId] FOREIGN KEY ([BarriereId]) REFERENCES [dbo].[T_Barrieres] ([IdBarriere]) ON DELETE NO ACTION;

ALTER TABLE [dbo].[T_Anomalies] ADD CONSTRAINT [FK_T_Anomalies_T_ScanEvenements_ScanId] FOREIGN KEY ([ScanId]) REFERENCES [dbo].[T_ScanEvenements] ([IdScan]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_Approbations] ADD CONSTRAINT [FK_T_Approbations_T_Bons_BonId] FOREIGN KEY ([BonId]) REFERENCES [dbo].[T_Bons] ([IdBon]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_ApprobationsSortie] ADD CONSTRAINT [FK_T_ApprobationsSortie_T_BonsSortie_BonSortieId] FOREIGN KEY ([BonSortieId]) REFERENCES [dbo].[T_BonsSortie] ([IdBon]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_BonEntreeHistory] ADD CONSTRAINT [FK_T_BonEntreeHistory_T_Bons_BonId] FOREIGN KEY ([BonId]) REFERENCES [dbo].[T_Bons] ([IdBon]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_BonEntreeHistory] ADD CONSTRAINT [FK_T_BonEntreeHistory_T_Bons_BonIdBon] FOREIGN KEY ([BonIdBon]) REFERENCES [dbo].[T_Bons] ([IdBon]);

ALTER TABLE [dbo].[T_BonSortieHistories] ADD CONSTRAINT [FK_T_BonSortieHistories_T_BonsSortie_BonSortieId] FOREIGN KEY ([BonSortieId]) REFERENCES [dbo].[T_BonsSortie] ([IdBon]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_BonsSortie] ADD CONSTRAINT [FK_T_BonsSortie_T_Bons_BonEntreeAssocieId] FOREIGN KEY ([BonEntreeAssocieId]) REFERENCES [dbo].[T_Bons] ([IdBon]) ON DELETE SET NULL;

ALTER TABLE [dbo].[T_Checkpoints] ADD CONSTRAINT [FK_T_Checkpoints_T_Sites_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [dbo].[T_Sites] ([Id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_Contrats] ADD CONSTRAINT [FK_T_Contrats_T_Compagnies_CompagnieId] FOREIGN KEY ([CompagnieId]) REFERENCES [dbo].[T_Compagnies] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [dbo].[T_Employees] ADD CONSTRAINT [FK_T_Employees_T_Compagnies_CompagnieId] FOREIGN KEY ([CompagnieId]) REFERENCES [dbo].[T_Compagnies] ([Id]);

ALTER TABLE [dbo].[T_Employees] ADD CONSTRAINT [FK_T_Employees_T_Departements_DepartementId] FOREIGN KEY ([DepartementId]) REFERENCES [dbo].[T_Departements] ([IdDepartement]);

ALTER TABLE [dbo].[T_HistoriqueScans] ADD CONSTRAINT [FK_T_HistoriqueScans_T_ScanEvenements_ScanId] FOREIGN KEY ([ScanId]) REFERENCES [dbo].[T_ScanEvenements] ([IdScan]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_ItinerairesPrevu] ADD CONSTRAINT [FK_T_ItinerairesPrevu_T_Bons_BonId] FOREIGN KEY ([BonId]) REFERENCES [dbo].[T_Bons] ([IdBon]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_ItinerairesSortie] ADD CONSTRAINT [FK_T_ItinerairesSortie_T_BonsSortie_BonSortieId] FOREIGN KEY ([BonSortieId]) REFERENCES [dbo].[T_BonsSortie] ([IdBon]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_Materiels] ADD CONSTRAINT [FK_T_Materiels_T_Bons_BonId] FOREIGN KEY ([BonId]) REFERENCES [dbo].[T_Bons] ([IdBon]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_MaterielsSortie] ADD CONSTRAINT [FK_T_MaterielsSortie_T_BonsSortie_BonSortieId] FOREIGN KEY ([BonSortieId]) REFERENCES [dbo].[T_BonsSortie] ([IdBon]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_PassagesCheckpoint] ADD CONSTRAINT [FK_T_PassagesCheckpoint_T_Checkpoints_CheckpointId] FOREIGN KEY ([CheckpointId]) REFERENCES [dbo].[T_Checkpoints] ([Id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_RaisonsSortie] ADD CONSTRAINT [FK_T_RaisonsSortie_T_CategoriesSortie_CategorieId] FOREIGN KEY ([CategorieId]) REFERENCES [dbo].[T_CategoriesSortie] ([Id]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_ScanEvenements] ADD CONSTRAINT [FK_T_ScanEvenements_T_Anomalies_AnomalieIdAnomalie] FOREIGN KEY ([AnomalieIdAnomalie]) REFERENCES [dbo].[T_Anomalies] ([IdAnomalie]);

ALTER TABLE [dbo].[T_ScanEvenements] ADD CONSTRAINT [FK_T_ScanEvenements_T_Barrieres_BarriereId] FOREIGN KEY ([BarriereId]) REFERENCES [dbo].[T_Barrieres] ([IdBarriere]) ON DELETE NO ACTION;

ALTER TABLE [dbo].[T_UtilisateurRoles] ADD CONSTRAINT [FK_T_UtilisateurRoles_T_Roles_IdRole] FOREIGN KEY ([IdRole]) REFERENCES [dbo].[T_Roles] ([IdRole]) ON DELETE CASCADE;

ALTER TABLE [dbo].[T_UtilisateurRoles] ADD CONSTRAINT [FK_T_UtilisateurRoles_T_Utilisateurs_IdUtilisateur] FOREIGN KEY ([IdUtilisateur]) REFERENCES [dbo].[T_Utilisateurs] ([IdUtilisateur]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260304085005_RenameTablesToDboConvention', N'10.0.2');

COMMIT;
GO

