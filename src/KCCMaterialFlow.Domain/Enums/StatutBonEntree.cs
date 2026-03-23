namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Statuts possibles d'un bon d'entrée de matériel dans le workflow.
/// </summary>
public enum StatutBonEntree
{
    /// <summary>
    /// Brouillon - Le bon est en cours de création, pas encore soumis.
    /// </summary>
    Brouillon = 0,

    /// <summary>
    /// En attente de validation par le responsable du département hôte.
    /// </summary>
    EnAttenteDepartement = 10,

    /// <summary>
    /// En attente de validation par le département Sécurité.
    /// </summary>
    EnAttenteSecurite = 20,

    /// <summary>
    /// Approuvé - Le bon a été validé par tous les niveaux requis.
    /// </summary>
    Approuve = 30,

    /// <summary>
    /// Retourné - Le bon a été retourné au demandeur pour corrections.
    /// </summary>
    Retourne = 40,

    /// <summary>
    /// Rejeté - Le bon a été définitivement rejeté.
    /// </summary>
    Rejete = 50,

    /// <summary>
    /// Annulé - Le bon a été annulé par le demandeur ou un administrateur.
    /// </summary>
    Annule = 60,

    /// <summary>
    /// Expiré - Le bon a dépassé sa date d'expiration.
    /// </summary>
    Expire = 70,

    /// <summary>
    /// Clôturé - Tous les matériels sont ressortis et le bon est fermé.
    /// </summary>
    Cloture = 80
}
