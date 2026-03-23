using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Module.BonSortie.Entities;

/// <summary>
/// Représente un Bon de Sortie Externe - matériel sortant du site KCC.
/// Peut être lié à un BonEntree existant (matériel contractant qui repart).
/// </summary>
public class BonSortieExterne : BonSortie
{
    /// <summary>
    /// Identifiant du BonEntree associé (optionnel - relation 0..1)
    /// Utilisé quand le matériel d'un contractant repart après sa mission.
    /// </summary>
    public int? BonEntreeAssocieId { get; set; }

    /// <summary>
    /// Type de matériel concerné (pour orientation vers le bon approbateur)
    /// </summary>
    [Required]
    public TypeMateriel TypeMateriel { get; set; } = TypeMateriel.Autre;

    /// <summary>
    /// Nom de la compagnie/destinataire externe
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string NomDestinataire { get; set; } = string.Empty;

    /// <summary>
    /// Adresse de destination externe
    /// </summary>
    [MaxLength(500)]
    public string? AdresseDestination { get; set; }

    /// <summary>
    /// Numéro de véhicule transportant le matériel
    /// </summary>
    [MaxLength(50)]
    public string? NumeroVehicule { get; set; }

    /// <summary>
    /// Nom du chauffeur/transporteur
    /// </summary>
    [MaxLength(200)]
    public string? NomChauffeur { get; set; }

    /// <summary>
    /// Numéro de téléphone du chauffeur
    /// </summary>
    [MaxLength(50)]
    public string? TelephoneChauffeur { get; set; }

    /// <summary>
    /// Navigation vers le BonEntree associé (si applicable)
    /// </summary>
    public virtual KCCMaterialFlow.Module.BonEntree.Entities.BonEntree? BonEntreeAssocie { get; set; }
}
