# AUDIT DÉTAILLÉ — Migration HTML Brut → Composants Radzen Blazor

> **Date** : Juin 2025  
> **Périmètre** : 17 fichiers `.razor` clés (formulaires, vues, dialogues, composants partagés)  
> **Objectif** : Identifier chaque élément HTML brut avec sa ligne exacte et la migration Radzen correspondante

---

## SYNTHÈSE EXÉCUTIVE

| Catégorie | Fichiers | Nb éléments à migrer |
|-----------|----------|---------------------|
| **✅ 100% Radzen** (aucun changement) | 6 fichiers | 0 |
| **⚠️ MIX** (Radzen + HTML brut) | 4 fichiers | ~35 |
| **🔴 Majoritairement HTML brut** | 7 fichiers | ~155+ |
| **TOTAL** | **17 fichiers** | **~190 éléments** |

---

## CATÉGORIE A — FICHIERS 100% RADZEN (Aucun changement nécessaire)

### A1. ApprobationDialog.razor (BonEntree)

**Chemin** : `Modules/BonEntree/.../Pages/ApprobationDialog.razor`  
**Composants** : `RadzenStack`, `RadzenText`, `RadzenFormField`, `RadzenTextArea`, `RadzenAlert`, `RadzenButton`  
**Verdict : ✅ Rien à faire**

### A2. ApprovalDialog.razor (BonSortie)

**Chemin** : `Modules/BonSortie/.../Pages/ApprovalDialog.razor`  
**Composants** : `RadzenStack`, `RadzenText`, `RadzenLabel`, `RadzenTextArea`, `RadzenButton`  
**Verdict : ✅ Rien à faire**

### A3. MaterielDialog.razor (BonSortie)

**Chemin** : `Modules/BonSortie/.../Pages/MaterielDialog.razor`  
**Composants** : `RadzenStack`, `RadzenLabel`, `RadzenTextBox`, `RadzenNumeric`, `RadzenButton`  
**Verdict : ✅ Rien à faire**

### A4. PretsList.razor (BonSortie)

**Chemin** : `Modules/BonSortie/.../Pages/PretsList.razor`  
**Composants** : `RadzenStack`, `RadzenRow`, `RadzenColumn`, `RadzenCard`, `RadzenText`, `RadzenDropDown`, `RadzenTextBox`, `RadzenButton`, `RadzenDataGrid`, `RadzenBadge`, `RadzenLink`, `RadzenIcon`  
**Verdict : ✅ Rien à faire**

### A5. ReturnLoanDialog.razor (BonSortie)

**Chemin** : `Modules/BonSortie/.../Pages/ReturnLoanDialog.razor`  
**Composants** : `RadzenStack`, `RadzenText`, `RadzenLabel`, `RadzenDropDown`, `RadzenTextArea`, `RadzenButton`  
**Verdict : ✅ Rien à faire**

### A6. WorkflowButtons.razor (Shared)

**Chemin** : `Modules/KCCMaterialFlow.Module.Shared/Components/Workflow/WorkflowButtons.razor`  
**Composants** : `RadzenButton` pour toutes les actions  
**Verdict : ✅ Rien à faire** (wrapper `<div>` acceptable pour layout)

---

## CATÉGORIE B — FICHIERS MIXTES (Radzen + HTML brut)

### B1. BonSortieList.razor ⚠️

**Chemin** : `Modules/BonSortie/.../Pages/BonSortieList.razor`  
**Composants Radzen déjà utilisés** : `RadzenDataGrid`, `RadzenIcon`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L16 | `<button class="btn-primary">` | `<RadzenButton ButtonStyle="ButtonStyle.Primary">` |
| L57-58 | `<input type="text" class="search-input">` | `<RadzenTextBox>` |
| L60-66 | `<select class="filter-select" @bind="filter.Statut">` (6 options) | `<RadzenDropDown>` |
| L68-73 | `<select class="filter-select" @bind="filter.TypeSortie">` (4 options) | `<RadzenDropDown>` |
| L74-76 | `<button class="btn-filter">` Filtrer | `<RadzenButton>` |
| L77-78 | `<button class="btn-filter">` Reset | `<RadzenButton>` |
| L94 | `<span class="badge badge-blue">` (type) | `<RadzenBadge>` |
| L104 | `<span class="badge @GetStatutClass(...)">` (statut) | `<RadzenBadge>` |
| L109 | `<button class="btn-ghost">` (voir) | `<RadzenButton ButtonStyle="ButtonStyle.Light">` |

**Total : 9 éléments à migrer**

---

### B2. CommentModal.razor ⚠️

**Chemin** : `Modules/KCCMaterialFlow.Module.Shared/Components/Workflow/CommentModal.razor`  
**Composants Radzen déjà utilisés** : `RadzenDialog`, `RadzenIcon`, `RadzenTextArea`, `RadzenButton`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L9 | `<h5 class="mb-3">` | `<RadzenText TextStyle="TextStyle.H5">` |
| L16 | `<p class="text-muted mb-3">` | `<RadzenText TextStyle="TextStyle.Body2">` |
| L20-24 | `<label class="form-label">` + `<span>*</span>` | `<RadzenFormField>` ou `<RadzenLabel>` |
| L32 | `<small class="text-muted">` | `<RadzenText TextStyle="TextStyle.Caption">` |
| L35-38 | `<div class="text-danger small">` (erreur validation) | `<RadzenAlert AlertStyle="AlertStyle.Danger" Size="AlertSize.Small">` |

**Total : 5 éléments à migrer**

---

### B3. ApprovalHistory.razor ⚠️

**Chemin** : `Modules/KCCMaterialFlow.Module.Shared/Components/Workflow/ApprovalHistory.razor`  
**Composants Radzen déjà utilisés** : `RadzenIcon`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L11 | `<h6 class="mb-3">` | `<RadzenText TextStyle="TextStyle.H6">` |
| L18 | `<p class="mt-2 mb-0">` (message vide) | `<RadzenText TextStyle="TextStyle.Body2">` |
| L31 | `<strong>` texte décision | `<RadzenText Style="font-weight:bold">` |
| L35 | `<small class="text-muted">` date | `<RadzenText TextStyle="TextStyle.Caption">` |
| L40 | `<p class="...">` commentaire | `<RadzenText TextStyle="TextStyle.Body2">` |
| L45-50 | `<style>` bloc CSS inline | Extraire vers fichier `.razor.css` |

**Total : 6 éléments à migrer**

---

### B4. StatusBadge.razor ⚠️ (IMPACT SYSTÉMIQUE)

**Chemin** : `Modules/KCCMaterialFlow.Module.Shared/Components/Base/StatusBadge.razor`  
**Composants Radzen déjà utilisés** : `RadzenIcon`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L8 | `<span class="badge @GetBadgeClass()">` composant entier | `<RadzenBadge BadgeStyle="..." Text="..." IsPill="true">` |

**Total : 1 élément** — mais impact sur **tous les fichiers** utilisant `<StatusBadge>`

---

### B5. PageHeader.razor ⚠️ (IMPACT SYSTÉMIQUE)

**Chemin** : `Modules/KCCMaterialFlow.Module.Shared/Components/Base/PageHeader.razor`  
**Composants Radzen déjà utilisés** : `RadzenIcon`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L11-25 | `<nav>` + `<ol class="breadcrumb">` + `<li>` + `<a href>` | `<RadzenBreadCrumb>` + `<RadzenBreadCrumbItem>` |
| L29 | `<h2 class="mb-0">` | `<RadzenText TextStyle="TextStyle.H2">` |
| L37 | `<small class="text-muted ms-2">` | `<RadzenText TextStyle="TextStyle.Caption">` |

**Total : 3 éléments** — mais impact sur **toutes les pages** utilisant `<PageHeader>`

---

## CATÉGORIE C — MAJORITAIREMENT HTML BRUT (Migration lourde)

### C1. BonEntreeNew.razor 🔴🔴 ÉLEVÉ

**Chemin** : `Modules/BonEntree/.../Pages/BonEntreeNew.razor`  
**Composants Radzen déjà utilisés** : `RadzenDatePicker`, `RadzenTextBox` (×3), `RadzenDropDown` (×4), `RadzenIcon`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L12 | `<EditForm Model="@request">` | `<RadzenTemplateForm>` |
| L45-88 | `<table class="info-table">` (table layout infos compagnie) | `<RadzenRow>`/`<RadzenColumn>` avec `<RadzenFormField>` |
| L87 | `<textarea @bind="@request.ReasonOnSite">` | `<RadzenTextArea>` |
| L94-112 | `<table class="escorteur-table">` (table layout escorteur) | `<RadzenRow>`/`<RadzenColumn>` |
| L100 | `<input type="text" @bind="@request.NomEscorteur">` | `<RadzenTextBox>` |
| L108 | `<input type="text" @bind="@request.FonctionEscorteur">` | `<RadzenTextBox>` |
| L114-185 | `<table class="materiels-table">` (table données matériels) | `<RadzenDataGrid>` avec colonnes éditables |
| L137 | `<input type="text" @bind="item.CodeProduitSerial">` | `<RadzenTextBox>` (dans template colonne) |
| L140 | `<input type="text" @bind="item.Designation">` | `<RadzenTextBox>` (dans template colonne) |
| L143 | `<input type="number" @bind="item.Quantite" min="1">` | `<RadzenNumeric Min="1">` (dans template colonne) |
| L160-161 | `<button class="btn-row" @onclick="RemoveMateriel">×` | `<RadzenButton Icon="close" Size="ButtonSize.Small">` |
| L184 | `<button class="btn-row add" @onclick="AddMateriel">+` | `<RadzenButton Icon="add" Size="ButtonSize.Small">` |
| L193 | `<textarea @bind="@request.Description">` | `<RadzenTextArea>` |
| L197-216 | `<table class="signatures-table">` (affichage seul) | `<RadzenRow>`/`<RadzenCard>` ou `<RadzenDataGrid ReadOnly>` |
| L250 | `<button class="btn-action btn-back">` Retour | `<RadzenButton Icon="arrow_back">` |
| L256-258 | `<button type="submit" class="btn-action btn-draft">` | `<RadzenButton ButtonType="ButtonType.Submit">` |
| L261-263 | `<button class="btn-action btn-submit">` Soumettre | `<RadzenButton ButtonStyle="ButtonStyle.Success">` |

**Total : 17 éléments à migrer** (dont 3 `<table>` complètes)

---

### C2. BonEntreeEdit.razor 🔴🔴 ÉLEVÉ

**Chemin** : `Modules/BonEntree/.../Pages/BonEntreeEdit.razor`  
**Composants Radzen déjà utilisés** : `RadzenProgressBarCircular`, `RadzenIcon`, `RadzenDatePicker`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L25 | `<button class="btn-cancel">` Retour (erreur 404) | `<RadzenButton>` |
| L34 | `<button class="btn-cancel">` Retour (non modifiable) | `<RadzenButton>` |
| L42-43 | `<button class="btn-back">` navigation retour | `<RadzenButton Icon="arrow_back">` |
| L47 | `<span class="status-badge status-draft">Brouillon` | `<RadzenBadge Text="Brouillon" BadgeStyle="BadgeStyle.Light">` |
| L58 | `<EditForm Model="@request">` | `<RadzenTemplateForm>` |
| L69 + L70 | `<label>` + `<input type="text">` (Nom Compagnie) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L73 + L74 | `<label>` + `<input type="text">` (N° Contrat) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L77 + L78 | `<label>` + `<input type="email">` (Email) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L81 + L82 | `<label>` + `<input type="text">` (Site Manager) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L85 + L86 | `<label>` + `<input type="text">` (Département) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L89 + L90 | `<label>` + `<textarea>` (Motif sur site) | `<RadzenFormField>` + `<RadzenTextArea>` |
| L104 + L105 | `<label>` + `<input type="text">` (Nom Escorteur) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L108 + L109 | `<label>` + `<input type="text">` (Fonction) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L129 | `<input type="text">` (Provenance) | `<RadzenTextBox>` ou `<RadzenDropDown>` |
| L134 | `<input type="text">` (Destination) | `<RadzenTextBox>` ou `<RadzenDropDown>` |
| L140-141 | `<textarea>` (Description additionnelle) | `<RadzenTextArea>` |
| L154-181 | `<table class="materiels-table">` | `<RadzenDataGrid>` avec colonnes éditables |
| L168 | `<input type="text">` (Désignation matériel) | `<RadzenTextBox>` (dans template) |
| L171 | `<input type="text">` (Code/Série matériel) | `<RadzenTextBox>` (dans template) |
| L174 | `<input type="number">` (Quantité) | `<RadzenNumeric>` (dans template) |
| L177 | `<button class="btn-remove">` (supprimer ligne) | `<RadzenButton Icon="delete">` |
| L194 | `<button class="btn-add-materiel">` | `<RadzenButton Icon="add">` |
| L204 | `<button class="btn-cancel">` Annuler | `<RadzenButton ButtonStyle="ButtonStyle.Light">` |
| L207-210 | `<button type="submit" class="btn-save">` | `<RadzenButton ButtonType="ButtonType.Submit">` |

**Total : 24 éléments à migrer** (dont 8 paires label+input, 1 `<table>`, 1 `<EditForm>`)

---

### C3. BonEntreeView.razor 🟠 MOYEN

**Chemin** : `Modules/BonEntree/.../Pages/BonEntreeView.razor` (578 lignes)  
**Composants Radzen déjà utilisés** : `RadzenProgressBarCircular`, `RadzenIcon`, `DialogService`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L31 | `<button class="btn btn-primary">← Retour` (accès refusé) | `<RadzenButton>` |
| L41 | `<button class="btn btn-primary">← Retour` (non trouvé) | `<RadzenButton>` |
| L72 | `<span class="paper-badge @GetStatusBadgeClass()">` (statut) | `<RadzenBadge>` |
| L73 | `<span class="paper-badge badge-entree">Entrée` | `<RadzenBadge Text="Entrée">` |
| L114-170 | `<table class="materials-table">` (tableau matériels lecture seule) | `<RadzenDataGrid>` |
| L142 | `<span style="color: #6b7280;">Épuisé` | `<RadzenBadge BadgeStyle="BadgeStyle.Light">` |
| L146 | `<span style="color: #f59e0b;">Partiel` | `<RadzenBadge BadgeStyle="BadgeStyle.Warning">` |
| L150 | `<span style="color: #059669;">Disponible` | `<RadzenBadge BadgeStyle="BadgeStyle.Success">` |
| L232-233 | `<button class="btn btn-danger">` Voir rejet | `<RadzenButton ButtonStyle="ButtonStyle.Danger">` |
| L241-242 | `<button class="btn btn-back">` Retour | `<RadzenButton>` |
| L248-249 | `<button class="btn btn-outline">` Modifier | `<RadzenButton ButtonStyle="ButtonStyle.Secondary">` |
| L251-252 | `<button class="btn btn-danger">` Supprimer | `<RadzenButton ButtonStyle="ButtonStyle.Danger">` |
| L254-255 | `<button class="btn btn-primary">` Soumettre | `<RadzenButton ButtonStyle="ButtonStyle.Primary">` |
| L260-261 | `<button class="btn btn-success">` Approuver | `<RadzenButton ButtonStyle="ButtonStyle.Success">` |
| L263-264 | `<button class="btn btn-warning">` Retourner | `<RadzenButton ButtonStyle="ButtonStyle.Warning">` |
| L266-267 | `<button class="btn btn-danger">` Rejeter | `<RadzenButton ButtonStyle="ButtonStyle.Danger">` |
| L272-273 | `<button class="btn btn-success">` Créer BSE | `<RadzenButton ButtonStyle="ButtonStyle.Success">` |
| L278-279 | `<button class="btn btn-primary">` Imprimer | `<RadzenButton ButtonStyle="ButtonStyle.Primary">` |

**Total : 18 éléments à migrer** (dont 1 `<table>`, 12 `<button>`, 4 `<span>` badges)

---

### C4. BonSortieNew.razor 🔴🔴🔴 CRITIQUE (le plus gros chantier)

**Chemin** : `Modules/BonSortie/.../Pages/BonSortieNew.razor` (729 lignes)  
**Composants Radzen déjà utilisés** : `RadzenProgressBar` (loading uniquement)

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L55 | `<input type="text" class="paper-row-input">` (Nom) | `<RadzenTextBox>` |
| L61 | `<input type="text" class="paper-row-input">` (Fonction) | `<RadzenTextBox>` |
| L67 | `<input type="text" class="paper-row-input">` (Département) | `<RadzenTextBox>` |
| L74-82 | `<select class="bem-dropdown">` + options dynamiques | `<RadzenDropDown>` |
| L88-89 | `<button class="btn-select-materials">` | `<RadzenButton>` |
| L95-140 | `<table class="materials-table">` | `<RadzenDataGrid>` avec colonnes éditables |
| L111 | `<input type="number">` (Quantité à sortir) | `<RadzenNumeric>` |
| L118-123 | `<select @bind="mat.SiteFromId">` (FROM) | `<RadzenDropDown>` |
| L126-131 | `<select @bind="mat.SiteToId">` (TO) | `<RadzenDropDown>` |
| L159 | `<input type="checkbox" id="fin_chantier">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L163 | `<input type="checkbox" id="fin_contrat">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L167 | `<input type="checkbox" id="mat_usage">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L171 | `<input type="checkbox" id="mat_declasse">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L178 | `<input type="checkbox" id="residu">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L182 | `<input type="checkbox" id="dechet">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L186 | `<input type="checkbox" id="emballage">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L190 | `<input type="checkbox" id="mat_desaffecte">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L197 | `<input type="checkbox" id="radio_protection">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L201 | `<input type="checkbox" id="troxilaire">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L208 | `<input type="checkbox" id="modification">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L212 | `<input type="checkbox" id="reparation">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L216 | `<input type="checkbox" id="renovation">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L228 | `<input type="checkbox" id="informatique">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L235 | `<input type="checkbox" id="circulaire">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L242 | `<input type="checkbox" id="prise_pret">` + `<label>` | `<RadzenCheckBox>` + `<RadzenLabel>` |
| L259 | `<input type="checkbox">` (checkpoints dynamiques) | `<RadzenCheckBox>` |
| L323 | `<button class="btn-back">` ← Retour | `<RadzenButton>` |
| L327 | `<button class="btn-draft">` Enregistrer brouillon | `<RadzenButton>` |
| L330-331 | `<button class="btn-submit">` Soumettre | `<RadzenButton ButtonStyle="ButtonStyle.Primary">` |
| **L342-375** | **DIV modal manuelle** (`modal-overlay`/`modal-content`/`modal-header`/`modal-body`/`modal-footer`) | **`DialogService.OpenAsync<>()` avec composant dédié** |
| L346 | `<button>` fermer modal (×) | Supprimé (géré par DialogService) |
| L354 | `<input type="checkbox">` dans modal | `<RadzenCheckBox>` |
| L361 | `<input type="number">` dans modal | `<RadzenNumeric>` |
| L372 | `<button class="btn-back">` Annuler modal | `<RadzenButton>` |
| L373 | `<button class="btn-submit">` Confirmer modal | `<RadzenButton>` |

**Total : 35 éléments à migrer** (dont 16 checkboxes, 1 `<table>`, 3 `<select>`, 1 modal DIV complète, 5 `<button>`)

---

### C5. BonSortieEdit.razor 🔴🔴 ÉLEVÉ

**Chemin** : `Modules/BonSortie/.../Pages/BonSortieEdit.razor` (556 lignes)  
**Composants Radzen déjà utilisés** : `RadzenProgressBarCircular`, `RadzenIcon`, `RadzenButton` (état erreur uniquement), `RadzenDropDown`, `RadzenDatePicker`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L47-49 | `<button class="back-btn">` | `<RadzenButton Icon="arrow_back">` |
| L52 | `<span class="status-badge ...">` | `<RadzenBadge>` |
| L63 | `<EditForm Model="@model">` | `<RadzenTemplateForm>` |
| L74 + L75 | `<label>` + `<input type="text" readonly>` (Demandeur) | `<RadzenFormField>` + `<RadzenTextBox ReadOnly="true">` |
| L78 + L79 | `<label>` + `<input type="text">` (Fonction) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L82 + L83 | `<label>` + `<input type="text">` (Département) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L97 | `<label class="form-label">` (Type de Matériel) | `<RadzenFormField>` (déjà RadzenDropDown pour la valeur) |
| L103 | `<span class="raison-badge ...">` | `<RadzenBadge>` |
| L137 + L138 | `<label>` + `<input type="text">` (Description) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L143 + L144 | `<label>` + `<input type="text">` (Destinataire) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L147 + L148 | `<label>` + `<input type="text">` (Adresse) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L166 + L167 | `<label>` + `<input type="text">` (N° Véhicule) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L170 + L171 | `<label>` + `<input type="text">` (Chauffeur) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L174 + L175 | `<label>` + `<input type="text">` (Tél. chauffeur) | `<RadzenFormField>` + `<RadzenTextBox>` |
| L192 | `<label>` (Date Retour) | `<RadzenFormField>` (déjà RadzenDatePicker pour la valeur) |
| L196 | `<label>` (Date Expiration) | `<RadzenFormField>` (déjà RadzenDatePicker pour la valeur) |
| L211 | `<label>` (Motif) | `<RadzenFormField>` |
| L212 | `<textarea class="form-input form-textarea">` | `<RadzenTextArea>` |
| L225-260 | `<table class="materiels-table">` | `<RadzenDataGrid>` avec colonnes éditables |
| L239 | `<input type="text">` (Désignation matériel) | `<RadzenTextBox>` (dans template colonne) |
| L242 | `<input type="text">` (Code/Série matériel) | `<RadzenTextBox>` (dans template colonne) |
| L245 | `<input type="number">` (Quantité matériel) | `<RadzenNumeric>` (dans template colonne) |
| L248 | `<button class="btn-remove">` (supprimer) | `<RadzenButton Icon="delete">` |
| L265 | `<button class="btn-add-materiel">` | `<RadzenButton Icon="add">` |
| L275 | `<button class="btn-cancel">` | `<RadzenButton ButtonStyle="ButtonStyle.Light">` |
| L278 | `<button type="submit" class="btn-save">` | `<RadzenButton ButtonType="ButtonType.Submit">` |

**Total : 26 éléments à migrer** (dont 14 paires label+input, 1 `<table>`, 1 `<EditForm>`)

---

### C6. BonSortieView.razor 🟠 MOYEN

**Chemin** : `Modules/BonSortie/.../Pages/BonSortieView.razor` (630 lignes)  
**Composants Radzen déjà utilisés** : `RadzenProgressBarCircular`, `RadzenIcon`, `DialogService`

| Ligne | Élément HTML brut | Migration vers |
|-------|-------------------|----------------|
| L31 | `<button class="btn btn-primary">← Retour` (accès refusé) | `<RadzenButton>` |
| L41 | `<button class="btn btn-primary">← Retour` (non trouvé) | `<RadzenButton>` |
| L72 | `<span class="paper-badge @GetStatusBadgeClass()">` | `<RadzenBadge>` |
| L73 | `<span class="paper-badge @GetTypeBadgeClass()">` | `<RadzenBadge>` |
| L121-160 | `<table class="materials-table">` | `<RadzenDataGrid>` |
| L226-227 | `<button class="btn btn-danger">` Voir rejet | `<RadzenButton ButtonStyle="ButtonStyle.Danger">` |
| L235-236 | `<button class="btn btn-back">` Retour | `<RadzenButton>` |
| L242-243 | `<button class="btn btn-outline">` Modifier | `<RadzenButton ButtonStyle="ButtonStyle.Secondary">` |
| L245-246 | `<button class="btn btn-danger">` Supprimer | `<RadzenButton ButtonStyle="ButtonStyle.Danger">` |
| L248-249 | `<button class="btn btn-primary">` Soumettre | `<RadzenButton ButtonStyle="ButtonStyle.Primary">` |
| L254-255 | `<button class="btn btn-success">` Approuver | `<RadzenButton ButtonStyle="ButtonStyle.Success">` |
| L257-258 | `<button class="btn btn-warning">` Retourner | `<RadzenButton ButtonStyle="ButtonStyle.Warning">` |
| L260-261 | `<button class="btn btn-danger">` Rejeter | `<RadzenButton ButtonStyle="ButtonStyle.Danger">` |
| L266-267 | `<button class="btn btn-outline">` Retour Prêt | `<RadzenButton>` |
| L272-273 | `<button class="btn btn-primary">` Imprimer | `<RadzenButton ButtonStyle="ButtonStyle.Primary">` |

**Total : 15 éléments à migrer** (dont 1 `<table>`, 11 `<button>`, 2 `<span>` badges)

---

## MATRICE DE PRIORITÉ DE MIGRATION

### P0 — Composants partagés (impact multiplié sur tout le projet)

| Fichier | Effort | Éléments | Impact |
|---------|--------|----------|--------|
| StatusBadge.razor | 10 min | 1 | Tous les fichiers utilisant `<StatusBadge>` |
| PageHeader.razor | 30 min | 3 | Toutes les pages avec en-tête |

### P1 — Formulaires de création (pages les plus utilisées)

| Fichier | Effort | Éléments |
|---------|--------|----------|
| **BonSortieNew.razor** | **4-5 heures** | **35** (modal DIV, 16 checkboxes, table, selects) |
| **BonEntreeNew.razor** | **2-3 heures** | **17** (tables layout + données, inputs, buttons) |

### P2 — Formulaires d'édition

| Fichier | Effort | Éléments |
|---------|--------|----------|
| **BonSortieEdit.razor** | **2-3 heures** | **26** (14 paires label+input, table, EditForm) |
| **BonEntreeEdit.razor** | **2-3 heures** | **24** (8 paires label+input, table, EditForm) |

### P3 — Pages de consultation / liste

| Fichier | Effort | Éléments |
|---------|--------|----------|
| BonEntreeView.razor | 1-2 heures | 18 (buttons, badges, table lecture seule) |
| BonSortieView.razor | 1-2 heures | 15 (buttons, badges, table lecture seule) |
| BonSortieList.razor | 30-45 min | 9 (filtres, buttons, badges) |

### P4 — Composants workflow (secondaires)

| Fichier | Effort | Éléments |
|---------|--------|----------|
| CommentModal.razor | 15 min | 5 |
| ApprovalHistory.razor | 20 min | 6 |

---

## RÉCAPITULATIF DES CONVERSIONS PAR TYPE

| Type HTML brut | Occurrences | Conversion Radzen |
|----------------|-------------|-------------------|
| `<button>` | **~45** | `<RadzenButton>` |
| `<input type="text">` | **~28** | `<RadzenTextBox>` |
| `<input type="number">` | **~6** | `<RadzenNumeric>` |
| `<input type="checkbox">` | **~18** | `<RadzenCheckBox>` |
| `<input type="email">` | **~1** | `<RadzenTextBox>` |
| `<textarea>` | **~6** | `<RadzenTextArea>` |
| `<select>` | **~7** | `<RadzenDropDown>` |
| `<table>` (données) | **~6** | `<RadzenDataGrid>` |
| `<table>` (layout) | **~3** | `<RadzenRow>` / `<RadzenColumn>` |
| `<EditForm>` | **~3** | `<RadzenTemplateForm>` |
| `<label>` + input | **~22 paires** | `<RadzenFormField>` |
| `<span class="badge">` | **~10** | `<RadzenBadge>` |
| DIV modal manuelle | **~1** | `DialogService.OpenAsync` |
| `<nav>` breadcrumb | **~1** | `<RadzenBreadCrumb>` |
| `<h2>`–`<h6>`, `<p>`, `<small>`, `<strong>` | **~10** | `<RadzenText>` |
| **TOTAL** | **~190** | |

---

## ESTIMATION D'EFFORT GLOBAL

| Phase | Fichiers | Effort estimé |
|-------|----------|---------------|
| P0 — Composants partagés | 2 | 0.5 jour |
| P1 — Formulaires création | 2 | 3-4 jours |
| P2 — Formulaires édition | 2 | 2-3 jours |
| P3 — Pages consultation/liste | 3 | 1.5-2 jours |
| P4 — Composants workflow | 2 | 0.5 jour |
| Tests & corrections | tous | 1-2 jours |
| **TOTAL** | **11 fichiers à migrer** | **~9-12 jours** |

---

## MODÈLE DE RÉFÉRENCE

Le module **Sécurité** est 100% Radzen et sert de modèle :
- **`ScanQRCode.razor`** → référence pour formulaire complet
- **`HistoriqueScans.razor`** → référence pour liste avec grille et filtres
- **`ScanDetailsDialog.razor`** → référence pour dialogue richement structuré
