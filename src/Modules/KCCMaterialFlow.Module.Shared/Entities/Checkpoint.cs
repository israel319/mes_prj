using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Checkpoint/Barriere que le bon doit traverser.
/// Utilisé pour le suivi et la détection d'anomalies.
/// NOTE: Sera fusionné dans l'entité Barriere lors de la Phase 2.
/// </summary>
public class Checkpoint : IEntity
{
    public int Id { get; set; }

    /// <summary>
    /// Id du site (barrière)
    /// </summary>
    public int SiteId { get; set; }

    /// <summary>
    /// Nom du checkpoint (ex: Barriere KTO, Barriere SKM)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Code court (ex: CHK-KTO)
    /// </summary>
    [MaxLength(20)]
    public string? Code { get; set; }

    /// <summary>
    /// Description ou instructions
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Indique si le checkpoint est actif
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Ordre par défaut dans l'itinéraire
    /// </summary>
    public int OrdreDefaut { get; set; } = 0;

    /// <summary>
    /// Navigation vers le site
    /// </summary>
    public virtual Site? Site { get; set; }
}
