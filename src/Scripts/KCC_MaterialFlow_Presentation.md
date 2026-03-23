---
marp: true
theme: default
paginate: true
backgroundColor: #fff
style: |
  section {
    font-family: Calibri, Arial, sans-serif;
  }
  h1 {
    color: #1F497D;
  }
  h2 {
    color: #4F81BD;
    border-bottom: 2px solid #4F81BD;
    padding-bottom: 10px;
  }
  strong {
    color: #C0504D;
  }
---

# KCC MaterialFlow

## Systeme de Gestion Securisee des Mouvements de Materiels

**Kamoto Copper Company**

---

## Contexte et Problematique

### Situation Actuelle
- Processus manuel de bons d'entree/sortie
- Falsification et substitution de documents
- Mouvements non autorises de materiels
- Tracabilite limitee et audits difficiles

### Impact
- **Pertes financieres significatives**
- **Risques operationnels et de conformite**
- **Investigations retardees**

---

## Solution Proposee

### KCC MaterialFlow

| Composant | Description |
|-----------|-------------|
| **Application Web** | Creation, approbations, administration, reporting |
| **Application Mobile** | Scan QR aux barrieres, verification temps reel |

### Technologies
- .NET 10 / Blazor Server
- SQL Server
- QR Code securise (hash/signature)

---

## Fonctionnalites Cles

### 1. Gestion des Bons
- **Bon d'Entree (BEM)** - Materiels entrants
- **Bon de Sortie (BSM)** - Interne et Externe/Retour

### 2. Workflows d'Approbation
- Chaines predefinies par categorie de materiel
- Non modifiables (securite)

### 3. Controle Quantitatif
- Suivi: Entre / Sorti / Restant sur site
- Deduction uniquement a la barriere finale

---

## Workflows par Type de Materiel

| Type | Circuit d'Approbation |
|------|----------------------|
| **Circulant** | Demandeur → Superviseur → GM → OPJ → Identification |
| **Equipement IT** | Demandeur → IT → Superviseur → GM → OPJ → Identification |
| **Residus/Dechets** | Demandeur → Environnement → Superviseur → GM → OPJ → Identification |
| **Protection Radiologique** | Demandeur → Environnement → Superviseur → GM → OPJ → Identification |
| **Materiels Pretes** | Demandeur → Superviseur → GM → OPJ → Identification |

---

## Securite aux Barrieres

### Barrieres Controlees
**KTO | LUILU | SKM | LUSANGA | KOV | MV | MASHAMBA | KTC**

### Protocole de Scan
1. **Authentification** - Validation du QR code (token signe)
2. **Verification** - Statut, validite, route, sequence
3. **Confirmation** - Affichage des details, validation agent
4. **Recu** - Impression automatique preuve de passage

### Regles
- Usage unique par barriere
- Routes obligatoires avec sequence imposee

---

## Gestion des Anomalies

### Detection Automatique
- Barriere hors route prevue
- Sequence incorrecte des checkpoints
- Bon expire ou invalide
- Tentative de reutilisation
- Fraude ou substitution suspectee

### Actions Automatiques
- Affichage: **"Document Anormal - Acces Refuse"**
- Journalisation pour audit
- Email automatique a Investigation (CC Identification)

---

## Avantages Strategiques

| Domaine | Benefices |
|---------|-----------|
| **Securite Renforcee** | Elimination falsification, QR securise, verification temps reel |
| **Controle Total** | Workflows non modifiables, tracabilite complete, audit trail |
| **Efficacite Operationnelle** | Digitalisation, tableaux de bord, rapports automatises |
| **Conformite** | Procedures internes, documentation audits, historique complet |

---

## Utilisateurs du Systeme

| Role | Responsabilites |
|------|-----------------|
| **Demandeurs** | Creation des demandes de bons |
| **Approbateurs** | Validation (Superviseur, IT, Environnement, GM, OPJ) |
| **Identification** | Approbation finale, extension des prets |
| **Agents Barriere** | Scan et verification mobile |
| **Investigation** | Reception alertes, acces anomalies |
| **Administrateurs** | Configuration, utilisateurs, roles |

---

## Budget Previsionnel

### Phase 1: Developpement et Deploiement
- Developpement logiciel (interne)
- Infrastructure serveur - *A evaluer*
- Equipements barrieres (scanners, imprimantes) - *A evaluer*
- Formation utilisateurs - *A planifier*

### Phase 2: Maintenance
- Support et maintenance annuelle - *A definir*

### Points a Clarifier
- Type scanners: Android portables ou PC?
- Infrastructure reseau aux checkpoints

---

## Risques et Mitigations

| Risque | Impact | Mitigation |
|--------|--------|------------|
| Perte connectivite barriere | Eleve | Connectivite redondante + monitoring |
| Duplication QR code | Eleve | Hash securise + usage unique |
| Contournement workflow | Eleve | RBAC strict + workflows non editables |
| Erreur de quantite | Moyen | Deduction barriere finale uniquement |
| Reponse anomalie retardee | Moyen | Alertes email automatiques |

---

## Livrables

1. **Application Web** - Gestion complete des bons
2. **Application Mobile** - Scan et verification barrieres
3. **Workflows configures** - Par categorie de materiel
4. **Generation QR securisee** - Impression controlee
5. **Detection d'anomalies** - Alertes automatiques
6. **Tableaux de bord** - Reporting et audit
7. **Documentation** - Guides utilisateurs et SOP
8. **Formation** - Support au deploiement

---

## Resultats Attendus

### Reduction des Risques
- Fin de la falsification de documents
- Elimination des mouvements non autorises
- Detection immediate des anomalies

### Amelioration Operationnelle
- Tracabilite complete des materiels
- Visibilite temps reel pour la direction
- Audits simplifies et rapides

### Protection des Actifs
- Controle strict a chaque barriere
- Gouvernance renforcee

---

## Prochaines Etapes

1. **Validation** - Approbation du projet
2. **Clarification** - Infrastructure barrieres (scanners/imprimantes)
3. **Configuration** - Hierarchies d'approbation et roles
4. **Deploiement Pilote** - Test sur une barriere
5. **Formation** - Utilisateurs cles
6. **Go-Live** - Deploiement progressif

---

# Questions ?

## KCC MaterialFlow

**Securiser. Controler. Tracer.**
