using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente une compagnie/société (contractor, sous-traitant, etc.)
/// </summary>
public class Compagnie
{
    /// <summary>
    /// Identifiant unique de la compagnie (clé primaire)
    /// </summary>
    [Key]
    public int IdCompagnie { get; set; }

    /// <summary>
    /// Nom de la compagnie
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Code court de la compagnie (optionnel)
    /// </summary>
    [MaxLength(20)]
    public string? Code { get; set; }

    /// <summary>
    /// Email de contact
    /// </summary>
    [MaxLength(200)]
    public string? Email { get; set; }

    /// <summary>
    /// Téléphone de contact
    /// </summary>
    [MaxLength(50)]
    public string? Telephone { get; set; }

    /// <summary>
    /// Nom du Site Manager de la compagnie
    /// </summary>
    [MaxLength(200)]
    public string? SiteManager { get; set; }

    /// <summary>
    /// Indique si la compagnie est active
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;

    /// <summary>
    /// Contrats de cette compagnie
    /// </summary>
    public virtual ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();

    /// <summary>
    /// Employés de cette compagnie
    /// </summary>
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
