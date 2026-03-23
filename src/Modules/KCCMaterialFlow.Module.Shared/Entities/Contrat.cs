using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente un contrat lié à une compagnie.
/// Une compagnie peut avoir un ou plusieurs contrats.
/// </summary>
public class Contrat
{
    /// <summary>
    /// Identifiant unique du contrat (clé primaire)
    /// </summary>
    [Key]
    public int IdContrat { get; set; }

    /// <summary>
    /// Numéro de Purchase Order (PO Number)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string PoNumber { get; set; } = string.Empty;

    /// <summary>
    /// Description du contrat
    /// </summary>
    [MaxLength(500)]
    public string? ContratDescription { get; set; }

    /// <summary>
    /// ID de la compagnie propriétaire du contrat
    /// </summary>
    public int CompagnieId { get; set; }

    /// <summary>
    /// Navigation vers la compagnie
    /// </summary>
    public virtual Compagnie Compagnie { get; set; } = null!;

    /// <summary>
    /// Indique si le contrat est actif
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;
}
