-- Script pour vérifier l'état du stock et les liaisons BEM/BSM

-- 1. Voir les matériels BEM avec leur stock disponible
SELECT 
    b.IdBon AS BemId,
    b.NumeroReference AS BemNumero,
    m.IdMateriel,
    m.Designation,
    m.Quantite AS QteInitiale,
    m.QuantiteDisponible AS QteDisponible,
    m.Quantite - m.QuantiteDisponible AS QteSortie
FROM bem.BonsEntree b
JOIN bem.Materiels m ON m.BonId = b.IdBon
ORDER BY b.IdBon DESC, m.IdMateriel;

-- 2. Voir les matériels BSM et leurs liaisons BEM
SELECT 
    bs.IdBon AS BsmId,
    bs.NumeroReference AS BsmNumero,
    bs.StatutActuel,
    ms.IdMateriel AS MaterielSortieId,
    ms.MaterielEntreeId,  -- <-- Clé pour le décrémentage
    ms.BonEntreeId,
    ms.BonEntreeNumero,
    ms.Designation,
    ms.Quantite AS QteSortie,
    ms.QuantiteInitialeBem,
    ms.QuantiteDisponible AS QteRestanteApresCreation
FROM bsm.BonsSortie bs
JOIN bsm.MaterielsS ms ON ms.BonSortieId = bs.IdBon
WHERE ms.MaterielEntreeId IS NOT NULL
ORDER BY bs.IdBon DESC, ms.IdMateriel;

-- 3. Jointure complète: BSM → BEM pour voir si MaterielEntreeId pointe vers le bon matériel
SELECT 
    bs.NumeroReference AS BsmNumero,
    bs.StatutActuel AS BsmStatut,
    ms.Designation AS MaterielBsm,
    ms.Quantite AS QteSortie,
    ms.MaterielEntreeId,
    bem.NumeroReference AS BemNumero,
    mat.IdMateriel AS BemMaterielId,
    mat.Designation AS MaterielBem,
    mat.Quantite AS BemQteInitiale,
    mat.QuantiteDisponible AS BemQteDisponible,
    CASE 
        WHEN mat.IdMateriel IS NULL THEN 'ERREUR: MaterielEntreeId ne pointe vers rien'
        WHEN ms.MaterielEntreeId = mat.IdMateriel THEN 'OK: Liaison correcte'
        ELSE 'ERREUR: Liaison incorrecte'
    END AS StatutLiaison
FROM bsm.BonsSortie bs
JOIN bsm.MaterielsS ms ON ms.BonSortieId = bs.IdBon
LEFT JOIN bem.Materiels mat ON mat.IdMateriel = ms.MaterielEntreeId
LEFT JOIN bem.BonsEntree bem ON bem.IdBon = mat.BonId
WHERE ms.MaterielEntreeId IS NOT NULL
ORDER BY bs.IdBon DESC;
