-- Script de mise à jour des rôles pour KCC Material Flow
-- Basé sur la chaîne d'approbation définie dans RoleUtilisateur.cs et WorkflowService.cs
-- Date: 2026-02-17

USE [AppDev_KCCMaterialFlow_DB_Dev];
GO

-- ============================================
-- Étape 1: Voir les rôles actuels
-- ============================================
PRINT 'Rôles existants avant mise à jour:';
SELECT IdRole, CodeRole, NomRole FROM dbo.Roles ORDER BY IdRole;
GO

-- ============================================
-- Étape 2: Mettre à jour les rôles existants
-- ============================================

-- Mettre à jour ADMIN
UPDATE dbo.Roles 
SET NomRole = 'Administrateur',
    Description = 'Accès complet au système. Gestion des utilisateurs, paramètres et configuration.',
    NiveauPriorite = 100,
    DateModification = GETDATE()
WHERE CodeRole = 'ADMIN';

-- Mettre à jour SUPERVISEUR
UPDATE dbo.Roles 
SET NomRole = 'Superviseur',
    Description = 'Première validation hiérarchique. Valide les demandes de son équipe.',
    NiveauPriorite = 20,
    DateModification = GETDATE()
WHERE CodeRole = 'SUPERVISEUR';

-- Renommer APPROBATEUR en GM (General Manager)
UPDATE dbo.Roles 
SET CodeRole = 'GM',
    NomRole = 'General Manager',
    Description = 'Validation direction. Approuve les bons au niveau département.',
    NiveauPriorite = 30,
    DateModification = GETDATE()
WHERE CodeRole = 'APPROBATEUR';

-- Renommer AGENT_SECURITE en Barriere
UPDATE dbo.Roles 
SET CodeRole = 'Barriere',
    NomRole = 'Agent Barrière',
    Description = 'Scan QR codes, contrôle physique aux checkpoints de sortie/entrée.',
    NiveauPriorite = 70,
    EstSysteme = 0,
    DateModification = GETDATE()
WHERE CodeRole = 'AGENT_SECURITE';

-- Renommer UTILISATEUR en Demandeur
UPDATE dbo.Roles 
SET CodeRole = 'Demandeur',
    NomRole = 'Demandeur',
    Description = 'Peut créer des bons et suivre ses demandes. Rôle par défaut pour tout utilisateur.',
    NiveauPriorite = 10,
    EstSysteme = 0,
    DateModification = GETDATE()
WHERE CodeRole = 'UTILISATEUR';

GO

-- ============================================
-- Étape 3: Ajouter les nouveaux rôles manquants
-- ============================================

-- OPJ
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE CodeRole = 'OPJ')
BEGIN
    INSERT INTO dbo.Roles (CodeRole, NomRole, Description, NiveauPriorite, EstActif, EstSysteme, DateCreation)
    VALUES ('OPJ', 'Officier de Police Judiciaire', 'Validation sécurité. Contrôle la conformité des sorties sensibles.', 40, 1, 1, GETDATE());
    PRINT 'Rôle OPJ ajouté';
END

-- IT
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE CodeRole = 'IT')
BEGIN
    INSERT INTO dbo.Roles (CodeRole, NomRole, Description, NiveauPriorite, EstActif, EstSysteme, DateCreation)
    VALUES ('IT', 'Département IT', 'Approbation du matériel informatique (ordinateurs, équipements réseau, etc.)', 50, 1, 1, GETDATE());
    PRINT 'Rôle IT ajouté';
END

-- Environnement
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE CodeRole = 'Environnement')
BEGIN
    INSERT INTO dbo.Roles (CodeRole, NomRole, Description, NiveauPriorite, EstActif, EstSysteme, DateCreation)
    VALUES ('Environnement', 'Département Environnement', 'Approbation résidus, radioprotection, matériel avec impact environnemental.', 50, 1, 1, GETDATE());
    PRINT 'Rôle Environnement ajouté';
END

-- Identification
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE CodeRole = 'Identification')
BEGIN
    INSERT INTO dbo.Roles (CodeRole, NomRole, Description, NiveauPriorite, EstActif, EstSysteme, DateCreation)
    VALUES ('Identification', 'Équipe Identification', 'Vérification finale, génération des QR codes, gestion des extensions de prêts.', 60, 1, 1, GETDATE());
    PRINT 'Rôle Identification ajouté';
END

-- Investigation
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE CodeRole = 'Investigation')
BEGIN
    INSERT INTO dbo.Roles (CodeRole, NomRole, Description, NiveauPriorite, EstActif, EstSysteme, DateCreation)
    VALUES ('Investigation', 'Investigation', 'Traitement des anomalies et enquêtes sur les incidents.', 80, 1, 1, GETDATE());
    PRINT 'Rôle Investigation ajouté';
END

GO

-- ============================================
-- Étape 4: Mettre à jour les utilisateurs avec les nouveaux codes
-- ============================================

-- Les utilisateurs avec UTILISATEUR deviennent Demandeur
UPDATE shared.Utilisateurs
SET Role = 'Demandeur'
WHERE Role IN ('UTILISATEUR', 'Utilisateur', 'USER', 'user') OR Role IS NULL;

-- Les utilisateurs avec AGENT_SECURITE deviennent Barriere
UPDATE shared.Utilisateurs
SET Role = 'Barriere'
WHERE Role IN ('AGENT_SECURITE', 'SecurityAgent', 'SECURITY', 'Agent_Securite', 'AgentSecurite');

-- Les utilisateurs avec APPROBATEUR deviennent GM
UPDATE shared.Utilisateurs
SET Role = 'GM'
WHERE Role IN ('APPROBATEUR', 'Approbateur');

-- Normaliser Admin
UPDATE shared.Utilisateurs
SET Role = 'Admin'
WHERE Role IN ('ADMIN', 'ADMINISTRATEUR', 'Administrator');

-- Normaliser Superviseur
UPDATE shared.Utilisateurs
SET Role = 'Superviseur'
WHERE Role IN ('SUPERVISEUR', 'SUPER', 'Sup', 'SUP');

GO

-- ============================================
-- Étape 5: Afficher les résultats
-- ============================================
PRINT '';
PRINT '=== RÔLES APRÈS MISE À JOUR ===';
SELECT 
    IdRole,
    CodeRole,
    NomRole,
    LEFT(Description, 60) AS Description,
    NiveauPriorite,
    CASE WHEN EstActif = 1 THEN 'Oui' ELSE 'Non' END AS Actif,
    CASE WHEN EstSysteme = 1 THEN 'Oui' ELSE 'Non' END AS Systeme
FROM dbo.Roles
ORDER BY NiveauPriorite;

PRINT '';
PRINT '=== UTILISATEURS PAR RÔLE ===';
SELECT 
    ISNULL(u.Role, '(non défini)') AS Role,
    COUNT(*) AS NombreUtilisateurs
FROM shared.Utilisateurs u
GROUP BY u.Role
ORDER BY COUNT(*) DESC;

PRINT '';
PRINT 'Mise à jour des rôles terminée avec succès!';
GO
