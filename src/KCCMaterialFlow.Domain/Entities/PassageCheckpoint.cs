using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Passage d'un bon à un checkpoint.
/// Uses BaseEntity.Id directly (column name is Id).
/// </summary>
public sealed class PassageCheckpoint : BaseEntity
{
    [MaxLength(10)]
    public string TypeBon { get; set; } = "BSM";

    public int BonId { get; set; }

    [MaxLength(50)]
    public string NumeroReference { get; set; } = string.Empty;

    public int CheckpointId { get; set; }
    public int OrdrePrevu { get; set; }
    public int? OrdreEffectif { get; set; }
    public DateTime? DatePrevue { get; set; }
    public DateTime? DateEffective { get; set; }
    public StatutPassage Statut { get; set; } = StatutPassage.Prevu;

    [MaxLength(100)]
    public string? ScannePar { get; set; }

    public bool EstAnomalie { get; set; }
    public TypeAnomalie? TypeAnomalie { get; set; }

    [MaxLength(1000)]
    public string? DescriptionAnomalie { get; set; }

    [MaxLength(1000)]
    public string? Observations { get; set; }

    [MaxLength(100)]
    public string? CoordonneeGPS { get; set; }

    public Checkpoint? Checkpoint { get; set; }
}
