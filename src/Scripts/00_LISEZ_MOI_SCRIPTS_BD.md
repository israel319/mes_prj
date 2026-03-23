# Scripts de correction de la Base de Données

## Vue d'ensemble

Ces scripts corrigent les problèmes identifiés dans l'audit BD.  
**Audit complet basé sur la base réelle** : voir [AUDIT_BD_REEL.md](AUDIT_BD_REEL.md)

**Base de données** : `AppDev_KCCMaterialFlow_DB_Dev`  
**Serveur** : `CDKTGNBK01127`

## Ordre d'exécution obligatoire

| # | Script | Description | Risque | Réversible |
|---|--------|-------------|--------|------------|
| 0 | `Phase0_Diagnostic.sql` | Diagnostic complet : tables, PK, FK, index, colonnes | Aucun | N/A (lecture seule) |
| 1 | `Phase1_PK_Rename.sql` | Renomme les PK `Id` → `IdXxx` (8 tables) + ajout colonne TypeMaterielDefaut + MotifRejet resize | Moyen | Oui |
| 2 | `Phase2_FK_Constraints.sql` | Ajoute 6 contraintes FK manquantes | Faible | Oui |
| 3 | `Phase3_Indexes.sql` | Ajoute 16 index de performance | Faible | Oui |
| 4 | ~~`Phase4_Cleanup_BonsSortie.sql`~~ | **OBSOLÈTE** — colonnes absentes en DB réelle | — | — |
| 5 | `Phase5_Cleanup_Orphans.sql` | Supprime 1 table + 3 colonnes orphelines | Moyen | Non |

## Détail des corrections

### Phase 1 — PK Rename (généré par EF Core migration)
Renomme les colonnes PK de 8 tables de `Id` → `IdXxx` :
- `T_SoldesMateriels.Id` → `IdSoldeMateriel`
- `T_Sites.Id` → `IdSite`
- `T_RaisonsSortie.Id` → `IdRaisonSortie`
- `T_NotificationsRejet.Id` → `IdNotificationRejet`
- `T_Employees.Id` → `IdEmployee`
- `T_Contrats.Id` → `IdContrat`
- `T_Compagnies.Id` → `IdCompagnie`
- `T_CategoriesSortie.Id` → `IdCategorieSortie`

**Note** : `T_Checkpoints.Id` et `T_PassagesCheckpoint.Id` gardent `Id` (par design EF).

### Phase 2 — FK Constraints
Ajoute 6 contraintes FK manquantes (vérifiées sur la base réelle) :
1. `T_Bons.ContratId` → `T_Contrats.IdContrat` (NO ACTION)
2. `T_ItinerairesPrevu.BarriereId` → `T_Barrieres.IdBarriere` (NO ACTION)
3. `T_ItinerairesSortie.BarriereId` → `T_Barrieres.IdBarriere` (NO ACTION)
4. `T_MaterielsSortie.MaterielEntreeId` → `T_Materiels.IdMateriel` (NO ACTION)
5. `T_MaterielsSortie.BonEntreeId` → `T_Bons.IdBon` (NO ACTION)
6. `T_Bons.BonSortieAssocieId` → `T_BonsSortie.IdBon` (SET NULL)

**FK déjà existante** : `T_Anomalies.BarriereId` → `T_Barrieres.IdBarriere` (confirmée en DB)

**FK non ajoutées (polymorphiques)** : `T_Anomalies.BonId`, `T_ScanEvenements.BonId`, `T_PassagesCheckpoint.BonId` — ces colonnes référencent soit un BEM soit un BSM selon `TypeBon`, une FK directe est impossible.

### Phase 3 — Indexes
16 index couvrant les requêtes les plus fréquentes.

### Phase 4 — OBSOLÈTE
~~Supprime 2 colonnes orphelines de T_BonsSortie~~  
**Annulé** : l'inspection de la base réelle a confirmé que `DepartementDestination` et `NomReceveur` **n'existent pas** dans T_BonsSortie.

### Phase 5 — Cleanup Orphelins (NOUVEAU)
Basé sur l'inspection réelle de la base :

1. **Supprimer `T_CategoriesSortieShared`** (table entière)
   - 7 lignes, aucun mapping EF, aucune FK, doublon de `T_CategoriesSortie`

2. **Supprimer `T_BonEntreeHistory.BonIdBon`** (colonne + FK + index)
   - Shadow property EF, 19 lignes toutes NULL, FK orpheline vers T_Bons

3. **Supprimer `T_BonsSortie.BonSortieInterne_BonEntreeAssocieId`** (colonne)
   - Shadow property TPH, 24 lignes toutes NULL, doublon de `BonEntreeAssocieId`

4. **Supprimer `T_ScanEvenements.AnomalieIdAnomalie`** (colonne + FK + index)
   - Shadow property one-to-one non configurée, table vide

## Prérequis

1. **SAUVEGARDER** la base de données avant toute exécution
2. Exécuter dans SSMS lot par lot (chaque `GO` est un lot)
3. Commencer par `Phase0_Diagnostic.sql` pour vérifier l'état actuel
4. L'application doit être **arrêtée** pendant l'exécution des scripts

## Après exécution

1. Exécuter `Phase0_Diagnostic.sql` à nouveau pour vérifier
2. Relancer l'application
3. Vérifier que les pages fonctionnent (BEM, BSM, Sécurité)
4. La migration EF `DB_Audit_Phase1` sera marquée comme appliquée automatiquement
