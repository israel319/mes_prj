namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Types de matériel pour les bons de sortie (détermine le workflow d'approbation)
/// </summary>
public enum TypeMateriel
{
    /// <summary>
    /// Circulaire - Validité 6 mois, pas de lien bon d'entrée obligatoire
    /// Workflow: Standard
    /// </summary>
    Circulaire = 1,

    /// <summary>
    /// Matériel Informatique - Nécessite approbation IT
    /// Workflow: + Département IT
    /// </summary>
    Informatique = 2,

    /// <summary>
    /// Fin de Chantier - Contrat terminé, matériel usagé, déclassé, conteneur
    /// Workflow: Standard
    /// </summary>
    FinChantier = 3,

    /// <summary>
    /// Résidu / Déchet - Emballage, matériel désaffecté
    /// Workflow: + Département Environnement
    /// </summary>
    Residu = 4,

    /// <summary>
    /// Radioprotection - Matériel troxilaire
    /// Workflow: + Département Environnement
    /// </summary>
    Radioprotection = 5,

    /// <summary>
    /// Modification - Réparation, rénovation
    /// Workflow: + Département Environnement
    /// </summary>
    Modification = 6,

    /// <summary>
    /// Mise en prêt - Date de retour obligatoire, alertes automatiques
    /// Workflow: Standard + Gestion retour
    /// </summary>
    Pret = 7,

    /// <summary>
    /// Autre type de matériel non catégorisé
    /// Workflow: Standard
    /// </summary>
    Autre = 99
}
