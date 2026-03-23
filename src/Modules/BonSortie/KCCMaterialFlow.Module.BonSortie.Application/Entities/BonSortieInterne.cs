using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Module.BonSortie.Entities;

/// <summary>
/// Représente un Bon de Sortie Interne - transfert de matériel entre départements KCC.
/// Le matériel reste sur le site mais change de département/localisation.
/// </summary>
public class BonSortieInterne : BonSortie
{
    /// <summary>
    /// Identifiant du BonEntree associé (obligatoire - traçabilité matériel)
    /// </summary>
    public int? BonEntreeAssocieId { get; set; }

    /// <summary>
    /// Type de matériel concerné (pour orientation vers le bon approbateur)
    /// </summary>
    [Required]
    public TypeMateriel TypeMateriel { get; set; } = TypeMateriel.Autre;

    /// <summary>
    /// Département d'origine du matériel
    /// </summary>
    [MaxLength(100)]
    public string? DepartementOrigine { get; set; }

    /// <summary>
    /// Fonction du responsable receveur
    /// </summary>
    [MaxLength(150)]
    public string? FonctionReceveur { get; set; }

    /// <summary>
    /// Email du responsable receveur pour notification
    /// </summary>
    [MaxLength(200)]
    public string? EmailReceveur { get; set; }

    /// <summary>
    /// Localisation précise de destination (bâtiment, bureau, etc.)
    /// </summary>
    [MaxLength(200)]
    public string? LocalisationDestination { get; set; }

    /// <summary>
    /// Date prévue du transfert
    /// </summary>
    public DateTime? DateTransfertPrevue { get; set; }

    /// <summary>
    /// Date effective du transfert (renseignée après confirmation)
    /// </summary>
    public DateTime? DateTransfertEffective { get; set; }
}
