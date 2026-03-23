using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonEntree.Entities;

/// <summary>
/// Représente une approbation dans le workflow selon le diagramme de classe.
/// </summary>
public class Approbation
{
    /// <summary>
    /// Identifiant unique de l'approbation (clé primaire)
    /// </summary>
    [Key]
    public int IdApprobation { get; set; }

    /// <summary>
    /// Identifiant du bon concerné (clé étrangère)
    /// </summary>
    public int BonId { get; set; }

    /// <summary>
    /// Ordre de l'étape dans le workflow
    /// </summary>
    public int OrdreEtape { get; set; }

    /// <summary>
    /// Nom de l'étape (ex: "Superviseur", "GM", "OPJ")
    /// </summary>
    [MaxLength(100)]
    public string? NomEtape { get; set; }

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
    /// Nom de l'approbateur qui a pris la décision
    /// </summary>
    [MaxLength(200)]
    public string? NomApprobateur { get; set; }

    /// <summary>
    /// Rôle de l'approbateur (Superviseur, GM, OPJ, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? RoleApprobateur { get; set; }

    /// <summary>
    /// Réserves éventuelles ou commentaire émis lors de l'approbation
    /// </summary>
    [MaxLength(1000)]
    public string? ReservesEventuelles { get; set; }

    /// <summary>
    /// Référence vers le bon parent
    /// </summary>
    public virtual Bon? Bon { get; set; }
}
