-- ============================================
-- Script d'insertion des données de référence pour KCC Material Flow
-- À exécuter après la migration AddReferenceDataTables
-- ============================================

-- Nettoyage préalable (optionnel - à décommenter si besoin de réinitialiser)
-- DELETE FROM ref.Employees
-- DELETE FROM ref.Compagnies
-- DELETE FROM ref.Sites
-- DELETE FROM ref.Departements
-- GO

-- ============================================
-- DÉPARTEMENTS KCC
-- ============================================
PRINT 'Insertion des départements...'

INSERT INTO ref.Departements (Nom, Code, Description, EstActif, OrdreAffichage)
VALUES
    ('Mining Operations', 'MINING', 'Opérations minières', 1, 1),
    ('Processing', 'PROCESS', 'Traitement du minerai', 1, 2),
    ('Maintenance', 'MAINT', 'Maintenance générale', 1, 3),
    ('Supply Chain', 'SUPPLY', 'Chaîne d''approvisionnement', 1, 4),
    ('Security', 'SEC', 'Sécurité', 1, 5),
    ('Finance', 'FIN', 'Finance et comptabilité', 1, 6),
    ('Human Resources', 'HR', 'Ressources humaines', 1, 7),
    ('Information Technology', 'IT', 'Technologies de l''information', 1, 8),
    ('HSE', 'HSE', 'Hygiène, Sécurité, Environnement', 1, 9),
    ('Administration', 'ADMIN', 'Administration générale', 1, 10),
    ('Projects', 'PROJ', 'Projets et développement', 1, 11),
    ('Laboratory', 'LAB', 'Laboratoire d''analyses', 1, 12),
    ('Geology', 'GEO', 'Géologie et exploration', 1, 13),
    ('Electrical', 'ELEC', 'Services électriques', 1, 14),
    ('Mechanical', 'MECH', 'Services mécaniques', 1, 15),
    ('Plant Operations', 'PLANT', 'Opérations usine', 1, 16),
    ('Engineering', 'ENG', 'Ingénierie', 1, 17),
    ('Quality Control', 'QC', 'Contrôle qualité', 1, 18),
    ('Logistics', 'LOG', 'Logistique', 1, 19),
    ('Procurement', 'PROC', 'Achats', 1, 20)
GO

-- ============================================
-- SITES KCC (FROM / TO)
-- ============================================
PRINT 'Insertion des sites KCC...'

INSERT INTO ref.Sites (Nom, Code, TypeSite, Adresse, EstInterne, EstActif, OrdreAffichage)
VALUES
    -- Sites de production KCC
    ('KTO - Kamoto Underground Mine', 'KTO', 'Mine Souterraine', 'Kolwezi', 1, 1, 1),
    ('LUILU - Luilu Metallurgical Plant', 'LUILU', 'Usine Métallurgique', 'Kolwezi', 1, 1, 2),
    ('SKM - South Katanga Mine', 'SKM', 'Mine', 'Kolwezi', 1, 1, 3),
    ('LUSANGA', 'LUSANGA', 'Site Opérationnel', 'Kolwezi', 1, 1, 4),
    ('KOV - Kamoto Oliveira Virgule', 'KOV', 'Mine à Ciel Ouvert', 'Kolwezi', 1, 1, 5),
    ('MV - Mashamba Venture', 'MV', 'Mine', 'Kolwezi', 1, 1, 6),
    ('MASHAMBA', 'MASHAMBA', 'Site Opérationnel', 'Kolwezi', 1, 1, 7),
    ('KTC - Kamoto Treatment Center', 'KTC', 'Centre de Traitement', 'Kolwezi', 1, 1, 8),
    
    -- Autres sites internes KCC
    ('Head Office KCC', 'HQ', 'Bureaux Administratifs', 'Kolwezi Centre', 1, 1, 10),
    ('KCC Warehouse Central', 'WH-CENTRAL', 'Entrepôt Central', 'Kolwezi', 1, 1, 11),
    ('KCC Warehouse Spare Parts', 'WH-SPARE', 'Entrepôt Pièces de Rechange', 'Kolwezi', 1, 1, 12),
    ('KCC Camp Kolwezi', 'CAMP-KWZ', 'Camp Résidentiel', 'Kolwezi', 1, 1, 13),
    ('KCC Workshop', 'WORKSHOP', 'Atelier Central', 'Kolwezi', 1, 1, 14),
    ('KCC Laboratory', 'LAB', 'Laboratoire', 'Luilu', 1, 1, 15),
    
    -- Barrières / Gates
    ('Gate 1 - Entrée Principale', 'GATE1', 'Barrière', 'Entrée principale KCC', 1, 1, 20),
    ('Gate 2 - Entrée Luilu', 'GATE2', 'Barrière', 'Entrée usine Luilu', 1, 1, 21),
    ('Gate 3 - Entrée KTO', 'GATE3', 'Barrière', 'Entrée mine KTO', 1, 1, 22),
    ('Gate 4 - Entrée KOV', 'GATE4', 'Barrière', 'Entrée mine KOV', 1, 1, 23),
    ('Gate 5 - Entrée Mashamba', 'GATE5', 'Barrière', 'Entrée Mashamba', 1, 1, 24),
    
    -- Sites externes
    ('Kolwezi Ville', 'KOLWEZI', 'Ville', 'Kolwezi, Lualaba', 0, 1, 30),
    ('Lubumbashi', 'LSHI', 'Ville', 'Lubumbashi, Haut-Katanga', 0, 1, 31),
    ('Likasi', 'LIKASI', 'Ville', 'Likasi, Haut-Katanga', 0, 1, 32),
    ('Fungurume', 'FUNG', 'Ville', 'Fungurume, Lualaba', 0, 1, 33),
    ('Kinshasa', 'KIN', 'Capitale', 'Kinshasa', 0, 1, 34),
    ('Afrique du Sud', 'RSA', 'International', 'Johannesburg, RSA', 0, 1, 40),
    ('Zambie', 'ZMB', 'International', 'Zambie', 0, 1, 41),
    ('Autre Site Externe', 'EXTERNE', 'Externe', 'À préciser', 0, 1, 99)
GO

-- ============================================
-- COMPAGNIES CONTRACTORS KCC
-- ============================================
PRINT 'Insertion des compagnies contractors...'

INSERT INTO ref.Compagnies (Nom, Code, NumeroContrat, Email, SiteManager, EstActif, DateCreation)
VALUES
    -- KCC (interne)
    ('Kamoto Copper Company SA', 'KCC', NULL, 'info@kamotocopper.com', NULL, 1, GETDATE()),
    
    -- Contractors majeurs
    ('SOMIKA SARL', 'SOMIKA', 'CTR-KCC-2024-001', 'contact@somika.cd', 'Jean-Marie Kabongo', 1, GETDATE()),
    ('EGMF - Entreprise Générale Malta Forrest', 'EGMF', 'CTR-KCC-2024-002', 'info@egmf.cd', 'Patrick Mutombo', 1, GETDATE()),
    ('Entreprise Générale des Travaux', 'EGT', 'CTR-KCC-2024-003', 'contact@egt.cd', 'Joseph Kalala', 1, GETDATE()),
    ('Mining & Civil Works SARL', 'MCW', 'CTR-KCC-2024-004', 'info@mcw.cd', 'François Tshisekedi', 1, GETDATE()),
    ('Groupe Forrest International', 'GFI', 'CTR-KCC-2024-005', 'info@forrest.cd', 'Michel Forrest', 1, GETDATE()),
    ('SEK - Société d''Exploitation de Kipushi', 'SEK', 'CTR-KCC-2024-006', 'info@sek.cd', 'André Mwenze', 1, GETDATE()),
    ('GECAMINES', 'GCM', 'CTR-KCC-2024-007', 'contact@gecamines.cd', 'Robert Lukusa', 1, GETDATE()),
    
    -- Services
    ('Sodexo RDC', 'SODEXO', 'CTR-KCC-2024-010', 'rdc@sodexo.com', 'Marie Kasongo', 1, GETDATE()),
    ('G4S Security DRC', 'G4S', 'CTR-KCC-2024-011', 'drc@g4s.com', 'Emmanuel Kabila', 1, GETDATE()),
    ('Securicom SARL', 'SECURICOM', 'CTR-KCC-2024-012', 'info@securicom.cd', 'Pierre Ngoy', 1, GETDATE()),
    
    -- Transport & Logistique
    ('Bolloré Transport & Logistics', 'BOLLORE', 'CTR-KCC-2024-020', 'info@bollore.cd', 'Claude Mbuyi', 1, GETDATE()),
    ('SDV Transami', 'SDV', 'CTR-KCC-2024-021', 'info@sdv.cd', 'Paul Kasanda', 1, GETDATE()),
    ('Transglobal Logistics', 'TGL', 'CTR-KCC-2024-022', 'contact@transglobal.cd', 'Eric Katende', 1, GETDATE()),
    
    -- Équipements & Fournitures
    ('Caterpillar DRC', 'CAT', 'CTR-KCC-2024-030', 'drc@cat.com', 'John Smith', 1, GETDATE()),
    ('Sandvik Mining', 'SANDVIK', 'CTR-KCC-2024-031', 'drc@sandvik.com', 'Lars Johansson', 1, GETDATE()),
    ('Atlas Copco', 'ATLAS', 'CTR-KCC-2024-032', 'drc@atlascopco.com', 'Hans Mueller', 1, GETDATE()),
    ('Epiroc', 'EPIROC', 'CTR-KCC-2024-033', 'drc@epiroc.com', 'Karl Andersson', 1, GETDATE()),
    
    -- Construction & Génie Civil
    ('Entreprise de Construction Minière', 'ECM', 'CTR-KCC-2024-040', 'info@ecm.cd', 'Simon Kyungu', 1, GETDATE()),
    ('Travaux Publics Congo', 'TPC', 'CTR-KCC-2024-041', 'contact@tpc.cd', 'Augustin Ilunga', 1, GETDATE()),
    ('SAFRICAS Congo', 'SAFRICAS', 'CTR-KCC-2024-042', 'info@safricas.cd', 'David Lukusa', 1, GETDATE()),
    
    -- Maintenance & Services techniques
    ('Maintenance Industrielle Congo', 'MIC', 'CTR-KCC-2024-050', 'info@mic.cd', 'Éric Mwamba', 1, GETDATE()),
    ('Congo Technical Services', 'CTS', 'CTR-KCC-2024-051', 'contact@cts.cd', 'Gilbert Kayembe', 1, GETDATE()),
    ('Ets Mwana Shaba', 'MWANASHABA', 'CTR-KCC-2024-052', 'info@mwanashaba.cd', 'Martin Tshilombo', 1, GETDATE())
GO

-- ============================================
-- EMPLOYÉS KCC (Internes - escorteurs)
-- ============================================
PRINT 'Insertion des employés KCC...'

DECLARE @MiningId INT = (SELECT Id FROM ref.Departements WHERE Code = 'MINING')
DECLARE @ProcessId INT = (SELECT Id FROM ref.Departements WHERE Code = 'PROCESS')
DECLARE @MaintId INT = (SELECT Id FROM ref.Departements WHERE Code = 'MAINT')
DECLARE @SecId INT = (SELECT Id FROM ref.Departements WHERE Code = 'SEC')
DECLARE @SupplyId INT = (SELECT Id FROM ref.Departements WHERE Code = 'SUPPLY')
DECLARE @ITId INT = (SELECT Id FROM ref.Departements WHERE Code = 'IT')
DECLARE @HSEId INT = (SELECT Id FROM ref.Departements WHERE Code = 'HSE')
DECLARE @AdminId INT = (SELECT Id FROM ref.Departements WHERE Code = 'ADMIN')
DECLARE @PlantId INT = (SELECT Id FROM ref.Departements WHERE Code = 'PLANT')
DECLARE @EngId INT = (SELECT Id FROM ref.Departements WHERE Code = 'ENG')
DECLARE @LogId INT = (SELECT Id FROM ref.Departements WHERE Code = 'LOG')
DECLARE @ElecId INT = (SELECT Id FROM ref.Departements WHERE Code = 'ELEC')
DECLARE @MechId INT = (SELECT Id FROM ref.Departements WHERE Code = 'MECH')

DECLARE @KCCId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'KCC')

INSERT INTO ref.Employees (Matricule, NomComplet, Prenom, Nom, Fonction, Email, DepartementId, CompagnieId, EstInterne, PeutEtreEscorteur, EstActif, DateCreation)
VALUES
    -- Direction & Management
    ('KCC-001', 'Jean-Pierre Mukendi Kabongo', 'Jean-Pierre', 'Mukendi Kabongo', 'General Manager', 'jp.mukendi@kamotocopper.com', @AdminId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-002', 'Patrick Tshisekedi Mulunda', 'Patrick', 'Tshisekedi Mulunda', 'Operations Director', 'p.tshisekedi@kamotocopper.com', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Mining Operations
    ('KCC-010', 'Joseph Kalala Mwenze', 'Joseph', 'Kalala Mwenze', 'Mining Superintendent', 'j.kalala@kamotocopper.com', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-011', 'François Kabongo Mutombo', 'François', 'Kabongo Mutombo', 'Mining Supervisor KTO', 'f.kabongo@kamotocopper.com', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-012', 'Emmanuel Tshilombo Kayembe', 'Emmanuel', 'Tshilombo Kayembe', 'Mining Supervisor KOV', 'e.tshilombo@kamotocopper.com', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-013', 'David Ilunga Kasongo', 'David', 'Ilunga Kasongo', 'Shift Boss', 'd.ilunga@kamotocopper.com', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-014', 'Albert Mwamba Ngoy', 'Albert', 'Mwamba Ngoy', 'Mining Engineer', 'a.mwamba@kamotocopper.com', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-015', 'Simon Katende Banza', 'Simon', 'Katende Banza', 'Drill & Blast Supervisor', 's.katende@kamotocopper.com', @MiningId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Processing / Plant
    ('KCC-020', 'Marie Kasongo Mbuyi', 'Marie', 'Kasongo Mbuyi', 'Plant Manager Luilu', 'm.kasongo@kamotocopper.com', @ProcessId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-021', 'Robert Lukusa Kabila', 'Robert', 'Lukusa Kabila', 'Process Superintendent', 'r.lukusa@kamotocopper.com', @ProcessId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-022', 'Christine Ngalula Kazadi', 'Christine', 'Ngalula Kazadi', 'Metallurgist', 'c.ngalula@kamotocopper.com', @ProcessId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-023', 'Pierre Mutombo Kalala', 'Pierre', 'Mutombo Kalala', 'Plant Supervisor', 'p.mutombo@kamotocopper.com', @PlantId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Maintenance
    ('KCC-030', 'André Mwenze Kyungu', 'André', 'Mwenze Kyungu', 'Maintenance Manager', 'a.mwenze@kamotocopper.com', @MaintId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-031', 'Gilbert Kayembe Tshombe', 'Gilbert', 'Kayembe Tshombe', 'Mechanical Superintendent', 'g.kayembe@kamotocopper.com', @MechId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-032', 'Éric Kasanda Mukendi', 'Éric', 'Kasanda Mukendi', 'Electrical Superintendent', 'e.kasanda@kamotocopper.com', @ElecId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-033', 'Paul Kalonji Nkulu', 'Paul', 'Kalonji Nkulu', 'Maintenance Planner', 'p.kalonji@kamotocopper.com', @MaintId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-034', 'Martin Tshilombo Wa Tshilombo', 'Martin', 'Tshilombo', 'Heavy Equipment Mechanic', 'm.tshilombo@kamotocopper.com', @MechId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Security
    ('KCC-040', 'Augustin Kabila Kyungu', 'Augustin', 'Kabila Kyungu', 'Security Manager', 'a.kabila@kamotocopper.com', @SecId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-041', 'Claude Ngoy Mwamba', 'Claude', 'Ngoy Mwamba', 'Security Supervisor Gate 1', 'c.ngoy@kamotocopper.com', @SecId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-042', 'Félix Lukusa Mutombo', 'Félix', 'Lukusa Mutombo', 'Security Supervisor Gate 2', 'f.lukusa@kamotocopper.com', @SecId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-043', 'Olivier Kasongo Ilunga', 'Olivier', 'Kasongo Ilunga', 'Security Officer', 'o.kasongo@kamotocopper.com', @SecId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-044', 'Serge Mbuyi Kalala', 'Serge', 'Mbuyi Kalala', 'Investigation Officer', 's.mbuyi@kamotocopper.com', @SecId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Supply Chain & Logistics
    ('KCC-050', 'Sarah Mwamba Kasongo', 'Sarah', 'Mwamba Kasongo', 'Supply Chain Manager', 's.mwamba@kamotocopper.com', @SupplyId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-051', 'Blaise Katende Ngoy', 'Blaise', 'Katende Ngoy', 'Warehouse Supervisor', 'b.katende@kamotocopper.com', @SupplyId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-052', 'Justin Tshombe Lukusa', 'Justin', 'Tshombe Lukusa', 'Logistics Coordinator', 'j.tshombe@kamotocopper.com', @LogId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-053', 'Évariste Nkulu Kabongo', 'Évariste', 'Nkulu Kabongo', 'Store Keeper', 'e.nkulu@kamotocopper.com', @SupplyId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- IT
    ('KCC-060', 'Israel Kasa Mbaya', 'Israel', 'Kasa Mbaya', 'IT Developer', 'israel.kasa@kamotocopper.com', @ITId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-061', 'Samuel Mbaya Tshisekedi', 'Samuel', 'Mbaya Tshisekedi', 'IT Manager', 's.mbaya@kamotocopper.com', @ITId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-062', 'Benjamin Ilunga Mwenze', 'Benjamin', 'Ilunga Mwenze', 'Network Administrator', 'b.ilunga@kamotocopper.com', @ITId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-063', 'Nathan Kabongo Kalala', 'Nathan', 'Kabongo Kalala', 'IT Support', 'n.kabongo@kamotocopper.com', @ITId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- HSE
    ('KCC-070', 'Grace Mbuyi Kasongo', 'Grace', 'Mbuyi Kasongo', 'HSE Manager', 'g.mbuyi@kamotocopper.com', @HSEId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-071', 'Didier Kyungu Mutombo', 'Didier', 'Kyungu Mutombo', 'Safety Officer', 'd.kyungu@kamotocopper.com', @HSEId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-072', 'Henriette Lukusa Ngoy', 'Henriette', 'Lukusa Ngoy', 'Environment Officer', 'h.lukusa@kamotocopper.com', @HSEId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Engineering
    ('KCC-080', 'Michel Tshilombo Kasanda', 'Michel', 'Tshilombo Kasanda', 'Chief Engineer', 'm.tshilombo2@kamotocopper.com', @EngId, @KCCId, 1, 1, 1, GETDATE()),
    ('KCC-081', 'Jacques Mwamba Ilunga', 'Jacques', 'Mwamba Ilunga', 'Project Engineer', 'j.mwamba@kamotocopper.com', @EngId, @KCCId, 1, 1, 1, GETDATE()),
    
    -- Administration
    ('KCC-090', 'Jeanne Kalala Mbuyi', 'Jeanne', 'Kalala Mbuyi', 'Administrative Manager', 'j.kalala@kamotocopper.com', @AdminId, @KCCId, 1, 0, 1, GETDATE()),
    ('KCC-091', 'Pascaline Ngoy Kasongo', 'Pascaline', 'Ngoy Kasongo', 'Executive Secretary', 'p.ngoy@kamotocopper.com', @AdminId, @KCCId, 1, 0, 1, GETDATE())
GO

-- ============================================
-- EMPLOYÉS CONTRACTORS (Externes)
-- ============================================
PRINT 'Insertion des employés contractors...'

DECLARE @SOMIKAId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'SOMIKA')
DECLARE @EGMFId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'EGMF')
DECLARE @EGTId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'EGT')
DECLARE @MCWId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'MCW')
DECLARE @G4SId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'G4S')
DECLARE @SODEXOId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'SODEXO')
DECLARE @BOLLOREId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'BOLLORE')
DECLARE @ECMId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'ECM')
DECLARE @MICId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'MIC')
DECLARE @CATId INT = (SELECT Id FROM ref.Compagnies WHERE Code = 'CAT')

INSERT INTO ref.Employees (Matricule, NomComplet, Prenom, Nom, Fonction, Email, CompagnieId, EstInterne, PeutEtreEscorteur, EstActif, DateCreation)
VALUES
    -- SOMIKA
    ('SMK-001', 'Jean-Marie Kabongo Muteba', 'Jean-Marie', 'Kabongo Muteba', 'Site Manager', 'jm.kabongo@somika.cd', @SOMIKAId, 0, 0, 1, GETDATE()),
    ('SMK-002', 'Bruno Kyungu Ilunga', 'Bruno', 'Kyungu Ilunga', 'Foreman', 'b.kyungu@somika.cd', @SOMIKAId, 0, 0, 1, GETDATE()),
    ('SMK-003', 'Cédric Kabila Nkulu', 'Cédric', 'Kabila Nkulu', 'Heavy Equipment Operator', 'c.kabila@somika.cd', @SOMIKAId, 0, 0, 1, GETDATE()),
    ('SMK-004', 'Désiré Lukusa Mwamba', 'Désiré', 'Lukusa Mwamba', 'Truck Driver', 'd.lukusa@somika.cd', @SOMIKAId, 0, 0, 1, GETDATE()),
    ('SMK-005', 'Eddy Kasongo Ngoy', 'Eddy', 'Kasongo Ngoy', 'Excavator Operator', 'e.kasongo@somika.cd', @SOMIKAId, 0, 0, 1, GETDATE()),
    
    -- EGMF
    ('EGM-001', 'Patrick Mutombo Kalala', 'Patrick', 'Mutombo Kalala', 'Project Director', 'p.mutombo@egmf.cd', @EGMFId, 0, 0, 1, GETDATE()),
    ('EGM-002', 'Daniel Mwenze Kabongo', 'Daniel', 'Mwenze Kabongo', 'Site Supervisor', 'd.mwenze@egmf.cd', @EGMFId, 0, 0, 1, GETDATE()),
    ('EGM-003', 'Éric Kasanda Tshilombo', 'Éric', 'Kasanda Tshilombo', 'Civil Engineer', 'e.kasanda@egmf.cd', @EGMFId, 0, 0, 1, GETDATE()),
    ('EGM-004', 'Fiston Ilunga Kayembe', 'Fiston', 'Ilunga Kayembe', 'Electrician', 'f.ilunga@egmf.cd', @EGMFId, 0, 0, 1, GETDATE()),
    
    -- EGT
    ('EGT-001', 'Joseph Kalala Mbuyi', 'Joseph', 'Kalala Mbuyi', 'Site Manager', 'j.kalala@egt.cd', @EGTId, 0, 0, 1, GETDATE()),
    ('EGT-002', 'Kevin Ngoy Lukusa', 'Kevin', 'Ngoy Lukusa', 'Foreman', 'k.ngoy@egt.cd', @EGTId, 0, 0, 1, GETDATE()),
    ('EGT-003', 'Lambert Mwamba Kasongo', 'Lambert', 'Mwamba Kasongo', 'Mason', 'l.mwamba@egt.cd', @EGTId, 0, 0, 1, GETDATE()),
    
    -- MCW
    ('MCW-001', 'François Tshisekedi Kalala', 'François', 'Tshisekedi Kalala', 'Site Manager', 'f.tshisekedi@mcw.cd', @MCWId, 0, 0, 1, GETDATE()),
    ('MCW-002', 'Guy Mbuyi Kabongo', 'Guy', 'Mbuyi Kabongo', 'Operations Supervisor', 'g.mbuyi@mcw.cd', @MCWId, 0, 0, 1, GETDATE()),
    ('MCW-003', 'Henri Kayembe Ngoy', 'Henri', 'Kayembe Ngoy', 'Equipment Operator', 'h.kayembe@mcw.cd', @MCWId, 0, 0, 1, GETDATE()),
    
    -- G4S Security
    ('G4S-001', 'Emmanuel Kabila Kasongo', 'Emmanuel', 'Kabila Kasongo', 'Security Manager', 'e.kabila@g4s.com', @G4SId, 0, 0, 1, GETDATE()),
    ('G4S-002', 'Ivan Nkulu Lukusa', 'Ivan', 'Nkulu Lukusa', 'Security Supervisor', 'i.nkulu@g4s.com', @G4SId, 0, 0, 1, GETDATE()),
    ('G4S-003', 'Jean Mutombo Mbuyi', 'Jean', 'Mutombo Mbuyi', 'Security Guard', 'j.mutombo@g4s.com', @G4SId, 0, 0, 1, GETDATE()),
    
    -- Sodexo
    ('SDX-001', 'Marie Kasongo Ilunga', 'Marie', 'Kasongo Ilunga', 'Catering Manager', 'm.kasongo@sodexo.com', @SODEXOId, 0, 0, 1, GETDATE()),
    ('SDX-002', 'Nadine Lukusa Kalala', 'Nadine', 'Lukusa Kalala', 'Chef', 'n.lukusa@sodexo.com', @SODEXOId, 0, 0, 1, GETDATE()),
    ('SDX-003', 'Olivier Mwamba Ngoy', 'Olivier', 'Mwamba Ngoy', 'Camp Manager', 'o.mwamba@sodexo.com', @SODEXOId, 0, 0, 1, GETDATE()),
    
    -- Bolloré
    ('BLR-001', 'Claude Mbuyi Kasongo', 'Claude', 'Mbuyi Kasongo', 'Logistics Manager', 'c.mbuyi@bollore.cd', @BOLLOREId, 0, 0, 1, GETDATE()),
    ('BLR-002', 'Pascal Kabongo Lukusa', 'Pascal', 'Kabongo Lukusa', 'Transport Coordinator', 'p.kabongo@bollore.cd', @BOLLOREId, 0, 0, 1, GETDATE()),
    ('BLR-003', 'Quentin Ilunga Mwamba', 'Quentin', 'Ilunga Mwamba', 'Truck Driver', 'q.ilunga@bollore.cd', @BOLLOREId, 0, 0, 1, GETDATE()),
    
    -- ECM
    ('ECM-001', 'Simon Kyungu Kalala', 'Simon', 'Kyungu Kalala', 'Construction Manager', 's.kyungu@ecm.cd', @ECMId, 0, 0, 1, GETDATE()),
    ('ECM-002', 'Thierry Ngoy Kabongo', 'Thierry', 'Ngoy Kabongo', 'Site Engineer', 't.ngoy@ecm.cd', @ECMId, 0, 0, 1, GETDATE()),
    ('ECM-003', 'Urbain Lukusa Mwenze', 'Urbain', 'Lukusa Mwenze', 'Foreman', 'u.lukusa@ecm.cd', @ECMId, 0, 0, 1, GETDATE()),
    
    -- MIC (Maintenance Industrielle Congo)
    ('MIC-001', 'Éric Mwamba Kasongo', 'Éric', 'Mwamba Kasongo', 'Maintenance Manager', 'e.mwamba@mic.cd', @MICId, 0, 0, 1, GETDATE()),
    ('MIC-002', 'Victor Kalala Ngoy', 'Victor', 'Kalala Ngoy', 'Mechanical Technician', 'v.kalala@mic.cd', @MICId, 0, 0, 1, GETDATE()),
    ('MIC-003', 'William Kabongo Ilunga', 'William', 'Kabongo Ilunga', 'Electrical Technician', 'w.kabongo@mic.cd', @MICId, 0, 0, 1, GETDATE()),
    
    -- Caterpillar
    ('CAT-001', 'Xavier Mbuyi Lukusa', 'Xavier', 'Mbuyi Lukusa', 'Service Manager', 'x.mbuyi@cat.com', @CATId, 0, 0, 1, GETDATE()),
    ('CAT-002', 'Yannick Kasongo Mwamba', 'Yannick', 'Kasongo Mwamba', 'Field Technician', 'y.kasongo@cat.com', @CATId, 0, 0, 1, GETDATE()),
    ('CAT-003', 'Zacharie Ngoy Kalala', 'Zacharie', 'Ngoy Kalala', 'Parts Coordinator', 'z.ngoy@cat.com', @CATId, 0, 0, 1, GETDATE())
GO

-- ============================================
-- RÉSUMÉ
-- ============================================
PRINT ''
PRINT '============================================'
PRINT 'Insertion des données de référence terminée!'
PRINT '============================================'
PRINT ''

SELECT 'Départements' AS [Table], COUNT(*) AS [Nombre] FROM ref.Departements
UNION ALL
SELECT 'Sites', COUNT(*) FROM ref.Sites
UNION ALL
SELECT 'Compagnies', COUNT(*) FROM ref.Compagnies
UNION ALL
SELECT 'Employés KCC (Internes)', COUNT(*) FROM ref.Employees WHERE EstInterne = 1
UNION ALL
SELECT 'Employés Contractors', COUNT(*) FROM ref.Employees WHERE EstInterne = 0
UNION ALL
SELECT 'Escorteurs disponibles', COUNT(*) FROM ref.Employees WHERE PeutEtreEscorteur = 1
GO
