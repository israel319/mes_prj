using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Compagnie/société (contractor, sous-traitant).
/// EF maps Id → IdCompagnie column.
/// </summary>
public sealed class Compagnie : BaseEntity
{
    [MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Code { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Telephone { get; set; }

    [MaxLength(200)]
    public string? SiteManager { get; set; }

    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;

    public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
