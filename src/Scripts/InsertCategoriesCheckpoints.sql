-- Script d'insertion des categories de sortie, raisons et checkpoints
-- A executer apres la migration AddCheckpointAndCategoriesTables

-- ============================================
-- CATEGORIES DE SORTIE
-- ============================================
PRINT 'Insertion des categories de sortie...'

INSERT INTO ref.CategoriesSortie (Nom, Code, Description, EstActif, OrdreAffichage)
VALUES
    ('Externe', 'EXT', 'Sortie de materiel vers l''exterieur du site KCC (contractant ou agent)', 1, 1),
    ('Interne', 'INT', 'Transfert de materiel entre departements/sites KCC', 1, 2)
GO

-- ============================================
-- RAISONS DE SORTIE - EXTERNE
-- ============================================
PRINT 'Insertion des raisons de sortie externes...'

DECLARE @CatExterneId INT = (SELECT Id FROM ref.CategoriesSortie WHERE Code = 'EXT')

INSERT INTO ref.RaisonsSortie (CategorieId, Nom, Code, Description, RequiertBonEntree, RequiertDetails, EstActif, OrdreAffichage, TypeMaterielDefaut)
VALUES
    (@CatExterneId, 'Fin de chantier', 'FIN_CHANTIER', 'Materiel du contractant qui repart apres fin de mission', 1, 1, 1, 1, 3),
    (@CatExterneId, 'Residu', 'RESIDU', 'Evacuation de residus ou dechets', 0, 1, 1, 2, 4),
    (@CatExterneId, 'Radio protection', 'RADIO_PROT', 'Materiel necessitant controle radio-protection', 0, 1, 1, 3, 5)
GO

-- ============================================
-- RAISONS DE SORTIE - INTERNE
-- ============================================
PRINT 'Insertion des raisons de sortie internes...'

DECLARE @CatInterneId INT = (SELECT Id FROM ref.CategoriesSortie WHERE Code = 'INT')

INSERT INTO ref.RaisonsSortie (CategorieId, Nom, Code, Description, RequiertBonEntree, RequiertDetails, EstActif, OrdreAffichage, TypeMaterielDefaut)
VALUES
    (@CatInterneId, 'Modifications', 'MODIF', 'Materiel envoye pour modifications', 0, 1, 1, 1, 6),
    (@CatInterneId, 'Reparation', 'REPAR', 'Materiel envoye pour reparation', 0, 1, 1, 2, 6),
    (@CatInterneId, 'Renovation', 'RENOV', 'Materiel envoye pour renovation', 0, 1, 1, 3, 6),
    (@CatInterneId, 'Informatique', 'INFO', 'Transfert de materiel informatique entre departements', 0, 0, 1, 4, 2),
    (@CatInterneId, 'Circulaire', 'CIRC', 'Mouvement circulaire de materiel entre sites', 0, 0, 1, 5, 1),
    (@CatInterneId, 'Prise en pret', 'PRET', 'Materiel prete temporairement a un autre departement', 0, 0, 1, 6, 7)
GO

-- ============================================
-- CHECKPOINTS (Barrieres)
-- ============================================
PRINT 'Insertion des checkpoints/barrieres...'

-- Recuperer les IDs des sites
DECLARE @KTO_Id INT = (SELECT Id FROM ref.Sites WHERE Code = 'KTO')
DECLARE @LUILU_Id INT = (SELECT Id FROM ref.Sites WHERE Code = 'LUILU')
DECLARE @SKM_Id INT = (SELECT Id FROM ref.Sites WHERE Code = 'SKM')
DECLARE @LUSANGA_Id INT = (SELECT Id FROM ref.Sites WHERE Code = 'LUSANGA')
DECLARE @KOV_Id INT = (SELECT Id FROM ref.Sites WHERE Code = 'KOV')
DECLARE @MV_Id INT = (SELECT Id FROM ref.Sites WHERE Code = 'MV')
DECLARE @MASHAMBA_Id INT = (SELECT Id FROM ref.Sites WHERE Code = 'MASHAMBA')
DECLARE @KTC_Id INT = (SELECT Id FROM ref.Sites WHERE Code = 'KTC')

INSERT INTO ref.Checkpoints (SiteId, Nom, Code, Description, EstActif, OrdreDefaut)
VALUES
    (@KTO_Id, 'Barriere KTO', 'CHK-KTO', 'Point de controle principal KTO', 1, 1),
    (@LUILU_Id, 'Barriere LUILU', 'CHK-LUILU', 'Point de controle LUILU', 1, 2),
    (@SKM_Id, 'Barriere SKM', 'CHK-SKM', 'Point de controle SKM', 1, 3),
    (@LUSANGA_Id, 'Barriere LUSANGA', 'CHK-LUSANGA', 'Point de controle LUSANGA', 1, 4),
    (@KOV_Id, 'Barriere KOV', 'CHK-KOV', 'Point de controle KOV', 1, 5),
    (@MV_Id, 'Barriere MV', 'CHK-MV', 'Point de controle MV', 1, 6),
    (@MASHAMBA_Id, 'Barriere MASHAMBA', 'CHK-MASHAMBA', 'Point de controle MASHAMBA', 1, 7),
    (@KTC_Id, 'Barriere KTC', 'CHK-KTC', 'Point de controle KTC', 1, 8)
GO

PRINT 'Insertion des donnees de reference terminee!'
PRINT ''
PRINT 'Resume:'
SELECT 'Categories Sortie' AS [Table], COUNT(*) AS [Nombre] FROM ref.CategoriesSortie
UNION ALL
SELECT 'Raisons Sortie', COUNT(*) FROM ref.RaisonsSortie
UNION ALL
SELECT 'Checkpoints', COUNT(*) FROM ref.Checkpoints
GO
