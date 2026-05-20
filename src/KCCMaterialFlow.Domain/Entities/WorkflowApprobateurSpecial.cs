using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Approbateur spécial dans la chaîne d'approbation des bons (Superintendent, OPJ, Identification).
/// Plusieurs lignes possibles par Type (ex. équipe Identification = 2 employés).
/// </summary>
public sealed class WorkflowApprobateurSpecial : BaseEntity
{
    public TypeApprobateurSpecial Type { get; set; }

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    /// <summary>Site auquel cet approbateur s'applique. NULL = global (tous sites).</summary>
    public int? SiteId { get; set; }
    public Site? Site { get; set; }

    /// <summary>Ordre d'apparition dans le Type (utile si plusieurs membres pour Identification).</summary>
    public int Ordre { get; set; } = 1;

    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;
}
