-- ============================================
-- SCRIPT DE DEPLOIEMENT - AppDev_SecurityPortal_DB
-- Base de donnees de test KCC Material Flow
-- Date: 2026-03-19
-- ============================================

USE [master]
GO

-- ============================================
-- ETAPE 1: CREATION DE LA BASE DE DONNEES (si elle n'existe pas)
-- ============================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'AppDev_SecurityPortal_DB')
BEGIN
    CREATE DATABASE [AppDev_SecurityPortal_DB]
    PRINT 'Base de donnees AppDev_SecurityPortal_DB creee avec succes.'
END
ELSE
BEGIN
    PRINT 'La base de donnees AppDev_SecurityPortal_DB existe deja.'
END
GO

USE [AppDev_SecurityPortal_DB]
GO

-- ============================================
-- ETAPE 2: CREATION DES SCHEMAS
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'ref')
BEGIN
    EXEC('CREATE SCHEMA ref')
    PRINT 'Schema [ref] cree.'
END
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'dbo')
BEGIN
    PRINT 'Schema [dbo] existe deja (par defaut).'
END
GO

-- ============================================
-- ETAPE 3: ACCES UTILISATEURS POUR TESTS
-- Modifier les noms d'utilisateurs selon vos besoins
-- ============================================
PRINT ''
PRINT '=== CONFIGURATION DES ACCES UTILISATEURS ==='
PRINT ''

-- Liste des utilisateurs de test (MODIFIER SELON VOS BESOINS)
-- Format: DOMAIN\Username

-- Exemple: Ajouter un utilisateur avec droits lecture/ecriture
/*
-- Utilisateur 1: Testeur principal
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'KCC\TestUser1')
BEGIN
    CREATE USER [KCC\TestUser1] FOR LOGIN [KCC\TestUser1]
    ALTER ROLE db_datareader ADD MEMBER [KCC\TestUser1]
    ALTER ROLE db_datawriter ADD MEMBER [KCC\TestUser1]
    PRINT 'Utilisateur KCC\TestUser1 ajoute avec droits lecture/ecriture.'
END
GO
*/

-- Pour ajouter vos utilisateurs, decommentez et modifiez le bloc ci-dessous:
-- ============================================

/*
-- UTILISATEURS DE TEST - DECOMMENTER ET MODIFIER
DECLARE @users TABLE (username NVARCHAR(100))
INSERT INTO @users VALUES
    ('KCC\user1'),
    ('KCC\user2'),
    ('KCC\user3')

DECLARE @username NVARCHAR(100)
DECLARE user_cursor CURSOR FOR SELECT username FROM @users

OPEN user_cursor
FETCH NEXT FROM user_cursor INTO @username

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = @username)
    BEGIN
        EXEC('CREATE USER [' + @username + '] FOR LOGIN [' + @username + ']')
        EXEC('ALTER ROLE db_datareader ADD MEMBER [' + @username + ']')
        EXEC('ALTER ROLE db_datawriter ADD MEMBER [' + @username + ']')
        PRINT 'Utilisateur ' + @username + ' ajoute.'
    END
    FETCH NEXT FROM user_cursor INTO @username
END

CLOSE user_cursor
DEALLOCATE user_cursor
*/

-- ============================================
-- ETAPE 4: DONNEES DE REFERENCE - STATUTS
-- ============================================
PRINT ''
PRINT '=== INSERTION DES STATUTS ==='

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Statuts')
BEGIN
    PRINT 'ATTENTION: La table Statuts n''existe pas encore. Executez d''abord les migrations EF Core.'
END
ELSE
BEGIN
    -- Nettoyer et inserer les statuts
    IF NOT EXISTS (SELECT 1 FROM dbo.Statuts WHERE CodeStatut = 'BROUILLON')
    BEGIN
        INSERT INTO dbo.Statuts (CodeStatut, LibelleStatut, Description, CouleurFond, CouleurTexte, OrdreAffichage, EstActif)
        VALUES
            ('BROUILLON', 'Brouillon', 'Bon en cours de creation', '#6c757d', '#ffffff', 1, 1),
            ('EN_ATTENTE_APPROBATION', 'En attente d''approbation', 'Bon soumis, en attente de validation', '#fd7e14', '#ffffff', 2, 1),
            ('APPROUVE', 'Approuve', 'Bon approuve par tous les niveaux', '#28a745', '#ffffff', 3, 1),
            ('REJETE', 'Rejete', 'Bon rejete', '#dc3545', '#ffffff', 4, 1),
            ('EN_COURS', 'En cours', 'Bon en cours de traitement', '#17a2b8', '#ffffff', 5, 1),
            ('TERMINE', 'Termine', 'Bon termine avec succes', '#28a745', '#ffffff', 6, 1),
            ('EXPIRE', 'Expire', 'Bon expire (validite depassee)', '#6c757d', '#ffffff', 7, 1),
            ('ANNULE', 'Annule', 'Bon annule', '#dc3545', '#ffffff', 8, 1),
            ('RETOURNE', 'Retourne', 'Bon retourne pour modification', '#ffc107', '#000000', 9, 1),
            ('ARCHIVE', 'Archive', 'Bon archive', '#6c757d', '#ffffff', 10, 1),
            ('INVESTIGATION', 'Investigation', 'Bon en investigation securite', '#dc3545', '#ffffff', 11, 1)
        PRINT 'Statuts inseres avec succes.'
    END
    ELSE
    BEGIN
        PRINT 'Les statuts existent deja.'
    END
END
GO

-- ============================================
-- ETAPE 5: DONNEES DE REFERENCE - ROLES
-- ============================================
PRINT ''
PRINT '=== INSERTION DES ROLES ==='

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Roles')
BEGIN
    PRINT 'ATTENTION: La table Roles n''existe pas encore. Executez d''abord les migrations EF Core.'
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE CodeRole = 'ADMIN')
    BEGIN
        INSERT INTO dbo.Roles (CodeRole, NomRole, Description, NiveauApprobation, EstActif)
        VALUES
            ('ADMIN', 'Administrateur', 'Acces complet au systeme', 999, 1),
            ('SUPERVISEUR', 'Superviseur', 'Superviseur de departement', 10, 1),
            ('GM', 'General Manager', 'Directeur General', 20, 1),
            ('OPJ', 'Officier Police Judiciaire', 'Officier de police judiciaire', 30, 1),
            ('IDENTIFICATION', 'Identification', 'Service d''identification des materiels', 40, 1),
            ('IT', 'IT', 'Service informatique', 50, 1),
            ('ENVIRONNEMENT', 'Environnement', 'Service environnement', 60, 1),
            ('INVESTIGATION', 'Investigation', 'Service d''investigation securite', 70, 1),
            ('BARRIERE', 'Barriere', 'Agent de barriere/checkpoint', 80, 1),
            ('USER', 'Utilisateur', 'Utilisateur standard (demandeur)', 0, 1)
        PRINT 'Roles inseres avec succes.'
    END
    ELSE
    BEGIN
        PRINT 'Les roles existent deja.'
    END
END
GO

-- ============================================
-- ETAPE 6: DONNEES DE REFERENCE - DEPARTEMENTS
-- ============================================
PRINT ''
PRINT '=== INSERTION DES DEPARTEMENTS ==='

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'ref' AND TABLE_NAME = 'Departements')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ref.Departements WHERE Code = 'MINING')
    BEGIN
        INSERT INTO ref.Departements (Nom, Code, Description, EstActif, OrdreAffichage)
        VALUES
            ('Mining Operations', 'MINING', 'Operations minieres', 1, 1),
            ('Processing', 'PROCESS', 'Traitement du minerai', 1, 2),
            ('Maintenance', 'MAINT', 'Maintenance generale', 1, 3),
            ('Supply Chain', 'SUPPLY', 'Chaine d''approvisionnement', 1, 4),
            ('Security', 'SEC', 'Securite', 1, 5),
            ('Finance', 'FIN', 'Finance et comptabilite', 1, 6),
            ('Human Resources', 'HR', 'Ressources humaines', 1, 7),
            ('Information Technology', 'IT', 'Technologies de l''information', 1, 8),
            ('HSE', 'HSE', 'Hygiene, Securite, Environnement', 1, 9),
            ('Administration', 'ADMIN', 'Administration generale', 1, 10),
            ('Projects', 'PROJ', 'Projets et developpement', 1, 11),
            ('Laboratory', 'LAB', 'Laboratoire d''analyses', 1, 12),
            ('Geology', 'GEO', 'Geologie et exploration', 1, 13),
            ('Electrical', 'ELEC', 'Services electriques', 1, 14),
            ('Mechanical', 'MECH', 'Services mecaniques', 1, 15),
            ('Plant Operations', 'PLANT', 'Operations usine', 1, 16),
            ('Engineering', 'ENG', 'Ingenierie', 1, 17),
            ('Quality Control', 'QC', 'Controle qualite', 1, 18),
            ('Logistics', 'LOG', 'Logistique', 1, 19),
            ('Procurement', 'PROC', 'Achats', 1, 20)
        PRINT 'Departements inseres avec succes.'
    END
    ELSE
    BEGIN
        PRINT 'Les departements existent deja.'
    END
END
ELSE
BEGIN
    PRINT 'ATTENTION: La table ref.Departements n''existe pas encore.'
END
GO

-- ============================================
-- ETAPE 7: DONNEES DE REFERENCE - SITES
-- ============================================
PRINT ''
PRINT '=== INSERTION DES SITES ==='

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'ref' AND TABLE_NAME = 'Sites')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ref.Sites WHERE Code = 'KTO')
    BEGIN
        INSERT INTO ref.Sites (Nom, Code, TypeSite, Adresse, EstInterne, EstActif, OrdreAffichage)
        VALUES
            -- Sites de production KCC
            ('KTO - Kamoto Underground Mine', 'KTO', 'Mine Souterraine', 'Kolwezi', 1, 1, 1),
            ('LUILU - Luilu Metallurgical Plant', 'LUILU', 'Usine Metallurgique', 'Kolwezi', 1, 1, 2),
            ('SKM - South Katanga Mine', 'SKM', 'Mine', 'Kolwezi', 1, 1, 3),
            ('LUSANGA', 'LUSANGA', 'Site Operationnel', 'Kolwezi', 1, 1, 4),
            ('KOV - Kamoto Oliveira Virgule', 'KOV', 'Mine a Ciel Ouvert', 'Kolwezi', 1, 1, 5),
            ('MV - Mashamba Venture', 'MV', 'Mine', 'Kolwezi', 1, 1, 6),
            ('MASHAMBA', 'MASHAMBA', 'Site Operationnel', 'Kolwezi', 1, 1, 7),
            ('KTC - Kamoto Treatment Center', 'KTC', 'Centre de Traitement', 'Kolwezi', 1, 1, 8),

            -- Autres sites internes KCC
            ('Head Office KCC', 'HQ', 'Bureaux Administratifs', 'Kolwezi Centre', 1, 1, 10),
            ('KCC Warehouse Central', 'WH-CENTRAL', 'Entrepot Central', 'Kolwezi', 1, 1, 11),
            ('KCC Warehouse Spare Parts', 'WH-SPARE', 'Entrepot Pieces de Rechange', 'Kolwezi', 1, 1, 12),
            ('KCC Camp Kolwezi', 'CAMP-KWZ', 'Camp Residentiel', 'Kolwezi', 1, 1, 13),
            ('KCC Workshop', 'WORKSHOP', 'Atelier Central', 'Kolwezi', 1, 1, 14),
            ('KCC Laboratory', 'LAB', 'Laboratoire', 'Luilu', 1, 1, 15),

            -- Barrieres / Gates
            ('Gate 1 - Entree Principale', 'GATE1', 'Barriere', 'Entree principale KCC', 1, 1, 20),
            ('Gate 2 - Entree Luilu', 'GATE2', 'Barriere', 'Entree usine Luilu', 1, 1, 21),
            ('Gate 3 - Entree KTO', 'GATE3', 'Barriere', 'Entree mine KTO', 1, 1, 22),
            ('Gate 4 - Entree KOV', 'GATE4', 'Barriere', 'Entree mine KOV', 1, 1, 23),
            ('Gate 5 - Entree Mashamba', 'GATE5', 'Barriere', 'Entree Mashamba', 1, 1, 24),

            -- Sites externes
            ('Kolwezi Ville', 'KOLWEZI', 'Ville', 'Kolwezi, Lualaba', 0, 1, 30),
            ('Lubumbashi', 'LSHI', 'Ville', 'Lubumbashi, Haut-Katanga', 0, 1, 31),
            ('Likasi', 'LIKASI', 'Ville', 'Likasi, Haut-Katanga', 0, 1, 32),
            ('Fungurume', 'FUNG', 'Ville', 'Fungurume, Lualaba', 0, 1, 33),
            ('Kinshasa', 'KIN', 'Capitale', 'Kinshasa', 0, 1, 34),
            ('Afrique du Sud', 'RSA', 'International', 'Johannesburg, RSA', 0, 1, 40),
            ('Zambie', 'ZMB', 'International', 'Zambie', 0, 1, 41),
            ('Autre Site Externe', 'EXTERNE', 'Externe', 'A preciser', 0, 1, 99)
        PRINT 'Sites inseres avec succes.'
    END
    ELSE
    BEGIN
        PRINT 'Les sites existent deja.'
    END
END
ELSE
BEGIN
    PRINT 'ATTENTION: La table ref.Sites n''existe pas encore.'
END
GO

-- ============================================
-- ETAPE 8: DONNEES DE REFERENCE - COMPAGNIES
-- ============================================
PRINT ''
PRINT '=== INSERTION DES COMPAGNIES ==='

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'ref' AND TABLE_NAME = 'Compagnies')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ref.Compagnies WHERE Code = 'KCC')
    BEGIN
        INSERT INTO ref.Compagnies (Nom, Code, NumeroContrat, Email, SiteManager, EstActif, DateCreation)
        VALUES
            ('Kamoto Copper Company SA', 'KCC', NULL, 'info@kamotocopper.com', NULL, 1, GETDATE()),
            ('SOMIKA SARL', 'SOMIKA', 'CTR-KCC-2024-001', 'contact@somika.cd', 'Jean-Marie Kabongo', 1, GETDATE()),
            ('EGMF - Entreprise Generale Malta Forrest', 'EGMF', 'CTR-KCC-2024-002', 'info@egmf.cd', 'Patrick Mutombo', 1, GETDATE()),
            ('Entreprise Generale des Travaux', 'EGT', 'CTR-KCC-2024-003', 'contact@egt.cd', 'Joseph Kalala', 1, GETDATE()),
            ('Mining & Civil Works SARL', 'MCW', 'CTR-KCC-2024-004', 'info@mcw.cd', 'Francois Tshisekedi', 1, GETDATE()),
            ('Groupe Forrest International', 'GFI', 'CTR-KCC-2024-005', 'info@forrest.cd', 'Michel Forrest', 1, GETDATE()),
            ('Kibali Gold Operations', 'KIBALI', 'CTR-KCC-2024-006', 'info@kibali.cd', 'Robert Kalonji', 1, GETDATE()),
            ('COPEFA SARL', 'COPEFA', 'CTR-KCC-2024-007', 'contact@copefa.cd', 'Pierre Mwamba', 1, GETDATE()),
            ('TEKTRA SARL', 'TEKTRA', 'CTR-KCC-2024-008', 'info@tektra.cd', 'Georges Kabeya', 1, GETDATE()),
            ('SNEL', 'SNEL', NULL, 'info@snel.cd', NULL, 1, GETDATE()),
            ('Autre Compagnie', 'AUTRE', NULL, NULL, NULL, 1, GETDATE())
        PRINT 'Compagnies inserees avec succes.'
    END
    ELSE
    BEGIN
        PRINT 'Les compagnies existent deja.'
    END
END
ELSE
BEGIN
    PRINT 'ATTENTION: La table ref.Compagnies n''existe pas encore.'
END
GO

-- ============================================
-- ETAPE 9: CATEGORIES ET RAISONS DE SORTIE
-- ============================================
PRINT ''
PRINT '=== INSERTION DES CATEGORIES DE SORTIE ==='

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CategoriesSortie')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.CategoriesSortie WHERE Code = 'PRET')
    BEGIN
        INSERT INTO dbo.CategoriesSortie (Code, Libelle, Description, EstActif, OrdreAffichage)
        VALUES
            ('PRET', 'Pret de materiel', 'Pret temporaire de materiel', 1, 1),
            ('SORTIE_INTERNE', 'Sortie Interne', 'Transfert entre sites KCC', 1, 2),
            ('SORTIE_EXTERNE', 'Sortie Externe', 'Sortie hors des sites KCC', 1, 3)
        PRINT 'Categories de sortie inserees.'
    END
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RaisonsSortie')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.RaisonsSortie WHERE Code = 'REPARATION')
    BEGIN
        INSERT INTO dbo.RaisonsSortie (Code, Libelle, Description, IdCategorie, RequiertRetour, DelaiRetourJours, EstActif, OrdreAffichage, TypeMaterielDefaut)
        VALUES
            ('REPARATION', 'Reparation', 'Envoi pour reparation externe', 3, 1, 30, 1, 1, 'Equipement'),
            ('VENTE', 'Vente', 'Vente de materiel', 3, 0, NULL, 1, 2, NULL),
            ('DON', 'Don', 'Don de materiel', 3, 0, NULL, 1, 3, NULL),
            ('TRANSFERT', 'Transfert permanent', 'Transfert definitif', 2, 0, NULL, 1, 4, NULL),
            ('MISSION', 'Mission', 'Materiel pour mission temporaire', 1, 1, 14, 1, 5, 'Vehicule'),
            ('EVENEMENT', 'Evenement', 'Materiel pour evenement', 1, 1, 7, 1, 6, NULL),
            ('RETOUR_FOURNISSEUR', 'Retour fournisseur', 'Retour au fournisseur', 3, 0, NULL, 1, 7, NULL),
            ('DESTRUCTION', 'Destruction', 'Materiel a detruire', 3, 0, NULL, 1, 8, NULL)
        PRINT 'Raisons de sortie inserees.'
    END
END
GO

-- ============================================
-- ETAPE 10: CHECKPOINTS (BARRIERES)
-- ============================================
PRINT ''
PRINT '=== INSERTION DES CHECKPOINTS ==='

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Checkpoints')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Checkpoints WHERE Code = 'CP-GATE1')
    BEGIN
        INSERT INTO dbo.Checkpoints (Code, Nom, Description, TypeCheckpoint, EstActif, OrdreAffichage)
        VALUES
            ('CP-GATE1', 'Gate 1 - Principale', 'Checkpoint entree principale KCC', 'Entree', 1, 1),
            ('CP-GATE2', 'Gate 2 - Luilu', 'Checkpoint entree usine Luilu', 'Entree', 1, 2),
            ('CP-GATE3', 'Gate 3 - KTO', 'Checkpoint entree mine KTO', 'Entree', 1, 3),
            ('CP-GATE4', 'Gate 4 - KOV', 'Checkpoint entree mine KOV', 'Entree', 1, 4),
            ('CP-GATE5', 'Gate 5 - Mashamba', 'Checkpoint entree Mashamba', 'Entree', 1, 5)
        PRINT 'Checkpoints inseres.'
    END
END
GO

-- ============================================
-- RESUME FINAL
-- ============================================
PRINT ''
PRINT '============================================'
PRINT 'DEPLOIEMENT TERMINE'
PRINT '============================================'
PRINT ''
PRINT 'Prochaines etapes:'
PRINT '1. Configurez la chaine de connexion dans appsettings.json:'
PRINT '   "Server=VOTRE_SERVEUR;Database=AppDev_SecurityPortal_DB;..."'
PRINT ''
PRINT '2. Executez les migrations EF Core:'
PRINT '   dotnet ef database update --project KCCMaterialFlow.Infrastructure --startup-project KCCMaterialFlow.Host'
PRINT ''
PRINT '3. Relancez ce script pour inserer les donnees de reference'
PRINT ''
PRINT '4. Decommentez la section UTILISATEURS DE TEST et ajoutez vos users'
PRINT ''
GO
