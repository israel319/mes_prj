namespace KCCMaterialFlow.Application.Features.BonEntree.DTOs;

/// <summary>
/// DTO pour un itinéraire prévu - 2 champs selon diagramme UML
/// </summary>
public class ItinerairePrevuDto
{
    public int IdItinerairePrevu { get; set; }
    public int OrdrePassage { get; set; }
    public int BarriereId { get; set; }
}
