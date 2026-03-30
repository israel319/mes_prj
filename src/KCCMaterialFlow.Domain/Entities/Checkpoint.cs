using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Checkpoint/Barrière pour suivi de passage.
/// Uses BaseEntity.Id directly (column name is Id).
/// </summary>
public sealed class Checkpoint : BaseEntity
{
    public int SiteId { get; set; }

    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Code { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool EstActif { get; set; } = true;
    public int OrdreDefaut { get; set; }

    public Site? Site { get; set; }
}
