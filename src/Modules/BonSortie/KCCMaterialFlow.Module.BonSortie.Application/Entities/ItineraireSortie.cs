using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonSortie.Entities;

/// <summary>
/// Représente un itinéraire prévu (barrière) pour un bon de sortie.
/// </summary>
public class ItineraireSortie
{
    /// <summary>
    /// Identifiant unique (clé primaire)
    /// </summary>
    [Key]
    public int IdItineraire { get; set; }

    /// <summary>
    /// Identifiant du bon de sortie (clé étrangère)
    /// </summary>
    public int BonSortieId { get; set; }

    /// <summary>
    /// Identifiant de la barrière
    /// </summary>
    public int BarriereId { get; set; }

    /// <summary>
    /// Ordre de passage
    /// </summary>
    public int OrdrePassage { get; set; }

    /// <summary>
    /// Date et heure de passage prévue
    /// </summary>
    public DateTime? DatePassagePrevue { get; set; }

    /// <summary>
    /// Date et heure de passage effective
    /// </summary>
    public DateTime? DatePassageEffective { get; set; }

    /// <summary>
    /// Statut du passage (Prévu, Passé, Anomalie)
    /// </summary>
    [MaxLength(50)]
    public string StatutPassage { get; set; } = "Prévu";

    /// <summary>
    /// Observations lors du passage
    /// </summary>
    [MaxLength(500)]
    public string? Observations { get; set; }

    /// <summary>
    /// Référence vers le bon de sortie
    /// </summary>
    public virtual BonSortie? BonSortie { get; set; }
}
