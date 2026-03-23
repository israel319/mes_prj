using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Module.BonEntree.Entities;

/// <summary>
/// Classe abstraite de base pour tous les types de bons (Entrée et Sortie).
/// Contient les propriétés communes selon le diagramme de classe.
/// </summary>
public abstract class Bon
{
    /// <summary>
    /// Identifiant unique du bon (clé primaire)
    /// </summary>
    [Key]
    public int IdBon { get; set; }

    /// <summary>
    /// Numéro de référence unique du bon (ex: BEM-2026-000001)
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

    // ===== PROPRIÉTÉS QR CODE =====

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
    public virtual ICollection<Materiel> Materiels { get; set; } = new List<Materiel>();

    /// <summary>
    /// Collection des approbations du workflow
    /// </summary>
    public virtual ICollection<Approbation> Approbations { get; set; } = new List<Approbation>();

    /// <summary>
    /// Collection des itinéraires prévus (barrières)
    /// </summary>
    public virtual ICollection<ItinerairePrevu> ItinerairesPrevu { get; set; } = new List<ItinerairePrevu>();

    /// <summary>
    /// Collection des historiques d'actions sur ce bon
    /// </summary>
    public virtual ICollection<BonEntreeHistory> Historiques { get; set; } = new List<BonEntreeHistory>();
}
