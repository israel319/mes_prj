# Analyse Clean Architecture - KCCMaterialFlow

> **Date**: 22 février 2026  
> **Version**: .NET 10 / EF Core 10.0.2 / Blazor Server  
> **Statut**: ✅ Architecture conforme avec recommandations

---

## 📐 Vue d'Ensemble de l'Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              HOST (Blazor Server)                           │
│              Point d'entrée, Composition Root, Pages globales               │
├─────────────────────────────────────────────────────────────────────────────┤
│                                   MODULES                                   │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐              │
│  │   BonEntree     │  │    BonSortie    │  │    Securite     │   Module     │
│  │  Application +  │  │  Application +  │  │  Application +  │   Shared     │
│  │  Presentation   │  │  Presentation   │  │  Presentation   │              │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘              │
├───────────┼────────────────────┼────────────────────┼────────────────────────┤
│           │                    │                    │         INFRASTRUCTURE│
│           │        DbContext, Configurations, Repositories concrets         │
├───────────┼────────────────────┼────────────────────┼────────────────────────┤
│           │                    │                    │                  CORE │
│           │              Abstractions, Services partagés, Entities          │
├───────────┼────────────────────┼────────────────────┼────────────────────────┤
│           │                    │                    │           APPLICATION │
│           │                     Interfaces, DTOs                             │
├───────────┼────────────────────┼────────────────────┼────────────────────────┤
│           │                    │                    │                DOMAIN │
│           │              Entités métier pures, Interfaces de base           │
└───────────┴────────────────────┴────────────────────┴────────────────────────┘
```

---

## 📁 Structure des Couches (de bas en haut)

### 1️⃣ KCCMaterialFlow.Domain (Couche la plus interne)

**Rôle**: Contient les entités métier pures et les interfaces de base - aucune dépendance externe.

```
KCCMaterialFlow.Domain/
├── Common/                          # Interfaces et classes de base
│   ├── AuditableEntity.cs          # Classe de base avec CreatedAt/UpdatedAt
│   ├── IAuditableEntity.cs         # Interface pour l'audit automatique
│   ├── IEntity.cs                  # Interface de base: int Id { get; set; }
│   └── ISoftDelete.cs              # Interface pour suppression logique
├── Entities/                        # Entités organisées par domaine
│   ├── BonEntree/                  # Entités du module Bon d'Entrée
│   │   ├── Approbation.cs          # Approbation d'un bon d'entrée
│   │   ├── Bon.cs                  # Classe abstraite de base (TPH)
│   │   ├── BonEntree.cs            # Entité bon d'entrée (hérite de Bon)
│   │   ├── BonEntreeHistory.cs     # Historique des modifications
│   │   ├── ItinerairePrevu.cs      # Itinéraires planifiés
│   │   └── Materiel.cs             # Matériels associés au bon
│   ├── BonSortie/                  # Entités du module Bon de Sortie
│   │   ├── ApprobationSortie.cs    # Approbations spécifiques sortie
│   │   ├── BonSortie.cs            # Bon de sortie principal
│   │   ├── BonSortieExterne.cs     # Sortie externe (hors site)
│   │   ├── BonSortieInterne.cs     # Sortie interne (entre zones)
│   │   ├── BonSortieHistory.cs     # Historique des sorties
│   │   ├── ItineraireSortie.cs     # Parcours de sortie
│   │   ├── MaterielSortie.cs       # Matériels à sortir
│   │   └── Pret.cs                 # Gestion des prêts temporaires
│   ├── Securite/                   # Entités du module Sécurité
│   │   ├── Anomalie.cs             # Anomalies détectées
│   │   ├── HistoriqueScan.cs       # Historique complet des scans
│   │   └── ScanEvenement.cs        # Événements de scan QR Code
│   └── Shared/                     # Entités partagées entre modules
└── Enums/                          # Énumérations métier si nécessaire
```

**Dépendances**: ❌ Aucune  
**Packages NuGet**: Aucun

**Pourquoi ici et pas ailleurs?**
- Les entités Domain représentent le **cœur métier** de l'application
- Elles ne doivent JAMAIS changer pour des raisons techniques (framework, BD, UI)
- Elles définissent les **règles business invariantes**
- Parfait pour les tests unitaires (aucune dépendance à mocker)

**Pourquoi `IEntity` et pas `BaseEntity`?**
- Interface = contrat minimal, pas d'héritage forcé
- Permet d'avoir des entités avec différentes clés (`IEntity.Id` vs `IBonEntity.IdBon`)
- Plus flexible pour les entités provenant de systèmes externes

---

### 2️⃣ KCCMaterialFlow.Application

**Rôle**: Définit les contrats (interfaces) et objets de transfert (DTOs) pour la communication entre couches.

```
KCCMaterialFlow.Application/
├── Common/                          # Classes utilitaires (actuellement vide)
├── DTOs/                            # Data Transfer Objects (actuellement vide)
│                                    # Note: Les DTOs sont dans les modules
└── Interfaces/                      # Interfaces de services
    ├── IBonEntreeLockService.cs    # Contrat verrouillage bon entrée
    ├── ICrossModuleService.cs      # Contrat communication inter-modules
    ├── ICurrentUserService.cs      # Contrat utilisateur courant
    ├── IEmailNotificationService.cs # Contrat notifications email
    ├── IModule.cs                  # Contrat définition d'un module
    ├── INotificationRejetService.cs # Contrat notifications de rejet
    ├── IQRCodeService.cs           # Contrat génération QR codes
    ├── IReferenceDataService.cs    # Contrat données de référence
    ├── IRepository.cs              # Contrat repository générique
    └── IWorkflowService.cs         # Contrat workflow d'approbation
```

**Dépendances**: 
- `KCCMaterialFlow.Domain` ✅

**Packages NuGet**: 
- `Microsoft.Extensions.DependencyInjection.Abstractions`

**Pourquoi séparé de Core?**
- Application = **contrats purs** (interfaces, DTOs)
- Core = **implémentations partagées** + abstractions spécifiques
- Permet aux modules de dépendre uniquement d'Application sans les implémentations

**⚠️ Observation**: Les interfaces sont dupliquées entre Application et Core.
- **Recommandation**: Garder uniquement dans Core pour éviter la duplication.

---

### 3️⃣ KCCMaterialFlow.Core

**Rôle**: Couche centrale contenant les abstractions, services partagés, entités référentielles et composants Blazor réutilisables.

```
KCCMaterialFlow.Core/
├── Abstractions/                    # Interfaces et contrats
│   ├── IAppDbContext.cs            # Abstraction du DbContext
│   ├── IAppDbContextFactory.cs     # Factory pour créer des DbContext
│   ├── IBonEntreeLockService.cs    # Service verrouillage bons
│   ├── ICrossModuleService.cs      # Communication inter-modules
│   ├── ICurrentUserService.cs      # Utilisateur courant
│   ├── IEmailNotificationService.cs # Notifications email
│   ├── IIdentityProvider.cs        # Abstraction identité Windows
│   ├── IModule.cs                  # Définition d'un module
│   ├── INotificationRejetService.cs # Notifications de rejet
│   ├── IQRCodeService.cs           # Génération QR codes
│   ├── IReferenceDataService.cs    # Données de référence
│   ├── IRepository.cs              # Pattern Repository
│   ├── IWorkflowService.cs         # Workflow d'approbation
│   └── NavMenuItem.cs              # Élément menu navigation
│
├── Components/                      # Composants Blazor réutilisables
│   ├── Base/                       # Composants de base (boutons, inputs)
│   ├── Layout/                     # Composants de mise en page
│   └── Workflow/                   # Composants workflow (approbation, timeline)
│
├── Constants/                       # Constantes métier
│   └── BonStatuts.cs               # États possibles des bons
│
├── Entities/                        # Entités de référence (lookup tables)
│   ├── CategorieSortie.cs          # Catégories de sortie
│   ├── Checkpoint.cs               # Points de contrôle
│   ├── Compagnie.cs                # Compagnies/Contractors
│   ├── Departement.cs              # Départements
│   ├── Employee.cs                 # Employés
│   ├── NotificationRejet.cs        # Notifications de rejet
│   ├── PassageCheckpoint.cs        # Passages aux checkpoints
│   ├── RaisonSortie.cs             # Raisons de sortie
│   ├── Site.cs                     # Sites/Localisations
│   └── SoldeMateriel.cs            # Soldes pour liaison BEM↔BSM
│
├── Enums/                           # Énumérations partagées
│   ├── BonStatut.cs                # États du workflow
│   ├── BonType.cs                  # Type de bon (Entrée/Sortie)
│   ├── NiveauApprobation.cs        # Niveaux d'approbation
│   ├── RoleUtilisateur.cs          # Rôles utilisateurs
│   ├── StatutBonEntree.cs          # Statuts spécifiques entrée
│   ├── StatutScan.cs               # États des scans
│   ├── TypeAnomalie.cs             # Types d'anomalies
│   └── TypeMateriel.cs             # Types de matériel
│
├── Services/                        # Implémentations de services
│   ├── CurrentUserService.cs       # Récupère l'utilisateur Windows
│   ├── EmailNotificationService.cs # Envoi d'emails
│   ├── QRCodeService.cs            # Génération QR codes
│   └── WorkflowService.cs          # Logique workflow
│
└── _Imports.razor                   # Imports Blazor globaux
```

**Dépendances**: 
- `KCCMaterialFlow.Domain` ✅
- `KCCMaterialFlow.Application` ✅

**Packages NuGet**: 
- `AutoMapper.Extensions.Microsoft.DependencyInjection` - Mapping DTOs
- `FluentValidation.DependencyInjectionExtensions` - Validation
- `Microsoft.Extensions.Caching.Abstractions` - Cache
- `Microsoft.Extensions.Configuration.Abstractions` - Configuration
- `Microsoft.Extensions.Logging.Abstractions` - Logging
- `QRCoder` - Génération QR codes
- `System.DirectoryServices.AccountManagement` - Active Directory

**Pourquoi `IAppDbContext` et `IAppDbContextFactory`?**
- Permet aux modules de **ne pas dépendre d'Infrastructure**
- Le service injecte `IAppDbContextFactory` au lieu de `IDbContextFactory<KCCMaterialFlowDbContext>`
- Infrastructure implémente ces interfaces (Dependency Inversion)
- Les modules restent testables avec des mocks

**Pourquoi les entités de référence dans Core et pas Domain?**
- Ces entités sont des **lookup tables** (Compagnie, Site, Departement)
- Elles ne contiennent pas de logique métier complexe
- Elles sont partagées entre TOUS les modules
- Domain contient les **agrégats métier** (BonEntree, BonSortie)

---

### 4️⃣ KCCMaterialFlow.Infrastructure

**Rôle**: Implémente l'accès aux données et les services externes. C'est la seule couche qui connaît Entity Framework.

```
KCCMaterialFlow.Infrastructure/
├── Data/
│   ├── AppDbContextFactory.cs       # Implémente IAppDbContextFactory
│   ├── DesignTimeDbContextFactory.cs # Pour les migrations EF
│   ├── KCCMaterialFlowDbContext.cs  # DbContext principal + IAppDbContext
│   ├── RepositoryBase.cs            # Implémentation générique Repository
│   └── Configurations/              # Configurations EF Core (Fluent API)
│       ├── AnomalieConfiguration.cs
│       ├── ApprobationConfiguration.cs
│       ├── ApprobationSortieConfiguration.cs
│       ├── AuditLogConfiguration.cs
│       ├── BarriereConfiguration.cs
│       ├── BonConfiguration.cs      # TPH pour hiérarchie Bon
│       ├── BonEntreeConfiguration.cs
│       ├── BonEntreeHistoryConfiguration.cs
│       ├── BonSortieConfiguration.cs
│       ├── BonSortieExterneConfiguration.cs
│       ├── BonSortieHistoryConfiguration.cs
│       ├── BonSortieInterneConfiguration.cs
│       ├── HistoriqueScanConfiguration.cs
│       ├── ItinerairePrevuConfiguration.cs
│       ├── ItineraireSortieConfiguration.cs
│       ├── MaterielConfiguration.cs
│       ├── MaterielSortieConfiguration.cs
│       ├── ParametreSystemeConfiguration.cs
│       ├── PretConfiguration.cs
│       ├── RoleConfiguration.cs
│       ├── ScanEvenementConfiguration.cs
│       ├── StatutConfiguration.cs
│       ├── TypeMaterielConfiguration.cs
│       └── UtilisateurConfiguration.cs
│
├── Extensions/
│   └── (Extensions EF si nécessaire)
│
├── Migrations/                       # Migrations Entity Framework
│   ├── 20260128055644_InitialCreate.cs
│   ├── 20260129125918_AddBonSortieModule.cs
│   └── ... (nombreuses migrations)
│
└── Services/
    ├── HttpContextIdentityProvider.cs # Implémente IIdentityProvider
    ├── NotificationRejetService.cs    # Implémente INotificationRejetService
    └── ReferenceDataService.cs        # Implémente IReferenceDataService
```

**Dépendances**: 
- `KCCMaterialFlow.Core` ✅
- `KCCMaterialFlow.Domain` ✅
- `KCCMaterialFlow.Module.Shared` ✅
- `KCCMaterialFlow.Module.*.Application` ✅ (pour les entités des modules)

**Packages NuGet**: 
- `Microsoft.EntityFrameworkCore.SqlServer` - Provider SQL Server
- `Microsoft.EntityFrameworkCore.Design` - Outils migrations
- `Microsoft.AspNetCore.Http.Abstractions` - Pour HttpContext

**Pourquoi les configurations EF dans Infrastructure et pas dans les modules?**
- **Clean Architecture**: La persistance appartient à Infrastructure
- Les modules ne doivent pas connaître EF Core
- Centralise toutes les configurations dans un seul endroit
- Facilite les migrations (un seul projet EF)

**Pourquoi `KCCMaterialFlowDbContext` implémente `IAppDbContext`?**
```csharp
public class KCCMaterialFlowDbContext : DbContext, IAppDbContext
```
- Permet de retourner `IAppDbContext` depuis la factory
- Les modules travaillent avec l'abstraction
- Le cast `(DbContext)` est fait dans les services pour accéder à `Set<T>()`

**Pourquoi `AppDbContextFactory` wrappe `IDbContextFactory<KCCMaterialFlowDbContext>`?**
- Blazor Server nécessite une factory pour créer des DbContext thread-safe
- L'abstraction `IAppDbContextFactory` cache le type concret
- Permet de changer d'implémentation (tests, autre BD) sans modifier les modules

---

### 5️⃣ Modules (Architecture Modulaire)

**Rôle**: Chaque module est une **vertical slice** autonome avec son Application et sa Presentation.

```
Modules/
├── KCCMaterialFlow.Module.Shared/    # Code partagé entre modules
│   ├── Components/                   # Composants Blazor partagés
│   ├── DTOs/                         # DTOs communs
│   ├── Entities/                     # Entités partagées (Role, Utilisateur, etc.)
│   ├── Services/                     # Services de données de référence
│   │   ├── AuditLogService.cs       # Audit
│   │   ├── BarriereService.cs       # Gestion barrières
│   │   ├── CategorieSortieService.cs
│   │   ├── DepartementService.cs    # ⭐ Utilise IAppDbContextFactory
│   │   ├── RoleService.cs
│   │   ├── SeedDataService.cs       # Données initiales
│   │   ├── TypeMaterielService.cs
│   │   └── UtilisateurService.cs
│   ├── SharedModule.cs              # Implémente IModule
│   └── ModuleInfo.cs                # Métadonnées du module
│
├── BonEntree/
│   ├── KCCMaterialFlow.Module.BonEntree.Application/
│   │   ├── DTOs/                    # BonEntreeListDto, BonEntreeViewDto, etc.
│   │   ├── Entities/                # Entités spécifiques Application
│   │   │   ├── Bon.cs              # Version Application avec IdBon
│   │   │   ├── BonEntree.cs
│   │   │   └── BonEntreeHistory.cs
│   │   ├── Mappings/               # Profils AutoMapper
│   │   ├── Repositories/           # Interfaces Repository
│   │   │   └── IBonEntreeRepository.cs
│   │   ├── Services/               # Services métier
│   │   │   ├── IBonEntreeService.cs
│   │   │   ├── BonEntreeService.cs
│   │   │   └── BonEntreeLockService.cs
│   │   ├── Validators/             # Validateurs FluentValidation
│   │   ├── BonEntreeModule.cs      # Configuration DI du module
│   │   └── ModuleInfo.cs
│   │
│   └── KCCMaterialFlow.Module.BonEntree.Presentation/
│       ├── Pages/                  # Pages Blazor
│       │   ├── BonEntreeNew.razor  # Création
│       │   ├── BonEntreeEdit.razor # Modification
│       │   ├── BonEntreeView.razor # Consultation
│       │   └── BonEntreePrint.razor # Impression
│       ├── wwwroot/               # Assets statiques
│       └── PresentationInfo.cs
│
├── BonSortie/                      # Structure identique à BonEntree
│   ├── KCCMaterialFlow.Module.BonSortie.Application/
│   └── KCCMaterialFlow.Module.BonSortie.Presentation/
│
└── Securite/                       # Module Sécurité (Scans, Anomalies)
    ├── KCCMaterialFlow.Module.Securite.Application/
    └── KCCMaterialFlow.Module.Securite.Presentation/
```

**Pourquoi Application ET Presentation séparés?**
- **Application**: Services, DTOs, interfaces - peut être testé unitairement
- **Presentation**: Pages Blazor, composants UI - nécessite des tests d'intégration
- Permet de réutiliser Application dans d'autres UIs (API REST, MAUI, etc.)

**Pourquoi les entités sont dupliquées (Domain + Module.Application)?**
- **Domain**: Entités "pures" avec `Id` (design original)
- **Module.Application**: Entités "opérationnelles" avec `IdBon` (schéma BD existant)
- **⚠️ Recommendation**: À terme, unifier vers une seule représentation

**Comment les modules s'enregistrent?**
```csharp
// Dans Program.cs
var bonEntreeModule = new BonEntreeModule();
bonEntreeModule.ConfigureServices(builder.Services);
```
Chaque module implémente `IModule.ConfigureServices()` pour enregistrer ses dépendances.

---

### 6️⃣ KCCMaterialFlow.Host (Composition Root)

**Rôle**: Point d'entrée de l'application, assemble toutes les couches, configure l'injection de dépendances.

```
KCCMaterialFlow.Host/
├── Components/
│   ├── App.razor                   # Composant racine Blazor
│   ├── Routes.razor                # Configuration routage
│   ├── _Imports.razor              # Imports globaux
│   ├── Dialogs/                    # Dialogues modaux
│   ├── Layout/                     # Layouts
│   │   ├── MainLayout.razor        # Layout principal avec menu
│   │   ├── NavMenu.razor           # Menu de navigation dynamique
│   │   ├── TopBar.razor            # Barre supérieure
│   │   ├── DevRoleSwitcher.razor   # Simulateur de rôles (DEV)
│   │   └── UnifiedSearch.razor     # Recherche globale
│   └── Pages/                      # Pages globales
│       ├── Dashboard.razor         # Tableau de bord
│       ├── MesApprobations.razor   # Approbations utilisateur
│       ├── ListeBons.razor         # Liste tous les bons
│       ├── SuiviBons.razor         # Suivi temps réel
│       ├── Error.razor             # Page d'erreur
│       └── Admin/                  # Pages administration
│
├── Repositories/                   # Implémentations concrètes Repository
│   ├── BonEntree/
│   │   └── BonEntreeRepository.cs
│   ├── BonSortie/
│   │   └── BonSortieRepository.cs
│   └── Securite/
│       ├── AnomalieRepository.cs
│       └── ScanRepository.cs
│
├── Services/                       # Services Host-level
│   ├── CrossModuleService.cs       # Communication inter-modules
│   ├── DatabaseRoleEnricherService.cs # Enrichit rôles depuis BD
│   ├── DevRoleClaimsTransformation.cs # Transformation claims (dev)
│   ├── DevRoleSwitcherService.cs   # Switch rôle simulation
│   ├── UnifiedSearchService.cs    # Recherche unifiée
│   └── UserNotificationService.cs  # Notifications UI
│
├── Middleware/                     # Middlewares ASP.NET Core (vide)
├── Properties/
│   └── launchSettings.json         # Configuration lancement
├── wwwroot/                        # Fichiers statiques
├── Logs/                           # Fichiers de log
│
├── Program.cs                      # 🔥 COMPOSITION ROOT 🔥
├── appsettings.json                # Configuration production
└── appsettings.Development.json    # Configuration développement
```

**Pourquoi les Repositories dans Host et pas Infrastructure?**

C'est une **décision architecturale intentionnelle** :

| Approche | Avantages | Inconvénients |
|----------|-----------|---------------|
| **Repositories dans Host** (actuel) | Host contrôle l'assemblage, pas besoin de référence circulaire | Duplication si plusieurs hosts |
| Repositories dans Infrastructure | Centralisation | Infrastructure doit référencer tous les modules |

Dans votre cas, **Host** est le seul point d'entrée, donc c'est acceptable.

**Program.cs - Le Composition Root**

```csharp
// 1. Configuration EF Core avec Factory
builder.Services.AddDbContextFactory<KCCMaterialFlowDbContext>(...);
builder.Services.AddScoped<IAppDbContextFactory, AppDbContextFactory>();

// 2. Enregistrement des services Core
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();

// 3. Enregistrement des modules
sharedModule.ConfigureServices(builder.Services);
bonEntreeModule.ConfigureServices(builder.Services);
bonSortieModule.ConfigureServices(builder.Services);
securiteModule.ConfigureServices(builder.Services);

// 4. Repositories (Composition Root les assemble)
builder.Services.AddScoped<IBonEntreeRepository, BonEntreeRepository>();
```

---

## ✅ Points Conformes Clean Architecture

### ✅ 1. Dependency Rule (Règle de Dépendance)

Les dépendances pointent **vers l'intérieur** :

```
Host → Infrastructure → Core → Application → Domain
  ↓         ↓            ↓          ↓
Modules  Modules      Modules    Modules
```

Aucune couche interne ne connaît les couches externes.

### ✅ 2. Dependency Inversion (Inversion des Dépendances)

```csharp
// Dans Core (abstraction)
public interface IAppDbContextFactory { ... }

// Dans Infrastructure (implémentation)
public class AppDbContextFactory : IAppDbContextFactory { ... }

// Dans Module (utilisation via abstraction)
public class DepartementService
{
    private readonly IAppDbContextFactory _dbContextFactory;
}
```

### ✅ 3. Séparation des Préoccupations

- **Domain**: Entités et règles métier pures
- **Core**: Services partagés et abstractions
- **Infrastructure**: Accès données (EF Core)
- **Modules**: Fonctionnalités verticales
- **Host**: Composition et présentation

### ✅ 4. Testabilité

Les modules dépendent d'interfaces :
- `IAppDbContextFactory` peut être mocké
- `ICurrentUserService` peut être mocké
- Services testables unitairement

---

## ⚠️ Points d'Attention et Recommandations

### ⚠️ 1. Duplication des Interfaces (Application vs Core)

**Problème**: Les interfaces existent dans les deux projets.

```
KCCMaterialFlow.Application/Interfaces/IWorkflowService.cs
KCCMaterialFlow.Core/Abstractions/IWorkflowService.cs
```

**Recommandation**: Garder uniquement dans **Core** et supprimer Application/Interfaces.

### ⚠️ 2. Entités Dupliquées (Domain vs Module.Application)

**Problème**:
```
Domain/Entities/BonEntree/Bon.cs          → int Id
Module.BonEntree.Application/Entities/Bon.cs → int IdBon
```

**Impact**: Les configurations EF utilisent les entités des modules (avec `IdBon`).

**Recommandation**: À terme, migrer vers une seule représentation d'entité.

### ⚠️ 3. Services dans Core avec Dépendances

**Observation**: `QRCodeService`, `CurrentUserService` sont dans Core avec des dépendances externes.

**OK si**: Ces services n'ont pas de dépendances vers Infrastructure.  
**Vérifié**: ✅ Ils utilisent seulement des abstractions (`IIdentityProvider`).

### ⚠️ 4. Composants Blazor dans Core

**Observation**: `Core/Components/` contient des composants Blazor.

**Acceptable si**: Ce sont des composants **réutilisables** sans logique métier (boutons, layouts).  
**Alternative**: Créer un projet `KCCMaterialFlow.UI.Components` séparé.

---

## 🔄 Flux de Données Typique

```
1. [Page Blazor] Utilisateur clique "Créer Bon"
          ↓
2. [BonEntree.Presentation] BonEntreeNew.razor
          ↓ Inject IBonEntreeService
3. [BonEntree.Application] BonEntreeService.CreateAsync()
          ↓ Inject IAppDbContextFactory
4. [Core] IAppDbContextFactory.CreateDbContext()
          ↓ Résolu par DI vers
5. [Infrastructure] AppDbContextFactory.CreateDbContext()
          ↓ Retourne
6. [Infrastructure] KCCMaterialFlowDbContext (implémente IAppDbContext)
          ↓ Le service cast vers DbContext
7. [BonEntree.Application] (DbContext)dbContext.Set<BonEntree>().Add(...)
          ↓
8. [Infrastructure] EF Core persiste en base SQL Server
```

---

## 📊 Graphe des Dépendances des Projets

```
                    ┌─────────────────────────────┐
                    │    KCCMaterialFlow.Host     │
                    │    (Composition Root)       │
                    └──────────────┬──────────────┘
                                   │ références
        ┌──────────────────────────┼──────────────────────────┐
        ↓                          ↓                          ↓
┌───────────────┐        ┌──────────────────┐       ┌─────────────────┐
│ Infrastructure │        │  Module.*.Pres   │       │  Module.Shared  │
└───────┬───────┘        └────────┬─────────┘       └────────┬────────┘
        │                         │                          │
        │                         ↓                          │
        │                ┌──────────────────┐                │
        │                │  Module.*.App    │←───────────────┘
        │                └────────┬─────────┘
        │                         │
        ↓                         ↓
┌───────────────────────────────────────────────────────────────────┐
│                      KCCMaterialFlow.Core                         │
└───────────────────────────────────┬───────────────────────────────┘
                                    │
                                    ↓
                    ┌───────────────────────────┐
                    │ KCCMaterialFlow.Application│
                    └───────────────┬───────────┘
                                    │
                                    ↓
                    ┌───────────────────────────┐
                    │  KCCMaterialFlow.Domain   │
                    │   (Aucune dépendance)     │
                    └───────────────────────────┘
```

---

## 📝 Conclusion

L'architecture est **globalement conforme** aux principes Clean Architecture :

| Principe | Statut | Note |
|----------|--------|------|
| Dependency Rule | ✅ | Les dépendances pointent vers l'intérieur |
| Dependency Inversion | ✅ | Abstractions dans Core, implémentations dans Infrastructure |
| Séparation des Couches | ✅ | Domain → Application → Core → Infrastructure → Host |
| Modules Découplés | ✅ | Module.Shared corrigé (plus de ref vers Infrastructure) |
| Testabilité | ✅ | Services injectent des interfaces mockables |

**Actions correctives mineures suggérées** :
1. Supprimer la duplication Application/Interfaces
2. Unifier les entités Domain/Module.Application à terme
3. Documenter le pattern de cast `(DbContext)` pour `IAppDbContext`

---

*Document généré automatiquement - Clean Architecture Analysis Tool*
