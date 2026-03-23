using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente un département (Host Department)
/// </summary>
public class Departement
{
    /// <summary>
    /// Identifiant unique du département (clé primaire)
    /// </summary>
    [Key]
    public int IdDepartement { get; set; }

    /// <summary>
    /// Code court du département (ex: "IT", "RH", "FIN")
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string CodeDepartement { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet du département
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string NomDepartement { get; set; } = string.Empty;

    /// <summary>
    /// Description du département
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Login du responsable du département
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ResponsableLogin { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet du responsable
    /// </summary>
    [MaxLength(200)]
    public string? ResponsableNom { get; set; }

    /// <summary>
    /// Email du responsable
    /// </summary>
    [MaxLength(200)]
    public string? ResponsableEmail { get; set; }

    /// <summary>
    /// Indique si le département est actif
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;

    /// <summary>
    /// Date de dernière modification
    /// </summary>
    public DateTime? DateModification { get; set; }

    /// <summary>
    /// Propriété de commodité pour Radzen DropDown (non mappée en base)
    /// </summary>
    public int Id => IdDepartement;

    /// <summary>
    /// Propriété de commodité pour Radzen DropDown (non mappée en base)
    /// </summary>
    public string Nom => NomDepartement;

    /// <summary>
    /// Employés de ce département
    /// </summary>
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
