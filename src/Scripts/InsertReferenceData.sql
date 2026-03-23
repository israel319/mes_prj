-- Script d'insertion des données de référence pour KCC Material Flow
-- À exécuter après la migration AddReferenceDataTables

-- ============================================
-- DÉPARTEMENTS
-- ============================================
PRINT 'Insertion des départements...'

INSERT INTO ref.Departements (Nom, Code, Description, EstActif, OrdreAffichage)
VALUES
    ('Mining Operations', 'MINING', 'Operations minieres', 1, 1),
    ('Processing Plant', 'PLANT', 'Usine de traitement', 1, 2),
    ('Maintenance', 'MAINT', 'Maintenance generale', 1, 3),
    ('Supply Chain', 'SUPPLY', 'Chaine d''approvisionnement', 1, 4),
    ('Security', 'SEC', 'Securite', 1, 5),
    ('Finance', 'FIN', 'Finance et comptabilite', 1, 6),
    ('Human Resources', 'HR', 'Ressources humaines', 1, 7),
    ('IT', 'IT', 'Technologies de l''information', 1, 8),
    ('HSE', 'HSE', 'Hygiene, Securite, Environnement', 1, 9),
    ('Administration', 'ADMIN', 'Administration generale', 1, 10),
    ('Projects', 'PROJ', 'Projets et developpement', 1, 11),
    ('Laboratory', 'LAB', 'Laboratoire', 1, 12),
    ('Geology', 'GEO', 'Geologie', 1, 13),
    ('Electrical', 'ELEC', 'Services electriques', 1, 14),
    ('Mechanical', 'MECH', 'Services mecaniques', 1, 15)
GO

-- ============================================
-- SITES (FROM / TO) - Sites KCC uniquement
-- ============================================
PRINT 'Insertion des sites...'

INSERT INTO ref.Sites (Nom, Code, TypeSite, EstInterne, EstActif, OrdreAffichage)
VALUES
    -- Les 8 sites KCC (utilisés pour FROM et TO)
    ('KTO', 'KTO', 'Site Minier', 1, 1, 1),
    ('LUILU', 'LUILU', 'Site Minier', 1, 1, 2),
    ('SKM', 'SKM', 'Site Minier', 1, 1, 3),
    ('LUSANGA', 'LUSANGA', 'Site Minier', 1, 1, 4),
    ('KOV', 'KOV', 'Site Minier', 1, 1, 5),
    ('MV', 'MV', 'Site Minier', 1, 1, 6),
    ('MASHAMBA', 'MASHAMBA', 'Site Minier', 1, 1, 7),
    ('KTC', 'KTC', 'Site Minier', 1, 1, 8)
GO

-- ============================================
-- COMPAGNIES (Contractors)
-- ============================================
PRINT 'Insertion des compagnies...'

INSERT INTO ref.Compagnies (Nom, Code, NumeroContrat, Email, EstActif, DateCreation)
VALUES
    ('KCC - Kamoto Copper Company', 'KCC', NULL, 'info@kcc.cd', 1, GETDATE()),
    ('SOMIKA', 'SOMIKA', 'CTR-2024-001', 'contact@somika.cd', 1, GETDATE()),
    ('EGMF', 'EGMF', 'CTR-2024-002', 'info@egmf.cd', 1, GETDATE()),
    ('Mining & Civil Works', 'MCW', 'CTR-2024-003', 'contact@mcw.cd', 1, GETDATE()),
    ('Groupe Forrest International', 'GFI', 'CTR-2024-004', 'info@forrest.cd', 1, GETDATE()),
    ('Sodexo', 'SODEXO', 'CTR-2024-005', 'cd@sodexo.com', 1, GETDATE()),
    ('G4S Security', 'G4S', 'CTR-2024-006', 'info.drc@g4s.com', 1, GETDATE()),
    ('Bolloré Transport & Logistics', 'BOLLORE', 'CTR-2024-007', 'info@bollore.cd', 1, GETDATE()),
    ('DHL Congo', 'DHL', 'CTR-2024-008', 'info.drc@dhl.com', 1, GETDATE()),
    ('Caterpillar', 'CAT', 'CTR-2024-009', 'support.cat@cat.com', 1, GETDATE()),
    ('Atlas Copco', 'ATLAS', 'CTR-2024-010', 'info@atlascopco.cd', 1, GETDATE()),
    ('Sandvik', 'SANDVIK', 'CTR-2024-011', 'info@sandvik.cd', 1, GETDATE())
GO

-- ============================================
-- EMPLOYÉS KCC (Internes - peuvent être escorteurs)
-- ============================================
PRINT 'Insertion des employés KCC...'

-- Récupérer les IDs des départements
DECLARE @MiningId INT = (SELECT Id FROM ref.Departements WHERE Code = 'MINING')
DECLARE @PlantId INT = (SELECT Id FROM ref.Departements WHERE Code = 'PLANT')
DECLARE @MaintId INT = (SELECT Id FROM ref.Departements WHERE Code = 'MAINT')
DECLARE @SecId INT = (SELECT Id FROM ref.Departements WHERE Code = 'SEC')
DECLARE @SupplyId INT = (SELECT Id FROM ref.Departements WHERE Code = 'SUPPLY')
DECLARE @ITId INT = (SELECT Id FROM ref.Departements WHERE Code = 'IT')
DECLARE @HSEId INT = (SELECT Id FROM ref.Departements WHERE Code = 'HSE')
DECLARE @AdminId INT = (SELECT Id FROM ref.Departements WHERE Code = 'ADMIN')

-- Récupérer l'ID de KCC
DECLARE @KCCId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'KCC')

INSERT INTO ref.Employees (Matricule, NomComplet, Prenom, Nom, Fonction, Email, DepartementId, CompagnieId, EstInterne, PeutEtreEscorteur, EstActif, DateCreation)
VALUES
    -- Mining Operations
    ('KCC001', 'Jean-Pierre Mukendi', 'Jean-Pierre', 'Mukendi', 'Mining Supervisor', 'jp.mukendi@kcc.cd', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC002', 'Patrick Kabongo', 'Patrick', 'Kabongo', 'Mining Engineer', 'p.kabongo@kcc.cd', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC003', 'David Tshilombo', 'David', 'Tshilombo', 'Shift Supervisor', 'tshilombo.d@kcc.cd', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Processing Plant
    ('KCC010', 'Marie Kasongo', 'Marie', 'Kasongo', 'Plant Supervisor', 'm.kasongo@kcc.cd', @PlantId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC011', 'Joseph Mwamba', 'Joseph', 'Mwamba', 'Process Engineer', 'j.mwamba@kcc.cd', @PlantId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Maintenance
    ('KCC020', 'Albert Kalala', 'Albert', 'Kalala', 'Maintenance Supervisor', 'a.kalala@kcc.cd', @MaintId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC021', 'Pierre Mutombo', 'Pierre', 'Mutombo', 'Maintenance Technician', 'p.mutombo@kcc.cd', @MaintId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Security
    ('KCC030', 'François Lukusa', 'François', 'Lukusa', 'Security Officer', 'f.lukusa@kcc.cd', @SecId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC031', 'Emmanuel Kayembe', 'Emmanuel', 'Kayembe', 'Security Supervisor', 'e.kayembe@kcc.cd', @SecId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Supply Chain
    ('KCC040', 'Christine Ngalula', 'Christine', 'Ngalula', 'Supply Chain Coordinator', 'c.ngalula@kcc.cd', @SupplyId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC041', 'Robert Ilunga', 'Robert', 'Ilunga', 'Warehouse Supervisor', 'r.ilunga@kcc.cd', @SupplyId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- IT
    ('KCC050', 'Israel Kasa', 'Israel', 'Kasa', 'IT Developer', 'israel.kasa@kamotocopper.com', @ITId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC051', 'Samuel Mbaya', 'Samuel', 'Mbaya', 'IT Support', 's.mbaya@kcc.cd', @ITId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- HSE
    ('KCC060', 'Grace Mbuyi', 'Grace', 'Mbuyi', 'HSE Officer', 'g.mbuyi@kcc.cd', @HSEId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC061', 'Paul Kalonji', 'Paul', 'Kalonji', 'HSE Supervisor', 'p.kalonji@kcc.cd', @HSEId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Admin
    ('KCC070', 'Sarah Lukusa', 'Sarah', 'Lukusa', 'Administrative Assistant', 's.lukusa@kcc.cd', @AdminId, @KCCId, 1, 0, 1, GETDATE())
GO

-- ============================================
-- EMPLOYÉS CONTRACTORS (Externes)
-- ============================================
PRINT 'Insertion des employés contractors...'

DECLARE @SOMIKAId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'SOMIKA')
DECLARE @EGMFId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'EGMF')
DECLARE @MCWId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'MCW')
DECLARE @G4SId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'G4S')

INSERT INTO ref.Employees (Matricule, NomComplet, Prenom, Nom, Fonction, Email, CompagnieId, EstInterne, PeutEtreEscorteur, EstActif, DateCreation)
VALUES
    -- SOMIKA
    ('SMK001', 'André Banza', 'André', 'Banza', 'Site Manager', 'a.banza@somika.cd', @SOMIKAId, 0, 0, 1, GETDATE()),
    ('SMK002', 'Bruno Kyungu', 'Bruno', 'Kyungu', 'Foreman', 'b.kyungu@somika.cd', @SOMIKAId, 0, 0, 1, GETDATE()),
    ('SMK003', 'Claude Kabila', 'Claude', 'Kabila', 'Equipment Operator', 'c.kabila@somika.cd', @SOMIKAId, 0, 0, 1, GETDATE()),
    
    -- EGMF
    ('EGM001', 'Daniel Mwenze', 'Daniel', 'Mwenze', 'Project Manager', 'd.mwenze@egmf.cd', @EGMFId, 0, 0, 1, GETDATE()),
    ('EGM002', 'Éric Kasanda', 'Éric', 'Kasanda', 'Civil Engineer', 'e.kasanda@egmf.cd', @EGMFId, 0, 0, 1, GETDATE()),
    
    -- MCW
    ('MCW001', 'Francis Tshisekedi', 'Francis', 'Tshisekedi', 'Site Manager', 'f.tshisekedi@mcw.cd', @MCWId, 0, 0, 1, GETDATE()),
    ('MCW002', 'Guy Mbuyi', 'Guy', 'Mbuyi', 'Supervisor', 'g.mbuyi@mcw.cd', @MCWId, 0, 0, 1, GETDATE()),
    
    -- G4S
    ('G4S001', 'Henri Kazadi', 'Henri', 'Kazadi', 'Security Manager', 'h.kazadi@g4s.com', @G4SId, 0, 0, 1, GETDATE()),
    ('G4S002', 'Ivan Nkulu', 'Ivan', 'Nkulu', 'Security Guard', 'i.nkulu@g4s.com', @G4SId, 0, 0, 1, GETDATE())
GO

PRINT 'Insertion des données de référence terminée!'
PRINT ''
PRINT 'Résumé:'
SELECT 'Départements' AS [Table], COUNT(*) AS [Nombre] FROM ref.Departements
UNION ALL
SELECT 'Sites', COUNT(*) FROM ref.Sites
UNION ALL
SELECT 'Compagnies', COUNT(*) FROM ref.Compagnies
UNION ALL
SELECT 'Employés', COUNT(*) FROM ref.Employees
GO
