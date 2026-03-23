namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Types d'actions possibles sur un bon d'entrée
/// </summary>
public enum ActionBonEntree
{
    /// <summary>
    /// Création du bon
    /// </summary>
    Creation = 1,

    /// <summary>
    /// Modification des informations
    /// </summary>
    Modification = 2,

    /// <summary>
    /// Soumission pour approbation
    /// </summary>
    Soumission = 3,

    /// <summary>
    /// Approbation par un approbateur
    /// </summary>
    Approbation = 4,

    /// <summary>
    /// Rejet par un approbateur
    /// </summary>
    Rejet = 5,

    /// <summary>
    /// Retour pour modification
    /// </summary>
    RetourModification = 6,

    /// <summary>
    /// Génération du QR Code
    /// </summary>
    GenerationQR = 7,

    /// <summary>
    /// Scan à une barrière
    /// </summary>
    ScanBarriere = 8,

    /// <summary>
    /// Entrée effective sur le site
    /// </summary>
    EntreeEffective = 9,

    /// <summary>
    /// Sortie d'un matériel
    /// </summary>
    SortieMateriel = 10,

    /// <summary>
    /// Clôture du bon (tous matériels ressortis)
    /// </summary>
    Cloture = 11,

    /// <summary>
    /// Annulation du bon
    /// </summary>
    Annulation = 12,

    /// <summary>
    /// Prolongation de la validité
    /// </summary>
    Prolongation = 13,

    /// <summary>
    /// Ajout d'un matériel
    /// </summary>
    AjoutMateriel = 14,

    /// <summary>
    /// Suppression d'un matériel
    /// </summary>
    SuppressionMateriel = 15,

    /// <summary>
    /// Signalement d'une anomalie
    /// </summary>
    Anomalie = 16,

    /// <summary>
    /// Impression du bon ou QR
    /// </summary>
    Impression = 17,

    /// <summary>
    /// Envoi de notification
    /// </summary>
    Notification = 18
}
