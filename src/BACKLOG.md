# KCCMaterialFlow - Backlog Complet
## Système de Gestion Automatisée des Bons d'Entrée et de Sortie

**Document Version:** 1.0  
**Date:** 27 janvier 2026  
**Total Estimated Story Points:** 380  
**Sprint Duration:** 1 Semaine  
**Project Duration:** 14 Semaines

---

## Table des Matières

1. [Résumé du Backlog](#1-résumé-du-backlog)
2. [Spécifications Techniques](#2-spécifications-techniques)
3. [Phase 0: Foundation (Semaines 1-2)](#3-phase-0-foundation-semaines-1-2)
4. [Phase 1: Module Bon d'Entrée (Semaines 3-5)](#4-phase-1-module-bon-dentrée-semaines-3-5)
5. [Phase 2: Module Bon de Sortie (Semaines 6-9)](#5-phase-2-module-bon-de-sortie-semaines-6-9)
6. [Phase 3: Module Sécurité & Barrières (Semaines 10-12)](#6-phase-3-module-sécurité--barrières-semaines-10-12)
7. [Phase 4: Intégration & Polish (Semaines 13-14)](#7-phase-4-intégration--polish-semaines-13-14)
8. [Annexes](#8-annexes)

---

## 1. Résumé du Backlog

| Phase | Module/Area | Nombre Tâches | Story Points |
|-------|-------------|---------------|--------------|
| Phase 0 | Foundation | 55 | 65 |
| Phase 1 | Bon d'Entrée (BEM) | 50 | 75 |
| Phase 2 | Bon de Sortie (BSM) | 65 | 100 |
| Phase 3 | Sécurité & Barrières | 45 | 70 |
| Phase 4 | Intégration | 40 | 70 |
| **Total** | | **255** | **380** |

---

## 2. Spécifications Techniques

### 2.1 Stack Technologique

| Composant | Technologie |
|-----------|-------------|
| Framework | .NET 10 |
| UI | Blazor Server |
| ORM | Entity Framework Core 10 |
| Base de données | SQL Server |
| Authentification | Windows Authentication |
| Email | SQL Server Database Mail |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Logging | Serilog |
| UI Components | Radzen Blazor |
| QR Code | QRCoder / ZXing.Net |

### 2.2 Structure de la Solution

```
src/
├── KCCMaterialFlow.sln
├── KCCMaterialFlow.Host/                    # Blazor Server App
├── KCCMaterialFlow.Core/                    # Framework partagé
├── Modules/
│   ├── KCCMaterialFlow.Module.Shared/       # Entités communes
│   ├── KCCMaterialFlow.Module.BonEntree/    # Module BEM
│   ├── KCCMaterialFlow.Module.BonSortie/    # Module BSM
│   └── KCCMaterialFlow.Module.Securite/     # Module Barrières & Scan
└── Tests/
    ├── KCCMaterialFlow.Core.Tests/
    ├── KCCMaterialFlow.Module.BonEntree.Tests/
    ├── KCCMaterialFlow.Module.BonSortie.Tests/
    └── KCCMaterialFlow.Module.Securite.Tests/
```

### 2.3 Rôles Utilisateurs

| Rôle | Code | Permissions |
|------|------|-------------|
| Demandeur | DEMANDEUR | Créer, modifier ses bons, suivre statut |
| Superviseur | SUPERVISEUR | Approuver bons de son département |
| Directeur Général | DG | Approuver après superviseur |
| OPJ Sécurité | OPJ | Validation sécurité |
| Département IT | IT | Approbation matériel informatique |
| Département Environnement | ENV | Approbation résidus/radioprotection |
| Équipe Identification | IDENTIFICATION | Vérification finale, génération QR, extension prêts |
| Agent Barrière | BARRIERE | Scan QR, contrôle physique |
| Investigation | INVESTIGATION | Traitement anomalies |
| Administrateur | ADMIN | Gestion complète système |

### 2.4 Statuts des Bons

| Statut | Code | Description |
|--------|------|-------------|
| Brouillon | DRAFT | Saisie initiale |
| En attente Superviseur | PENDING_SUP | Attente validation superviseur |
| En attente DG | PENDING_DG | Attente validation DG |
| En attente IT | PENDING_IT | Attente validation IT (si applicable) |
| En attente Environnement | PENDING_ENV | Attente validation Environnement (si applicable) |
| En attente OPJ | PENDING_OPJ | Attente validation sécurité |
| En attente Identification | PENDING_ID | Attente vérification finale |
| Approuvé | APPROVED | QR Code généré, prêt pour impression |
| En transit | IN_TRANSIT | Matériel en mouvement |
| Complété | COMPLETED | Passage toutes barrières confirmé |
| Archivé | ARCHIVED | Lié à sortie/clôturé |
| Rejeté | REJECTED | Refusé à une étape |
| Investigation | INVESTIGATION | Anomalie détectée |

### 2.5 Types de Matériel (Bon de Sortie)

| Type | Code | Workflow Spécifique |
|------|------|---------------------|
| Circulaire | CIRCULAIRE | Standard (validité 6 mois) |
| Informatique | INFORMATIQUE | + Approbation IT |
| Fin de Chantier | FIN_CHANTIER | Standard |
| Résidu/Déchet | RESIDU | + Approbation Environnement |
| Radioprotection | RADIOPROTECTION | + Approbation Environnement |
| Modification | MODIFICATION | + Approbation Environnement |
| Mise en prêt | PRET | Standard + Date retour obligatoire |

### 2.6 Barrières (Checkpoints)

| ID | Nom | Localisation |
|----|-----|--------------|
| 1 | KTO | À définir |
| 2 | LUILU | À définir |
| 3 | SKM | À définir |
| 4 | LUSANGA | À définir |
| 5 | KOV | À définir |
| 6 | MV | À définir |
| 7 | MASHAMBA | À définir |
| 8 | KTC | À définir |

---

## 3. Phase 0: Foundation (Semaines 1-2)

### 3.1 Structure Solution & Projets (Semaine 1)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-001 | Créer KCCMaterialFlow.sln | Créer fichier solution dans `src/` | Solution s'ouvre dans VS 2025 | 1 | Critical |
| F-002 | Créer KCCMaterialFlow.Host | Projet Blazor Server .NET 10 | `dotnet new blazor -n KCCMaterialFlow.Host -int Server` | 1 | Critical |
| F-003 | Créer KCCMaterialFlow.Core | Class library pour framework | `dotnet new classlib -n KCCMaterialFlow.Core` | 1 | Critical |
| F-004 | Créer KCCMaterialFlow.Module.Shared | Class library entités partagées | Projet créé et référencé | 1 | Critical |
| F-005 | Créer KCCMaterialFlow.Module.BonEntree | Class library module BEM | Projet créé et référencé | 1 | Critical |
| F-006 | Créer KCCMaterialFlow.Module.BonSortie | Class library module BSM | Projet créé et référencé | 1 | Critical |
| F-007 | Créer KCCMaterialFlow.Module.Securite | Class library module Sécurité | Projet créé et référencé | 1 | Critical |
| F-008 | Configurer références projets Host | Références Host → Core, tous Modules | Solution compile | 1 | Critical |
| F-009 | Configurer références projets Modules | Références Modules → Core, Shared | Solution compile | 1 | Critical |
| F-010 | Configurer packages NuGet Core | EF Core, AutoMapper, FluentValidation, Serilog | Packages installés | 2 | Critical |
| F-011 | Configurer packages NuGet Host | Blazor, Authentication, Radzen.Blazor, QRCoder | Packages installés | 2 | Critical |
| F-012 | Structure dossiers Core | Abstractions/, Components/, Data/, Extensions/, Services/ | Dossiers créés | 1 | High |
| F-013 | Structure dossiers Shared | Entities/, Services/, Components/, DTOs/, Enums/ | Dossiers créés | 1 | High |
| F-014 | Structure dossiers BonEntree | Entities/, Services/, Repositories/, Pages/, DTOs/, Validators/ | Dossiers créés | 1 | High |
| F-015 | Structure dossiers BonSortie | Entities/, Services/, Repositories/, Pages/, DTOs/, Validators/ | Dossiers créés | 1 | High |
| F-016 | Structure dossiers Securite | Entities/, Services/, Pages/, Components/ | Dossiers créés | 1 | High |

### 3.2 Core Framework - Abstractions (Semaine 1)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-017 | Créer IModule interface | ModuleId, ModuleName, RoutePrefix, ConfigureServices() | Interface documentée | 2 | Critical |
| F-018 | Créer NavMenuItem class | Id, Label, Href, Icon, ParentId, Order | Classe fonctionnelle | 1 | Critical |
| F-019 | Créer IRepository<T> interface | GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync, DeleteAsync | Interface générique complète | 2 | Critical |
| F-020 | Créer IWorkflowService interface | ApproveAsync, RejectAsync, ReturnAsync, GetNextApprover() | Interface workflow dynamique | 2 | Critical |
| F-021 | Créer IEmailNotificationService interface | SendEmailAsync, SendBonNotificationAsync<T> | Interface email | 1 | High |
| F-022 | Créer ICurrentUserService interface | GetCurrentUser, GetUserLogin, IsInRole | Interface contexte utilisateur | 1 | High |
| F-023 | Créer IQRCodeService interface | GenerateQRCode, ValidateQRCode, HashCode | Interface QR Code | 2 | Critical |

### 3.3 Core Framework - Enums (Semaine 1)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-024 | Créer BonType enum | Entree, SortieInterne, SortieExterne | Enum défini | 1 | Critical |
| F-025 | Créer BonStatut enum | Draft, PendingSup, PendingDG, PendingOPJ, Approved, etc. | Enum complet | 1 | Critical |
| F-026 | Créer TypeMateriel enum | Circulaire, Informatique, FinChantier, Residu, Radioprotection, Modification, Pret | Enum complet | 1 | Critical |
| F-027 | Créer RoleUtilisateur enum | Demandeur, Superviseur, DG, OPJ, IT, Environnement, Identification, Barriere, Investigation, Admin | Enum complet | 1 | Critical |
| F-028 | Créer StatutScan enum | Conforme, Anomalie, Expire, HorsItineraire | Enum complet | 1 | Critical |
| F-029 | Créer TypeAnomalie enum | BarriereInappropriee, DocumentExpire, DocumentInexistant, TentativeEchange | Enum complet | 1 | Critical |

### 3.4 Core Framework - Data Layer (Semaine 1)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-030 | Créer KCCMaterialFlowDbContext | DbContext avec OnModelCreating modulaire | DbContext compile | 3 | Critical |
| F-031 | Configurer DbContext dans Program.cs | Connection string SQL Server | Connexion fonctionne | 2 | Critical |
| F-032 | Créer RepositoryBase<T> | Implémentation générique CRUD | Repository fonctionnel | 3 | Critical |
| F-033 | Tester connectivité DB | Vérifier connexion | `context.Database.CanConnect()` = true | 1 | Critical |

### 3.5 Core Framework - Base Components (Semaine 1-2)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-034 | Créer BaseFormPage.razor | Composant base pour New/Edit avec validation | Composant héritable | 3 | Critical |
| F-035 | Créer BaseViewPage.razor | Composant base pour View read-only | Composant héritable | 3 | Critical |
| F-036 | Créer BaseListPage.razor | Composant base avec DataGrid, search, pagination | Composant héritable | 3 | Critical |
| F-037 | Créer DataGrid.razor | Grille configurable avec tri, pagination | Grille fonctionnelle | 3 | Critical |
| F-038 | Créer FileUpload.razor | Upload multi-fichiers avec drag-drop | Upload fonctionne | 3 | Critical |
| F-039 | Créer FileList.razor | Liste fichiers avec download/delete | Liste fonctionnelle | 2 | High |
| F-040 | Créer LoadingSpinner.razor | Indicateur de chargement | Spinner s'affiche | 1 | High |
| F-041 | Créer ConfirmDialog.razor | Modal de confirmation | Dialog fonctionnel | 2 | High |
| F-042 | Créer PageHeader.razor | Titre page avec breadcrumb | Header s'affiche | 2 | High |
| F-043 | Créer StatusBadge.razor | Badge coloré selon statut | Badge s'affiche correctement | 1 | High |

### 3.6 Core Framework - Workflow Components (Semaine 2)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-044 | Créer WorkflowButtons.razor | Boutons Approuver/Rejeter selon rôle et statut | Boutons conditionnels | 3 | Critical |
| F-045 | Créer CommentModal.razor | Modal saisie commentaire workflow | Modal capture commentaire | 2 | Critical |
| F-046 | Créer ApprovalHistory.razor | Timeline historique approbations | Timeline s'affiche | 3 | Critical |
| F-047 | Créer ApproverSelector.razor | Sélecteur approbateur (cas ad-hoc) | Sélection fonctionne | 2 | High |

### 3.7 Core Framework - Services (Semaine 2)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-048 | Créer WorkflowService.cs | Logique workflow dynamique selon type matériel | Service fonctionnel | 4 | Critical |
| F-049 | Créer EmailNotificationService.cs | Envoi emails via sp_send_dbmail | Emails envoyés | 3 | Critical |
| F-050 | Créer CurrentUserService.cs | Récupération identité Windows Auth | User retourné | 2 | Critical |
| F-051 | Créer QRCodeService.cs | Génération et validation QR Code haché | QR Code généré | 3 | Critical |
| F-052 | Créer HashService.cs | Hachage sécurisé pour QR Code | Hash fonctionne | 2 | Critical |

### 3.8 Host Project Setup (Semaine 2)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-053 | Configurer Program.cs avec modules | Enregistrement tous modules | Modules enregistrés | 2 | Critical |
| F-054 | Configurer Windows Authentication | Negotiate auth dans Program.cs | User disponible | 2 | Critical |
| F-055 | Créer App.razor | Composant racine avec router | App s'affiche | 1 | Critical |
| F-056 | Créer Routes.razor | Configuration routing modules | Routes découvertes | 2 | Critical |
| F-057 | Créer MainLayout.razor | Layout principal header/nav/content | Layout s'affiche | 2 | Critical |
| F-058 | Créer NavMenu.razor | Menu navigation dynamique par module | Menu s'affiche | 3 | Critical |
| F-059 | Créer TopBar.razor | Barre supérieure avec user info | TopBar s'affiche | 2 | High |
| F-060 | Configurer appsettings.json | Connection strings, email settings | Config chargée | 1 | Critical |
| F-061 | Configurer Serilog | Logging fichier et DB | Logs écrits | 2 | High |
| F-062 | Créer Home.razor dashboard | Page accueil avec métriques | Dashboard s'affiche | 3 | High |

### 3.9 Shared Module - Entities (Semaine 2)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-063 | Créer SharedModule.cs | Implémentation IModule | Module s'enregistre | 2 | Critical |
| F-064 | Créer Utilisateur.cs entity | idUtilisateur, nomComplet, fonction, departement, role | Entity mappée | 2 | Critical |
| F-065 | Créer UtilisateurConfiguration.cs | EF Core configuration | Config appliquée | 1 | Critical |
| F-066 | Créer Departement.cs entity | idDepartement, nomDepartement, responsableLogin | Entity mappée | 2 | Critical |
| F-067 | Créer DepartementConfiguration.cs | EF Core configuration | Config appliquée | 1 | Critical |
| F-068 | Créer Barriere.cs entity | idBarriere, nomBarriere, localisation | Entity mappée | 2 | Critical |
| F-069 | Créer BarriereConfiguration.cs | EF Core configuration | Config appliquée | 1 | Critical |

### 3.10 Shared Module - Services (Semaine 2)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-070 | Créer IUtilisateurService interface | GetByLoginAsync, GetByDepartementAsync, SearchAsync | Interface définie | 1 | Critical |
| F-071 | Créer UtilisateurService.cs | Implémentation avec cache 30 min | Service fonctionnel | 2 | Critical |
| F-072 | Créer IDepartementService interface | GetAllAsync, GetByIdAsync | Interface définie | 1 | Critical |
| F-073 | Créer DepartementService.cs | Implémentation avec cache 1h | Service fonctionnel | 2 | Critical |
| F-074 | Créer IBarriereService interface | GetAllAsync, GetByIdAsync | Interface définie | 1 | Critical |
| F-075 | Créer BarriereService.cs | Implémentation avec cache 1h | Service fonctionnel | 2 | Critical |

### 3.11 Shared Module - Components (Semaine 2)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| F-076 | Créer DepartementSelector.razor | Dropdown sélection département | Composant fonctionnel | 2 | Critical |
| F-077 | Créer UtilisateurSelector.razor | Autocomplete sélection utilisateur | Composant fonctionnel | 2 | Critical |
| F-078 | Créer BarriereSelector.razor | Dropdown sélection barrière | Composant fonctionnel | 2 | High |

---

## 4. Phase 1: Module Bon d'Entrée (Semaines 3-5)

### 4.1 BonEntree Module - Setup & Entities (Semaine 3)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BEM-001 | Créer BonEntreeModule.cs | IModule avec ModuleId="BEM", RoutePrefix="/bon-entree" | Module s'enregistre | 2 | Critical |
| BEM-002 | Créer Bon.cs entity (classe mère) | numeroReference, dateCreation, dateExpiration, statutActuel, destination, provenance, description, quantite | Entity abstraite | 3 | Critical |
| BEM-003 | Créer BonConfiguration.cs | EF Core configuration avec TPH/TPT | Config appliquée | 2 | Critical |
| BEM-004 | Créer BonEntree.cs entity | Hérite Bon + numeroContrat, nomCompagnie, emailContractant, siteManager, hostDepartment, reasonOnSite, nomEscorteur, fonctionEscorteur | Entity mappée | 3 | Critical |
| BEM-005 | Créer BonEntreeConfiguration.cs | EF Core configuration avec FK | Config appliquée | 2 | Critical |
| BEM-006 | Créer Materiel.cs entity | codeProduitSerial, designation, quantite, provenance, destination | Entity mappée | 2 | Critical |
| BEM-007 | Créer MaterielConfiguration.cs | EF Core configuration avec FK vers Bon | Relation 1-N | 1 | Critical |
| BEM-008 | Créer Approbation.cs entity | ordreEtape, decision, dateAction, reservesEventuelles, utilisateurId, bonId | Entity mappée | 2 | Critical |
| BEM-009 | Créer ApprobationConfiguration.cs | EF Core configuration | Config appliquée | 1 | Critical |
| BEM-010 | Créer ItinerairePrevu.cs entity | ordrePassage, bonId, barriereId | Entity mappée | 2 | Critical |
| BEM-011 | Créer ItinerairePrevuConfiguration.cs | EF Core configuration | Config appliquée | 1 | Critical |
| BEM-012 | Créer BonEntreeHistory.cs entity | historyId, bonId, action, actionBy, actionDate, comment | Entity mappée | 2 | Critical |

### 4.2 BonEntree Module - Repository Layer (Semaine 3)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BEM-013 | Créer IBonEntreeRepository interface | GetByIdAsync, GetByNumeroAsync, SearchAsync, CreateAsync, UpdateAsync | Interface complète | 2 | Critical |
| BEM-014 | Créer BonEntreeRepository.cs | Implémentation avec EF Core, includes | CRUD fonctionnel | 3 | Critical |
| BEM-015 | Implémenter GetByIdAsync avec includes | Charger Materiels, Approbations, Itineraire, History | Données liées chargées | 2 | Critical |
| BEM-016 | Implémenter SearchAsync avec filtres | Filtrer par Statut, DateRange, Compagnie, Departement | Recherche fonctionnelle | 3 | Critical |
| BEM-017 | Implémenter pagination | Skip/Take avec total count | Pagination fonctionne | 2 | High |

### 4.3 BonEntree Module - Service Layer (Semaine 3-4)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BEM-018 | Créer IBonEntreeService interface | CreateAsync, UpdateAsync, GetAsync, GetListAsync, GetPendingApprovalsAsync | Interface complète | 2 | Critical |
| BEM-019 | Créer BonEntreeService.cs | Implémentation logique métier | Service fonctionnel | 3 | Critical |
| BEM-020 | Implémenter CreateAsync | Validation, génération numéro référence, sauvegarde, notification | Bon créé avec numéro | 3 | Critical |
| BEM-021 | Implémenter génération numéro référence | Format: BEM-YYYY-NNNNN auto-incrémenté | Numéro unique généré | 2 | Critical |
| BEM-022 | Implémenter UpdateAsync | Validation statut permet edit, mise à jour | Modifications sauvées | 2 | Critical |
| BEM-023 | Implémenter GetPendingApprovalsAsync | Bons en attente approbation user courant | Liste correcte | 3 | Critical |
| BEM-024 | Implémenter workflow Approve/Reject | Transition statut, notification prochain approbateur | Workflow fonctionne | 4 | Critical |
| BEM-025 | Implémenter génération QR Code | Après approbation finale, QR Code haché généré | QR Code disponible | 3 | Critical |
| BEM-026 | Implémenter calcul itinéraire | Déterminer barrières selon provenance/destination | Itinéraire calculé | 3 | High |

### 4.4 BonEntree Module - DTOs & Validators (Semaine 4)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BEM-027 | Créer BonEntreeCreateDto | DTO formulaire création | DTO défini | 2 | Critical |
| BEM-028 | Créer BonEntreeUpdateDto | DTO formulaire modification | DTO défini | 1 | Critical |
| BEM-029 | Créer BonEntreeViewDto | DTO affichage complet | DTO défini | 2 | Critical |
| BEM-030 | Créer BonEntreeListDto | DTO liste/recherche | DTO défini | 1 | Critical |
| BEM-031 | Créer MaterielDto | DTO grille matériels | DTO défini | 1 | High |
| BEM-032 | Créer BonEntreeCreateValidator | Règles FluentValidation | Validation fonctionne | 2 | Critical |
| BEM-033 | Créer BonEntreeUpdateValidator | Règles FluentValidation | Validation fonctionne | 1 | Critical |
| BEM-034 | Créer AutoMapper profile | Mapping entities ↔ DTOs | Mappings fonctionnent | 2 | High |

### 4.5 BonEntree Module - Pages (Semaine 4)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BEM-035 | Créer BonEntreeNew.razor | Formulaire création avec tous champs | Formulaire complet | 5 | Critical |
| BEM-036 | Implémenter section Compagnie | Champs contrat, nom, email, site manager | Saisie fonctionne | 2 | Critical |
| BEM-037 | Implémenter section Contexte KCC | Host Department, Reason on site | Saisie fonctionne | 2 | Critical |
| BEM-038 | Implémenter section Escorteur | Nom, Fonction | Saisie fonctionne | 1 | Critical |
| BEM-039 | Implémenter grille Matériels | Add/Edit/Delete matériels | CRUD grille fonctionne | 3 | Critical |
| BEM-040 | Implémenter dates validité | Date création, date expiration | Dates sélectionnables | 2 | Critical |
| BEM-041 | Implémenter validation formulaire | Client et server-side | Erreurs affichées | 2 | Critical |
| BEM-042 | Créer BonEntreeEdit.razor | Formulaire modification (statut=Brouillon/Retourné) | Edit fonctionne | 4 | Critical |
| BEM-043 | Implémenter restrictions edit | Champs readonly selon statut | Restrictions respectées | 2 | High |
| BEM-044 | Créer BonEntreeView.razor | Vue lecture seule + historique + workflow buttons | Vue complète | 4 | Critical |
| BEM-045 | Implémenter boutons workflow | Approuver/Rejeter selon rôle | Boutons conditionnels | 3 | Critical |
| BEM-046 | Implémenter affichage QR Code | QR Code visible après approbation finale | QR affiché | 2 | High |
| BEM-047 | Implémenter impression bon | Bouton impression avec QR Code | Impression fonctionne | 2 | High |
| BEM-048 | Créer BonEntreeList.razor | Liste avec recherche, filtres, pagination | Liste fonctionnelle | 4 | Critical |
| BEM-049 | Implémenter filtres recherche | Par statut, date, compagnie | Filtres fonctionnent | 2 | High |
| BEM-050 | Implémenter export Excel | Export résultats recherche | Excel téléchargé | 2 | Medium |

---

## 5. Phase 2: Module Bon de Sortie (Semaines 6-9)

### 5.1 BonSortie Module - Setup & Entities (Semaine 6)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BSM-001 | Créer BonSortieModule.cs | IModule avec ModuleId="BSM", RoutePrefix="/bon-sortie" | Module s'enregistre | 2 | Critical |
| BSM-002 | Créer BonSortie.cs entity | Hérite Bon + nom, fonction, departement, motifMateriel (Enum) | Entity mappée | 3 | Critical |
| BSM-003 | Créer BonSortieConfiguration.cs | EF Core configuration | Config appliquée | 2 | Critical |
| BSM-004 | Créer BonSortieExterne.cs entity | Hérite BonSortie + bonEntreeAssocieId, typeMateriel | Entity mappée | 2 | Critical |
| BSM-005 | Créer BonSortieExterneConfiguration.cs | EF Core config + FK vers BonEntree | Relation 0..1 | 2 | Critical |
| BSM-006 | Créer BonSortieInterne.cs entity | Hérite BonSortie + typeMateriel | Entity mappée | 2 | Critical |
| BSM-007 | Créer BonSortieInterneConfiguration.cs | EF Core configuration | Config appliquée | 1 | Critical |
| BSM-008 | Créer BonSortieHistory.cs entity | Historique actions | Entity mappée | 2 | Critical |
| BSM-009 | Créer Pret.cs entity (extension BonSortie) | dateRetourPrevue, dateRetourEffective, estRetourne | Entity pour prêts | 2 | High |
| BSM-010 | Créer PretConfiguration.cs | EF Core configuration | Config appliquée | 1 | High |

### 5.2 BonSortie Module - Repository Layer (Semaine 6)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BSM-011 | Créer IBonSortieRepository interface | CRUD + SearchAsync avec filtres spécifiques | Interface complète | 2 | Critical |
| BSM-012 | Créer BonSortieRepository.cs | Implémentation polymorphique (Interne/Externe) | Repository fonctionnel | 3 | Critical |
| BSM-013 | Implémenter GetByIdAsync avec includes | Charger type concret + relations | Données chargées | 2 | Critical |
| BSM-014 | Implémenter SearchAsync | Filtrer par type, statut, département, typeMateriel | Recherche fonctionne | 3 | Critical |
| BSM-015 | Implémenter GetBonsEnPret | Récupérer bons de prêt avec date échéance | Liste prêts | 2 | High |
| BSM-016 | Implémenter GetPretsExpirant | Prêts expirant dans N jours | Liste alertes | 2 | High |

### 5.3 BonSortie Module - Service Layer (Semaine 6-7)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BSM-017 | Créer IBonSortieService interface | CreateAsync, UpdateAsync, GetAsync, GetListAsync, workflow methods | Interface complète | 2 | Critical |
| BSM-018 | Créer BonSortieService.cs | Logique métier avec workflows dynamiques | Service fonctionnel | 4 | Critical |
| BSM-019 | Implémenter CreateAsync Interne | Création bon sortie interne | Bon créé | 3 | Critical |
| BSM-020 | Implémenter CreateAsync Externe | Création bon sortie externe avec lien BonEntree obligatoire | Lien validé | 3 | Critical |
| BSM-021 | Implémenter génération numéro référence | Format: BSM-YYYY-NNNNN ou BSI-YYYY-NNNNN | Numéro unique | 2 | Critical |
| BSM-022 | Implémenter workflow standard | Demandeur → Superviseur → DG → OPJ → Identification | Workflow fonctionne | 3 | Critical |
| BSM-023 | Implémenter workflow Informatique | + Étape IT avant Superviseur | Workflow IT | 2 | Critical |
| BSM-024 | Implémenter workflow Environnement | + Étape Environnement avant Superviseur (Résidu, Radioprotection, Modification) | Workflow ENV | 2 | Critical |
| BSM-025 | Implémenter logique Circulaire | Validité 6 mois, pas de lien BonEntree obligatoire | Règles appliquées | 2 | Critical |
| BSM-026 | Implémenter logique Prêt | Date retour obligatoire, alertes, extension | Prêts gérés | 3 | Critical |
| BSM-027 | Implémenter alerte prêt J-7 | Notification Identification 1 semaine avant échéance | Alerte envoyée | 2 | High |
| BSM-028 | Implémenter alerte prêt expiré | Mail auto Investigation si date dépassée | Alerte envoyée | 2 | High |
| BSM-029 | Implémenter extension date prêt | Seule Identification peut étendre | Restriction appliquée | 2 | High |
| BSM-030 | Implémenter génération QR Code | Après approbation Identification | QR Code généré | 3 | Critical |
| BSM-031 | Implémenter liaison Entrée-Sortie | Verrouillage référence entrée | Lien créé | 2 | Critical |

### 5.4 BonSortie Module - DTOs & Validators (Semaine 7)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BSM-032 | Créer BonSortieCreateDto | DTO création avec type discriminator | DTO défini | 2 | Critical |
| BSM-033 | Créer BonSortieInterneCreateDto | DTO spécifique interne | DTO défini | 1 | Critical |
| BSM-034 | Créer BonSortieExterneCreateDto | DTO spécifique externe + bonEntreeId | DTO défini | 2 | Critical |
| BSM-035 | Créer BonSortieUpdateDto | DTO modification | DTO défini | 1 | Critical |
| BSM-036 | Créer BonSortieViewDto | DTO affichage complet | DTO défini | 2 | Critical |
| BSM-037 | Créer BonSortieListDto | DTO liste/recherche | DTO défini | 1 | Critical |
| BSM-038 | Créer PretDto | DTO gestion prêts | DTO défini | 1 | High |
| BSM-039 | Créer BonSortieCreateValidator | Validation avec règles conditionnelles par type | Validation fonctionne | 3 | Critical |
| BSM-040 | Créer AutoMapper profile | Mappings polymorphiques | Mappings fonctionnent | 2 | High |

### 5.5 BonSortie Module - Pages (Semaine 7-8)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BSM-041 | Créer BonSortieNew.razor | Formulaire création avec switch Interne/Externe | Formulaire complet | 5 | Critical |
| BSM-042 | Implémenter sélection type bon | Radio Interne/Externe | Switch fonctionne | 2 | Critical |
| BSM-043 | Implémenter section Demandeur | Nom, Fonction, Département (auto-fill si Windows Auth) | Saisie fonctionne | 2 | Critical |
| BSM-044 | Implémenter sélection typeMateriel | Dropdown selon liste Enum | Sélection fonctionne | 2 | Critical |
| BSM-045 | Implémenter affichage conditionnel | Champs spécifiques selon typeMateriel | Champs affichés/masqués | 3 | Critical |
| BSM-046 | Implémenter recherche BonEntree (Externe) | Autocomplete pour lier un bon d'entrée | Recherche fonctionne | 3 | Critical |
| BSM-047 | Implémenter validation lien obligatoire | BonEntreeId requis si Externe (sauf Circulaire) | Validation appliquée | 2 | Critical |
| BSM-048 | Implémenter grille Matériels | Add/Edit/Delete + import depuis BonEntree lié | CRUD grille fonctionne | 3 | Critical |
| BSM-049 | Implémenter champ Motif | Zone texte motif sortie | Saisie fonctionne | 1 | Critical |
| BSM-050 | Implémenter date retour (Prêt) | Champ date obligatoire si typeMateriel = Prêt | Champ conditionnel | 2 | Critical |
| BSM-051 | Créer BonSortieEdit.razor | Formulaire modification | Edit fonctionne | 4 | Critical |
| BSM-052 | Créer BonSortieView.razor | Vue complète + workflow dynamique | Vue fonctionne | 5 | Critical |
| BSM-053 | Implémenter affichage BonEntree lié | Lien cliquable vers BonEntree associé | Navigation fonctionne | 2 | High |
| BSM-054 | Implémenter boutons workflow dynamiques | Selon typeMateriel et rôle courant | Boutons corrects | 3 | Critical |
| BSM-055 | Implémenter approbation IT | Boutons spécifiques rôle IT | Workflow IT | 2 | Critical |
| BSM-056 | Implémenter approbation Environnement | Boutons spécifiques rôle ENV | Workflow ENV | 2 | Critical |
| BSM-057 | Créer BonSortieList.razor | Liste avec filtres avancés | Liste fonctionnelle | 4 | Critical |
| BSM-058 | Implémenter filtre typeMateriel | Dropdown filtrage | Filtre fonctionne | 2 | High |
| BSM-059 | Implémenter filtre typebon | Interne/Externe/Tous | Filtre fonctionne | 1 | High |

### 5.6 BonSortie Module - Gestion Prêts (Semaine 8)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BSM-060 | Créer PretsList.razor | Liste bons de prêt avec statut retour | Liste fonctionnelle | 3 | High |
| BSM-061 | Implémenter indicateurs échéance | Couleur selon jours restants | Indicateurs affichés | 2 | High |
| BSM-062 | Créer PretExtension.razor | Modal extension date (Identification only) | Extension fonctionne | 2 | High |
| BSM-063 | Créer PretRetour.razor | Confirmation retour matériel | Retour enregistré | 2 | High |
| BSM-064 | Implémenter job alerte J-7 | Background service notification | Alertes envoyées | 3 | High |
| BSM-065 | Implémenter job prêts expirés | Background service notification Investigation | Alertes envoyées | 3 | High |

### 5.7 BonSortie Module - Components (Semaine 8)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| BSM-066 | Créer TypeMaterielSelector.razor | Dropdown avec icônes/couleurs | Sélection visuelle | 2 | High |
| BSM-067 | Créer BonEntreeSearcher.razor | Composant recherche bon d'entrée | Recherche fonctionne | 3 | Critical |
| BSM-068 | Créer WorkflowIndicator.razor | Affichage étapes workflow selon type | Indicateur visuel | 3 | High |
| BSM-069 | Créer PretCountdown.razor | Compte à rebours date retour | Countdown affiché | 2 | Medium |

---

## 6. Phase 3: Module Sécurité & Barrières (Semaines 10-12)

### 6.1 Securite Module - Setup & Entities (Semaine 10)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| SEC-001 | Créer SecuriteModule.cs | IModule avec ModuleId="SEC", RoutePrefix="/securite" | Module s'enregistre | 2 | Critical |
| SEC-002 | Créer ScanEvenement.cs entity | dateHeureScan, statutScan, bonId, barriereId, agentLogin | Entity mappée | 2 | Critical |
| SEC-003 | Créer ScanEvenementConfiguration.cs | EF Core configuration avec FK | Config appliquée | 1 | Critical |
| SEC-004 | Créer Anomalie.cs entity | typeAnomalie, dateSignalement, description, bonId, scanId, estTraitee | Entity mappée | 2 | Critical |
| SEC-005 | Créer AnomalieConfiguration.cs | EF Core configuration | Config appliquée | 1 | Critical |
| SEC-006 | Créer HistoriqueScan.cs entity | Table spéciale pour Identification (tous mouvements) | Entity mappée | 2 | High |

### 6.2 Securite Module - Repository Layer (Semaine 10)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| SEC-007 | Créer IScanRepository interface | CreateScanAsync, GetScansByBonAsync, GetScansParBarriere | Interface complète | 2 | Critical |
| SEC-008 | Créer ScanRepository.cs | Implémentation queries scans | Repository fonctionnel | 3 | Critical |
| SEC-009 | Créer IAnomalieRepository interface | CreateAsync, GetByBonAsync, GetNonTraiteesAsync | Interface complète | 2 | Critical |
| SEC-010 | Créer AnomalieRepository.cs | Implémentation queries anomalies | Repository fonctionnel | 2 | Critical |

### 6.3 Securite Module - Service Layer (Semaine 10-11)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| SEC-011 | Créer IScanService interface | ValidateScanAsync, ProcessScanAsync, GetScanHistoryAsync | Interface complète | 2 | Critical |
| SEC-012 | Créer ScanService.cs | Logique validation scan QR | Service fonctionnel | 4 | Critical |
| SEC-013 | Implémenter ValidateScanAsync | Vérifier QR valide, non expiré, barrière correcte | Validation complète | 4 | Critical |
| SEC-014 | Implémenter vérification itinéraire | Comparer barrière scannée vs itinéraire prévu | Contrôle ordre passage | 3 | Critical |
| SEC-015 | Implémenter règle unicité scan | QR = 1 scan confirmé par barrière max | Unicité respectée | 2 | Critical |
| SEC-016 | Implémenter détection anomalies | Barrière hors itinéraire, doc expiré, doc inexistant | Anomalies détectées | 3 | Critical |
| SEC-017 | Implémenter génération anomalie | Créer Anomalie + notification Investigation | Anomalie créée + email | 2 | Critical |
| SEC-018 | Implémenter preuve passage | Génération reçu après scan conforme | Reçu généré | 2 | High |
| SEC-019 | Créer IAnomalieService interface | CreateAsync, MarkAsTraiteeAsync, GetDashboardAsync | Interface complète | 2 | Critical |
| SEC-020 | Créer AnomalieService.cs | Gestion anomalies Investigation | Service fonctionnel | 3 | Critical |

### 6.4 Securite Module - QR Code (Semaine 11)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| SEC-021 | Implémenter génération QR haché | Numéro unique encapsulé haché (invisible en clair) | QR sécurisé généré | 3 | Critical |
| SEC-022 | Implémenter validation QR haché | Décodage et vérification hash | Validation fonctionne | 3 | Critical |
| SEC-023 | Implémenter QR unique non-reproductible | Hash avec sel unique par bon | Sécurité renforcée | 2 | Critical |
| SEC-024 | Créer QRCodeDisplay.razor | Composant affichage QR dans pages | QR affiché | 2 | High |

### 6.5 Securite Module - Pages Scan (Semaine 11)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| SEC-025 | Créer ScanQRCode.razor | Page scan avec camera/input QR | Page fonctionnelle | 5 | Critical |
| SEC-026 | Implémenter lecture QR (camera) | Accès caméra pour scan | Camera fonctionne | 3 | Critical |
| SEC-027 | Implémenter lecture QR (manuel) | Saisie manuelle code si besoin | Saisie fonctionne | 2 | Critical |
| SEC-028 | Implémenter affichage résultat scan | Nom, Fonction, Département, Provenance, Destination | Infos affichées | 2 | Critical |
| SEC-029 | Implémenter popup confirmation | Bouton confirmer le passage | Confirmation fonctionne | 2 | Critical |
| SEC-030 | Implémenter affichage erreur | Message "Document Anormal - Accès Impossible" | Erreur affichée | 2 | Critical |
| SEC-031 | Implémenter impression reçu | Après confirmation, impression preuve passage | Reçu imprimé | 2 | High |
| SEC-032 | Créer HistoriqueScans.razor | Liste tous scans pour Identification | Liste fonctionnelle | 3 | High |

### 6.6 Securite Module - Pages Anomalies (Semaine 11-12)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| SEC-033 | Créer AnomaliesList.razor | Liste anomalies pour Investigation | Liste fonctionnelle | 3 | Critical |
| SEC-034 | Implémenter filtres anomalies | Par type, date, statut traité | Filtres fonctionnent | 2 | High |
| SEC-035 | Créer AnomalieDetail.razor | Détail anomalie avec bon associé | Détail affiché | 2 | Critical |
| SEC-036 | Implémenter traitement anomalie | Marquer comme traitée avec commentaire | Traitement fonctionne | 2 | Critical |
| SEC-037 | Créer AnomaliesDashboard.razor | Tableau de bord anomalies | Dashboard affiché | 3 | High |

### 6.7 Securite Module - Pages Admin (Semaine 12)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| SEC-038 | Créer Admin/Barrieres.razor | CRUD barrières | Gestion fonctionne | 3 | High |
| SEC-039 | Créer Admin/ItinerairesConfig.razor | Configuration itinéraires par provenance/destination | Config fonctionne | 3 | High |
| SEC-040 | Créer Admin/AgentsBarriere.razor | Affectation agents aux barrières | Affectation fonctionne | 2 | High |

### 6.8 Securite Module - Notifications (Semaine 12)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| SEC-041 | Créer template AnomalieDetectee.cshtml | Email template anomalie | Template fonctionne | 2 | High |
| SEC-042 | Implémenter envoi auto Investigation | Email à Investigation lors anomalie | Email envoyé | 2 | High |
| SEC-043 | Implémenter copie Identification | CC Identification sur emails anomalie | CC fonctionne | 1 | High |
| SEC-044 | Créer template ScanConforme.cshtml | Email confirmation passage (optionnel) | Template fonctionne | 1 | Medium |
| SEC-045 | Créer template PretExpirant.cshtml | Email alerte prêt J-7 | Template fonctionne | 2 | High |

---

## 7. Phase 4: Intégration & Polish (Semaines 13-14)

### 7.1 Cross-Module Integration (Semaine 13)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| INT-001 | Créer MesBons.razor | Tous bons de l'utilisateur (Entrée + Sortie) | Liste unifiée | 4 | Critical |
| INT-002 | Créer MesApprobations.razor | Bons en attente approbation tous modules | Liste unifiée | 4 | Critical |
| INT-003 | Implémenter recherche unifiée | Recherche globale depuis home | Recherche fonctionne | 3 | High |
| INT-004 | Mettre à jour Home.razor | Métriques par module, activité récente | Dashboard complet | 3 | High |
| INT-005 | Implémenter navigation cross-module | Liens entre BonSortie ↔ BonEntree | Navigation fluide | 2 | High |
| INT-006 | Vérifier liaison Entrée-Sortie complète | Archivage bon entrée quand sortie complétée | Archivage auto | 2 | Critical |

### 7.2 Reporting & Dashboards (Semaine 13)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| INT-007 | Créer Reports/Transactions.razor | Rapport toutes transactions | Rapport fonctionnel | 3 | High |
| INT-008 | Créer Reports/Historique.razor | Historique par bon avec timeline | Historique affiché | 3 | High |
| INT-009 | Créer Reports/Histogramme.razor | Histogramme flux par période | Graphique affiché | 3 | High |
| INT-010 | Créer Reports/AnomaliesStats.razor | Statistiques anomalies | Stats affichées | 2 | High |
| INT-011 | Créer Reports/PretsEnCours.razor | Bons de prêt actifs | Liste prêts | 2 | High |
| INT-012 | Implémenter export PDF rapports | Export PDF des rapports | PDF généré | 3 | Medium |
| INT-013 | Implémenter export Excel rapports | Export Excel des rapports | Excel généré | 2 | Medium |

### 7.3 Admin Module (Semaine 13)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| INT-014 | Créer Admin/Utilisateurs.razor | CRUD utilisateurs avec rôles | Gestion fonctionne | 3 | Critical |
| INT-015 | Créer Admin/Departements.razor | CRUD départements | Gestion fonctionne | 2 | High |
| INT-016 | Créer Admin/Roles.razor | CRUD rôles et permissions | Gestion fonctionne | 3 | High |
| INT-017 | Créer Admin/Statuts.razor | CRUD statuts avec couleurs | Gestion fonctionne | 2 | High |
| INT-018 | Créer Admin/TypesMateriel.razor | CRUD types matériel avec workflows | Gestion fonctionne | 2 | High |
| INT-019 | Créer Admin/Parametres.razor | Paramètres système (durées, emails) | Gestion fonctionne | 2 | High |
| INT-020 | Créer Admin/AuditLog.razor | Visualisation logs audit | Logs affichés | 2 | Medium |

### 7.4 Performance & Optimization (Semaine 13)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| INT-021 | Analyser queries SQL Profiler | Identifier requêtes lentes | Requêtes identifiées | 2 | High |
| INT-022 | Créer indexes base de données | Index sur colonnes recherchées | Indexes créés | 2 | High |
| INT-023 | Optimiser queries listes | Projection DTO dans queries | Queries optimisées | 2 | High |
| INT-024 | Implémenter compiled queries | EF Core compiled queries | Queries précompilées | 2 | Medium |
| INT-025 | Tester temps chargement pages | Toutes pages < 2 secondes | Performance OK | 2 | Critical |
| INT-026 | Load test 50 utilisateurs | Test charge concurrente | Système stable | 2 | High |

### 7.5 Security Review (Semaine 14)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| INT-027 | Vérifier Windows Auth | Auth fonctionne tous scénarios | Auth OK | 2 | Critical |
| INT-028 | Vérifier autorisations par rôle | Accès restreint selon rôle | Authz OK | 2 | Critical |
| INT-029 | Vérifier validation inputs | Validation server-side complète | Pas injection | 2 | Critical |
| INT-030 | Vérifier sécurité fichiers | Types et tailles fichiers contrôlés | Upload sécurisé | 2 | Critical |
| INT-031 | Vérifier protection SQL injection | Requêtes paramétrées partout | Pas SQL injection | 2 | Critical |
| INT-032 | Vérifier sécurité QR Code | Hash non réversible, non prédictible | QR sécurisé | 2 | Critical |

### 7.6 Testing (Semaine 14)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| INT-033 | Créer tests unitaires Core | Coverage > 80% | Tests passent | 3 | High |
| INT-034 | Créer tests unitaires BonEntree | Coverage > 80% | Tests passent | 3 | High |
| INT-035 | Créer tests unitaires BonSortie | Coverage > 80% | Tests passent | 3 | High |
| INT-036 | Créer tests unitaires Securite | Coverage > 80% | Tests passent | 3 | High |
| INT-037 | Créer tests intégration workflow | Test E2E création → approbation → scan | Tests passent | 4 | Critical |
| INT-038 | UAT Module Bon Entrée | Tests utilisateurs réels | Sign-off reçu | 2 | Critical |
| INT-039 | UAT Module Bon Sortie | Tests utilisateurs réels | Sign-off reçu | 2 | Critical |
| INT-040 | UAT Module Sécurité | Tests agents barrière réels | Sign-off reçu | 2 | Critical |

### 7.7 Documentation (Semaine 14)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| INT-041 | Créer README.md | Overview projet, setup instructions | README complet | 2 | High |
| INT-042 | Créer ARCHITECTURE.md | Documentation technique architecture | Doc complète | 2 | High |
| INT-043 | Créer DEPLOYMENT.md | Guide déploiement IIS | Guide complet | 2 | High |
| INT-044 | Créer USER-GUIDE.md | Manuel utilisateur | Guide complet | 3 | High |
| INT-045 | Documenter APIs services | XML documentation publiques | Comments complets | 2 | Medium |

### 7.8 Deployment (Semaine 14)

| ID | Tâche | Description | Critères d'Acceptation | SP | Priorité |
|----|-------|-------------|------------------------|-----|----------|
| INT-046 | Créer scripts déploiement | PowerShell scripts IIS | Scripts fonctionnent | 2 | Critical |
| INT-047 | Créer appsettings.Production.json | Configuration production | Config prête | 1 | Critical |
| INT-048 | Créer endpoint health check | /health pour monitoring | Endpoint répond 200 | 1 | High |
| INT-049 | Déployer staging | Déploiement environnement test | Déploiement OK | 2 | Critical |
| INT-050 | Tester staging complet | Tests fonctionnels staging | Tous tests passent | 2 | Critical |
| INT-051 | Créer procédure rollback | Documentation et test rollback | Rollback documenté | 2 | Critical |
| INT-052 | Créer checklist go-live | Liste vérifications finale | Checklist complète | 1 | Critical |
| INT-053 | Déployer production | Mise en production | Système live | 3 | Critical |

---

## 8. Annexes

### 8.1 Référence Story Points

| Story Points | Effort Estimé | Exemples Tâches |
|--------------|---------------|-----------------|
| 1 | 2-4 heures | Entity simple, config, enum |
| 2 | 4-8 heures (1 jour) | Entity avec config, service simple, composant basique |
| 3 | 1-2 jours | Repository complet, composant complexe, service avec logique |
| 4 | 2-3 jours | Service workflow complet, page complexe |
| 5 | 3-4 jours | Page majeure avec multiples features |

### 8.2 Définitions Priorité

| Priorité | Définition | Impact Planning |
|----------|------------|-----------------|
| Critical | Obligatoire pour milestone phase | Bloque la phase |
| High | Important pour fonctionnalité | Devrait être dans la phase |
| Medium | Améliore qualité/UX | Peut être différé si nécessaire |
| Low | Nice to have | Peut passer à sprint futur |

### 8.3 Dépendances Clés

```
Phase 0 (Foundation)
    └── Phase 1 (Bon Entrée)
            └── Phase 2 (Bon Sortie) [nécessite BonEntree pour Externe]
                    └── Phase 3 (Sécurité) [nécessite les deux types de bons]
                            └── Phase 4 (Intégration)
```

### 8.4 Risques Identifiés

| Risque | Impact | Mitigation |
|--------|--------|------------|
| Complexité workflow dynamique | High | Prototyper tôt en Phase 0 |
| Intégration scanner QR | Medium | Prévoir alternative saisie manuelle |
| Performance scan temps réel | High | Architecture optimisée, tests charge |
| Sécurité QR Code | Critical | Hash robuste, tests sécurité |
| Adoption utilisateurs barrière | Medium | Formation, UX simple |

### 8.5 Métriques de Succès

| Métrique | Cible |
|----------|-------|
| Temps création bon | < 3 minutes |
| Temps scan barrière | < 5 secondes |
| Temps chargement pages | < 2 secondes |
| Disponibilité système | > 99.5% |
| Taux anomalies faux positifs | < 1% |
| Satisfaction utilisateurs | > 80% |

---

**Document généré le 27 janvier 2026**  
**Projet: KCCMaterialFlow - Système de Gestion Automatisée des Bons d'Entrée et de Sortie**
