using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonSortie.Entities;

/// <summary>
/// Historique des actions effectuées sur un bon de sortie.
/// Permet de tracer toutes les modifications et transitions de statut.
/// </summary>
public class BonSortieHistory
{
    /// <summary>
    /// Identifiant unique de l'entrée d'historique
    /// </summary>
    [Key]
    public int IdHistory { get; set; }

    /// <summary>
    /// Identifiant du bon de sortie concerné
    /// </summary>
    public int BonSortieId { get; set; }

    /// <summary>
    /// Type d'action effectuée (Création, Modification, Soumission, Approbation, Rejet, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TypeAction { get; set; } = string.Empty;

    /// <summary>
    /// Statut avant l'action
    /// </summary>
    [MaxLength(50)]
    public string? StatutAvant { get; set; }

    /// <summary>
    /// Statut après l'action
    /// </summary>
    [MaxLength(50)]
    public string? StatutApres { get; set; }

    /// <summary>
    /// Description détaillée de l'action
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Login de l'utilisateur ayant effectué l'action
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UtilisateurLogin { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet de l'utilisateur
    /// </summary>
    [MaxLength(200)]
    public string? UtilisateurNom { get; set; }

    /// <summary>
    /// Date et heure de l'action
    /// </summary>
    public DateTime DateAction { get; set; } = DateTime.Now;

    /// <summary>
    /// Adresse IP de l'utilisateur (pour audit)
    /// </summary>
    [MaxLength(50)]
    public string? AdresseIP { get; set; }

    /// <summary>
    /// Navigation vers le bon de sortie
    /// </summary>
    public virtual BonSortie? BonSortie { get; set; }
}
