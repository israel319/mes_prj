using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Type de matériel configurable avec workflows.
/// Named TypeMaterielEntity to avoid conflict with TypeMateriel enum.
/// EF maps to the same table as before. Id → IdTypeMateriel column.
/// </summary>
public sealed class TypeMaterielEntity : BaseEntity
{
    [MaxLength(50)]
    public string CodeType { get; set; } = string.Empty;

    [MaxLength(150)]
    public string NomType { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Categorie { get; set; }

    [MaxLength(50)]
    public string? Icone { get; set; }

    [MaxLength(20)]
    public string? Couleur { get; set; }

    public bool RequiertApprobationDepartement { get; set; } = true;
    public bool RequiertApprobationDirection { get; set; }
    public int NiveauxApprobation { get; set; } = 1;
    public int DureeValiditeDefautJours { get; set; } = 30;
    public int DureeMaximumJours { get; set; } = 365;
    public bool NumeroSerieObligatoire { get; set; }
    public bool PhotoObligatoire { get; set; }

    [MaxLength(2000)]
    public string? ChampsPersonnalises { get; set; }

    [MaxLength(4000)]
    public string? WorkflowConfig { get; set; }

    public int Ordre { get; set; }
    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DateModification { get; set; }
}
