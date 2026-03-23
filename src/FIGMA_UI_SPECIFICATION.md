# 🎨 KCCMaterialFlow - Spécifications UI pour Figma

**Document Version:** 1.0  
**Date:** 13 février 2026  
**Projet:** Système de Gestion des Bons d'Entrée et de Sortie Matériel  
**Client:** KCC (Kamoto Copper Company)

---

## 📋 Table des Matières

1. [Présentation du Projet](#1-présentation-du-projet)
2. [Identité Visuelle](#2-identité-visuelle)
3. [Rôles Utilisateurs](#3-rôles-utilisateurs)
4. [Module 1: Bon d'Entrée Matériel (BEM)](#4-module-1-bon-dentrée-matériel-bem)
5. [Module 2: Bon de Sortie Matériel (BSM)](#5-module-2-bon-de-sortie-matériel-bsm)
6. [Module 3: Sécurité & Barrières](#6-module-3-sécurité--barrières)
7. [Composants Transversaux](#7-composants-transversaux)
8. [Workflow d'Approbation](#8-workflow-dapprobation)
9. [Responsive Design](#9-responsive-design)
10. [Spécifications Techniques](#10-spécifications-techniques)

---

## 1. Présentation du Projet

### 1.1 Description Générale

**KCCMaterialFlow** est une application web de gestion des mouvements de matériels sur un site minier/industriel. Elle permet de :

- **Tracer les entrées** de matériels (équipements contractants, livraisons)
- **Tracer les sorties** de matériels (transferts internes, prêts, fin de chantier)
- **Contrôler les passages** aux barrières de sécurité via scan QR code
- **Gérer les approbations** multi-niveaux selon le type de matériel

### 1.2 Flux Principal

```
┌─────────────────────────────────────────────────────────────────────┐
│                         FLUX D'ENTRÉE (BEM)                         │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  CONTRACTANT    →    DEMANDEUR    →    SUPERVISEUR    →    GM      │
│  (arrive site)       (créé BEM)        (approuve)         (approuve)│
│                                                                     │
│       ↓                                                             │
│                                                                     │
│     OPJ        →    IDENTIFICATION    →    IMPRESSION    →  SCAN   │
│  (sécurité)         (vérifie/QR)          (QR Code)      (barrières)│
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                         FLUX DE SORTIE (BSM)                        │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  DEMANDEUR     →    SUPERVISEUR    →    GM    →    [IT/ENV]        │
│  (créé BSM)         (approuve)        (approuve)   (si requis)     │
│                                                                     │
│       ↓                                                             │
│                                                                     │
│     OPJ        →    IDENTIFICATION    →    IMPRESSION    →  SCAN   │
│  (sécurité)         (vérifie/QR)          (QR Code)      (barrières)│
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### 1.3 Entités Principales

| Entité | Description |
|--------|-------------|
| **BonEntree (BEM)** | Bon d'Entrée Matériel - Équipements entrant sur site |
| **BonSortie (BSM)** | Bon de Sortie Matériel - Équipements sortant du site |
| **Materiel** | Ligne de matériel (désignation, quantité, N° série) |
| **Approbation** | Action d'approbation/rejet par un valideur |
| **Checkpoint** | Barrière de sécurité (KTO, SKM, LUILU, etc.) |
| **PassageCheckpoint** | Enregistrement d'un scan à une barrière |

---

## 2. Identité Visuelle

### 2.1 Palette de Couleurs

| Couleur | Code HEX | Usage |
|---------|----------|-------|
| **KCC Green** | `#00B193` | Couleur principale, actions positives, succès |
| **KCC Green Dark** | `#009980` | Hover, accent |
| **KCC Green Light** | `#E6F7F4` | Backgrounds, sélections |
| **KCC Slate** | `#253C45` | Headers, texte principal, navigation |
| **KCC Slate Light** | `#3A5561` | Sous-titres, texte secondaire |
| **KCC Beige** | `#BB8748` | Accents, warnings, attention |
| **KCC Beige Light** | `#FEF3C7` | Highlights, trajets |
| **White** | `#FFFFFF` | Fond des cartes |
| **Gray 100** | `#F5F7FA` | Fond de page |
| **Gray 200** | `#E5E7EB` | Bordures, séparateurs |
| **Red** | `#DC2626` | Erreurs, rejets, suppressions |
| **Orange** | `#F59E0B` | Warnings, en attente |
| **Blue** | `#3B82F6` | Informations, liens |

### 2.2 Typographie

| Usage | Font | Taille | Poids |
|-------|------|--------|-------|
| **H1 - Titre page** | Inter/Roboto | 24px | 700 (Bold) |
| **H2 - Section** | Inter/Roboto | 18px | 600 (Semi-bold) |
| **H3 - Sous-section** | Inter/Roboto | 14px | 600 |
| **Body** | Inter/Roboto | 14px | 400 (Regular) |
| **Small** | Inter/Roboto | 12px | 400 |
| **Label** | Inter/Roboto | 12px | 600 |
| **Code/Numéro** | Fira Code/Mono | 13px | 500 |

### 2.3 Spacing & Layout

| Élément | Valeur |
|---------|--------|
| **Page padding** | 24px |
| **Card padding** | 16-24px |
| **Section gap** | 24px |
| **Element gap** | 16px |
| **Border radius (cards)** | 12px |
| **Border radius (buttons)** | 8px |
| **Border radius (inputs)** | 6px |

### 2.4 Ombres

```css
/* Card Shadow */
box-shadow: 0 2px 12px rgba(0,0,0,0.08);

/* Dropdown Shadow */
box-shadow: 0 4px 20px rgba(0,0,0,0.15);

/* Modal Shadow */
box-shadow: 0 8px 32px rgba(0,0,0,0.2);
```

---

## 3. Rôles Utilisateurs

### 3.1 Liste des Rôles

| Rôle | Code | Accès | Description |
|------|------|-------|-------------|
| **Demandeur** | DEMANDEUR | Création, suivi | Employé qui crée les demandes de bons |
| **Superviseur** | SUPERVISEUR | Approbation N1 | Chef d'équipe, première validation |
| **General Manager** | GM | Approbation N2 | Directeur, deuxième validation |
| **OPJ Sécurité** | OPJ | Approbation Sécu | Officier de Police Judiciaire |
| **Département IT** | IT | Approbation IT | Valide les matériels informatiques |
| **Environnement** | ENV | Approbation Env | Valide résidus, radioprotection |
| **Identification** | IDENTIFICATION | Vérification finale | Génère QR codes, extensions prêts |
| **Agent Barrière** | BARRIERE | Scan QR | Contrôle physique aux checkpoints |
| **Investigation** | INVESTIGATION | Anomalies | Traite les cas suspects |
| **Admin** | ADMIN | Tout | Administration système |

### 3.2 Écrans par Rôle

```
DEMANDEUR:
├── Dashboard personnel
├── Mes demandes (BEM/BSM)
├── Créer un bon (BEM/BSM)
├── Suivre mes bons
└── Historique

SUPERVISEUR / GM:
├── Dashboard approbations
├── Bons en attente
├── Approuver / Rejeter
└── Historique validations

OPJ:
├── Dashboard sécurité
├── Bons en attente validation
├── Alertes
└── Rapports

IDENTIFICATION:
├── Dashboard vérification
├── Bons à vérifier
├── Génération QR codes
├── Extensions prêts
└── Retours de prêts

BARRIERE:
├── Scanner QR Code (mobile-first)
├── Historique passages
├── Signaler anomalie
└── Liste bons autorisés

INVESTIGATION:
├── Dashboard anomalies
├── Cas ouverts
├── Traiter anomalie
└── Rapports

ADMIN:
├── Dashboard général
├── Gestion utilisateurs
├── Gestion sites/barrières
├── Configuration workflow
├── Audit logs
└── Rapports globaux
```

---

## 4. Module 1: Bon d'Entrée Matériel (BEM)

### 4.1 Liste des Bons d'Entrée

**Route:** `/bon-entree`

#### Layout
```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER (KCC Slate #253C45)                                          │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ 📥 Bons d'Entrée Matériel                    [+ Nouveau BEM]    │ │
│ └─────────────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│ STATISTIQUES (4 cartes en grille)                                  │
│ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐               │
│ │ Total    │ │ En cours │ │ Approuvés│ │ Rejetés  │               │
│ │   156    │ │    12    │ │   138    │ │    6     │               │
│ │ 📊       │ │ ⏳       │ │ ✅       │ │ ❌       │               │
│ └──────────┘ └──────────┘ └──────────┘ └──────────┘               │
│                                                                     │
│ FILTRES                                                            │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ 🔍 Rechercher...  │ Statut ▼ │ Compagnie ▼ │ Date ▼ │ Réinit.  │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ TABLEAU                                                            │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ N° Référence  │ Compagnie    │ Destination │ Date    │ Statut  │ │
│ ├───────────────┼──────────────┼─────────────┼─────────┼─────────┤ │
│ │ BEM-2026-001  │ SINOHYDRO    │ KTO → SKM   │ 13/02   │ ✅      │ │
│ │ BEM-2026-002  │ CHEC         │ LUILU → KOV │ 12/02   │ ⏳      │ │
│ │ BEM-2026-003  │ SNEL         │ KTO → KTC   │ 11/02   │ ❌      │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ PAGINATION                                                         │
│ ◄ 1 2 3 ... 10 ►                         Afficher: 10 │ 25 │ 50   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

#### Colonnes du tableau
| Colonne | Largeur | Description |
|---------|---------|-------------|
| N° Référence | 150px | BEM-YYYY-NNNNNN (lien cliquable) |
| Compagnie | 180px | Nom de la compagnie |
| Trajet | 150px | FROM → TO |
| Date création | 100px | JJ/MM/AAAA |
| Créé par | 150px | Nom du demandeur |
| Statut | 120px | Badge coloré |
| Actions | 100px | 👁️ Voir │ ✏️ Modifier │ 🖨️ Imprimer |

#### Badges de statut
| Statut | Couleur | Icône |
|--------|---------|-------|
| Brouillon | Gray | 📝 |
| En attente Sup | Orange | ⏳ |
| En attente GM | Orange | ⏳ |
| En attente OPJ | Orange | ⏳ |
| Approuvé | Green | ✅ |
| Rejeté | Red | ❌ |
| En transit | Blue | 🚛 |
| Complété | Green Dark | ✓✓ |

---

### 4.2 Création d'un BEM

**Route:** `/bon-entree/creer`

#### Layout du formulaire (1 page avec sections)

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER (KCC Slate)                                                  │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ 📥 Nouveau Bon d'Entrée                         13 février 2026 │ │
│ └─────────────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│ SECTION 1: INFORMATIONS COMPAGNIE                                  │
│ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━                                │
│ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐       │
│ │ N° Contrat      │ │ Nom Compagnie * │ │ Email Contract. │       │
│ │ [___________]   │ │ [SINOHYDRO  ▼]  │ │ [___________]   │       │
│ └─────────────────┘ └─────────────────┘ └─────────────────┘       │
│                                                                     │
│ SECTION 2: TRAJET                                                  │
│ ━━━━━━━━━━━━━━━━━                                                 │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │                                                                 │ │
│ │   ┌─────────────┐         ══════►         ┌─────────────┐      │ │
│ │   │   FROM *    │                         │    TO *     │      │ │
│ │   │ [KTO     ▼] │                         │ [SKM     ▼] │      │ │
│ │   │  Provenance │                         │ Destination │      │ │
│ │   └─────────────┘                         └─────────────┘      │ │
│ │                                                                 │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ SECTION 3: RESPONSABLES                                            │
│ ━━━━━━━━━━━━━━━━━━━━━━━━                                          │
│ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐       │
│ │ Site Manager *  │ │ Host Dept *     │ │ Reason on Site *│       │
│ │ [___________]   │ │ [IT        ▼]   │ │ [___________]   │       │
│ └─────────────────┘ └─────────────────┘ └─────────────────┘       │
│ ┌─────────────────┐ ┌─────────────────┐                           │
│ │ Nom Escorteur * │ │ Fonction Escor. │                           │
│ │ [___________]   │ │ [___________]   │                           │
│ └─────────────────┘ └─────────────────┘                           │
│                                                                     │
│ SECTION 4: MATÉRIELS                                               │
│ ━━━━━━━━━━━━━━━━━━━━                                              │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ N° Série/Code │ Désignation              │ Qté  │ Prov │ Dest  │ │
│ ├───────────────┼──────────────────────────┼──────┼──────┼───────┤ │
│ │ SN-001234     │ Générateur diesel 500kVA │ 1    │ KTO  │ SKM   │ │
│ │ SN-001235     │ Câbles électriques       │ 500m │ KTO  │ SKM   │ │
│ │ [+ Ajouter une ligne]                                           │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ SECTION 5: OBSERVATIONS                                            │
│ ━━━━━━━━━━━━━━━━━━━━━━━                                           │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ [                                                               ] │
│ │ [          Zone de texte multilignes                           ] │
│ │ [                                                               ] │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│ FOOTER ACTIONS                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ [Annuler]                    [Brouillon]  [Créer & Soumettre ►] │ │
│ └─────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

#### Champs du formulaire

| Champ | Type | Requis | Validation |
|-------|------|--------|------------|
| N° Contrat | Text | Non | Max 50 car. |
| Nom Compagnie | Dropdown + recherche | Oui | Liste prédéfinie |
| Email Contractant | Email | Non | Format email |
| Provenance (FROM) | Dropdown | Oui | Sites actifs |
| Destination (TO) | Dropdown | Oui | Sites actifs |
| Site Manager | Text | Oui | Max 200 car. |
| Host Department | Dropdown | Oui | Départements |
| Reason on Site | Textarea | Oui | Max 1000 car. |
| Nom Escorteur | Text | Oui | Max 200 car. |
| Fonction Escorteur | Text | Non | Max 150 car. |
| Observations | Textarea | Non | Max 1000 car. |

---

### 4.3 Vue Détaillée BEM

**Route:** `/bon-entree/{id}`

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER (KCC Slate)                                                  │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ 📥 BEM-2026-000145                                              │ │
│ │                                                                 │ │
│ │ ┌───────────┐  SINOHYDRO International    Créé le 13/02/2026   │ │
│ │ │    QR     │  Par: Jean KABILA           ┌──────────────────┐ │ │
│ │ │   CODE    │                             │  ✅ APPROUVÉ     │ │ │
│ │ │           │                             └──────────────────┘ │ │
│ │ └───────────┘  [🖨️ Imprimer]  [✏️ Modifier]  [📋 Dupliquer]   │ │
│ └─────────────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│ TRAJET (Fond beige/jaune #FEF3C7)                                  │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │                                                                 │ │
│ │   🏭 KTO                ══════►══════►              🏭 SKM      │ │
│ │   Provenance                                       Destination │ │
│ │                                                                 │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ INFORMATIONS GÉNÉRALES                                             │
│ ┌─────────────────┬─────────────────┬─────────────────┐           │
│ │ N° Contrat      │ Site Manager    │ Host Department │           │
│ │ CTR-2026-001    │ Paul MBUYI      │ IT              │           │
│ ├─────────────────┼─────────────────┼─────────────────┤           │
│ │ Reason on Site  │ Escorteur       │ Validité        │           │
│ │ Installation... │ Marc LUNDA      │ 13/02 → 13/03   │           │
│ └─────────────────┴─────────────────┴─────────────────┘           │
│                                                                     │
│ MATÉRIELS                                                          │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ Code/Série    │ Désignation            │ Qté  │ Provenance │TO │ │
│ ├───────────────┼────────────────────────┼──────┼────────────┼───┤ │
│ │ GEN-500-01    │ Générateur diesel      │ 1    │ KTO        │SKM│ │
│ │ CBL-E-2024    │ Câbles électriques     │ 500m │ KTO        │SKM│ │
│ │ TFR-380-01    │ Transformateur 380V    │ 2    │ KTO        │SKM│ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ WORKFLOW D'APPROBATION                                             │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │                                                                 │ │
│ │  ✅ ──────► ✅ ──────► ✅ ──────► ✅ ──────► ✅               │ │
│ │  SUP        GM         OPJ        ID         FINAL              │ │
│ │  12/02      12/02      13/02      13/02      13/02              │ │
│ │                                                                 │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ HISTORIQUE DES PASSAGES                                            │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ Barrière    │ Date/Heure        │ Agent         │ Statut       │ │
│ ├─────────────┼───────────────────┼───────────────┼──────────────┤ │
│ │ Barrière KTO│ 13/02/26 08:15    │ Pierre MUMBA  │ ✅ Passé     │ │
│ │ Barrière SKM│ 13/02/26 09:45    │ Jean KALONJI  │ ✅ Passé     │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

### 4.4 Impression BEM (Format SEC-FM-141B)

**Route:** `/bon-entree/{id}/print`

Format A4 portrait, design officiel avec :
- En-tête KCC avec logo
- Titre "MATERIAL ENTRY CLEARANCE FORM" + code SEC-FM-141(B)
- QR Code en haut à droite
- Informations compagnie
- Tableau des matériels
- Zone signatures (Escorteur, Superviseur, GM, OPJ)
- Pied de page avec validité

---

## 5. Module 2: Bon de Sortie Matériel (BSM)

### 5.1 Types de Sortie

| Type | Catégorie | Icône | Couleur Badge | Description |
|------|-----------|-------|---------------|-------------|
| **Informatique** | INTERNE | 💻 | Bleu | Transfert équipement IT |
| **Circulaire** | INTERNE | 📄 | Bleu | Documents/courrier interne |
| **Modification** | INTERNE | 🔧 | Bleu | Équipement en réparation |
| **Prêt** | INTERNE | 🔄 | Bleu | Sortie temporaire (retour obligatoire) |
| **Fin de chantier** | EXTERNE | 🏗️ | Orange | Matériel contractant qui repart |
| **Résidu** | EXTERNE | 🗑️ | Orange | Déchets à évacuer |
| **Radio-protection** | EXTERNE | ☢️ | Orange | Matériel contrôlé |

### 5.2 Liste des BSM

**Route:** `/bon-sortie`

Même structure que BEM avec :
- Colonnes: N° Réf, Type sortie, Demandeur, Destination, Date, Statut
- Filtres additionnels: Type de sortie, Catégorie (Interne/Externe)
- Badges de type avec icônes

### 5.3 Création BSM (Formulaire Optimisé)

**Route:** `/bon-sortie/creer`

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER (KCC Slate)                                                  │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ 📤 Nouveau Bon de Sortie                        13 février 2026 │ │
│ └─────────────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│ SECTION 1: TYPE DE SORTIE *                                        │
│ ━━━━━━━━━━━━━━━━━━━━━━━━━━                                        │
│ (Grille de cartes cliquables - sélection directe)                  │
│                                                                     │
│ ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐       │
│ │    💻      │ │    📄      │ │    🔧      │ │    🔄      │       │
│ │Informatique│ │ Circulaire │ │Modification│ │   Prêt     │       │
│ │ ┌────────┐ │ │ ┌────────┐ │ │ ┌────────┐ │ │ ┌────────┐ │       │
│ │ │INTERNE │ │ │ │INTERNE │ │ │ │INTERNE │ │ │ │INTERNE │ │       │
│ │ └────────┘ │ │ └────────┘ │ │ └────────┘ │ │ └────────┘ │       │
│ └────────────┘ └────────────┘ └────────────┘ └────────────┘       │
│                                                                     │
│ ┌────────────┐ ┌────────────┐ ┌────────────┐                      │
│ │    🏗️      │ │    🗑️      │ │    ☢️      │                      │
│ │Fin chantier│ │  Résidu    │ │Radio-prot. │                      │
│ │ ┌────────┐ │ │ ┌────────┐ │ │ ┌────────┐ │                      │
│ │ │EXTERNE │ │ │ │EXTERNE │ │ │ │EXTERNE │ │                      │
│ │ └────────┘ │ │ └────────┘ │ │ └────────┘ │                      │
│ └────────────┘ └────────────┘ └────────────┘                      │
│                                                                     │
│ ═══════════════════════════════════════════════════════════════════│
│ (Sections suivantes apparaissent après sélection du type)          │
│                                                                     │
│ SECTION 2: DEMANDEUR                                               │
│ ━━━━━━━━━━━━━━━━━━━━                                              │
│ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐       │
│ │ Nom complet *   │ │ Fonction *      │ │ Département *   │       │
│ │ [Jean KABILA]   │ │ [Ingénieur IT]  │ │ [IT          ▼] │       │
│ └─────────────────┘ └─────────────────┘ └─────────────────┘       │
│                                                                     │
│ SECTION 3: TRAJET                                                  │
│ ━━━━━━━━━━━━━━━━━                                                 │
│ ┌──────────────────────────┐   ┌──────────────────────────┐       │
│ │ Provenance (FROM) *      │   │ Destination (TO) *       │       │
│ │ [SKM                  ▼] │ ► │ [KTO                  ▼] │       │
│ └──────────────────────────┘   └──────────────────────────┘       │
│                                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ Motif de la sortie *                                            │ │
│ │ [Transfert d'équipement IT vers le datacenter principal...    ] │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ SECTION 4: DURÉE DU PRÊT (si type = Prêt)                         │
│ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━                         │
│ ┌─────────────────────────┐                                        │
│ │ Date de retour prévue * │                                        │
│ │ [📅 15/08/2026        ] │  (Max 180 jours)                       │
│ └─────────────────────────┘                                        │
│                                                                     │
│ SECTION 5: VÉHICULE (si catégorie = Externe)                       │
│ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━                         │
│ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐       │
│ │ N° Véhicule     │ │ Chauffeur       │ │ Téléphone       │       │
│ │ [KIN-1234-AB]   │ │ [Pierre MBUYI]  │ │ [+243 xxx]      │       │
│ └─────────────────┘ └─────────────────┘ └─────────────────┘       │
│                                                                     │
│ SECTION 6: BARRIÈRES À TRAVERSER *                                │
│ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━                                │
│ (Cliquez dans l'ordre de passage)                                  │
│                                                                     │
│ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐       │
│ │ ☑ ┌─┐          │ │ ☐               │ │ ☑ ┌─┐          │       │
│ │   │1│ KTO      │ │    LUILU        │ │   │2│ SKM      │       │
│ │   └─┘ CHK-KTO  │ │    CHK-LUILU    │ │   └─┘ CHK-SKM  │       │
│ └─────────────────┘ └─────────────────┘ └─────────────────┘       │
│                                                                     │
│ ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐       │
│ │ ☐               │ │ ☐               │ │ ☐               │       │
│ │    LUSANGA      │ │    KOV          │ │    MASHAMBA     │       │
│ │    CHK-LUSANGA  │ │    CHK-KOV      │ │    CHK-MASHAMBA │       │
│ └─────────────────┘ └─────────────────┘ └─────────────────┘       │
│                                                                     │
│ Itinéraire: KTO → SKM                                              │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ 🗺️ KTO ═══════════► SKM                                        │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ SECTION 7: MATÉRIELS *                                             │
│ ━━━━━━━━━━━━━━━━━━━━━━                                            │
│                                                                     │
│ (Si type Externe ou requiert BEM)                                  │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ Rechercher un BEM: [BEM-2026-000145     ] [🔍 Rechercher]       │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ BEM Sélectionné:                                                    │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ ✅ BEM-2026-000145 - SINOHYDRO                          [X]    │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ OU (Si type Interne sans BEM requis)                               │
│                                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ ☑ │ Code/Série    │ Désignation         │ Qté  │ Suppr.        │ │
│ ├───┼───────────────┼─────────────────────┼──────┼───────────────┤ │
│ │ ☑ │ [PC-2024-001] │ [Laptop Dell XPS]   │ [1]  │ 🗑️            │ │
│ │ ☑ │ [MON-HP-24]   │ [Moniteur HP 24"]   │ [2]  │ 🗑️            │ │
│ │   │ [+ Ajouter un matériel]                                     │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ ✅ 2 matériel(s) sélectionné(s)                                    │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│ FOOTER ACTIONS                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ [Annuler]                    [Brouillon]  [Créer & Soumettre ►] │ │
│ └─────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

### 5.4 Vue Détaillée BSM

**Route:** `/bon-sortie/{id}`

Similaire à BEM avec sections additionnelles :
- Type de sortie avec badge coloré
- Date de retour (si prêt)
- Informations véhicule (si externe)
- Liaison vers BEM source (si applicable)

### 5.5 Gestion des Prêts

**Route:** `/bon-sortie/prets`

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER                                                              │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ 🔄 Prêts en cours                                               │ │
│ └─────────────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│ ALERTES RETARDS                                                    │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ ⚠️ 3 prêts arrivent à échéance cette semaine                    │ │
│ │ 🔴 2 prêts sont en retard                                       │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ LISTE DES PRÊTS                                                    │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ N° BSM       │ Matériel        │ Échéance  │ Jours │ Actions   │ │
│ ├──────────────┼─────────────────┼───────────┼───────┼───────────┤ │
│ │ BSM-2026-045 │ Laptop Dell     │ 15/02     │ 🔴 -2 │ [Retour]  │ │
│ │ BSM-2026-067 │ Générateur      │ 20/02     │ ⚠️ 7  │ [Étendre] │ │
│ │ BSM-2026-089 │ Outillage       │ 15/03     │ ✅ 30 │ [Voir]    │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

#### Actions Prêts
- **Enregistrer retour** : Dialog pour confirmer réception
- **Demander extension** : Dialog avec nouvelle date (max 180j total)
- **Voir détails** : Navigation vers vue BSM

---

## 6. Module 3: Sécurité & Barrières

### 6.1 Scanner QR Code (Mobile-First)

**Route:** `/securite/scanner`

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER                                                              │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ 📱 Scanner - Barrière KTO                                       │ │
│ └─────────────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│                    ┌───────────────────────┐                       │
│                    │                       │                       │
│                    │                       │                       │
│                    │    📷 CAMERA VIEW     │                       │
│                    │                       │                       │
│                    │    ┌─────────┐        │                       │
│                    │    │ ░░░░░░░ │        │                       │
│                    │    │ ░ QR  ░ │        │                       │
│                    │    │ ░░░░░░░ │        │                       │
│                    │    └─────────┘        │                       │
│                    │                       │                       │
│                    └───────────────────────┘                       │
│                                                                     │
│                    [📷 Scanner un QR Code]                         │
│                                                                     │
│ OU                                                                  │
│                                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ Saisir le numéro: [BEM-2026-000___] [🔍]                        │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│ DERNIER SCAN                                                        │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ ✅ BEM-2026-000145                                              │ │
│ │    SINOHYDRO - Générateur diesel                                │ │
│ │    Scanné à 14:32                                               │ │
│ └─────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

### 6.2 Résultat du Scan

#### Scan Valide ✅
```
┌─────────────────────────────────────────────────────────────────────┐
│                                                                     │
│                         ✅ BON VALIDE                               │
│                                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ BEM-2026-000145                                                 │ │
│ │ SINOHYDRO International                                         │ │
│ ├─────────────────────────────────────────────────────────────────┤ │
│ │ Trajet: KTO → SKM                                               │ │
│ │ Validité: 13/02/2026 - 13/03/2026                              │ │
│ ├─────────────────────────────────────────────────────────────────┤ │
│ │ MATÉRIELS:                                                      │ │
│ │ • Générateur diesel 500kVA (1)                                  │ │
│ │ • Câbles électriques (500m)                                     │ │
│ │ • Transformateur 380V (2)                                       │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ [✅ Valider le passage]                                         │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

#### Scan Invalide ❌
```
┌─────────────────────────────────────────────────────────────────────┐
│                                                                     │
│                         ❌ BON INVALIDE                             │
│                                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ BEM-2026-000089                                                 │ │
│ │ ⚠️ Ce bon n'est pas autorisé à cette barrière                   │ │
│ │                                                                 │ │
│ │ Raison: Barrière non incluse dans l'itinéraire                 │ │
│ │ Itinéraire prévu: LUILU → KOV                                   │ │
│ │ Barrière actuelle: KTO                                          │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ [⚠️ Signaler une anomalie]                                      │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### 6.3 Dashboard Anomalies

**Route:** `/securite/anomalies`

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER                                                              │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ ⚠️ Anomalies                                                    │ │
│ └─────────────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│ STATISTIQUES                                                       │
│ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐               │
│ │ Ouvertes │ │ En cours │ │ Résolues │ │ Ce mois  │               │
│ │    8     │ │    3     │ │   45     │ │   12     │               │
│ │ 🔴       │ │ 🟠       │ │ ✅       │ │ 📊       │               │
│ └──────────┘ └──────────┘ └──────────┘ └──────────┘               │
│                                                                     │
│ ANOMALIES RÉCENTES                                                 │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ Type          │ Bon           │ Barrière │ Date    │ Statut    │ │
│ ├───────────────┼───────────────┼──────────┼─────────┼───────────┤ │
│ │ 🔴 Hors itin. │ BEM-2026-089  │ KTO      │ 13/02   │ 🔴 Ouvert │ │
│ │ 🟠 QR expiré  │ BSM-2026-045  │ SKM      │ 12/02   │ 🟠 En trt │ │
│ │ ✅ Matériel   │ BEM-2026-012  │ LUILU    │ 10/02   │ ✅ Résolu │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### 6.4 Types d'Anomalies

| Type | Icône | Description |
|------|-------|-------------|
| Hors itinéraire | 🔴 | Bon scanné à une barrière non prévue |
| QR expiré | 🟠 | Bon dont la validité est dépassée |
| Matériel manquant | 🟡 | Quantité/matériel ne correspond pas |
| QR inconnu | ⚫ | QR code non reconnu dans le système |
| Doublon | 🟣 | Même bon scanné plusieurs fois |

---

## 7. Composants Transversaux

### 7.1 Navigation Principale

```
┌─────────────────────────────────────────────────────────────────────┐
│ SIDEBAR (KCC Slate #253C45)                                         │
│                                                                     │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ 🏭 KCC MaterialFlow                                             │ │
│ │ ─────────────────────                                           │ │
│ │                                                                 │ │
│ │ 📊 Dashboard                                                    │ │
│ │                                                                 │ │
│ │ 📥 Bons d'Entrée                                                │ │
│ │    ├─ Liste                                                     │ │
│ │    ├─ Nouveau                                                   │ │
│ │    └─ En attente (12)                                          │ │
│ │                                                                 │ │
│ │ 📤 Bons de Sortie                                               │ │
│ │    ├─ Liste                                                     │ │
│ │    ├─ Nouveau                                                   │ │
│ │    ├─ En attente (8)                                           │ │
│ │    └─ Prêts (3 ⚠️)                                              │ │
│ │                                                                 │ │
│ │ 🔐 Sécurité                                                     │ │
│ │    ├─ Scanner                                                   │ │
│ │    ├─ Passages                                                  │ │
│ │    └─ Anomalies (2 🔴)                                          │ │
│ │                                                                 │ │
│ │ ⚙️ Administration                                               │ │
│ │    ├─ Utilisateurs                                              │ │
│ │    ├─ Sites & Barrières                                         │ │
│ │    └─ Configuration                                             │ │
│ │                                                                 │ │
│ │ ─────────────────────                                           │ │
│ │ 👤 Jean KABILA                                                  │ │
│ │    Demandeur - IT                                               │ │
│ │ [Déconnexion]                                                   │ │
│ └─────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

### 7.2 Header Global

```
┌─────────────────────────────────────────────────────────────────────┐
│ [☰]  KCC MaterialFlow           [🔔 3]  [👤 Jean KABILA ▼]        │
└─────────────────────────────────────────────────────────────────────┘
```

### 7.3 Composants UI Réutilisables

#### Boutons
| Type | Style | Usage |
|------|-------|-------|
| Primary | Green (#00B193) | Actions principales |
| Secondary | Beige (#BB8748) | Actions secondaires |
| Danger | Red (#DC2626) | Suppressions, rejets |
| Ghost | Transparent + border | Annuler, actions neutres |

#### Cards
- Border-radius: 12px
- Padding: 16-24px
- Shadow: 0 2px 12px rgba(0,0,0,0.08)

#### Inputs
- Border: 1px solid #E5E7EB
- Border-radius: 6px
- Focus: border-color #00B193

#### Badges de Statut
| Statut | Background | Text |
|--------|------------|------|
| Brouillon | #F3F4F6 | #6B7280 |
| En attente | #FEF3C7 | #B45309 |
| Approuvé | #D1FAE5 | #065F46 |
| Rejeté | #FEE2E2 | #991B1B |
| En transit | #DBEAFE | #1D4ED8 |

### 7.4 Dialog d'Approbation

```
┌─────────────────────────────────────────────────────────────────────┐
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │                    Validation du Bon                            │ │
│ │                    BEM-2026-000145                              │ │
│ │ ─────────────────────────────────────────────────────────────── │ │
│ │                                                                 │ │
│ │ Action:                                                         │ │
│ │ ○ ✅ Approuver                                                  │ │
│ │ ○ ❌ Rejeter                                                    │ │
│ │ ○ ↩️ Retourner pour modification                                │ │
│ │                                                                 │ │
│ │ Commentaire:                                                    │ │
│ │ ┌─────────────────────────────────────────────────────────────┐ │ │
│ │ │                                                             │ │ │
│ │ │                                                             │ │ │
│ │ └─────────────────────────────────────────────────────────────┘ │ │
│ │                                                                 │ │
│ │ ─────────────────────────────────────────────────────────────── │ │
│ │                               [Annuler]  [Confirmer]            │ │
│ └─────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

### 7.5 Notifications Toast

```
┌─────────────────────────────────────────────────────────────────────┐
│ SUCCÈS (Green)                                                      │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ ✅ Bon BEM-2026-000145 créé avec succès                    [X]  │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ ERREUR (Red)                                                        │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ ❌ Erreur lors de la création du bon                       [X]  │ │
│ └─────────────────────────────────────────────────────────────────┘ │
│                                                                     │
│ WARNING (Orange)                                                    │
│ ┌─────────────────────────────────────────────────────────────────┐ │
│ │ ⚠️ Le prêt BSM-2026-045 arrive à échéance dans 2 jours    [X]  │ │
│ └─────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 8. Workflow d'Approbation

### 8.1 Visualisation du Workflow

```
┌─────────────────────────────────────────────────────────────────────┐
│                                                                     │
│ WORKFLOW STANDARD (BEM/BSM)                                         │
│                                                                     │
│  ┌─────┐     ┌─────┐     ┌─────┐     ┌─────┐     ┌─────┐          │
│  │ 📝  │ ──► │ 👤  │ ──► │ 👔  │ ──► │ 🔐  │ ──► │ 🆔  │          │
│  │Draft│     │ SUP │     │ GM  │     │ OPJ │     │ ID  │          │
│  └─────┘     └─────┘     └─────┘     └─────┘     └─────┘          │
│                                                                     │
│ ════════════════════════════════════════════════════════════════════│
│                                                                     │
│ WORKFLOW AVEC VALIDATION SPÉCIALE (Informatique/Radio-protection)  │
│                                                                     │
│  ┌─────┐     ┌─────┐     ┌─────┐     ┌─────┐     ┌─────┐     ┌───┐│
│  │ 📝  │ ──► │ 👤  │ ──► │ 👔  │ ──► │💻/☢️│ ──► │ 🔐  │ ──► │🆔 ││
│  │Draft│     │ SUP │     │ GM  │     │IT/ENV│    │ OPJ │     │ID ││
│  └─────┘     └─────┘     └─────┘     └─────┘     └─────┘     └───┘│
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### 8.2 États du Workflow

| État | Icône | Couleur | Description |
|------|-------|---------|-------------|
| Non atteint | ○ | Gray | Étape future |
| En cours | ◐ | Orange | Attente validation |
| Validé | ● | Green | Approuvé |
| Rejeté | ✕ | Red | Refusé |
| Skipped | ○ | Gray dotted | Non applicable |

---

## 9. Responsive Design

### 9.1 Breakpoints

| Breakpoint | Taille | Usage |
|------------|--------|-------|
| Mobile | < 640px | Scanner barrière, vue simplifiée |
| Tablet | 640-1024px | Formulaires, listes |
| Desktop | > 1024px | Dashboard complet, tous modules |

### 9.2 Adaptations Mobile

- Navigation: Burger menu
- Tableaux: Cards empilées
- Formulaires: Une colonne
- Scanner: Full screen camera

### 9.3 Écran Scanner (Mobile-Only)

Le scanner est optimisé pour usage mobile aux barrières :
- Caméra plein écran
- Bouton scan large
- Résultat avec gros indicateur ✅/❌
- Actions tactiles larges

---

## 10. Spécifications Techniques

### 10.1 Données de Référence

#### Sites/Barrières
| ID | Code | Nom |
|----|------|-----|
| 1 | KTO | Kamoto |
| 2 | LUILU | Luilu |
| 3 | SKM | Sukimu |
| 4 | LUSANGA | Lusanga |
| 5 | KOV | Kovo |
| 6 | MV | Muva |
| 7 | MASHAMBA | Mashamba |
| 8 | KTC | Katanga |

#### Départements
- IT (Informatique)
- HR (Ressources Humaines)
- FIN (Finance)
- OPS (Opérations)
- SEC (Sécurité)
- ENV (Environnement)
- MAINT (Maintenance)
- LOG (Logistique)

### 10.2 Formats

| Donnée | Format |
|--------|--------|
| N° BEM | BEM-YYYY-NNNNNN |
| N° BSM | BSM-YYYY-NNNNNN |
| Date | JJ/MM/AAAA |
| Heure | HH:MM |
| Téléphone | +243 XXX XXX XXX |

### 10.3 Validations

| Champ | Règle |
|-------|-------|
| Email | Format email valide |
| Téléphone | Format international |
| Quantité | > 0 |
| Date retour prêt | Entre J+1 et J+180 |
| Observations | Max 1000 caractères |

---

## 📎 Annexes

### A. Écrans à Designer (Liste complète)

#### Module BEM
1. [ ] Liste des BEM
2. [ ] Création BEM (formulaire)
3. [ ] Vue détaillée BEM
4. [ ] Édition BEM
5. [ ] Impression BEM (A4)

#### Module BSM
6. [ ] Liste des BSM
7. [ ] Création BSM (formulaire optimisé)
8. [ ] Vue détaillée BSM
9. [ ] Édition BSM
10. [ ] Impression BSM (A4)
11. [ ] Liste des prêts
12. [ ] Dialog retour prêt
13. [ ] Dialog extension prêt

#### Module Sécurité
14. [ ] Scanner QR (mobile)
15. [ ] Résultat scan valide
16. [ ] Résultat scan invalide
17. [ ] Dashboard anomalies
18. [ ] Détail anomalie
19. [ ] Historique passages

#### Transversal
20. [ ] Dashboard principal
21. [ ] Navigation sidebar
22. [ ] Dialog approbation
23. [ ] Login page
24. [ ] Page 404
25. [ ] Page erreur

### B. Assets Requis

- Logo KCC (PNG/SVG)
- Icônes métier (matériels, barrières)
- Illustrations état vide
- Favicon

### C. Contacts

**Projet:** KCCMaterialFlow  
**Client:** Kamoto Copper Company  
**Date:** Février 2026

---

*Document généré pour la création de maquettes Figma*
