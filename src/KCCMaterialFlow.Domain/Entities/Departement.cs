using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Département (Host Department).
/// EF maps Id → IdDepartement column.
/// </summary>
public sealed class Departement : BaseEntity
{
    [MaxLength(20)]
    public string CodeDepartement { get; set; } = string.Empty;

    [MaxLength(200)]
    public string NomDepartement { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string ResponsableLogin { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ResponsableNom { get; set; }

    [MaxLength(200)]
    public string? ResponsableEmail { get; set; }

    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DateModification { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
