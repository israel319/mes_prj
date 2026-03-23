# Plan de Migration vers Clean Architecture

## Vue d'ensemble

Ce document décrit le plan de migration progressif de KCCMaterialFlow vers une architecture Clean Architecture stricte, adaptée aux applications d'entreprise durables.

**Date de début** : 18 février 2026  
**Projet** : KCCMaterialFlow  
**Objectif** : Architecture maintenable, testable et évolutive

---

## Structure cible

```
src/
├── Domain/
│   └── KCCMaterialFlow.Domain/              ← Entités pures (POCO), Enums, Value Objects
│
├── Application/
│   └── KCCMaterialFlow.Application/         ← Interfaces, Use Cases, DTOs globaux
│
├── Infrastructure/
│   └── KCCMaterialFlow.Infrastructure/      ← DbContext, Repositories, Services externes
│
├── Modules/
│   ├── BonEntree/
│   │   ├── BonEntree.Application/           ← Use Cases, DTOs, Validators du module
│   │   └── BonEntree.Presentation/          ← Pages Blazor, ViewModels
│   │
│   ├── BonSortie/
│   │   ├── BonSortie.Application/
│   │   └── BonSortie.Presentation/
│   │
│   └── Securite/
│       ├── Securite.Application/
│       └── Securite.Presentation/
│
└── Host/
    └── KCCMaterialFlow.Host/                ← Composition Root (DI, Configuration)
```

---

## Règles Clean Architecture

### Règle de dépendance (Dependency Rule)
Les dépendances pointent **uniquement vers l'intérieur** :
- **Domain** : Aucune dépendance externe (POCO purs)
- **Application** : Dépend de Domain uniquement
- **Infrastructure** : Dépend de Application et Domain
- **Presentation** : Dépend de Application et Domain
- **Host** : Dépend de tout (Composition Root)

### Principes
1. Les **Entités** du Domain ne contiennent aucune annotation EF (`[Key]`, `[Required]`, etc.)
2. Les **Interfaces** sont définies dans Application, implémentées dans Infrastructure
3. Les **Use Cases** (services métier) sont dans Application
4. Les **Repositories** et **DbContext** sont dans Infrastructure
5. Les **Pages Blazor** sont dans Presentation

---

## Phases de Migration

---

### Phase 1 : Création du projet Domain ✅ COMPLÉTÉE (18/02/2026)

**Objectif** : Extraire les entités pures sans annotations EF

**Étapes** :
- [x] Créer le projet `KCCMaterialFlow.Domain`
- [x] Créer la structure de dossiers (Entities, Enums, ValueObjects, Common)
- [x] Copier les entités de Core/Entities vers Domain (sans annotations EF)
- [x] Copier les entités des modules vers Domain
- [x] Créer les interfaces de base (IAuditableEntity, ISoftDelete, etc.)
- [x] Ajouter la référence Domain à tous les projets
- [x] Compiler et vérifier

**Résultat** : Solution compile avec succès. Toutes les entités POCO créées.

**Fichiers concernés** :
- `KCCMaterialFlow.Core/Entities/*` → `KCCMaterialFlow.Domain/Entities/Shared/`
- `Module.BonEntree/Entities/*` → `KCCMaterialFlow.Domain/Entities/BonEntree/`
- `Module.BonSortie/Entities/*` → `KCCMaterialFlow.Domain/Entities/BonSortie/`
- `Module.Securite/Entities/*` → `KCCMaterialFlow.Domain/Entities/Securite/`
- `KCCMaterialFlow.Core/Enums/*` → `KCCMaterialFlow.Domain/Enums/`

**Impact** : Faible - Ajout de projet, pas de suppression

---

### Phase 2 : Création du projet Application ✅ COMPLÉTÉE (18/02/2026)

**Objectif** : Centraliser les interfaces et créer la couche Use Cases

**Étapes** :
- [x] Créer le projet `KCCMaterialFlow.Application`
- [x] Déplacer les interfaces de Core/Abstractions vers Application/Interfaces
- [x] Créer les dossiers (Interfaces, DTOs, Common, Behaviors)
- [x] Définir les interfaces de repositories génériques
- [x] Déplacer les DTOs partagés (classes associées aux interfaces)
- [x] Ajouter référence Domain au projet Application
- [x] Mettre à jour les références de tous les projets

**Résultat** : Solution compile avec succès. Toutes les interfaces migrées vers Application.

**Fichiers créés** :
- `KCCMaterialFlow.Application/Interfaces/IRepository.cs`
- `KCCMaterialFlow.Application/Interfaces/ICurrentUserService.cs` (+ CurrentUserInfo)
- `KCCMaterialFlow.Application/Interfaces/IModule.cs` (+ NavMenuItem)
- `KCCMaterialFlow.Application/Interfaces/IEmailNotificationService.cs`
- `KCCMaterialFlow.Application/Interfaces/IQRCodeService.cs` (+ QRCodeResult, QRCodeValidationResult, QRCodeDecodedInfo)
- `KCCMaterialFlow.Application/Interfaces/IReferenceDataService.cs`
- `KCCMaterialFlow.Application/Interfaces/IBonEntreeLockService.cs` (+ BonEntreeAvailabilityResult, BonEntreeBasicInfo, BonEntreeDetailsForSortie, MaterielForSortie, MaterielStockDecrement, StockUpdateResult)
- `KCCMaterialFlow.Application/Interfaces/ICrossModuleService.cs` (+ CrossModuleBonSortieInfo)
- `KCCMaterialFlow.Application/Interfaces/INotificationRejetService.cs`
- `KCCMaterialFlow.Application/Interfaces/IWorkflowService.cs` (+ WorkflowResult, NextApproverInfo, WorkflowStep)

**Impact** : Faible - Les interfaces existantes dans Core restent disponibles pour compatibilité

---

### Phase 3 : Réorganisation Infrastructure ⏸️ REPORTÉE

**Objectif** : Consolider toute l'infrastructure (EF, Services externes)

**Statut** : Reportée - Les configurations Fluent API généraient des différences de schéma avec la base existante (indexes, valeurs par défaut, comportements FK). Session dédiée requise pour les modifications DB.

**Étapes** :
- [x] Créer les configurations Fluent API pour les entités Shared → ROLLBACK
- [ ] Reporter à session dédiée : modifications DB, suppression annotations EF
- [ ] Créer les repositories spécifiques
- [ ] Configurer le DbContext pour utiliser les configurations
- [ ] Tester et appliquer les migrations EF

**Raison du report** :
Les configs Fluent API ajoutaient ~30 nouveaux indexes et changements FK non désirés. Appliquer ces changements nécessite une session de maintenance dédiée.

**Note** : Le snapshot EF est synchronisé avec le modèle actuel. 17 migrations préservées

**Structure** :
```
KCCMaterialFlow.Infrastructure/
├── Data/
│   ├── KCCMaterialFlowDbContext.cs
│   ├── RepositoryBase.cs
│   └── DesignTimeDbContextFactory.cs
├── Configurations/
│   ├── Shared/
│   │   ├── CompagnieConfiguration.cs
│   │   ├── SiteConfiguration.cs
│   │   └── ...
│   ├── BonEntree/
│   │   ├── BonEntreeConfiguration.cs
│   │   └── ...
│   ├── BonSortie/
│   └── Securite/
├── Repositories/
│   ├── BonEntreeRepository.cs
│   ├── BonSortieRepository.cs
│   └── ...
├── Services/
│   ├── ReferenceDataService.cs
│   └── NotificationRejetService.cs
└── Migrations/
```

**Impact** : Moyen - Modifications EF, requiert tests approfondis

---

### Phase 4 : Restructuration des Modules ✅ COMPLÉTÉE

**Objectif** : Séparer Application et Presentation dans chaque module

**Approche adoptée** : Migration pragmatique sans casser l'existant
- Les projets `.Application` ré-exportent les types du module existant
- Les projets `.Presentation` sont prêts à recevoir les Pages
- Le module original conserve Services/Repositories/Entities (fonctionne avec la BD)

**Statut par module** :

#### BonEntree ✅ STRUCTURE CRÉÉE
- [x] Créer `KCCMaterialFlow.Module.BonEntree.Application`
- [x] Créer `KCCMaterialFlow.Module.BonEntree.Presentation`
- [x] Configurer les global usings pour ré-export
- [x] Ajouter les projets à la solution
- [x] Vérifier la compilation
- [ ] Migrer les Pages vers Presentation (optionnel - prochaine session)

#### BonSortie ✅ STRUCTURE CRÉÉE
- [x] Créer `KCCMaterialFlow.Module.BonSortie.Application`
- [x] Créer `KCCMaterialFlow.Module.BonSortie.Presentation`
- [x] Configurer les global usings pour ré-export
- [x] Ajouter les projets à la solution
- [x] Vérifier la compilation
- [ ] Migrer les Pages vers Presentation (optionnel - prochaine session)

#### Securite ✅ STRUCTURE CRÉÉE
- [x] Créer `KCCMaterialFlow.Module.Securite.Application`
- [x] Créer `KCCMaterialFlow.Module.Securite.Presentation`
- [x] Configurer les global usings pour ré-export
- [x] Ajouter les projets à la solution
- [x] Vérifier la compilation
- [ ] Migrer les Pages vers Presentation (optionnel - prochaine session)

**Structure actuelle (tous les modules)** :
```
Modules/
├── BonEntree/
│   ├── KCCMaterialFlow.Module.BonEntree.Application/
│   │   ├── GlobalUsings.cs           ← Ré-exporte tous les types du module
│   │   └── *.csproj                  ← Référence le module existant
│   │
│   └── KCCMaterialFlow.Module.BonEntree.Presentation/
│       ├── _Imports.razor
│       ├── Pages/
│       │   └── _Placeholder.razor    ← Prêt pour migration des Pages
│       └── *.csproj                  ← Référence Application et Core
│
├── BonSortie/
│   ├── KCCMaterialFlow.Module.BonSortie.Application/
│   │   ├── GlobalUsings.cs
│   │   └── *.csproj
│   │
│   └── KCCMaterialFlow.Module.BonSortie.Presentation/
│       ├── _Imports.razor
│       ├── Pages/
│       │   └── _Placeholder.razor
│       └── *.csproj
│
├── Securite/
│   ├── KCCMaterialFlow.Module.Securite.Application/
│   │   ├── GlobalUsings.cs
│   │   └── *.csproj
│   │
│   └── KCCMaterialFlow.Module.Securite.Presentation/
│       ├── _Imports.razor
│       ├── Pages/
│       │   └── _Placeholder.razor
│       └── *.csproj
│
├── KCCMaterialFlow.Module.BonEntree/  ← Module original (Services, Repos, Entities)
├── KCCMaterialFlow.Module.BonSortie/
├── KCCMaterialFlow.Module.Securite/
└── KCCMaterialFlow.Module.Shared/
```

**Résultat** : 15 projets - Solution compile avec succès

**Avantages de cette approche** :
1. ✅ Ne casse pas l'application existante
2. ✅ Permet une migration progressive des Pages
3. ✅ Les projets Application/Presentation sont prêts pour l'avenir
4. ✅ Compilation validée de toute la solution

---

### Phase 5 : Nettoyage et finalisation

**Objectif** : Supprimer le code obsolète et valider l'architecture

**Étapes** :
- [ ] Supprimer `KCCMaterialFlow.Core` (devenu obsolète)
- [ ] Supprimer les anciens projets modules
- [ ] Supprimer `Module.Shared` (intégré dans Application/Infrastructure)
- [ ] Vérifier toutes les dépendances
- [ ] Exécuter tous les tests
- [ ] Mettre à jour la documentation
- [ ] Créer un diagramme d'architecture final

**Impact** : Faible - Suppression de code obsolète uniquement

---

## Diagramme des dépendances (cible)

```
┌─────────────────────────────────────────────────────────────────┐
│                           HOST                                   │
│                   (Composition Root)                             │
└─────────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        ▼                     ▼                     ▼
┌───────────────┐   ┌───────────────┐   ┌───────────────────────┐
│ BonEntree     │   │ BonSortie     │   │ Securite              │
│ .Presentation │   │ .Presentation │   │ .Presentation         │
└───────────────┘   └───────────────┘   └───────────────────────┘
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐   ┌───────────────┐   ┌───────────────────────┐
│ BonEntree     │   │ BonSortie     │   │ Securite              │
│ .Application  │   │ .Application  │   │ .Application          │
└───────────────┘   └───────────────┘   └───────────────────────┘
        │                     │                     │
        └──────────┬──────────┴──────────┬──────────┘
                   ▼                     ▼
        ┌───────────────────┐   ┌───────────────────┐
        │ KCCMaterialFlow   │   │ KCCMaterialFlow   │
        │ .Application      │◄──│ .Infrastructure   │
        └───────────────────┘   └───────────────────┘
                   │                     │
                   └──────────┬──────────┘
                              ▼
                   ┌───────────────────┐
                   │ KCCMaterialFlow   │
                   │ .Domain           │
                   └───────────────────┘
```

---

## Checklist de validation

### Après chaque phase :
- [ ] La solution compile sans erreurs
- [ ] L'application démarre correctement
- [ ] Les fonctionnalités existantes fonctionnent
- [ ] Aucune régression introduite

### Validation finale :
- [ ] Aucune référence circulaire
- [ ] Domain n'a aucune dépendance externe
- [ ] Application ne dépend que de Domain
- [ ] Infrastructure implémente les interfaces de Application
- [ ] Tests unitaires passent (si existants)
- [ ] Performance maintenue

---

## Notes de migration

### Phase 1 - Notes
_À compléter pendant l'exécution_

### Phase 2 - Notes
- Package Microsoft.Extensions.DependencyInjection.Abstractions ajouté pour IServiceCollection
- Les interfaces de Core/Abstractions conservées pour compatibilité ascendante
- Toutes les classes DTO associées aux interfaces copiées avec les interfaces
- IReferenceDataService utilise les entités Domain (Compagnie, Departement, Site, Employee, etc.)
- INotificationRejetService utilise l'entité Domain NotificationRejet

### Phase 3 - Notes
- 10 configurations Fluent API créées puis supprimées (ROLLBACK)
- Les configs généraient ~30 nouveaux indexes et changements FK non désirés
- Snapshot EF récupéré après corruption pendant le rollback
- Session dédiée nécessaire pour modifications de schéma DB
- DataAnnotations dans Core/Entities conservées pour l'instant

### Phase 4 - Notes
_À compléter pendant l'exécution_

### Phase 5 - Notes
_À compléter pendant l'exécution_

---

## Historique des changements

| Date | Phase | Action | Statut |
|------|-------|--------|--------|
| 2026-02-18 | Pré-migration | Création Infrastructure, déplacement DbContext | ✅ Terminé |
| 2026-02-18 | Phase 1 | Création projet Domain avec entités POCO | ✅ Terminé |
| 2026-02-18 | Phase 2 | Création projet Application avec interfaces | ✅ Terminé |
| 2026-02-18 | Phase 3 | Configurations Fluent API | ⏸️ Reporté |
| 2026-02-18 | Phase 4 | Restructuration des Modules | 🔄 En cours |

