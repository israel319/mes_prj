namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Statuts possibles d'un scan de QR Code à une barrière
/// </summary>
public enum StatutScan
{
    /// <summary>
    /// Conforme - Scan valide, passage autorisé
    /// </summary>
    Conforme = 1,

    /// <summary>
    /// Anomalie - Problème détecté lors du scan
    /// </summary>
    Anomalie = 2,

    /// <summary>
    /// Expiré - Document dont la date de validité est dépassée
    /// </summary>
    Expire = 3,

    /// <summary>
    /// Hors Itinéraire - Barrière non prévue dans l'itinéraire du bon
    /// </summary>
    HorsItineraire = 4
}
