using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente un statut possible pour les bons (entrée/sortie).
/// Permet de personnaliser les statuts avec couleurs et workflows.
/// </summary>
public class Statut
{
    /// <summary>
    /// Identifiant unique du statut (clé primaire)
    /// </summary>
    [Key]
    public int IdStatut { get; set; }

    /// <summary>
    /// Code technique du statut (ex: "EN_ATTENTE", "APPROUVE", "REJETE")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string CodeStatut { get; set; } = string.Empty;

    /// <summary>
    /// Libellé affiché du statut (ex: "En attente", "Approuvé", "Rejeté")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LibelleStatut { get; set; } = string.Empty;

    /// <summary>
    /// Description du statut
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Type de bon concerné (BonEntree, BonSortie, Tous)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TypeBon { get; set; } = "Tous";

    /// <summary>
    /// Couleur de fond pour l'affichage (format hex ex: "#28a745")
    /// </summary>
    [MaxLength(20)]
    public string CouleurFond { get; set; } = "#6c757d";

    /// <summary>
    /// Couleur du texte pour l'affichage (format hex ex: "#ffffff")
    /// </summary>
    [MaxLength(20)]
    public string CouleurTexte { get; set; } = "#ffffff";

    /// <summary>
    /// Icône associée au statut (classe CSS ex: "bi-check-circle")
    /// </summary>
    [MaxLength(50)]
    public string? Icone { get; set; }

    /// <summary>
    /// Ordre d'affichage dans les listes et workflows
    /// </summary>
    public int Ordre { get; set; } = 0;

    /// <summary>
    /// Indique si ce statut est un statut final (terminé)
    /// </summary>
    public bool EstFinal { get; set; } = false;

    /// <summary>
    /// Indique si ce statut requiert une action utilisateur
    /// </summary>
    public bool RequiertAction { get; set; } = false;

    /// <summary>
    /// Statuts suivants possibles (IDs séparés par virgule)
    /// </summary>
    [MaxLength(200)]
    public string? StatutsSuivants { get; set; }

    /// <summary>
    /// Indique si le statut est actif
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Indique si c'est un statut système non modifiable
    /// </summary>
    public bool EstSysteme { get; set; } = false;

    /// <summary>
    /// Date de création de l'enregistrement
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;

    /// <summary>
    /// Date de dernière modification
    /// </summary>
    public DateTime? DateModification { get; set; }
}
