-- ============================================================
-- Migration : Ajout de la colonne RoleCode dans T_ApprobationsSortie
-- Date      : 2026-03-13
-- Objectif  : Stocker le rôle requis par étape directement en BD
--             pour une chaine d'approbation 100% pilotée par la BD.
-- ============================================================

-- 1. Ajouter la colonne (nullable d'abord pour rétrocompatibilité)
ALTER TABLE dbo.T_ApprobationsSortie
    ADD RoleCode NVARCHAR(50) NULL;
GO

-- 2. Renseigner les lignes existantes avec une valeur déduite de NomEtape
--    (meilleur effort sur les anciens enregistrements)
UPDATE dbo.T_ApprobationsSortie
SET RoleCode = CASE
    WHEN UPPER(NomEtape) LIKE '%INFORMATIQUE%' OR UPPER(NomEtape) LIKE '% IT%' OR UPPER(NomEtape) = 'IT'  THEN 'IT'
    WHEN UPPER(NomEtape) LIKE '%SUPERVISEUR%'                                                              THEN 'Superviseur'
    WHEN UPPER(NomEtape) LIKE '%GENERAL MANAGER%' OR UPPER(NomEtape) = 'GM'                               THEN 'GM'
    WHEN UPPER(NomEtape) LIKE '%ENVIRONNEMENT%'                                                            THEN 'Environnement'
    WHEN UPPER(NomEtape) LIKE '%OPJ%'                                                                      THEN 'OPJ'
    WHEN UPPER(NomEtape) LIKE '%IDENTIFICATION%'                                                           THEN 'Identification'
    ELSE NomEtape   -- fallback : réutilise NomEtape comme RoleCode pour les cas inconnus
END
WHERE RoleCode IS NULL;
GO

-- 3. Passer la colonne NOT NULL avec valeur par défaut vide
ALTER TABLE dbo.T_ApprobationsSortie
    ALTER COLUMN RoleCode NVARCHAR(50) NOT NULL;
GO

ALTER TABLE dbo.T_ApprobationsSortie
    ADD CONSTRAINT DF_ApprobationsSortie_RoleCode DEFAULT ('') FOR RoleCode;
GO

-- 4. Index pour la requête principale : "bons en attente pour mon rôle"
CREATE NONCLUSTERED INDEX IX_ApprobationsSortie_RoleCode_Decision
    ON dbo.T_ApprobationsSortie (RoleCode, Decision)
    INCLUDE (BonSortieId, OrdreEtape);
GO

PRINT 'Migration AddRoleCodeToApprobationsSortie terminée avec succès.';
GO
