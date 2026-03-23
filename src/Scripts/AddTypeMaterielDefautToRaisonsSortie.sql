-- ============================================
-- Script de migration: Ajout TypeMaterielDefaut à T_RaisonsSortie
-- Unifie le mapping RaisonSortie → TypeMateriel (enum workflow)
-- 
-- Valeurs TypeMateriel (enum):
--   1 = Circulaire
--   2 = Informatique
--   3 = FinChantier
--   4 = Residu
--   5 = Radioprotection
--   6 = Modification
--   7 = Pret
--  99 = Autre
-- ============================================

-- 1. Ajouter la colonne si elle n'existe pas
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.T_RaisonsSortie') 
    AND name = 'TypeMaterielDefaut'
)
BEGIN
    ALTER TABLE dbo.T_RaisonsSortie ADD TypeMaterielDefaut INT NULL;
    PRINT 'Colonne TypeMaterielDefaut ajoutée à dbo.T_RaisonsSortie'
END
ELSE
BEGIN
    PRINT 'Colonne TypeMaterielDefaut existe déjà'
END
GO

-- 2. Peupler les valeurs pour les raisons existantes
UPDATE dbo.T_RaisonsSortie SET TypeMaterielDefaut = 3 WHERE Code = 'FIN_CHANTIER';  -- FinChantier
UPDATE dbo.T_RaisonsSortie SET TypeMaterielDefaut = 4 WHERE Code = 'RESIDU';        -- Residu
UPDATE dbo.T_RaisonsSortie SET TypeMaterielDefaut = 5 WHERE Code = 'RADIO_PROT';    -- Radioprotection
UPDATE dbo.T_RaisonsSortie SET TypeMaterielDefaut = 6 WHERE Code = 'MODIF';         -- Modification
UPDATE dbo.T_RaisonsSortie SET TypeMaterielDefaut = 6 WHERE Code = 'REPAR';         -- Modification (réparation)
UPDATE dbo.T_RaisonsSortie SET TypeMaterielDefaut = 6 WHERE Code = 'RENOV';         -- Modification (rénovation)
UPDATE dbo.T_RaisonsSortie SET TypeMaterielDefaut = 2 WHERE Code = 'INFO';          -- Informatique
UPDATE dbo.T_RaisonsSortie SET TypeMaterielDefaut = 1 WHERE Code = 'CIRC';          -- Circulaire
UPDATE dbo.T_RaisonsSortie SET TypeMaterielDefaut = 7 WHERE Code = 'PRET';          -- Pret
GO

-- 3. Vérification
PRINT ''
PRINT 'Vérification du mapping RaisonSortie → TypeMateriel:'
SELECT Code, Nom, TypeMaterielDefaut,
    CASE TypeMaterielDefaut
        WHEN 1 THEN 'Circulaire'
        WHEN 2 THEN 'Informatique'
        WHEN 3 THEN 'FinChantier'
        WHEN 4 THEN 'Residu'
        WHEN 5 THEN 'Radioprotection'
        WHEN 6 THEN 'Modification'
        WHEN 7 THEN 'Pret'
        WHEN 99 THEN 'Autre'
        ELSE '(non mappé)'
    END AS TypeMaterielLabel
FROM dbo.T_RaisonsSortie
ORDER BY Code;
GO
