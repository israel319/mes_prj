namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Types d'anomalies pouvant être détectées lors d'un scan
/// </summary>
public enum TypeAnomalie
{
    /// <summary>
    /// Barrière inappropriée - La barrière scannée n'est pas dans l'itinéraire prévu
    /// </summary>
    BarriereInappropriee = 1,

    /// <summary>
    /// Document expiré - La date d'expiration du bon est dépassée
    /// </summary>
    DocumentExpire = 2,

    /// <summary>
    /// Document inexistant - Le QR Code ne correspond à aucun bon valide
    /// </summary>
    DocumentInexistant = 3,

    /// <summary>
    /// Tentative d'échange - Suspicion d'utilisation frauduleuse du document
    /// </summary>
    TentativeEchange = 4,

    /// <summary>
    /// Document déjà utilisé - QR Code déjà scanné et confirmé à cette barrière
    /// </summary>
    DocumentDejaUtilise = 5,

    /// <summary>
    /// Ordre de passage incorrect - Barrière scannée avant une barrière obligatoire précédente
    /// </summary>
    OrdrePassageIncorrect = 6,

    /// <summary>
    /// Statut invalide - Le bon n'est pas dans un statut permettant le passage
    /// </summary>
    StatutInvalide = 7
}
