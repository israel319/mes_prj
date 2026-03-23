using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonSortie.Entities;

/// <summary>
/// Représente une approbation dans le workflow d'un bon de sortie.
/// </summary>
public class ApprobationSortie
{
    /// <summary>
    /// Identifiant unique de l'approbation (clé primaire)
    /// </summary>
    [Key]
    public int IdApprobation { get; set; }

    /// <summary>
    /// Identifiant du bon de sortie concerné (clé étrangère)
    /// </summary>
    public int BonSortieId { get; set; }

    /// <summary>
    /// Ordre de l'étape dans le workflow
    /// </summary>
    public int OrdreEtape { get; set; }

    /// <summary>
    /// Code du rôle requis pour approuver cette étape (ex: "IT", "Superviseur", "GM").
    /// Stocké depuis WorkflowEtapeConfig.RoleCode au moment du submit — source de vérité pour l'autorisation.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// Nom lisible de l'étape (ex: "Département IT", "General Manager", "OPJ")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string NomEtape { get; set; } = string.Empty;

    /// <summary>
    /// Décision prise (Approuvé, Rejeté, En attente)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Decision { get; set; } = "En attente";

    /// <summary>
    /// Date et heure de l'action
    /// </summary>
    public DateTime? DateAction { get; set; }

    /// <summary>
    /// Login de l'approbateur
    /// </summary>
    [MaxLength(100)]
    public string? ApprobateurLogin { get; set; }

    /// <summary>
    /// Nom de l'approbateur
    /// </summary>
    [MaxLength(200)]
    public string? ApprobateurNom { get; set; }

    /// <summary>
    /// Commentaires ou réserves
    /// </summary>
    [MaxLength(1000)]
    public string? Commentaire { get; set; }

    /// <summary>
    /// Référence vers le bon de sortie parent
    /// </summary>
    public virtual BonSortie? BonSortie { get; set; }
}
