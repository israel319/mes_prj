using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Employé (escorteur, demandeur, etc.).
/// EF maps Id → IdEmployee column.
/// </summary>
public sealed class Employee : BaseEntity
{
    [MaxLength(50)]
    public string? Matricule { get; set; }

    [MaxLength(200)]
    public string NomComplet { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Prenom { get; set; }

    [MaxLength(100)]
    public string? Nom { get; set; }

    [MaxLength(200)]
    public string? Fonction { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Telephone { get; set; }

    public int? DepartementId { get; set; }
    public Departement? Departement { get; set; }

    public int? CompagnieId { get; set; }
    public Compagnie? Compagnie { get; set; }

    public bool EstInterne { get; set; } = true;
    public bool PeutEtreEscorteur { get; set; }
    public bool EstActif { get; set; } = true;

    [MaxLength(100)]
    public string? Login { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;
}
