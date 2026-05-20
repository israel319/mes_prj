# Plan de Test d'Acceptation Utilisateur (UAT)
## Projet : KCC Material Flow

| Champ | Valeur |
|---|---|
| **Version document** | 1.0 |
| **Date** | 13 mai 2026 |
| **Application** | KCCMaterialFlow (Blazor Server .NET 10) |
| **Environnement cible** | UAT — `https://uat.kccmaterialflow.local` |
| **Responsable UAT** | _________________________ |
| **Période d'exécution** | du _____ au _____ |

---

## 1. Objectif

Valider que l'application **KCC Material Flow** répond aux exigences fonctionnelles et métier pour la gestion des **bons d'entrée**, **bons de sortie**, **workflows d'approbation**, **scans aux checkpoints**, **anomalies** et **administration**, avant la mise en production.

## 2. Périmètre

### Inclus
- Authentification & autorisation (rôles : Admin, Approbateur, Agent Sécurité, Demandeur, Magasinier)
- Module **Bons d'entrée** (création, édition, approbation, impression)
- Module **Bons de sortie** (interne / externe, prêts, retours, extensions)
- Module **Sécurité** (scan QR, checkpoints, anomalies, historique)
- Module **Administration** (utilisateurs, rôles, types matériels, statuts, paramètres système, workflows, audit, imports)
- Tableaux de bord et suivi
- Notifications de rejet

### Exclus
- Tests de charge / performance (couverts hors UAT)
- Intégrations externes ERP (mockées)

## 3. Pré-requis

- [ ] Base de données UAT restaurée avec jeu de données représentatif
- [ ] Comptes de test créés pour chaque rôle (cf. annexe A)
- [ ] Sites, compagnies, contrats et checkpoints paramétrés
- [ ] Workflows d'approbation configurés (au moins 1 par département)
- [ ] Imprimante de test ou export PDF disponible
- [ ] Lecteur QR / smartphone de test pour le scan
- [ ] Navigateurs supportés : Chrome ≥ 120, Edge ≥ 120, Firefox ≥ 120

## 4. Critères d'acceptation globaux

- ≥ **95 %** des cas de test passent (statut **OK**)
- **0** anomalie bloquante (sévérité 1) ouverte
- ≤ **3** anomalies majeures (sévérité 2) ouvertes avec contournement validé
- Tous les workflows métier critiques (création → approbation → sortie → scan) exécutés sans erreur

## 5. Sévérités d'anomalie

| Niveau | Description | Exemple |
|---|---|---|
| **S1 — Bloquant** | Empêche l'utilisation d'une fonction critique, pas de contournement | Impossible de créer un bon |
| **S2 — Majeur** | Fonction altérée, contournement existe | Recherche lente, bouton imprimer absent sur 1 vue |
| **S3 — Mineur** | Inconfort, cosmétique fonctionnel | Libellé erroné, tri inversé |
| **S4 — Cosmétique** | Visuel uniquement | Couleur, alignement |

---

## 6. Cas de test

### 6.1 Authentification & sécurité

| ID | Titre | Étapes | Résultat attendu | Statut | Observations |
|---|---|---|---|---|---|
| AUTH-01 | Connexion utilisateur valide | 1. Aller sur `/`<br>2. Saisir login + mot de passe valides<br>3. Cliquer **Se connecter** | Redirection vers `/dashboard`, nom affiché | ☐ | |
| AUTH-02 | Connexion mot de passe invalide | Saisir mauvais mot de passe | Message d'erreur, pas de redirection | ☐ | |
| AUTH-03 | Accès refusé sans rôle | Utilisateur non autorisé accède à `/admin/utilisateurs` | Page **AccessDenied** affichée | ☐ | |
| AUTH-04 | Déconnexion | Cliquer **Déconnexion** | Redirection page de login, session invalidée | ☐ | |
| AUTH-05 | Impersonation (Admin) | Admin → `/admin/impersonation` → choisir utilisateur | Connecté sous identité tierce, bandeau visible | ☐ | |

### 6.2 Bons d'entrée

| ID | Titre | Étapes | Résultat attendu | Statut | Observations |
|---|---|---|---|---|---|
| BE-01 | Création nouveau bon d'entrée | 1. `/bons-entree/nouveau`<br>2. Renseigner Compagnie, Escorteur, Trajet, Matériels, Réserves<br>3. Soumettre | Bon créé statut **En attente**, n° généré | ☐ | |
| BE-02 | Validation des champs obligatoires | Soumettre formulaire vide | Messages d'erreur sur chaque champ requis | ☐ | |
| BE-03 | Bordures inputs visibles | Inspecter les 5 sections du formulaire | Toutes bordures grises 1px, focus teal | ☐ | |
| BE-04 | Édition bon en attente | Ouvrir un bon en attente → modifier → enregistrer | Modifications persistées, historique mis à jour | ☐ | |
| BE-05 | Approbation niveau 1 | Approbateur N1 → `/mes-approbations` → approuver | Statut passe au niveau suivant | ☐ | |
| BE-06 | Rejet avec commentaire | Approbateur → rejeter + saisir motif | Statut **Rejeté**, notification générée | ☐ | |
| BE-07 | Impression bon approuvé | `/bons-entree/{id}/print` | Mise en page A4 propre, tous champs présents | ☐ | |
| BE-08 | Visualisation bon | `/bons-entree/{id}/view` | Lecture seule, historique visible | ☐ | |

### 6.3 Bons de sortie

| ID | Titre | Étapes | Résultat attendu | Statut | Observations |
|---|---|---|---|---|---|
| BS-01 | Création bon sortie **interne** | Créer bon, type interne, destinataire interne | Bon créé, workflow interne lancé | ☐ | |
| BS-02 | Création bon sortie **externe** | Créer bon, type externe, prestataire | Champs spécifiques (transporteur, plaque) | ☐ | |
| BS-03 | Ajout matériels au bon | Dialog **MaterielDialog** → ajouter ≥ 2 matériels | Liste mise à jour, totaux calculés | ☐ | |
| BS-04 | Suppression matériel ligne | Supprimer une ligne | Ligne retirée, recalcul OK | ☐ | |
| BS-05 | Approbation multi-niveaux | Cycle N1 → N2 → N3 selon workflow | Chaque niveau valide, statut final **Approuvé** | ☐ | |
| BS-06 | Prêt — extension durée | `/bons-sortie/pret/{id}/extension` | Nouvelle date retour enregistrée | ☐ | |
| BS-07 | Prêt — retour matériel | `/bons-sortie/pret/{id}/retour` + dialog | Statut **Retourné**, solde mis à jour | ☐ | |
| BS-08 | Liste & filtres | `/bons-sortie` filtrer par statut, date, site | Résultats cohérents avec filtres | ☐ | |
| BS-09 | Impression bon sortie | `/bons-sortie/{id}/print` | PDF / impression A4 conforme | ☐ | |

### 6.4 Sécurité — Scan & anomalies

| ID | Titre | Étapes | Résultat attendu | Statut | Observations |
|---|---|---|---|---|---|
| SEC-01 | Scan QR à l'entrée checkpoint | Agent scanne QR du bon | Passage enregistré, statut **PassageOK** | ☐ | |
| SEC-02 | Scan QR invalide / expiré | Scanner code inconnu | Message d'erreur, anomalie créée auto | ☐ | |
| SEC-03 | Signalement anomalie manuel | `SignalerAnomalieDialog` → type + description + photo | Anomalie créée statut **Ouverte** | ☐ | |
| SEC-04 | Traitement anomalie | Agent → `TraiterAnomalieDialog` → résolution | Statut **Traitée**, traçabilité OK | ☐ | |
| SEC-05 | Réouverture anomalie | `ReouvririAnomalieDialog` avec motif | Statut **Réouverte**, historique gardé | ☐ | |
| SEC-06 | Dashboard anomalies | `/securite/anomalies-dashboard` | KPI ouvertes/traitées/rouvertes corrects | ☐ | |
| SEC-07 | Historique scans | `/securite/historique-scans` filtres date / checkpoint | Liste paginée, export OK | ☐ | |
| SEC-08 | Preuve de passage | `PreuvePassageDialog` → upload photo | Photo liée au passage, visible dans détail | ☐ | |

### 6.5 Administration

| ID | Titre | Étapes | Résultat attendu | Statut | Observations |
|---|---|---|---|---|---|
| ADM-01 | Création utilisateur | `/admin/utilisateurs` → nouveau | Utilisateur créé, rôle attribué | ☐ | |
| ADM-02 | Modification rôle | Changer rôle utilisateur existant | Permissions mises à jour à la prochaine connexion | ☐ | |
| ADM-03 | Désactivation utilisateur | Désactiver compte | Connexion refusée pour ce compte | ☐ | |
| ADM-04 | Gestion types matériels | CRUD via `/admin/types-materiels` | Création / édition / suppression OK | ☐ | |
| ADM-05 | Gestion statuts | CRUD `/admin/statuts` | Idem | ☐ | |
| ADM-06 | Paramètres système | Modifier un paramètre | Valeur prise en compte sans redémarrage | ☐ | |
| ADM-07 | Workflow approbateurs | Définir étapes & approbateurs spéciaux | Configuration utilisée par bons concernés | ☐ | |
| ADM-08 | Workflow par département | Configurer dans `WorkflowDepartementGrid` | Bon créé respecte chaîne d'approbation | ☐ | |
| ADM-09 | Audit logs | `/admin/audit-logs` filtres | Toutes opérations sensibles tracées | ☐ | |
| ADM-10 | Diff audit log | Ouvrir `AuditLogDiffDialog` | Avant/après lisible | ☐ | |
| ADM-11 | Import données | `/admin/import-donnees` charger fichier Excel | Lignes importées, erreurs reportées | ☐ | |

### 6.6 Tableaux de bord & suivi

| ID | Titre | Étapes | Résultat attendu | Statut | Observations |
|---|---|---|---|---|---|
| DASH-01 | Dashboard accueil | `/dashboard` | KPI bons en attente, approuvés, rejetés affichés | ☐ | |
| DASH-02 | Suivi bons | `/suivi-bons` filtres période, type, statut | Liste correcte, export Excel | ☐ | |
| DASH-03 | Liste rejets | `/rejets` | Bons rejetés avec motifs, accès rapide | ☐ | |
| DASH-04 | Notification rejet | Demandeur reçoit notif après rejet | Notification visible, lien vers bon | ☐ | |

### 6.7 Compatibilité & UI

| ID | Titre | Étapes | Résultat attendu | Statut | Observations |
|---|---|---|---|---|---|
| UI-01 | Affichage Chrome | Naviguer principales pages | Aucun défaut visuel | ☐ | |
| UI-02 | Affichage Edge | Idem | Idem | ☐ | |
| UI-03 | Affichage Firefox | Idem | Idem | ☐ | |
| UI-04 | Responsive ≥ 1024 px | Réduire fenêtre | Mise en page reste lisible | ☐ | |
| UI-05 | Icônes Material | Vérifier toutes icônes Radzen | Icônes correctes (pas de carrés) | ☐ | |
| UI-06 | Bordures formulaires | Inspecter inputs sur tous formulaires | Bordures complètes, focus teal | ☐ | |
| UI-07 | Impression A4 | Imprimer / aperçu sur tous écrans `*Print` | Marges respectées, contenu non tronqué | ☐ | |

---

## 7. Suivi des anomalies

| # | Cas test | Description | Sévérité | Statut | Date | Assigné à | Résolu le |
|---|---|---|---|---|---|---|---|
| 1 | | | | | | | |
| 2 | | | | | | | |

## 8. Décision finale

| Décision | Cocher | Commentaire |
|---|---|---|
| ✅ **Acceptée** — passage en production validé | ☐ | |
| ⚠️ **Acceptée sous réserve** — corrections mineures à livrer | ☐ | |
| ❌ **Refusée** — anomalies bloquantes à corriger puis nouveau cycle UAT | ☐ | |

**Signatures**

| Rôle | Nom | Date | Signature |
|---|---|---|---|
| Chef de projet métier | | | |
| Responsable IT | | | |
| Sponsor | | | |

---

## Annexe A — Comptes de test

| Login | Rôle | Mot de passe | Périmètre |
|---|---|---|---|
| `uat.admin` | Admin | _____________ | Toutes fonctions |
| `uat.approb1` | Approbateur N1 | _____________ | Département A |
| `uat.approb2` | Approbateur N2 | _____________ | Département A |
| `uat.demandeur` | Demandeur | _____________ | Création bons |
| `uat.magasinier` | Magasinier | _____________ | Sorties / réceptions |
| `uat.securite` | Agent Sécurité | _____________ | Scans / anomalies |

## Annexe B — Données de test

- **Sites** : Site KCC-1, Site KCC-2
- **Compagnies** : COMP-A (interne), COMP-B (sous-traitant)
- **Checkpoints** : Entrée principale, Sortie magasin, Barrière nord
- **Matériels** : 20 matériels variés (outils, équipements, EPI)
- **Bons** : 5 bons d'entrée + 5 bons de sortie pré-créés à divers statuts
