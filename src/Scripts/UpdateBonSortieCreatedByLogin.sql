-- Script pour mettre à jour les anciens bons de sortie avec CreatedByLogin
-- Exécuter ce script après la migration AddCreatedByLoginToBonSortie

-- Mise à jour basée sur le NomDemandeur en cherchant l'utilisateur correspondant
UPDATE bs
SET bs.CreatedByLogin = u.Login
FROM bsm.BonsSortie bs
INNER JOIN dbo.Utilisateurs u ON bs.NomDemandeur = u.NomComplet
WHERE bs.CreatedByLogin = '' OR bs.CreatedByLogin IS NULL;

-- Pour les bons sans correspondance, utiliser le premier utilisateur admin comme fallback
-- (À adapter selon vos besoins)
-- UPDATE bsm.BonsSortie
-- SET CreatedByLogin = 'admin'
-- WHERE CreatedByLogin = '' OR CreatedByLogin IS NULL;

-- Vérifier les résultats
SELECT IdBon, NumeroReference, NomDemandeur, CreatedByLogin 
FROM bsm.BonsSortie
ORDER BY IdBon;
