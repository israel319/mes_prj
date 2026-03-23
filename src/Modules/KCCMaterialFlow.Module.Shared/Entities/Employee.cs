using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente un employé (escorteur, demandeur, etc.)
/// </summary>
public class Employee
{
    /// <summary>
    /// Identifiant unique de l'employé (clé primaire)
    /// </summary>
    [Key]
    public int IdEmployee { get; set; }

    /// <summary>
    /// Matricule de l'employé
    /// </summary>
    [MaxLength(50)]
    public string? Matricule { get; set; }

    /// <summary>
    /// Nom complet de l'employé
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string NomComplet { get; set; } = string.Empty;

    /// <summary>
    /// Prénom
    /// </summary>
    [MaxLength(100)]
    public string? Prenom { get; set; }

    /// <summary>
    /// Nom de famille
    /// </summary>
    [MaxLength(100)]
    public string? Nom { get; set; }

    /// <summary>
    /// Fonction/Poste de l'employé
    /// </summary>
    [MaxLength(200)]
    public string? Fonction { get; set; }

    /// <summary>
    /// Email professionnel
    /// </summary>
    [MaxLength(200)]
    public string? Email { get; set; }

    /// <summary>
    /// Téléphone
    /// </summary>
    [MaxLength(50)]
    public string? Telephone { get; set; }

    /// <summary>
    /// ID du département
    /// </summary>
    public int? DepartementId { get; set; }

    /// <summary>
    /// Département de l'employé
    /// </summary>
    public virtual Departement? Departement { get; set; }

    /// <summary>
    /// ID de la compagnie (pour les contracteurs)
    /// </summary>
    public int? CompagnieId { get; set; }

    /// <summary>
    /// Compagnie de l'employé (si contracteur)
    /// </summary>
    public virtual Compagnie? Compagnie { get; set; }

    /// <summary>
    /// Indique si c'est un employé KCC (interne) ou contracteur (externe)
    /// </summary>
    public bool EstInterne { get; set; } = true;

    /// <summary>
    /// Indique si l'employé peut être escorteur
    /// </summary>
    public bool PeutEtreEscorteur { get; set; } = false;

    /// <summary>
    /// Indique si l'employé est actif
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Login Windows/AD (si applicable)
    /// </summary>
    [MaxLength(100)]
    public string? Login { get; set; }

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;
}
