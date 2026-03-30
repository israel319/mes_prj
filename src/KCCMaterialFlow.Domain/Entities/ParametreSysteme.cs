using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Paramètre système configurable.
/// EF maps Id → IdParametre column.
/// </summary>
public sealed class ParametreSysteme : BaseEntity
{
    [MaxLength(100)]
    public string Cle { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Valeur { get; set; } = string.Empty;

    [MaxLength(50)]
    public string TypeDonnee { get; set; } = "String";

    [MaxLength(50)]
    public string Categorie { get; set; } = "General";

    [MaxLength(200)]
    public string Libelle { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? ValeurDefaut { get; set; }

    [MaxLength(2000)]
    public string? ValeursPossibles { get; set; }

    public int? ValeurMin { get; set; }
    public int? ValeurMax { get; set; }

    [MaxLength(20)]
    public string? Unite { get; set; }

    public int Ordre { get; set; }
    public bool NecessiteRedemarrage { get; set; }
    public bool EstVisible { get; set; } = true;
    public bool EstModifiable { get; set; } = true;
    public bool EstSysteme { get; set; }

    [MaxLength(100)]
    public string? ModifieParLogin { get; set; }

    public DateTime? DateModification { get; set; }
}
