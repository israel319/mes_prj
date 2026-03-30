using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Étape d'itinéraire prévu (barrière) pour un bon d'entrée.
/// EF maps Id → IdItineraire column.
/// </summary>
public sealed class ItinerairePrevu : BaseEntity
{
    public int BonId { get; private set; }
    public int OrdrePassage { get; private set; }
    public int BarriereId { get; private set; }

    private ItinerairePrevu() { }

    public ItinerairePrevu(int bonId, int ordrePassage, int barriereId)
    {
        BonId = bonId;
        OrdrePassage = ordrePassage;
        BarriereId = barriereId;
    }
}
