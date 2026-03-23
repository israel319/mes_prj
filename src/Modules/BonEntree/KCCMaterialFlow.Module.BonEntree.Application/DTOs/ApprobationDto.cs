namespace KCCMaterialFlow.Module.BonEntree.DTOs;

/// <summary>
/// DTO pour une approbation - 4 champs selon diagramme UML
/// </summary>
public class ApprobationDto
{
    /// <summary>
    /// Identifiant
    /// </summary>
    public int IdApprobation { get; set; }

    /// <summary>
    /// Ordre de l'étape dans le workflow
    /// </summary>
    public int OrdreEtape { get; set; }

    /// <summary>
    /// Décision (En attente, Approuvé, Rejeté)
    /// </summary>
    public string Decision { get; set; } = "En attente";

    /// <summary>
    /// Date de l'action
    /// </summary>
    public DateTime? DateAction { get; set; }

    /// <summary>
    /// Réserves éventuelles
    /// </summary>
    public string? ReservesEventuelles { get; set; }
}
