# 🔍 AUDIT PROFESSIONNEL DE LA BASE DE DONNÉES
## KCCMaterialFlow — `AppDev_KCCMaterialFlow_DB_Dev`

**Date** : Juillet 2025  
**Technologie** : SQL Server / EF Core 10.0.2 / .NET 10.0  
**Convention de nommage** : `dbo.T_NomTable`

---

## TABLE DES MATIÈRES

1. [Vue d'ensemble](#1-vue-densemble)
2. [Inventaire des tables](#2-inventaire-des-tables)
3. [CRITIQUE — Système dual d'entités](#3-critique--système-dual-dentités)
4. [Problèmes de clés primaires (PK)](#4-problèmes-de-clés-primaires-pk)
5. [Problèmes de clés étrangères (FK)](#5-problèmes-de-clés-étrangères-fk)
6. [Dénormalisation excessive](#6-dénormalisation-excessive)
7. [Champs string au lieu de FK/Enum](#7-champs-string-au-lieu-de-fkenum)
8. [Tables redondantes](#8-tables-redondantes)
9. [Tables de référence manquantes](#9-tables-de-référence-manquantes)
10. [Incohérences d'héritage (TPH)](#10-incohérences-dhéritage-tph)
11. [Index manquants](#11-index-manquants)
12. [Plan d'action priorisé](#12-plan-daction-priorisé)
13. [Schéma cible](#13-schéma-cible)

---

## 1. Vue d'ensemble

### Score de santé actuel : 4/10 ⚠️

| Critère | Score | Problème majeur |
|---------|-------|-----------------|
| Normalisation | 3/10 | Dénormalisation massive (15+ champs redondants) |
| Intégrité référentielle | 4/10 | FK manquantes, strings au lieu de FK |
| Cohérence des PK | 5/10 | Mix `Id` / `IdXxx` selon les tables |
| Architecture entités | 2/10 | Système dual Domain vs Module (52 entités pour ~26 concepts) |
| Tables de référence | 4/10 | T_Statuts existe mais n'est utilisée par aucune FK |
| Redondance | 3/10 | Tables dupliquées (Checkpoint/Barriere, HistoriqueScan/ScanEvenement) |
| Conventions de nommage | 7/10 | Bonne convention `T_NomTable` mais PK incohérentes |

### Statistique actuelle
- **35+ tables** dans le schéma `dbo`
- **52 entités C#** (28 Domain + 24 Module) pour ~26 concepts métier
- **39 fichiers** importent encore des entités Domain (migration partielle)
- **15+ champs dénormalisés** qui devraient être des JOINs
- **10+ champs string** qui devraient être des FK vers des tables de référence

---

## 2. Inventaire des tables

### Tables Métier (Bons d'Entrée) — TPH
| Table | Entité | PK | Description |
|-------|--------|-----|------------|
| `T_Bons` | Bon → BonEntree | `IdBon` | TPH avec discriminateur `TypeDiscriminator` |
| `T_Materiels` | Materiel | `IdMateriel` | Matériels liés à un BEM |
| `T_Approbations` | Approbation | `IdApprobation` | Workflow d'approbation BEM |
| `T_ItinerairesPrevu` | ItinerairePrevu | `IdItineraire` | Itinéraire de passage BEM |
| `T_BonEntreeHistory` | BonEntreeHistory | `IdHistory` | Historique actions BEM |

### Tables Métier (Bons de Sortie) — TPH
| Table | Entité | PK | Description |
|-------|--------|-----|------------|
| `T_BonsSortie` | BonSortie → BonSortieExterne/Interne/Pret | `IdBon` | TPH avec discriminateur `TypeSortie` |
| `T_MaterielsSortie` | MaterielSortie | `IdMateriel` | Matériels liés à un BSM |
| `T_ApprobationsSortie` | ApprobationSortie | `IdApprobation` | Workflow d'approbation BSM |
| `T_ItinerairesSortie` | ItineraireSortie | `IdItineraire` | Itinéraire de passage BSM |
| `T_BonSortieHistories` | BonSortieHistory | `IdHistory` | Historique actions BSM |

### Tables Sécurité
| Table | Entité | PK | Description |
|-------|--------|-----|------------|
| `T_ScanEvenements` | ScanEvenement | `IdScan` | Événements de scan QR |
| `T_HistoriqueScans` | HistoriqueScan | `IdHistorique` | ⚠️ **REDONDANT** avec ScanEvenement |
| `T_Anomalies` | Anomalie | `IdAnomalie` | Anomalies signalées |

### Tables de Référence
| Table | Entité | PK | Description |
|-------|--------|-----|------------|
| `T_Compagnies` | Compagnie | `Id` | Compagnies contractantes |
| `T_Contrats` | Contrat | `Id` | Contrats (PO Numbers) |
| `T_Departements` | Departement | `IdDepartement` | Départements KCC |
| `T_Employees` | Employee | `Id` | Employés |
| `T_Sites` | Site | `Id` | Sites KCC |
| `T_Checkpoints` | Checkpoint | `Id` | ⚠️ **REDONDANT** avec Barriere |
| `T_Barrieres` | Barriere | `IdBarriere` | Barrières de sécurité |
| `T_CategoriesSortie` | CategorieSortie | `Id` | Catégories de sortie |
| `T_RaisonsSortie` | RaisonSortie | `Id` | Raisons de sortie |
| `T_SoldesMateriels` | SoldeMateriel | `Id` | ⚠️ **REDONDANT** avec Materiel.QuantiteDisponible |
| `T_PassagesCheckpoint` | PassageCheckpoint | `Id` | Passages aux checkpoints |
| `T_NotificationsRejet` | NotificationRejet | `Id` | Notifications de rejet |

### Tables Administration
| Table | Entité | PK | Description |
|-------|--------|-----|------------|
| `T_Utilisateurs` | Utilisateur | `IdUtilisateur` | Utilisateurs du système |
| `T_Roles` | Role | `IdRole` | Rôles (RBAC) |
| `T_UtilisateurRoles` | UtilisateurRole | `IdUtilisateurRole` | Liaison N:N Utilisateur↔Role |
| `T_Statuts` | Statut | `IdStatut` | ⚠️ Existe mais **AUCUNE FK n'y pointe** |
| `T_TypesMateriels` | TypeMateriel | `IdTypeMateriel` | Types de matériels |
| `T_ParametresSysteme` | ParametreSysteme | `IdParametre` | Paramètres configurables |
| `T_AuditLogs` | AuditLog | `IdAuditLog` (long) | Journal d'audit |

---

## 3. CRITIQUE — Système dual d'entités

### Le problème le plus grave 🔴

L'application maintient **DEUX jeux d'entités** pour les mêmes concepts :

| Concept | Entité Domain (`KCCMaterialFlow.Domain.Entities`) | Entité Module (`Module.*.Entities`) | Utilisée par EF ? |
|---------|--------------------------------------------------|--------------------------------------|-------------------|
| Bon (base) | `Domain.Entities.BonEntree.Bon` | `Module.BonEntree.Entities.Bon` | **Module** ✅ |
| BonEntree | `Domain.Entities.BonEntree.BonEntree` | `Module.BonEntree.Entities.BonEntree` | **Module** ✅ |
| Materiel | `Domain.Entities.BonEntree.Materiel` | `Module.BonEntree.Entities.Materiel` | **Module** ✅ |
| Approbation | `Domain.Entities.BonEntree.Approbation` | `Module.BonEntree.Entities.Approbation` | **Module** ✅ |
| BonSortie | `Domain.Entities.BonSortie.BonSortie` | `Module.BonSortie.Entities.BonSortie` | **Module** ✅ |
| Pret | `Domain.Entities.BonSortie.Pret` | `Module.BonSortie.Entities.Pret` | **Module** ✅ |
| ScanEvenement | `Domain.Entities.Securite.ScanEvenement` | `Module.Securite.Entities.ScanEvenement` | **Module** ✅ |
| Anomalie | `Domain.Entities.Securite.Anomalie` | `Module.Securite.Entities.Anomalie` | **Module** ✅ |
| Compagnie | `Domain.Entities.Shared.Compagnie` | ❌ n'existe pas | **Domain** ✅ |
| Contrat | `Domain.Entities.Shared.Contrat` | ❌ n'existe pas | **Domain** ✅ |
| Departement | `Domain.Entities.Shared.Departement` | ❌ n'existe pas | **Domain** ✅ |
| Employee | `Domain.Entities.Shared.Employee` | ❌ n'existe pas | **Domain** ✅ |
| Checkpoint | `Domain.Entities.Shared.Checkpoint` | `Module.Shared.Entities.Barriere` | **Les deux !** ⚠️ |

### Impact
- **Confusion** : Les développeurs ne savent pas quelle entité utiliser
- **Désynchronisation** : Les entités Domain ont divergé des entités Module (propriétés différentes)
- **39 fichiers** importent encore les entités Domain → couplage croisé entre couches

### Recommandation
**Supprimer les entités Domain** (sauf les enums) et tout migrer vers les entités Module. Les entités de référence `Shared` (Compagnie, Contrat, etc.) doivent être déplacées vers `Module.Shared.Entities`.

---

## 4. Problèmes de clés primaires (PK)

### Incohérence de nommage

| Convention | Tables concernées | Exemples |
|-----------|-------------------|----------|
| `Id` simple | 9 tables | T_Compagnies, T_Contrats, T_Sites, T_Employees, T_Checkpoints, T_CategoriesSortie, T_RaisonsSortie, T_SoldesMateriels, T_PassagesCheckpoint |
| `IdNomEntité` | 14 tables | T_Bons (IdBon), T_Departements (IdDepartement), T_Utilisateurs (IdUtilisateur), T_Barrieres (IdBarriere), T_Roles (IdRole), T_Statuts (IdStatut), etc. |

### Recommandation
Standardiser sur **`Id` simple** pour toutes les tables (convention EF Core standard), ou sur **`IdNomEntité`** (convention KCC existante majoritaire). Comme la convention KCC `IdXxx` est majoritaire (14/23), adopter `IdXxx` partout.

### Plan de correction PK
```
T_Compagnies    : Id → IdCompagnie
T_Contrats      : Id → IdContrat
T_Sites         : Id → IdSite
T_Employees     : Id → IdEmployee
T_Checkpoints   : Id → IdCheckpoint  (ou suppression car redondant)
T_CategoriesSortie : Id → IdCategorieSortie
T_RaisonsSortie : Id → IdRaisonSortie
T_SoldesMateriels : Id → IdSoldeMateriel (ou suppression car redondant)
T_PassagesCheckpoint : Id → IdPassageCheckpoint
T_NotificationsRejet : Id → IdNotificationRejet
```

---

## 5. Problèmes de clés étrangères (FK)

### FK manquantes 🔴

| Table | Champ | Devrait pointer vers | Actuellement |
|-------|-------|---------------------|-------------|
| `T_Bons` (BonEntree) | `ContratId` | T_Contrats | ❌ **Pas de FK configurée** — le champ existe mais aucune HasOne/HasMany dans BonEntreeConfiguration |
| `T_ItinerairesPrevu` | `BarriereId` | T_Barrieres | ❌ **Pas de FK ni navigation** |
| `T_ItinerairesSortie` | `BarriereId` | T_Barrieres | ❌ **Pas de FK ni navigation** |
| `T_Anomalies` | `BonId` | T_Bons OU T_BonsSortie | ❌ **FK polymorphe impossible** — `TypeBon` string discrimine |
| `T_Anomalies` | `BarriereId` | T_Barrieres | ❌ **Pas de FK configurée** |
| `T_Anomalies` | `ScanId` | T_ScanEvenements | ❌ **Pas de FK configurée** |
| `T_ScanEvenements` | `BonId` | T_Bons OU T_BonsSortie | ❌ **FK polymorphe impossible** — `TypeBon` string discrimine |
| `T_HistoriqueScans` | `ScanId` | T_ScanEvenements | ❌ **Pas de FK configurée** |
| `T_MaterielsSortie` | `MaterielEntreeId` | T_Materiels | ❌ **Pas de FK configurée** — référence orpheline |
| `T_MaterielsSortie` | `BonEntreeId` | T_Bons | ❌ **Pas de FK — champ dénormalisé** |
| `T_BonsSortie` (Externe) | `BonEntreeAssocieId` | T_Bons | ✅ FK existe (BonSortieExterneConfiguration) |
| `T_Bons` (BonEntree) | `BonSortieAssocieId` | T_BonsSortie | ❌ **Pas de FK configurée** |

### FK polymorphes (BonId + TypeBon) 🟡
Les tables `T_ScanEvenements`, `T_Anomalies`, `T_HistoriqueScans` utilisent un pattern `BonId` + `TypeBon` (string "BEM" ou "BSM") pour pointer vers T_Bons **OU** T_BonsSortie. Ce pattern empêche toute contrainte FK.

**Solution** : Séparer en deux FK explicites : `BonEntreeId` (nullable) + `BonSortieId` (nullable) avec une contrainte CHECK `(BonEntreeId IS NOT NULL AND BonSortieId IS NULL) OR (BonEntreeId IS NULL AND BonSortieId IS NOT NULL)`.

---

## 6. Dénormalisation excessive

### Champs dénormalisés identifiés 🔴

| Table | Champ dénormalisé | Source réelle | Impact |
|-------|-------------------|---------------|--------|
| `T_Bons` (BonEntree) | `NomCompagnie` | JOIN T_Contrats → T_Compagnies.Nom | Désynchronisation si Compagnie modifiée |
| `T_Bons` (BonEntree) | `NumeroContrat` | JOIN T_Contrats.PoNumber | Désynchronisation si Contrat modifié |
| `T_Bons` (BonEntree) | `BonSortieAssocieNumero` | JOIN T_BonsSortie.NumeroReference | Désynchronisation |
| `T_MaterielsSortie` | `BonEntreeNumero` | JOIN T_Bons.NumeroReference | Désynchronisation |
| `T_MaterielsSortie` | `CodeProduitSerial` | JOIN T_Materiels.CodeProduitSerial | Dupliqué du matériel source |
| `T_MaterielsSortie` | `Designation` | JOIN T_Materiels.Designation | Dupliqué du matériel source |
| `T_MaterielsSortie` | `QuantiteInitialeBem` | JOIN T_Materiels.Quantite | Snapshot historique (acceptable) |
| `T_HistoriqueScans` | `CodeBarriere` | JOIN T_Barrieres.CodeBarriere | Dupliqué |
| `T_HistoriqueScans` | `NomBarriere` | JOIN T_Barrieres.NomBarriere | Dupliqué |
| `T_HistoriqueScans` | `NumeroReferenceBon` | JOIN T_Bons/T_BonsSortie | Dupliqué |
| `T_ScanEvenements` | `NumeroReferenceBon` | JOIN T_Bons/T_BonsSortie | Dupliqué |
| `T_ScanEvenements` | `AgentNom` | JOIN T_Utilisateurs.NomComplet | Dupliqué |
| `T_Anomalies` | `NumeroReferenceBon` | JOIN T_Bons/T_BonsSortie | Dupliqué |
| `T_Anomalies` | `SignaleParNom` | JOIN T_Utilisateurs.NomComplet | Dupliqué |

### Analyse
- **13 champs** sont des copies dénormalisées qui risquent la désynchronisation
- Seul `QuantiteInitialeBem` dans MaterielSortie est un snapshot historique légitime (la quantité au moment de la sortie)
- Les noms d'agents (`AgentNom`, `SignaleParNom`, `ActionByNom`) sont des snapshots d'affichage — **tolérables** pour l'audit trail mais doivent être marqués comme tels

### Recommandation
- **Supprimer** : `NomCompagnie`, `NumeroContrat`, `BonSortieAssocieNumero`, `BonEntreeNumero`, `CodeBarriere`, `NomBarriere`, `NumeroReferenceBon`
- **Remplacer par** : FK + JOIN (eager loading ou projection DTO)
- **Conserver** comme snapshots d'audit : `AgentNom`, `ActionByNom`, `SignaleParNom`, `QuantiteInitialeBem`

---

## 7. Champs string au lieu de FK/Enum

### Problème : Strings "magiques" au lieu de références typées 🔴

| Table | Champ | Valeurs possibles | Solution |
|-------|-------|-------------------|----------|
| `T_Bons` | `StatutActuel` (varchar 30) | "Draft", "EnAttenteApprobation", "Approuvé", "Rejeté", "Expiré" | FK → `T_Statuts.CodeStatut` |
| `T_BonsSortie` | `StatutActuel` (varchar 30) | "Draft", "Soumis", "Approuvé", "Rejeté" | FK → `T_Statuts.CodeStatut` |
| `T_Approbations` | `Decision` (varchar 50) | "En attente", "Approuvé", "Rejeté", "Approuvé avec réserves" | Enum C# `DecisionApprobation` |
| `T_ApprobationsSortie` | `Decision` (varchar 50) | Idem | Enum C# `DecisionApprobation` |
| `T_ScanEvenements` | `StatutScan` (varchar 30) | "Valid", "Invalid", "Expired", "AlreadyUsed", "NotFound" | Enum C# `StatutScan` (déjà existe dans Domain!) |
| `T_ScanEvenements` | `TypeMouvement` (varchar 20) | "Entree", "Sortie" | Enum C# (2 valeurs) |
| `T_ScanEvenements` | `TypeBon` (varchar 10) | "BEM", "BSM" | Enum C# `BonType` (existe déjà!) |
| `T_ItinerairesSortie` | `StatutPassage` (varchar 50) | "Prévu", "Passé", "Anomalie" | Enum C# `StatutPassage` (existe déjà!) |
| `T_Anomalies` | `TypeAnomalie` (varchar 50) | "QRCodeInvalide", "BonExpire", "MaterielManquant", etc. | Enum C# `TypeAnomalie` (existe déjà!) |
| `T_Anomalies` | `NiveauGravite` (varchar 20) | "Faible", "Moyen", "Eleve", "Critique" | Enum C# ou FK table ref |
| `T_HistoriqueScans` | `TypeMouvement`, `TypeBon` | Idem ScanEvenement | Enums existants |
| `T_BonSortieHistories` | `TypeAction` (varchar 50) | "Création", "Modification", "Soumission", etc. | Enum C# `ActionBonSortie` (à créer) |
| `T_Barrieres` | `TypeBarriere` (varchar 50) | "Entrée", "Sortie", "Mixte" | Enum C# `TypeBarriere` |

### Paradoxe T_Statuts
La table `T_Statuts` existe avec 12+ statuts seedés (Draft, En attente approbation, Approuvé, etc.) avec couleurs et icônes, **MAIS aucune entité ne la référence par FK**. Le champ `StatutActuel` dans `T_Bons` et `T_BonsSortie` est un varchar stockant la valeur en texte libre.

### Paradoxe Enums Domain
Le projet `KCCMaterialFlow.Domain/Enums/` contient déjà les enums :
- `StatutScan` ✅
- `StatutPassage` ✅ 
- `TypeAnomalie` ✅
- `BonType` ✅
- `BonStatut` ✅

Mais les entités Module stockent ces valeurs comme **strings** et ne les utilisent pas.

### Recommandation
1. **Utiliser les enums existants** avec conversion `HasConversion<string>()` pour compatibilité DB
2. **Créer un enum `DecisionApprobation`** : `EnAttente`, `Approuve`, `Rejete`, `ApprouveAvecReserves`
3. **Relier `StatutActuel`** à `T_Statuts` via FK (ou utiliser l'enum `BonStatut` existant)

---

## 8. Tables redondantes

### 8.1 `T_HistoriqueScans` vs `T_ScanEvenements` 🔴

| Propriété | ScanEvenement | HistoriqueScan |
|-----------|--------------|----------------|
| Scan ID | `IdScan` (PK) | `ScanId` (FK vers ScanEvenement) |
| Date/heure | `DateHeureScan` | `DateHeureMouvement` |
| Type mouvement | `TypeMouvement` | `TypeMouvement` |
| Type bon | `TypeBon` | `TypeBon` |
| N° référence | `NumeroReferenceBon` | `NumeroReferenceBon` |
| Barrière | `BarriereId` (FK) | `CodeBarriere`, `NomBarriere` (dénormalisé!) |
| Statut scan | `StatutScan` | ❌ absent |
| Agent | `AgentLogin`, `AgentNom` | ❌ absent |
| QR data | `QRCodeData`, `QRCodeHash` | ❌ absent |
| Matériels | ❌ absent | `NombreMateriels`, `ResumeMateriels`, `MaterielsJson` |
| Direction | ❌ absent | `Direction`, `Provenance`, `Destination` |

**Verdict** : `HistoriqueScan` est une **vue dénormalisée** de `ScanEvenement` + données du bon. Elle devrait être :
- Une **vue SQL** (pas une table physique), OU
- Des champs supplémentaires sur `ScanEvenement`

### 8.2 `T_Checkpoints` vs `T_Barrieres` 🔴

| Propriété | Checkpoint (Domain) | Barriere (Module) |
|-----------|---------------------|-------------------|
| ID | `Id` | `IdBarriere` |
| Code | `Code` | `CodeBarriere` |
| Nom | `Nom` | `NomBarriere` |
| Site | `SiteId` (FK → Sites) | ❌ absent |
| Localisation | ❌ absent | `Localisation` |
| Type | ❌ absent | `TypeBarriere` |
| Horaires | ❌ absent | `HorairesOuverture` |
| Téléphone | ❌ absent | `Telephone` |
| Actif | `EstActif` | `EstActive` |
| Ordre | `OrdreDefaut` | `OrdreAffichage` |

**Verdict** : Même concept métier. `Barriere` est la version enrichie. **Fusionner** en gardant `Barriere` et y ajoutant `SiteId` comme FK.

### 8.3 `T_SoldesMateriels` vs `T_Materiels.QuantiteDisponible` 🟡

`SoldeMateriel` contient `MaterielId`, `BonEntreeId`, `QuantiteTotale`, `QuantiteSortie`, `QuantiteRestante`. Or `Materiel` a déjà `Quantite` et `QuantiteDisponible`.

**Verdict** : Table redondante. Les soldes doivent être calculés depuis `Materiel.Quantite` - somme des `MaterielSortie.Quantite` des BSM approuvés.

### 8.4 `T_PassagesCheckpoint` — À migrer 🟡

Liée à `Checkpoint` (ancien). Les passages sont maintenant gérés par `ScanEvenement` + `ItinerairePrevu`/`ItineraireSortie`. Potentiellement obsolète.

---

## 9. Tables de référence manquantes

### Tables à créer pour normaliser

| Table proposée | Remplace | Valeurs |
|---------------|----------|---------|
| _(Utiliser `T_Statuts` existante)_ | Strings `StatutActuel` dans Bons | Draft, EnAttenteApprobation, Approuvé, Rejeté, Expiré, Cloturé |
| `T_TypesAnomalie` | String `TypeAnomalie` dans Anomalies | QRCodeInvalide, BonExpire, MaterielManquant, MaterielExcedentaire, etc. |
| `T_NiveauxGravite` | String `NiveauGravite` dans Anomalies | Faible, Moyen, Elevé, Critique |

**Alternative préférée** : Utiliser les **enums C#** existants avec `HasConversion<string>()` plutôt que créer de nouvelles tables pour des listes de 3-5 valeurs. Réserver les tables de référence pour les ensembles dynamiques (T_Statuts avec couleurs/icônes).

---

## 10. Incohérences d'héritage (TPH)

### Deux hiérarchies parallèles et séparées

```
HIÉRARCHIE 1 : T_Bons (discriminateur: TypeDiscriminator)
├── Bon (abstraite) ── communes: IdBon, NumeroReference, DateCreation, etc.
└── BonEntree ── spécifiques: NomDemandeur, ContratId, NomCompagnie, etc.

HIÉRARCHIE 2 : T_BonsSortie (discriminateur: TypeSortie)
├── BonSortie (abstraite) ── communes: IdBon, NumeroReference, DateCreation, etc.
├── BonSortieExterne ── spécifiques: NomDestinataire, NumeroVehicule, etc.
├── BonSortieInterne ── spécifiques: DepartementOrigine, FonctionReceveur, etc.
└── Pret (extends Externe) ── spécifiques: DateRetourPrevue, EstRetourne, etc.
```

### Problème : Champs dupliqués entre les deux hiérarchies

| Champ | T_Bons (Bon) | T_BonsSortie (BonSortie) |
|-------|-------------|-------------------------|
| `NumeroReference` | ✅ | ✅ (dupliqué) |
| `DateCreation` | ✅ | ✅ (dupliqué) |
| `DateExpiration` | ✅ | ✅ (dupliqué) |
| `StatutActuel` | ✅ | ✅ (dupliqué) |
| `Destination` | ✅ | ✅ (dupliqué) |
| `Provenance` | ✅ | ✅ (dupliqué) |
| `Description` | ✅ | ✅ (dupliqué) |
| `Quantite` | ✅ | ✅ (dupliqué) |
| `QRCodeData/Base64/Hash` | ✅ | ✅ (dupliqué) |
| `DateGenerationQR` | ✅ | ✅ (dupliqué) |

**12 propriétés identiques** sont définies dans les deux classes abstraites. C'est une violation flagrante de DRY.

### Analyse architecturale
Il serait théoriquement possible d'unifier en une seule table TPH `T_Bons` contenant BonEntree, BonSortieExterne, BonSortieInterne et Pret. **CEPENDANT**, cela créerait une table avec ~60 colonnes dont beaucoup nullables, ce qui est déconseillé.

### Recommandation
Garder les deux tables séparées (T_Bons, T_BonsSortie) mais :
1. **Extraire une interface commune** `IBon` dans Domain pour les propriétés partagées
2. S'assurer que les conventions sont identiques (mêmes maxLength, mêmes defaults)

---

## 11. Index manquants

### Index recommandés

| Table | Colonnes | Type | Justification |
|-------|----------|------|---------------|
| `T_Bons` | `StatutActuel` | Non-clustered | Filtrage fréquent par statut |
| `T_Bons` | `ContratId` | Non-clustered | FK lookup |
| `T_Bons` | `DateCreation` | Non-clustered | Tri chronologique |
| `T_BonsSortie` | `StatutActuel` | Non-clustered | Filtrage fréquent |
| `T_BonsSortie` | `CreatedByLogin` | Non-clustered | "Mes bons" requête fréquente |
| `T_BonsSortie` | `DateCreation` | Non-clustered | Tri chronologique |
| `T_MaterielsSortie` | `MaterielEntreeId` | Non-clustered | Traçabilité entrée→sortie |
| `T_MaterielsSortie` | `BonEntreeId` | Non-clustered | Recherche par BEM source |
| `T_ScanEvenements` | `BonId, TypeBon` | Non-clustered composite | Historique scans d'un bon |
| `T_ScanEvenements` | `DateHeureScan` | Non-clustered | Recherche chronologique |
| `T_ScanEvenements` | `BarriereId` | Non-clustered | Activité par barrière |
| `T_Anomalies` | `BonId, TypeBon` | Non-clustered composite | Anomalies d'un bon |
| `T_Anomalies` | `EstTraitee` | Non-clustered filtered | Anomalies ouvertes |
| `T_AuditLogs` | `DateAction` | Non-clustered | Recherche chronologique |
| `T_AuditLogs` | `UtilisateurLogin` | Non-clustered | Audit par utilisateur |
| `T_AuditLogs` | `Categorie, EntiteType` | Non-clustered composite | Audit par module |

---

## 12. Plan d'action priorisé

### Phase 1 — Critique (Semaine 1-2) 🔴

| # | Action | Impact | Risque |
|---|--------|--------|--------|
| 1.1 | **Supprimer les entités Domain dupliquées** (garder uniquement Module entities) | Élimine 28 entités mortes, clarifie l'architecture | Moyen — 39 fichiers à migrer |
| 1.2 | **Migrer Shared entities** (Compagnie, Contrat, Departement, Employee, Site, etc.) vers `Module.Shared.Entities` | Unifie la source de vérité | Moyen — refactoring des imports |
| 1.3 | **Fusionner Checkpoint → Barriere** (ajouter SiteId à Barriere, supprimer T_Checkpoints) | Élimine la confusion | Moyen — migration SQL |
| 1.4 | **Ajouter les FK manquantes** (ContratId, BarriereId, MaterielEntreeId, ScanId) | Intégrité garantie | Faible |

### Phase 2 — Important (Semaine 3-4) 🟡

| # | Action | Impact | Risque |
|---|--------|--------|--------|
| 2.1 | **Normaliser la dénormalisation** — supprimer NomCompagnie, NumeroContrat, BonEntreeNumero, CodeBarriere, NomBarriere, NumeroReferenceBon | Intégrité données, moins de bugs | Moyen — modifier les requêtes |
| 2.2 | **Résoudre les FK polymorphes** — BonId+TypeBon → BonEntreeId + BonSortieId | Contraintes FK réelles | Moyen — migration SQL |
| 2.3 | **Convertir strings en enums** — StatutScan, TypeMouvement, TypeBon, TypeAnomalie, NiveauGravite, Decision | Type-safety, IntelliSense | Faible |
| 2.4 | **Standardiser les PK** — tout en `IdNomEntité` | Cohérence | Faible |

### Phase 3 — Optimisation (Semaine 5-6) 🟢

| # | Action | Impact | Risque |
|---|--------|--------|--------|
| 3.1 | **Supprimer T_SoldesMateriels** — calculer le solde dynamiquement | Élimine la redondance | Faible — modifier le service |
| 3.2 | **Transformer T_HistoriqueScans en vue SQL** (ou intégrer dans ScanEvenement) | Élimine la table redondante | Moyen |
| 3.3 | **Relier StatutActuel à T_Statuts** ou convertir en enum `BonStatut` | Exploiter la table existante | Moyen |
| 3.4 | **Évaluer T_PassagesCheckpoint** — migrer vers ItinerairePrevu/ScanEvenement ou supprimer | Nettoyage | Faible |
| 3.5 | **Ajouter les index manquants** (cf. section 11) | Performance | Faible |

---

## 13. Schéma cible

### Après corrections — Vue d'ensemble

```
┌─────────────────────────────────────────────────────────┐
│                     TABLES DE RÉFÉRENCE                 │
├─────────────────────────────────────────────────────────┤
│ T_Compagnies (IdCompagnie, Nom, Code, Email, Tel)       │
│ T_Contrats (IdContrat, PoNumber, CompagnieId→FK)        │
│ T_Departements (IdDepartement, Nom, Code)               │
│ T_Sites (IdSite, Nom, Code)                             │
│ T_Barrieres (IdBarriere, Code, Nom, SiteId→FK, Type)   │◄── Fusion Checkpoint+Barriere
│ T_Employees (IdEmployee, ...)                           │
│ T_CategoriesSortie (IdCategorie, Nom, Code)             │
│ T_RaisonsSortie (IdRaison, CategorieId→FK)              │
│ T_Statuts (IdStatut, CodeStatut, LibelleStatut, ...)    │◄── Enfin utilisée par FK!
│ T_TypesMateriels (IdTypeMateriel, CodeType, ...)        │
│ T_Roles (IdRole, CodeRole, ...)                         │
│ T_ParametresSysteme (IdParametre, Cle, Valeur)          │
└─────────────────────────────────────────────────────────┘
           │              │            │
           ▼              ▼            ▼
┌──────────────────────────────────────────────────────────┐
│                      BONS D'ENTRÉE (TPH)                 │
├──────────────────────────────────────────────────────────┤
│ T_Bons (IdBon, NumeroReference, StatutId→FK,             │
│         ContratId→FK, ...)                               │◄── FK vers Statuts et Contrats
│ T_Materiels (IdMateriel, BonId→FK)                       │
│ T_Approbations (IdApprobation, BonId→FK, Decision:enum)  │
│ T_ItinerairesPrevu (IdItineraire, BonId→FK,              │
│                     BarriereId→FK)                        │◄── FK vers Barrieres
│ T_BonEntreeHistory (IdHistory, BonId→FK, Action:enum)    │
└──────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────┐
│                     BONS DE SORTIE (TPH)                 │
├──────────────────────────────────────────────────────────┤
│ T_BonsSortie (IdBon, NumeroReference, StatutId→FK,       │
│               TypeSortie, ...)                           │◄── FK vers Statuts
│ T_MaterielsSortie (IdMateriel, BonSortieId→FK,           │
│                    MaterielEntreeId→FK)                   │◄── FK vers Materiels
│ T_ApprobationsSortie (IdApprobation, BonSortieId→FK)     │
│ T_ItinerairesSortie (IdItineraire, BonSortieId→FK,       │
│                      BarriereId→FK)                       │◄── FK vers Barrieres
│ T_BonSortieHistories (IdHistory, BonSortieId→FK)         │
└──────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────┐
│                        SÉCURITÉ                          │
├──────────────────────────────────────────────────────────┤
│ T_ScanEvenements (IdScan, BarriereId→FK,                 │
│                   BonEntreeId→FK(null), BonSortieId→FK,  │◄── FK explicites
│                   StatutScan:enum, TypeMouvement:enum)   │
│ T_Anomalies (IdAnomalie, ScanId→FK, BarriereId→FK,      │
│              BonEntreeId→FK(null), BonSortieId→FK(null), │
│              TypeAnomalie:enum, NiveauGravite:enum)       │
└──────────────────────────────────────────────────────────┘
❌ T_HistoriqueScans → SUPPRIMÉE (remplacée par vue SQL)
❌ T_SoldesMateriels → SUPPRIMÉE (calcul dynamique)
❌ T_Checkpoints → SUPPRIMÉE (fusionnée dans T_Barrieres)
❌ T_PassagesCheckpoint → SUPPRIMÉE (remplacée par ScanEvenement)

┌──────────────────────────────────────────────────────────┐
│                     ADMINISTRATION                       │
├──────────────────────────────────────────────────────────┤
│ T_Utilisateurs (IdUtilisateur, Login, NomComplet, ...)   │
│ T_UtilisateurRoles (IdUtilisateurRole,                   │
│                     IdUtilisateur→FK, IdRole→FK)         │
│ T_AuditLogs (IdAuditLog, DateAction, ...)                │
│ T_NotificationsRejet (IdNotification, ...)               │
└──────────────────────────────────────────────────────────┘
```

### Résumé des gains
| Métrique | Avant | Après | Gain |
|----------|-------|-------|------|
| Entités C# | 52 | ~26 | -50% |
| Tables | 35+ | ~28 | -20% |
| Champs dénormalisés | 15+ | 4 (snapshots d'audit) | -75% |
| FK manquantes | 11 | 0 | -100% |
| Strings magiques | 13 | 0 | -100% |

---

*Rapport généré dans le cadre de l'audit professionnel de la base de données KCCMaterialFlow.*
*Prochaine étape : Validation par l'équipe, puis implémentation phase par phase.*
