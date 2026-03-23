namespace KCCMaterialFlow.Module.BonEntree.DTOs;

/// <summary>
/// DTO pour un itinéraire prévu - 2 champs selon diagramme UML
/// </summary>
public class ItinerairePrevuDto
{
    /// <summary>
    /// Identifiant
    /// </summary>
    public int IdItinerairePrevu { get; set; }

    /// <summary>
    /// Ordre de passage
    /// </summary>
    public int OrdrePassage { get; set; }

    /// <summary>
    /// Identifiant de la barrière
    /// </summary>
    public int BarriereId { get; set; }
}
