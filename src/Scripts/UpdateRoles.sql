-- Script de mise à jour des rôles pour KCC Material Flow
-- Basé sur la chaîne d'approbation définie dans RoleUtilisateur.cs et WorkflowService.cs
-- Date: 2026-02-17

USE [AppDev_KCCMaterialFlow_DB_Dev];
GO

-- ============================================
-- Étape 1: Nettoyer les rôles existants (optionnel - à décommenter si nécessaire)
-- ============================================
-- DELETE FROM shared.UtilisateurRoles;
-- DELETE FROM shared.Roles;
-- GO

-- ============================================
-- Étape 2: Insérer/Mettre à jour les rôles selon la chaîne d'approbation
-- ============================================

-- Utilisation de MERGE pour insérer ou mettre à jour
MERGE INTO dbo.Roles AS target
USING (
    VALUES 
    -- Rôles de base du workflow d'approbation (selon RoleUtilisateur enum)
    (1,  'Demandeur',       'Demandeur',                    'Peut créer des bons et suivre ses demandes. Rôle par défaut pour tout utilisateur.', 10, 1, 0),
    (2,  'Superviseur',     'Superviseur',                  'Première validation hiérarchique. Valide les demandes de son équipe.', 20, 1, 1),
    (3,  'GM',              'General Manager',              'Validation direction. Approuve les bons au niveau département.', 30, 1, 1),
    (4,  'OPJ',             'Officier de Police Judiciaire', 'Validation sécurité. Contrôle la conformité des sorties sensibles.', 40, 1, 1),
    (5,  'IT',              'Département IT',               'Approbation du matériel informatique (ordinateurs, équipements réseau, etc.)', 50, 1, 1),
    (6,  'Environnement',   'Département Environnement',    'Approbation résidus, radioprotection, matériel avec impact environnemental.', 50, 1, 1),
    (7,  'Identification',  'Équipe Identification',        'Vérification finale, génération des QR codes, gestion des extensions de prêts.', 60, 1, 1),
    (8,  'Barriere',        'Agent Barrière',               'Scan QR codes, contrôle physique aux checkpoints de sortie/entrée.', 70, 1, 0),
    (9,  'Investigation',   'Investigation',                'Traitement des anomalies et enquêtes sur les incidents.', 80, 1, 1),
    (10, 'Admin',           'Administrateur',               'Accès complet au système. Gestion des utilisateurs, paramètres et configuration.', 100, 1, 1)
) AS source (IdRole, CodeRole, NomRole, Description, NiveauPriorite, EstActif, EstSysteme)
ON target.IdRole = source.IdRole
WHEN MATCHED THEN
    UPDATE SET 
        CodeRole = source.CodeRole,
        NomRole = source.NomRole,
        Description = source.Description,
        NiveauPriorite = source.NiveauPriorite,
        EstActif = source.EstActif,
        EstSysteme = source.EstSysteme,
        DateModification = GETDATE()
WHEN NOT MATCHED THEN
    INSERT (IdRole, CodeRole, NomRole, Description, NiveauPriorite, EstActif, EstSysteme, DateCreation)
    VALUES (source.IdRole, source.CodeRole, source.NomRole, source.Description, source.NiveauPriorite, source.EstActif, source.EstSysteme, GETDATE());
GO

-- ============================================
-- Étape 3: Vérifier que la colonne Role des utilisateurs utilise les bons codes
-- ============================================
-- Mise à jour des utilisateurs avec des anciens codes de rôle
UPDATE shared.Utilisateurs
SET Role = 'Demandeur'
WHERE Role IN ('UTILISATEUR', 'Utilisateur', 'USER', 'user') OR Role IS NULL;

UPDATE shared.Utilisateurs
SET Role = 'Admin'
WHERE Role IN ('ADMIN', 'ADMINISTRATEUR', 'Administrator');

UPDATE shared.Utilisateurs
SET Role = 'Superviseur'
WHERE Role IN ('SUPERVISEUR', 'SUPER', 'Sup', 'SUP');

UPDATE shared.Utilisateurs
SET Role = 'Barriere'
WHERE Role IN ('AGENT_SECURITE', 'SecurityAgent', 'SECURITY', 'Agent_Securite', 'AgentSecurite');

UPDATE shared.Utilisateurs
SET Role = 'Identification'
WHERE Role IN ('IDENTIFICATION', 'Ident', 'IDENT');

UPDATE shared.Utilisateurs
SET Role = 'GM'
WHERE Role IN ('GENERAL_MANAGER', 'GeneralManager', 'DG', 'Directeur');

UPDATE shared.Utilisateurs
SET Role = 'OPJ'
WHERE Role IN ('POLICE', 'Police', 'OfficierPolice');

UPDATE shared.Utilisateurs
SET Role = 'IT'
WHERE Role IN ('INFORMATIQUE', 'Informatique', 'TI', 'Tech');

UPDATE shared.Utilisateurs
SET Role = 'Environnement'
WHERE Role IN ('ENV', 'ENVIRONNEMENT', 'Environment');

UPDATE shared.Utilisateurs
SET Role = 'Investigation'
WHERE Role IN ('INVESTIGATION', 'Enquete', 'ENQUETE');

GO

-- ============================================
-- Étape 4: Afficher les rôles mis à jour
-- ============================================
SELECT 
    IdRole,
    CodeRole,
    NomRole,
    Description,
    NiveauPriorite,
    CASE WHEN EstActif = 1 THEN 'Oui' ELSE 'Non' END AS Actif,
    CASE WHEN EstSysteme = 1 THEN 'Oui' ELSE 'Non' END AS Systeme
FROM dbo.Roles
ORDER BY NiveauPriorite;
GO

-- ============================================
-- Étape 5: Résumé des utilisateurs par rôle
-- ============================================
SELECT 
    ISNULL(u.Role, '(non défini)') AS Role,
    COUNT(*) AS NombreUtilisateurs
FROM shared.Utilisateurs u
WHERE u.EstActif = 1
GROUP BY u.Role
ORDER BY COUNT(*) DESC;
GO

PRINT 'Mise à jour des rôles terminée avec succès!';
GO
