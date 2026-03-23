BEGIN TRANSACTION;
CREATE TABLE [dbo].[T_Activites] (
    [IdActivite] int NOT NULL IDENTITY,
    [CodeActivite] nvarchar(50) NOT NULL,
    [NomActivite] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Module] nvarchar(50) NOT NULL,
    [Categorie] nvarchar(50) NULL,
    [OrdreAffichage] int NOT NULL DEFAULT 0,
    [EstActif] bit NOT NULL DEFAULT CAST(1 AS bit),
    [EstSysteme] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DateCreation] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_T_Activites] PRIMARY KEY ([IdActivite])
);

CREATE TABLE [dbo].[T_UtilisateurActivites] (
    [IdUtilisateurActivite] int NOT NULL IDENTITY,
    [IdUtilisateur] int NOT NULL,
    [IdActivite] int NOT NULL,
    [DateAttribution] datetime2 NOT NULL DEFAULT (GETDATE()),
    [AttribueParLogin] nvarchar(100) NULL,
    [EstActif] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_T_UtilisateurActivites] PRIMARY KEY ([IdUtilisateurActivite]),
    CONSTRAINT [FK_T_UtilisateurActivites_T_Activites_IdActivite] FOREIGN KEY ([IdActivite]) REFERENCES [dbo].[T_Activites] ([IdActivite]) ON DELETE CASCADE,
    CONSTRAINT [FK_T_UtilisateurActivites_T_Utilisateurs_IdUtilisateur] FOREIGN KEY ([IdUtilisateur]) REFERENCES [dbo].[T_Utilisateurs] ([IdUtilisateur]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdActivite', N'Categorie', N'CodeActivite', N'DateCreation', N'Description', N'EstActif', N'EstSysteme', N'Module', N'NomActivite', N'OrdreAffichage') AND [object_id] = OBJECT_ID(N'[dbo].[T_Activites]'))
    SET IDENTITY_INSERT [dbo].[T_Activites] ON;
INSERT INTO [dbo].[T_Activites] ([IdActivite], [Categorie], [CodeActivite], [DateCreation], [Description], [EstActif], [EstSysteme], [Module], [NomActivite], [OrdreAffichage])
VALUES (1, N'Création', N'BEM_CREER', '2024-01-01T00:00:00.0000000', N'Saisir et enregistrer un nouveau bon d''entrée matériel', CAST(1 AS bit), CAST(1 AS bit), N'BonEntree', N'Créer un Bon d''Entrée', 10),
(2, N'Modification', N'BEM_MODIFIER', '2024-01-01T00:00:00.0000000', N'Éditer un bon d''entrée en brouillon', CAST(1 AS bit), CAST(1 AS bit), N'BonEntree', N'Modifier un Bon d''Entrée', 20),
(3, N'Workflow', N'BEM_SOUMETTRE', '2024-01-01T00:00:00.0000000', N'Envoyer un bon d''entrée en approbation', CAST(1 AS bit), CAST(1 AS bit), N'BonEntree', N'Soumettre un Bon d''Entrée', 30),
(4, N'Approbation', N'BEM_APPROUVER', '2024-01-01T00:00:00.0000000', N'Valider et approuver un bon d''entrée soumis', CAST(1 AS bit), CAST(1 AS bit), N'BonEntree', N'Approuver un Bon d''Entrée', 40),
(5, N'Approbation', N'BEM_REJETER', '2024-01-01T00:00:00.0000000', N'Rejeter un bon d''entrée avec motif obligatoire', CAST(1 AS bit), CAST(1 AS bit), N'BonEntree', N'Rejeter un Bon d''Entrée', 50),
(6, N'Approbation', N'BEM_RETOURNER', '2024-01-01T00:00:00.0000000', N'Renvoyer un bon d''entrée au demandeur pour corrections', CAST(1 AS bit), CAST(1 AS bit), N'BonEntree', N'Retourner un Bon d''Entrée', 60),
(7, N'Suppression', N'BEM_SUPPRIMER', '2024-01-01T00:00:00.0000000', N'Supprimer un bon d''entrée en statut brouillon', CAST(1 AS bit), CAST(1 AS bit), N'BonEntree', N'Supprimer un brouillon BEM', 70),
(8, N'Export', N'BEM_IMPRIMER', '2024-01-01T00:00:00.0000000', N'Imprimer ou télécharger le PDF d''un bon d''entrée', CAST(1 AS bit), CAST(1 AS bit), N'BonEntree', N'Imprimer / Exporter PDF un BEM', 80),
(10, N'Création', N'BSM_CREER', '2024-01-01T00:00:00.0000000', N'Saisir et enregistrer un nouveau bon de sortie matériel', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Créer un Bon de Sortie', 100),
(11, N'Modification', N'BSM_MODIFIER', '2024-01-01T00:00:00.0000000', N'Éditer un bon de sortie en brouillon', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Modifier un Bon de Sortie', 110),
(12, N'Workflow', N'BSM_SOUMETTRE', '2024-01-01T00:00:00.0000000', N'Envoyer un bon de sortie dans la chaîne d''approbation', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Soumettre un Bon de Sortie', 120),
(13, N'Approbation', N'BSM_APPROUVER', '2024-01-01T00:00:00.0000000', N'Valider et approuver un bon de sortie à l''étape courante', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Approuver un Bon de Sortie', 130),
(14, N'Approbation', N'BSM_REJETER', '2024-01-01T00:00:00.0000000', N'Rejeter un bon de sortie avec motif obligatoire', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Rejeter un Bon de Sortie', 140),
(15, N'Approbation', N'BSM_RETOURNER', '2024-01-01T00:00:00.0000000', N'Renvoyer un bon de sortie au demandeur pour corrections', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Retourner un Bon de Sortie', 150),
(16, N'Suppression', N'BSM_SUPPRIMER', '2024-01-01T00:00:00.0000000', N'Supprimer un bon de sortie en statut brouillon', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Supprimer un brouillon BSM', 160),
(17, N'Export', N'BSM_IMPRIMER', '2024-01-01T00:00:00.0000000', N'Imprimer ou télécharger le PDF d''un bon de sortie', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Imprimer / Exporter PDF un BSM', 170),
(18, N'Prêts', N'PRET_RETOUR', '2024-01-01T00:00:00.0000000', N'Confirmer le retour d''un matériel prêté', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Enregistrer un retour de prêt', 180),
(19, N'Prêts', N'PRET_EXTENSION', '2024-01-01T00:00:00.0000000', N'Prolonger la date de retour d''un prêt en cours', CAST(1 AS bit), CAST(1 AS bit), N'BonSortie', N'Demander une extension de prêt', 190),
(20, N'Scan', N'SEC_SCANNER', '2024-01-01T00:00:00.0000000', N'Scanner un QR code à la barrière pour contrôler un passage', CAST(1 AS bit), CAST(1 AS bit), N'Securite', N'Scanner un QR Code', 200),
(21, N'Scan', N'SEC_CONFIRMER_PASSAGE', '2024-01-01T00:00:00.0000000', N'Valider le passage d''un matériel à la barrière après scan', CAST(1 AS bit), CAST(1 AS bit), N'Securite', N'Confirmer un passage', 210),
(22, N'Anomalies', N'SEC_SIGNALER_ANOMALIE', '2024-01-01T00:00:00.0000000', N'Signaler manuellement une anomalie lors d''un contrôle', CAST(1 AS bit), CAST(1 AS bit), N'Securite', N'Signaler une anomalie', 220),
(23, N'Anomalies', N'SEC_TRAITER_ANOMALIE', '2024-01-01T00:00:00.0000000', N'Résoudre une anomalie avec commentaire et action corrective', CAST(1 AS bit), CAST(1 AS bit), N'Securite', N'Traiter une anomalie', 230),
(24, N'Anomalies', N'SEC_REOUVRIR_ANOMALIE', '2024-01-01T00:00:00.0000000', N'Réouvrir une anomalie traitée pour investigation complémentaire', CAST(1 AS bit), CAST(1 AS bit), N'Securite', N'Réouvrir une anomalie', 240),
(25, N'Consultation', N'SEC_VOIR_HISTORIQUE', '2024-01-01T00:00:00.0000000', N'Voir l''historique complet des scans QR et passages', CAST(1 AS bit), CAST(1 AS bit), N'Securite', N'Consulter l''historique des scans', 250),
(26, N'Administration', N'SEC_GERER_BARRIERES', '2024-01-01T00:00:00.0000000', N'Créer, modifier, activer/désactiver les barrières de contrôle', CAST(1 AS bit), CAST(1 AS bit), N'Securite', N'Gérer les barrières', 260),
(27, N'Administration', N'SEC_GERER_ITINERAIRES', '2024-01-01T00:00:00.0000000', N'Configurer les itinéraires et séquences de checkpoints', CAST(1 AS bit), CAST(1 AS bit), N'Securite', N'Gérer les itinéraires', 270),
(28, N'Administration', N'SEC_GERER_AGENTS', '2024-01-01T00:00:00.0000000', N'Affecter et gérer les agents aux barrières', CAST(1 AS bit), CAST(1 AS bit), N'Securite', N'Gérer les agents de barrière', 280),
(30, N'Administration', N'ADMIN_UTILISATEURS', '2024-01-01T00:00:00.0000000', N'Créer, modifier, activer/désactiver les utilisateurs', CAST(1 AS bit), CAST(1 AS bit), N'Admin', N'Gérer les utilisateurs', 300),
(31, N'Administration', N'ADMIN_ROLES', '2024-01-01T00:00:00.0000000', N'Créer, modifier les rôles et leurs permissions', CAST(1 AS bit), CAST(1 AS bit), N'Admin', N'Gérer les rôles', 310),
(32, N'Administration', N'ADMIN_DEPARTEMENTS', '2024-01-01T00:00:00.0000000', N'Créer, modifier, activer/désactiver les départements', CAST(1 AS bit), CAST(1 AS bit), N'Admin', N'Gérer les départements', 320),
(33, N'Administration', N'ADMIN_TYPES_MATERIELS', '2024-01-01T00:00:00.0000000', N'Créer, modifier les types de matériel', CAST(1 AS bit), CAST(1 AS bit), N'Admin', N'Gérer les types de matériel', 330),
(34, N'Administration', N'ADMIN_STATUTS', '2024-01-01T00:00:00.0000000', N'Créer, modifier les statuts de workflow', CAST(1 AS bit), CAST(1 AS bit), N'Admin', N'Gérer les statuts', 340),
(35, N'Administration', N'ADMIN_PARAMETRES', '2024-01-01T00:00:00.0000000', N'Configurer les paramètres globaux de l''application', CAST(1 AS bit), CAST(1 AS bit), N'Admin', N'Gérer les paramètres système', 350),
(36, N'Administration', N'ADMIN_AUDIT', '2024-01-01T00:00:00.0000000', N'Voir les logs d''audit des actions système', CAST(1 AS bit), CAST(1 AS bit), N'Admin', N'Consulter le journal d''audit', 360),
(37, N'Administration', N'ADMIN_ACTIVITES', '2024-01-01T00:00:00.0000000', N'Assigner et retirer des activités aux utilisateurs', CAST(1 AS bit), CAST(1 AS bit), N'Admin', N'Gérer les activités utilisateurs', 370),
(40, N'Consultation', N'VOIR_TOUS_BONS', '2024-01-01T00:00:00.0000000', N'Consulter la liste complète de tous les bons du système', CAST(1 AS bit), CAST(1 AS bit), N'Transversal', N'Voir tous les bons', 400),
(41, N'Approbation', N'VOIR_APPROBATIONS', '2024-01-01T00:00:00.0000000', N'Consulter la liste des bons en attente d''approbation', CAST(1 AS bit), CAST(1 AS bit), N'Transversal', N'Voir les approbations en attente', 410),
(42, N'Export', N'EXPORT_EXCEL', '2024-01-01T00:00:00.0000000', N'Exporter les listes de bons et l''historique au format Excel', CAST(1 AS bit), CAST(1 AS bit), N'Transversal', N'Exporter les données en Excel', 420),
(43, N'Consultation', N'VOIR_HISTORIQUE', '2024-01-01T00:00:00.0000000', N'Consulter l''historique complet des bons et mouvements', CAST(1 AS bit), CAST(1 AS bit), N'Transversal', N'Consulter l''historique', 430),
(44, N'Consultation', N'VOIR_TABLEAU_BORD', '2024-01-01T00:00:00.0000000', N'Accéder au tableau de bord avec statistiques et raccourcis', CAST(1 AS bit), CAST(1 AS bit), N'Transversal', N'Voir le tableau de bord', 440);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IdActivite', N'Categorie', N'CodeActivite', N'DateCreation', N'Description', N'EstActif', N'EstSysteme', N'Module', N'NomActivite', N'OrdreAffichage') AND [object_id] = OBJECT_ID(N'[dbo].[T_Activites]'))
    SET IDENTITY_INSERT [dbo].[T_Activites] OFF;

CREATE INDEX [IX_Activites_Categorie] ON [dbo].[T_Activites] ([Categorie]);

CREATE UNIQUE INDEX [IX_Activites_CodeActivite] ON [dbo].[T_Activites] ([CodeActivite]);

CREATE INDEX [IX_Activites_EstActif] ON [dbo].[T_Activites] ([EstActif]);

CREATE INDEX [IX_Activites_Module] ON [dbo].[T_Activites] ([Module]);

CREATE INDEX [IX_UtilisateurActivites_IdActivite] ON [dbo].[T_UtilisateurActivites] ([IdActivite]);

CREATE INDEX [IX_UtilisateurActivites_IdUtilisateur] ON [dbo].[T_UtilisateurActivites] ([IdUtilisateur]);

CREATE UNIQUE INDEX [IX_UtilisateurActivites_Utilisateur_Activite] ON [dbo].[T_UtilisateurActivites] ([IdUtilisateur], [IdActivite]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260312130834_AddActiviteTable', N'10.0.2');

COMMIT;
GO

