using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Enregistre le passage d'un bon a un checkpoint.
/// Permet la detection d'anomalies si l'ordre n'est pas respecte.
/// NOTE: Phase 3 - sera refactorisé avec le nouveau modèle Barriere.
/// </summary>
public class PassageCheckpoint : IEntity
{
    public int Id { get; set; }

    /// <summary>
    /// Type de bon (BEM ou BSM)
    /// </summary>
    [MaxLength(10)]
    public string TypeBon { get; set; } = "BSM";

    /// <summary>
    /// Id du bon (BonEntree ou BonSortie)
    /// </summary>
    public int BonId { get; set; }

    /// <summary>
    /// Numero de reference du bon
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string NumeroReference { get; set; } = string.Empty;

    /// <summary>
    /// Id du checkpoint prevu
    /// </summary>
    public int CheckpointId { get; set; }

    /// <summary>
    /// Ordre prevu dans l'itineraire (1, 2, 3...)
    /// </summary>
    public int OrdrePrevu { get; set; }

    /// <summary>
    /// Ordre effectif de passage (peut differer si anomalie)
    /// </summary>
    public int? OrdreEffectif { get; set; }

    /// <summary>
    /// Date et heure prevue de passage
    /// </summary>
    public DateTime? DatePrevue { get; set; }

    /// <summary>
    /// Date et heure effective de passage (scan du QR code)
    /// </summary>
    public DateTime? DateEffective { get; set; }

    /// <summary>
    /// Statut du passage
    /// </summary>
    public StatutPassage Statut { get; set; } = StatutPassage.Prevu;

    /// <summary>
    /// Login de l'agent qui a scanne le QR code
    /// </summary>
    [MaxLength(100)]
    public string? ScannePar { get; set; }

    /// <summary>
    /// Indique si une anomalie a ete detectee
    /// </summary>
    public bool EstAnomalie { get; set; } = false;

    /// <summary>
    /// Type d'anomalie detectee
    /// </summary>
    public TypeAnomalie? TypeAnomalie { get; set; }

    /// <summary>
    /// Description de l'anomalie
    /// </summary>
    [MaxLength(1000)]
    public string? DescriptionAnomalie { get; set; }

    /// <summary>
    /// Observations de l'agent
    /// </summary>
    [MaxLength(1000)]
    public string? Observations { get; set; }

    /// <summary>
    /// Coordonnees GPS du scan (optionnel)
    /// </summary>
    [MaxLength(100)]
    public string? CoordonneeGPS { get; set; }

    /// <summary>
    /// Navigation vers le checkpoint
    /// </summary>
    public virtual Checkpoint? Checkpoint { get; set; }
}
