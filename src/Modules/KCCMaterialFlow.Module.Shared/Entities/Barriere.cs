using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente une barrière de sécurité (point de contrôle) de l'entreprise.
/// Les agents de sécurité sont affectés à une barrière pour contrôler les entrées/sorties.
/// </summary>
public class Barriere
{
    /// <summary>
    /// Identifiant unique de la barrière (clé primaire)
    /// </summary>
    [Key]
    public int IdBarriere { get; set; }

    /// <summary>
    /// Code unique de la barrière (ex: "BAR-01", "GATE-MAIN")
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string CodeBarriere { get; set; } = string.Empty;

    /// <summary>
    /// Nom de la barrière (ex: "Barrière Principale", "Entrée Nord")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string NomBarriere { get; set; } = string.Empty;

    /// <summary>
    /// Localisation physique de la barrière (ex: "Entrée principale - Bâtiment A")
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Localisation { get; set; } = string.Empty;

    /// <summary>
    /// Description détaillée de la barrière
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Type de barrière (ex: "Entrée", "Sortie", "Mixte")
    /// </summary>
    [MaxLength(50)]
    public string TypeBarriere { get; set; } = "Mixte";

    /// <summary>
    /// Indique si la barrière est active et opérationnelle
    /// </summary>
    public bool EstActive { get; set; } = true;

    /// <summary>
    /// Ordre d'affichage dans les listes déroulantes
    /// </summary>
    public int OrdreAffichage { get; set; } = 0;

    /// <summary>
    /// Horaires d'ouverture (ex: "24/7", "06:00-22:00")
    /// </summary>
    [MaxLength(100)]
    public string? HorairesOuverture { get; set; }

    /// <summary>
    /// Numéro de téléphone de la barrière
    /// </summary>
    [MaxLength(50)]
    public string? Telephone { get; set; }

    /// <summary>
    /// Date de création de l'enregistrement
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;

    /// <summary>
    /// Date de dernière modification
    /// </summary>
    public DateTime? DateModification { get; set; }
}
