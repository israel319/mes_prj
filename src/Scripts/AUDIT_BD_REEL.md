# Audit Base de Données Réelle

**Base** : `AppDev_KCCMaterialFlow_DB_Dev`  
**Serveur** : `CDKTGNBK01127`  
**Date d'inspection** : 2026-03-10  
**Méthode** : Requêtes directes sur `sys.*` via `sqlcmd`

---

## 1. Inventaire des Tables (34 tables)

| # | Table | Lignes | PK | PK encore nommée `Id` ? | Statut |
|---|-------|--------|----|-----------------------|--------|
| 1 | T_Anomalies | 0 | IdAnomalie | Non | ✅ OK |
| 2 | T_Approbations | 94 | IdApprobation | Non | ✅ OK |
| 3 | T_ApprobationsSortie | 62 | IdApprobation | Non | ✅ OK |
| 4 | T_AuditLogs | 4 | IdAuditLog | Non | ✅ OK |
| 5 | T_Barrieres | 8 | IdBarriere | Non | ✅ OK |
| 6 | T_BonEntreeHistory | 19 | IdHistory | Non | ⚠️ Colonne orpheline `BonIdBon` |
| 7 | T_Bons | 26 | IdBon | Non | ⚠️ FK manquantes |
| 8 | T_BonSortieHistories | 89 | IdHistory | Non | ✅ OK |
| 9 | T_BonsSortie | 24 | IdBon | Non | ⚠️ Colonne orpheline |
| 10 | **T_CategoriesSortie** | **2** | **Id** | **OUI** | ⚠️ PK à renommer |
| 11 | **T_CategoriesSortieShared** | **7** | IdCategorie | Non | **❌ À SUPPRIMER** |
| 12 | **T_Checkpoints** | **8** | **Id** | **OUI (par design)** | ✅ OK |
| 13 | **T_Compagnies** | **12** | **Id** | **OUI** | ⚠️ PK à renommer |
| 14 | **T_Contrats** | **11** | **Id** | **OUI** | ⚠️ PK à renommer |
| 15 | T_Departements | 10 | IdDepartement | Non | ✅ OK |
| 16 | **T_Employees** | **25** | **Id** | **OUI** | ⚠️ PK à renommer |
| 17 | T_HistoriqueScans | 0 | IdHistorique | Non | ✅ OK |
| 18 | T_ItinerairesPrevu | 0 | IdItineraire | Non | ⚠️ FK manquante |
| 19 | T_ItinerairesSortie | 4 | IdItineraire | Non | ⚠️ FK manquante |
| 20 | T_Materiels | 45 | IdMateriel | Non | ✅ OK |
| 21 | T_MaterielsSortie | 31 | IdMateriel | Non | ⚠️ FK manquantes |
| 22 | **T_NotificationsRejet** | **3** | **Id** | **OUI** | ⚠️ PK à renommer |
| 23 | T_ParametresSysteme | 14 | IdParametre | Non | ✅ OK |
| 24 | **T_PassagesCheckpoint** | **0** | **Id** | **OUI (par design)** | ✅ OK |
| 25 | **T_RaisonsSortie** | **7** | **Id** | **OUI** | ⚠️ PK à renommer |
| 26 | T_Roles | 10 | IdRole | Non | ✅ OK |
| 27 | T_ScanEvenements | 0 | IdScan | Non | ⚠️ Colonne orpheline |
| 28 | **T_Sites** | **8** | **Id** | **OUI** | ⚠️ PK à renommer |
| 29 | **T_SoldesMateriels** | **0** | **Id** | **OUI** | ⚠️ PK à renommer |
| 30 | T_Statuts | 8 | IdStatut | Non | ⚠️ Pas référencée par FK |
| 31 | T_TypesMateriels | 5 | IdTypeMateriel | Non | ✅ OK |
| 32 | T_UtilisateurRoles | 1 | IdUtilisateurRole | Non | ✅ OK |
| 33 | T_Utilisateurs | 7 | IdUtilisateur | Non | ✅ OK |

---

## 2. Tables à SUPPRIMER

### 2.1 `T_CategoriesSortieShared` — **SUPPRIMER**

- **7 lignes**, structure enrichie (10 colonnes)
- **AUCUN mapping EF Core** — pas de configuration, pas d'entité
- **AUCUNE FK** — ni entrante ni sortante
- **Doublon conceptuel** de `T_CategoriesSortie` (2 lignes, structure différente)
- **Origine** : probablement une version précédente de l'entité `CategoriesSortie` du module Shared, qui a été remplacée par `T_CategoriesSortie`
- **Données** : catégories de sortie (FIN_CHANTIER, RESIDU, RADIOPROTECTION, etc.) — ces données peuvent être réinsérées dans `T_CategoriesSortie` si nécessaire

### Comparaison des deux tables :

| Aspect | T_CategoriesSortie (GARDE) | T_CategoriesSortieShared (SUPPR) |
|--------|---------------------------|--------------------------------|
| Lignes | 2 | 7 |
| PK | `Id` (à renommer) | `IdCategorie` |
| Colonnes | 9 (Nom, Code, RequiertBarrieres, RequiertBonEntree, TypeEntite) | 10 (Libelle, RequiertApprobationIT, RequiertApprobationEnvironnement) |
| EF Mapping | ✅ oui (`CategoriesSortieConfiguration`) | ❌ non |
| FK entrante | ✅ `T_RaisonsSortie.CategorieId` | ❌ aucune |

---

## 3. Colonnes Orphelines à SUPPRIMER

### 3.1 `T_BonEntreeHistory.BonIdBon` — **SUPPRIMER**

- **Type** : `int NULL`
- **19 lignes** : **toutes NULL** (0 valeurs non-null)
- **Origine** : propriété shadow EF Core créée historiquement quand la relation `BonEntreeHistory → Bon` n'était pas correctement configurée (migration `AddCreatedByLoginToBonSortie`)
- **FK orpheline** : `FK_T_BonEntreeHistory_T_Bons_BonIdBon` (doublon de `FK_T_BonEntreeHistory_T_Bons_BonId`)
- **Index orphelin** : `IX_T_BonEntreeHistory_BonIdBon`
- **Action** : Supprimer la FK + index + colonne

### 3.2 `T_BonsSortie.BonSortieInterne_BonEntreeAssocieId` — **SUPPRIMER**

- **Type** : `int NULL`
- **24 lignes** : **toutes NULL** (0 valeurs non-null)
- **Origine** : propriété shadow EF Core créée pour le type dérivé `BonSortieInterne` (héritage TPH), migration `AddBemBsmLiaisonColumns`
- **Pas de FK, pas d'index**
- **Doublon fonctionnel** de `BonEntreeAssocieId` (qui lui est utilisé et a une FK)
- **Action** : Supprimer la colonne

### 3.3 `T_ScanEvenements.AnomalieIdAnomalie` — **SUPPRIMER**

- **Type** : `int NULL`
- **0 lignes** dans la table (table vide)
- **Origine** : propriété shadow EF Core pour la navigation `virtual Anomalie? Anomalie` (one-to-one) dans l'entité `ScanEvenement`, pas configurée explicitement. Le config EF actuel utilise `HasMany(Anomalies).WithOne().HasForeignKey(ScanId)` (one-to-many par `ScanId` côté Anomalie), rendant cette colonne inutile.
- **FK existante** : `FK_T_ScanEvenements_T_Anomalies_AnomalieIdAnomalie`
- **Index existant** : `IX_T_ScanEvenements_AnomalieIdAnomalie`
- **Action** : Supprimer la FK + index + colonne

### 3.4 `T_BonsSortie.DepartementDestination` et `NomReceveur` — **DÉJÀ ABSENTES**

- Ces colonnes mentionnées dans l'audit précédent **N'EXISTENT PAS** dans la base réelle
- Le script `Phase4_Cleanup_BonsSortie.sql` est donc **OBSOLÈTE**

---

## 4. FK Existantes (23 contraintes)

| # | Table enfant | Colonne FK | Table parent | Colonne PK | ON DELETE |
|---|-------------|-----------|-------------|-----------|----------|
| 1 | T_Anomalies | BarriereId | T_Barrieres | IdBarriere | NO_ACTION |
| 2 | T_Anomalies | ScanId | T_ScanEvenements | IdScan | CASCADE |
| 3 | T_Approbations | BonId | T_Bons | IdBon | CASCADE |
| 4 | T_ApprobationsSortie | BonSortieId | T_BonsSortie | IdBon | CASCADE |
| 5 | T_BonEntreeHistory | BonId | T_Bons | IdBon | CASCADE |
| 6 | T_BonEntreeHistory | **BonIdBon** | T_Bons | IdBon | NO_ACTION | ← **ORPHELINE** |
| 7 | T_BonSortieHistories | BonSortieId | T_BonsSortie | IdBon | CASCADE |
| 8 | T_BonsSortie | BonEntreeAssocieId | T_Bons | IdBon | SET_NULL |
| 9 | T_Checkpoints | SiteId | T_Sites | **Id** | CASCADE | ← **PK sera renommée** |
| 10 | T_Contrats | CompagnieId | T_Compagnies | **Id** | NO_ACTION | ← **PK sera renommée** |
| 11 | T_Employees | CompagnieId | T_Compagnies | **Id** | NO_ACTION | ← **PK sera renommée** |
| 12 | T_Employees | DepartementId | T_Departements | IdDepartement | NO_ACTION |
| 13 | T_HistoriqueScans | ScanId | T_ScanEvenements | IdScan | CASCADE |
| 14 | T_ItinerairesPrevu | BonId | T_Bons | IdBon | CASCADE |
| 15 | T_ItinerairesSortie | BonSortieId | T_BonsSortie | IdBon | CASCADE |
| 16 | T_Materiels | BonId | T_Bons | IdBon | CASCADE |
| 17 | T_MaterielsSortie | BonSortieId | T_BonsSortie | IdBon | CASCADE |
| 18 | T_PassagesCheckpoint | CheckpointId | T_Checkpoints | **Id** | CASCADE | ← **PK garde `Id`** |
| 19 | T_RaisonsSortie | CategorieId | T_CategoriesSortie | **Id** | CASCADE | ← **PK sera renommée** |
| 20 | T_ScanEvenements | **AnomalieIdAnomalie** | T_Anomalies | IdAnomalie | NO_ACTION | ← **ORPHELINE** |
| 21 | T_ScanEvenements | BarriereId | T_Barrieres | IdBarriere | NO_ACTION |
| 22 | T_UtilisateurRoles | IdRole | T_Roles | IdRole | CASCADE |
| 23 | T_UtilisateurRoles | IdUtilisateur | T_Utilisateurs | IdUtilisateur | CASCADE |

---

## 5. FK MANQUANTES (colonnes `*Id` sans contrainte et non PK)

| # | Table | Colonne | Devrait référencer | Priorité | Notes |
|---|-------|---------|-------------------|----------|-------|
| 1 | **T_Bons** | **ContratId** | T_Contrats.Id → IdContrat | 🔴 Haute | 26 bons, 11 contrats |
| 2 | **T_Bons** | **BonSortieAssocieId** | T_BonsSortie.IdBon | 🔴 Haute | Liaison BEM↔BSM |
| 3 | **T_ItinerairesPrevu** | **BarriereId** | T_Barrieres.IdBarriere | 🔴 Haute | Itinéraire sécurité |
| 4 | **T_ItinerairesSortie** | **BarriereId** | T_Barrieres.IdBarriere | 🔴 Haute | Itinéraire sécurité |
| 5 | **T_MaterielsSortie** | **MaterielEntreeId** | T_Materiels.IdMateriel | 🔴 Haute | Traçabilité |
| 6 | **T_MaterielsSortie** | **BonEntreeId** | T_Bons.IdBon | 🔴 Haute | Traçabilité |
| 7 | T_Anomalies | BonId | — | ⚪ Ne pas ajouter | Polymorphique (BEM ou BSM via TypeBon) |
| 8 | T_ScanEvenements | BonId | — | ⚪ Ne pas ajouter | Polymorphique (BEM ou BSM via TypeBon) |
| 9 | T_PassagesCheckpoint | BonId | — | ⚪ Ne pas ajouter | Polymorphique (TypeBon) |
| 10 | T_SoldesMateriels | BonEntreeId | T_Bons.IdBon | 🟡 Basse | Table vide, peut être supprimée |
| 11 | T_SoldesMateriels | MaterielEntreeId | T_Materiels.IdMateriel | 🟡 Basse | Table vide, peut être supprimée |

**Note** : Les colonnes `BonId` dans T_Anomalies, T_ScanEvenements et T_PassagesCheckpoint sont **polymorphiques** (elles référencent soit un BEM soit un BSM selon la colonne `TypeBon`). On **ne peut pas** créer de FK directe car la table cible varie.

---

## 6. PK à Renommer (`Id` → `IdXxx`)

8 tables dont la PK est encore nommée `Id` en DB (confirmé en DB réelle) :

| Table | PK actuelle | PK cible | FK impactées |
|-------|------------|---------|--------------|
| T_CategoriesSortie | Id | IdCategorie | T_RaisonsSortie.CategorieId |
| T_Compagnies | Id | IdCompagnie | T_Contrats.CompagnieId, T_Employees.CompagnieId |
| T_Contrats | Id | IdContrat | (aucune FK entrante aujourd'hui) |
| T_Employees | Id | IdEmployee | (aucune FK entrante) |
| T_NotificationsRejet | Id | IdNotification | (aucune FK entrante) |
| T_RaisonsSortie | Id | IdRaison | (aucune FK entrante) |
| T_Sites | Id | IdSite | T_Checkpoints.SiteId |
| T_SoldesMateriels | Id | IdSolde | (aucune FK entrante) |

**T_Checkpoints** et **T_PassagesCheckpoint** gardent `Id` par design (convention EF `HasKey(c => c.Id)`).

Script existant : `Phase1_PK_Rename.sql` (généré par migration EF `DB_Audit_Phase1`)

---

## 7. T_Statuts — Table Isolée

- **8 lignes** avec les statuts du workflow (BROUILLON, EN_ATTENTE_APPROBATION, APPROUVE, etc.)
- **AUCUNE table** ne la référence par FK
- **Pas utilisée en tant que FK** : l'application stocke le statut en tant que `string` dans `T_Bons.StatutActuel` et `T_BonsSortie.StatutActuel`
- **Utilisation réelle** : table de référence UI pour affichage (couleurs, icônes, transitions)
- **Recommandation** : **GARDER** mais considérer une normalisation future (FK vers T_Statuts)

---

## 8. Migrations Appliquées (20 migrations)

Dernière migration appliquée : `20260304085005_RenameTablesToDboConvention`  
Migration `20260310135804_DB_Audit_Phase1` : **GÉNÉRÉE mais PAS appliquée**

---

## 9. Index (105 index non-PK)

Les tables disposent d'une couverture d'index extensible. Points notables :
- `T_CategoriesSortieShared` a des index (`IX_CategoriesSortie_Code`, `IX_CategoriesSortie_Ordre`) → seront supprimés avec la table
- `IX_T_BonEntreeHistory_BonIdBon` → sera supprimé avec la colonne orpheline
- `IX_T_ScanEvenements_AnomalieIdAnomalie` → sera supprimé avec la colonne orpheline

---

## 10. Plan d'Exécution Recommandé

### Ordre d'exécution des scripts :

| Étape | Script | Actions | Prérequis |
|-------|--------|---------|-----------|
| 0 | `Phase0_Diagnostic.sql` | Diagnostic complet | Aucun |
| 1 | `Phase1_PK_Rename.sql` | Renommer 8 PK (`Id` → `IdXxx`) | Sauvegarde DB |
| 2 | `Phase2_FK_Constraints.sql` | Ajouter 6 FK manquantes | Phase 1 |
| 3 | `Phase3_Indexes.sql` | Ajouter index performances | Phase 1 |
| 4 | ~~Phase4_Cleanup_BonsSortie.sql~~ | ~~Supprimer DepartementDestination, NomReceveur~~ | **OBSOLÈTE — colonnes absentes** |
| 5 | **Phase5_Cleanup_Orphans.sql** (NOUVEAU) | Supprimer table + colonnes orphelines | Phase 1 |

### Détail Phase 5 :
1. Supprimer `T_CategoriesSortieShared` (table entière)
2. Supprimer `T_BonEntreeHistory.BonIdBon` (FK + index + colonne)
3. Supprimer `T_BonsSortie.BonSortieInterne_BonEntreeAssocieId` (colonne)
4. Supprimer `T_ScanEvenements.AnomalieIdAnomalie` (FK + index + colonne)

---

## 11. Corrections EF Core Appliquées (Code C#)

Pour empêcher EF Core de recréer les colonnes orphelines lors de la prochaine migration, les corrections suivantes ont été appliquées :

### 11.1 `ScanEvenement.cs` — Suppression navigation `Anomalie`

- **Fichier** : `Modules/Securite/.../Entities/ScanEvenement.cs`
- **Problème** : La propriété `public virtual Anomalie? Anomalie` (one-to-one) n'était pas configurée dans EF, créant automatiquement la colonne shadow `AnomalieIdAnomalie`
- **Correction** : Navigation supprimée. La collection `ICollection<Anomalie> Anomalies` (one-to-many via `ScanId`) reste et suffit
- **Impact UI** : `ScanDetailsDialog.razor` corrigé pour utiliser `Anomalies.FirstOrDefault()` au lieu de `Anomalie`

### 11.2 `BonSortieInterneConfiguration.cs` — Mapping explicite `BonEntreeAssocieId`

- **Fichier** : `Infrastructure/Data/Configurations/BonSortieInterneConfiguration.cs`
- **Problème** : En TPH, EF préfixe les propriétés des types dérivés avec `{TypeName}_` quand pas de `HasColumnName` → `BonSortieInterne_BonEntreeAssocieId`
- **Correction** : Ajout de `builder.Property(b => b.BonEntreeAssocieId).HasColumnName("BonEntreeAssocieId")` pour réutiliser la colonne existante (qui a déjà une FK vers `T_Bons`)
- **Résultat** : EF utilisera la colonne `BonEntreeAssocieId` existante, pas la shadow `BonSortieInterne_BonEntreeAssocieId`

### 11.3 `BonEntreeHistoryConfiguration.cs` — Vérification OK

- **Fichier** : `Infrastructure/Data/Configurations/BonEntreeHistoryConfiguration.cs`
- **Config actuelle** : `HasOne(h => h.Bon).WithMany().HasForeignKey(h => h.BonId)` — correctement mappée
- **Résultat** : EF ne recréera PAS `BonIdBon` (artefact historique de la migration `AddCreatedByLoginToBonSortie`)

### Build : ✅ 0 erreurs, 0 warnings
