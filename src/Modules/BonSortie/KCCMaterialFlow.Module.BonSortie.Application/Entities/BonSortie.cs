using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonSortie.Entities;

/// <summary>
/// Représente un Bon de Sortie Matériel (BSM) - SEC-FM-141(A).
/// Classe de base pour les sorties externes et internes.
/// Entité indépendante avec ses propres propriétés et collections.
/// </summary>
public abstract class BonSortie
{
    /// <summary>
    /// Identifiant unique du bon (clé primaire)
    /// </summary>
    [Key]
    public int IdBon { get; set; }

    /// <summary>
    /// Numéro de référence unique du bon (ex: BSE-2026-000001)
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string NumeroReference { get; set; } = string.Empty;

    /// <summary>
    /// Date de création du bon
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;

    /// <summary>
    /// Date d'expiration du bon (validité)
    /// </summary>
    public DateTime DateExpiration { get; set; }

    /// <summary>
    /// Statut actuel du bon dans le workflow
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string StatutActuel { get; set; } = "Draft";

    /// <summary>
    /// Lieu de destination du matériel (TO)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Destination { get; set; } = string.Empty;

    /// <summary>
    /// Lieu de provenance du matériel (FROM)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Provenance { get; set; } = string.Empty;

    /// <summary>
    /// Description générale / observations
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Quantité totale de matériels (calculée)
    /// </summary>
    public int Quantite { get; set; }

    /// <summary>
    /// Nom complet du demandeur de la sortie
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string NomDemandeur { get; set; } = string.Empty;

    /// <summary>
    /// Fonction/titre du demandeur dans l'entreprise
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string FonctionDemandeur { get; set; } = string.Empty;

    /// <summary>
    /// Département du demandeur
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DepartementDemandeur { get; set; } = string.Empty;

    /// <summary>
    /// Login de l'utilisateur qui a créé le bon (propriétaire)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string CreatedByLogin { get; set; } = string.Empty;

    /// <summary>
    /// Motif de la sortie du matériel (raison de la demande)
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string MotifSortie { get; set; } = string.Empty;

    /// <summary>
    /// Code de la raison/type de sortie (ex: FIN_CHANTIER, PRET, INFORMATIQUE, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? RaisonSortieCode { get; set; }

    /// <summary>
    /// Indique si la sortie est définitive ou temporaire (prêt)
    /// </summary>
    public bool EstDefinitif { get; set; } = true;

    // ===== PROPRIÉTÉS QR CODE (BSM-030) =====

    /// <summary>
    /// Données encodées dans le QR Code (JSON: IdBon, Type, Reference, Hash)
    /// </summary>
    [MaxLength(500)]
    public string? QRCodeData { get; set; }

    /// <summary>
    /// Image du QR Code encodée en Base64
    /// </summary>
    public string? QRCodeBase64 { get; set; }

    /// <summary>
    /// Hash de vérification du QR Code
    /// </summary>
    [MaxLength(128)]
    public string? QRCodeHash { get; set; }

    /// <summary>
    /// Date et heure de génération du QR Code
    /// </summary>
    public DateTime? DateGenerationQR { get; set; }

    // ===== COLLECTIONS =====

    /// <summary>
    /// Collection des matériels associés à ce bon
    /// </summary>
    public virtual ICollection<MaterielSortie> Materiels { get; set; } = new List<MaterielSortie>();

    /// <summary>
    /// Collection des approbations du workflow
    /// </summary>
    public virtual ICollection<ApprobationSortie> Approbations { get; set; } = new List<ApprobationSortie>();

    /// <summary>
    /// Collection des itinéraires prévus (barrières)
    /// </summary>
    public virtual ICollection<ItineraireSortie> Itineraires { get; set; } = new List<ItineraireSortie>();

    /// <summary>
    /// Collection des historiques d'actions sur ce bon
    /// </summary>
    public virtual ICollection<BonSortieHistory> Historiques { get; set; } = new List<BonSortieHistory>();
}
