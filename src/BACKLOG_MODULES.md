# 📋 BACKLOG - Modules Restants KCCMaterialFlow

**Date de création:** 28 janvier 2026  
**Statut:** Module BonEntree ✅ Terminé

---

## 🎯 Vue d'ensemble des modules

| Module | Priorité | Statut | Dépendances |
|--------|----------|--------|-------------|
| BonEntree | 🔴 P1 | ✅ Terminé | - |
| BonSortie | 🔴 P1 | ⏳ À faire | BonEntree (Materiel) |
| Securite | 🟠 P2 | ⏳ À faire | BonEntree, BonSortie |
| Reporting | 🟡 P3 | ⏳ À faire | Tous |

---

## 📦 Module BonSortie

### Description
Gestion des Bons de Sortie Matériel (BSM) permettant de tracer la sortie de matériels du site.

### Entités à créer (selon pattern BonEntree)

#### BonSortie.cs (hérite de Bon)
```
Champs spécifiques (à confirmer selon formulaire SEC-FM-xxx):
- MotifSortie: string(1000) - Raison de la sortie
- DestinataireNom: string(200) - Nom du destinataire
- DestinataireContact: string(200)? - Contact
- TransporteurNom: string(200)? - Nom du transporteur
- NumeroVehicule: string(50)? - Immatriculation
- NomChauffeur: string(200)? - Nom du chauffeur
- ReferenceBonEntree: int? - Lien vers BonEntree d'origine (si matériel entré)
```

### Fichiers à créer // OU JE SUIS 

```
Modules/KCCMaterialFlow.Module.BonSortie/
├── Entities/
│   ├── BonSortie.cs                    # Hérite de Bon (dans BonEntree)
│   └── Configurations/
│       └── BonSortieConfiguration.cs
├── DTOs/
│   ├── BonSortieViewDto.cs
│   ├── BonSortieListDto.cs
│   └── MaterielSortieDto.cs           # Si différent de MaterielDto
├── Repositories/
│   ├── IBonSortieRepository.cs
│   └── BonSortieRepository.cs
├── Services/
│   ├── IBonSortieService.cs
│   └── BonSortieService.cs
├── Validators/
│   └── CreateBonSortieRequestValidator.cs
├── Pages/
│   ├── _Imports.razor
│   ├── BonSortieList.razor + .cs
│   ├── BonSortieNew.razor + .cs
│   ├── BonSortieEdit.razor
│   ├── BonSortieView.razor
│   └── BonSortiePrint.razor
├── Mappings/
│   └── BonSortieMappingProfile.cs
└── BonSortieModule.cs                  # IModule implementation
```

### Workflow BSM
```
Draft → PendingSup → PendingGM → PendingOPJ → Approved → InTransit → Completed
```

### Points d'attention
- [ ] Réutiliser l'entité `Materiel` existante (BonId générique)
- [ ] Lien optionnel vers BonEntree pour traçabilité
- [ ] Même structure de numérotation: BSM-YYYY-NNNNNN
- [ ] Utiliser `BonStatuts` constantes (pas de magic strings)

### Tâches détaillées

| # | Tâche | Temps estimé | Statut |
|---|-------|--------------|--------|
| 1 | Créer BonSortie.cs + Configuration | 30min | ⬜ |
| 2 | Créer DTOs (View, List) | 30min | ⬜ |
| 3 | Créer IBonSortieRepository + impl | 1h | ⬜ |
| 4 | Créer IBonSortieService + impl | 1h30 | ⬜ |
| 5 | Créer Validators | 20min | ⬜ |
| 6 | Créer BonSortieList.razor | 45min | ⬜ |
| 7 | Créer BonSortieNew.razor | 45min | ⬜ |
| 8 | Créer BonSortieView.razor | 30min | ⬜ |
| 9 | Créer BonSortieEdit.razor | 30min | ⬜ |
| 10 | Créer BonSortiePrint.razor | 30min | ⬜ |
| 11 | Créer BonSortieModule.cs | 15min | ⬜ |
| 12 | Créer migration EF | 15min | ⬜ |
| 13 | Tests manuels workflow | 30min | ⬜ |
| **Total** | | **~8h** | |

---

## 🔒 Module Securite

### Description
Gestion des barrières de sécurité et scan des QR codes pour le suivi des passages.

### Entités existantes (Module.Shared)
- `Barriere` ✅ Existe
- `Utilisateur` ✅ Existe

### Entités à créer

#### ScanPassage.cs
```csharp
- IdScan: int (PK)
- BonId: int (FK → Bon)
- BarriereId: int (FK → Barriere)
- AgentLogin: string(100)
- AgentNom: string(200)
- DateScan: DateTime
- TypePassage: string (Entree/Sortie)
- Observations: string(500)?
- Latitude: decimal?
- Longitude: decimal?
```

#### Anomalie.cs
```csharp
- IdAnomalie: int (PK)
- BonId: int (FK → Bon)
- BarriereId: int (FK → Barriere)
- TypeAnomalie: string
- Description: string(1000)
- DateSignalement: DateTime
- AgentLogin: string(100)
- Resolu: bool
- DateResolution: DateTime?
```

### Fichiers à créer

```
Modules/KCCMaterialFlow.Module.Securite/
├── Entities/
│   ├── ScanPassage.cs
│   ├── Anomalie.cs
│   └── Configurations/
│       ├── ScanPassageConfiguration.cs
│       └── AnomalieConfiguration.cs
├── DTOs/
│   ├── ScanPassageDto.cs
│   └── AnomalieDto.cs
├── Services/
│   ├── IScanService.cs
│   └── ScanService.cs
├── Pages/
│   ├── _Imports.razor
│   ├── Dashboard.razor              # Tableau de bord sécurité
│   ├── ScanQR.razor                 # Page de scan
│   ├── HistoriquePassages.razor     # Liste des passages
│   └── Anomalies.razor              # Gestion anomalies
└── SecuriteModule.cs
```

### Fonctionnalités clés
- [ ] Scan QR Code (camera mobile)
- [ ] Validation bon vs barrière attendue
- [ ] Historique des passages en temps réel
- [ ] Signalement d'anomalies
- [ ] Dashboard avec statistiques

### Tâches détaillées

| # | Tâche | Temps estimé | Statut |
|---|-------|--------------|--------|
| 1 | Créer ScanPassage.cs + Config | 30min | ⬜ |
| 2 | Créer Anomalie.cs + Config | 30min | ⬜ |
| 3 | Créer IScanService + impl | 2h | ⬜ |
| 4 | Créer Dashboard.razor | 1h | ⬜ |
| 5 | Créer ScanQR.razor (avec JS interop) | 2h | ⬜ |
| 6 | Créer HistoriquePassages.razor | 45min | ⬜ |
| 7 | Créer Anomalies.razor | 45min | ⬜ |
| 8 | Créer SecuriteModule.cs | 15min | ⬜ |
| 9 | Migration EF | 15min | ⬜ |
| 10 | Tests | 1h | ⬜ |
| **Total** | | **~9h** | |

---

## 📊 Module Reporting (P3)

### Description
Rapports et statistiques pour la direction.

### Fonctionnalités
- Rapport journalier des mouvements
- Statistiques par période
- Export Excel/PDF
- Tableau de bord direction

### À définir après BonSortie et Securite

---

## ⚠️ Règles à suivre (Leçons apprises)

### 1. Structure des entités
```
✅ FAIRE:
- Définir les champs AVANT de coder (selon UML/formulaire)
- Maximum 10-15 champs par entité
- Utiliser string pour les statuts
- FK générique (BonId) plutôt que spécifique (BonEntreeId)

❌ NE PAS FAIRE:
- Ajouter des champs "au cas où"
- Utiliser des enums pour les statuts
- Créer des FK spécifiques par type
```

### 2. Constantes et helpers
```
✅ FAIRE:
- Utiliser BonStatuts.Draft au lieu de "Draft"
- Utiliser BonStatuts.GetLabel() pour affichage
- Utiliser ApprobationDecisions.Approuve

❌ NE PAS FAIRE:
- Magic strings dans le code
- Dupliquer les switch/case pour les labels
```

### 3. Validators
```
✅ FAIRE:
- Créer les validators DÈS la création des Request DTOs
- Un fichier = tous les validators du module

❌ NE PAS FAIRE:
- Oublier les validators (risque données invalides)
```

### 4. Pages Razor
```
✅ FAIRE:
- List, New, Edit, View, Print (si applicable)
- Code-behind séparé (.razor.cs) pour les pages complexes
- Inline @code pour les pages simples

❌ NE PAS FAIRE:
- Oublier une page CRUD
- Références à des layouts inexistants
```

### 5. Imports
```
✅ FAIRE:
- Vérifier _Imports.razor du dossier Pages
- Inclure Radzen, Radzen.Blazor
- Inclure les namespaces du module

❌ NE PAS FAIRE:
- Importer des namespaces inexistants
- Oublier @using pour les nouveaux DTOs
```

---

## 📅 Planning suggéré

| Semaine | Module | Livrable |
|---------|--------|----------|
| S1 | BonSortie | Entités + Repository + Service |
| S2 | BonSortie | Pages + Tests |
| S3 | Securite | Entités + Service |
| S4 | Securite | Pages Scan + Dashboard |
| S5 | Reporting | À définir |

---

## 🔍 Checklist avant chaque module

- [ ] Formulaire papier / UML analysé
- [ ] Liste des champs définie et validée
- [ ] Entités créées avec Configuration EF
- [ ] DTOs créés (View, List, Create, Update)
- [ ] Repository interface + implémentation
- [ ] Service interface + implémentation
- [ ] Validators FluentValidation
- [ ] Pages CRUD (List, New, Edit, View)
- [ ] Page Print si applicable
- [ ] Module.cs enregistré
- [ ] Migration EF créée et appliquée
- [ ] Build sans erreurs
- [ ] Test manuel du workflow

---

*Dernière mise à jour: 28 janvier 2026*
