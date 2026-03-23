using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Securite.Entities;

/// <summary>
/// Représente un événement de scan QR effectué à une barrière.
/// Enregistre chaque passage (entrée ou sortie) d'un bon/matériel.
/// </summary>
public class ScanEvenement
{
    /// <summary>
    /// Identifiant unique du scan (clé primaire)
    /// </summary>
    [Key]
    public int IdScan { get; set; }

    /// <summary>
    /// Date et heure du scan
    /// </summary>
    public DateTime DateHeureScan { get; set; } = DateTime.Now;

    /// <summary>
    /// Statut du scan : Valid, Invalid, Expired, AlreadyUsed, NotFound
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string StatutScan { get; set; } = "Valid";

    /// <summary>
    /// Type de mouvement : Entree, Sortie
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string TypeMouvement { get; set; } = "Sortie";

    /// <summary>
    /// Identifiant du bon scanné (FK vers Bon ou BonSortie)
    /// </summary>
    public int? BonId { get; set; }

    /// <summary>
    /// Type de bon : BEM (Bon d'Entrée) ou BSM (Bon de Sortie)
    /// </summary>
    [MaxLength(10)]
    public string? TypeBon { get; set; }

    /// <summary>
    /// Numéro de référence du bon scanné (pour affichage)
    /// </summary>
    [MaxLength(30)]
    public string? NumeroReferenceBon { get; set; }

    /// <summary>
    /// Identifiant de la barrière où le scan a eu lieu (FK vers Barriere)
    /// </summary>
    public int BarriereId { get; set; }

    /// <summary>
    /// Login de l'agent de sécurité qui a effectué le scan
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string AgentLogin { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet de l'agent (pour affichage)
    /// </summary>
    [MaxLength(200)]
    public string? AgentNom { get; set; }

    /// <summary>
    /// Données brutes du QR Code scanné
    /// </summary>
    [MaxLength(1000)]
    public string? QRCodeData { get; set; }

    /// <summary>
    /// Hash de vérification du QR Code
    /// </summary>
    [MaxLength(128)]
    public string? QRCodeHash { get; set; }

    /// <summary>
    /// Message d'erreur ou d'information sur le scan
    /// </summary>
    [MaxLength(500)]
    public string? Message { get; set; }

    /// <summary>
    /// Observations de l'agent de sécurité
    /// </summary>
    [MaxLength(1000)]
    public string? Observations { get; set; }

    /// <summary>
    /// Indique si une anomalie a été signalée pour ce scan
    /// </summary>
    public bool AnomalieSignalee { get; set; } = false;

    /// <summary>
    /// Numéro de la preuve de passage générée
    /// </summary>
    [MaxLength(50)]
    public string? NumeroPreuve { get; set; }

    /// <summary>
    /// Lieu de provenance du matériel
    /// </summary>
    [MaxLength(200)]
    public string? ProvenanceLieu { get; set; }

    /// <summary>
    /// Lieu de destination du matériel
    /// </summary>
    [MaxLength(200)]
    public string? DestinationLieu { get; set; }

    /// <summary>
    /// Navigation vers la barrière
    /// </summary>
    public virtual Shared.Entities.Barriere? Barriere { get; set; }

    /// <summary>
    /// Collection des anomalies liées à ce scan
    /// </summary>
    public virtual ICollection<Anomalie> Anomalies { get; set; } = new List<Anomalie>();
}

/// <summary>
/// Statuts possibles d'un scan
/// </summary>
public static class StatutScanValues
{
    public const string Valid = "Valid";
    public const string Invalid = "Invalid";
    public const string Expired = "Expired";
    public const string AlreadyUsed = "AlreadyUsed";
    public const string NotFound = "NotFound";
    public const string Unauthorized = "Unauthorized";
}

/// <summary>
/// Types de mouvement
/// </summary>
public static class TypeMouvementValues
{
    public const string Entree = "Entree";
    public const string Sortie = "Sortie";
}
