using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonEntree.Entities;

/// <summary>
/// Représente une étape de l'itinéraire prévu selon le diagramme de classe.
/// </summary>
public class ItinerairePrevu
{
    /// <summary>
    /// Identifiant unique de l'étape d'itinéraire (clé primaire)
    /// </summary>
    [Key]
    public int IdItineraire { get; set; }

    /// <summary>
    /// Identifiant du bon concerné (clé étrangère)
    /// </summary>
    public int BonId { get; set; }

    /// <summary>
    /// Ordre de passage à cette barrière
    /// </summary>
    public int OrdrePassage { get; set; }

    /// <summary>
    /// Identifiant de la barrière (clé étrangère vers Barriere)
    /// </summary>
    public int BarriereId { get; set; }

    /// <summary>
    /// Référence vers le bon parent
    /// </summary>
    public virtual Bon? Bon { get; set; }
}
