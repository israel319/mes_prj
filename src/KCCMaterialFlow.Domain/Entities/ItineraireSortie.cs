using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Étape d'itinéraire prévu (barrière) pour un bon de sortie.
/// EF maps Id → IdItineraire column.
/// </summary>
public sealed class ItineraireSortie : BaseEntity
{
    public int BonId { get; set; }
    public int OrdrePassage { get; set; }
    public int BarriereId { get; set; }

    [System.ComponentModel.DataAnnotations.MaxLength(50)]
    public string StatutPassage { get; set; } = "Prévu";
    public DateTime? DatePassageEffective { get; set; }

    private ItineraireSortie() { }

    public ItineraireSortie(int bonId, int ordrePassage, int barriereId)
    {
        BonId = bonId;
        OrdrePassage = ordrePassage;
        BarriereId = barriereId;
    }
}
