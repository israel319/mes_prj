using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente un paramètre système configurable.
/// Stocke les paramètres globaux de l'application (durées, emails, options).
/// </summary>
public class ParametreSysteme
{
    /// <summary>
    /// Identifiant unique du paramètre (clé primaire)
    /// </summary>
    [Key]
    public int IdParametre { get; set; }

    /// <summary>
    /// Clé unique du paramètre (ex: "DUREE_VALIDITE_DEFAUT", "EMAIL_ADMIN")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Cle { get; set; } = string.Empty;

    /// <summary>
    /// Valeur du paramètre
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string Valeur { get; set; } = string.Empty;

    /// <summary>
    /// Type de données du paramètre (String, Integer, Boolean, Date, Json)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TypeDonnee { get; set; } = "String";

    /// <summary>
    /// Catégorie du paramètre pour le regroupement (ex: "General", "Email", "Securite", "Workflow")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Categorie { get; set; } = "General";

    /// <summary>
    /// Libellé affiché pour ce paramètre
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Libelle { get; set; } = string.Empty;

    /// <summary>
    /// Description détaillée du paramètre et de son utilisation
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Valeur par défaut du paramètre
    /// </summary>
    [MaxLength(2000)]
    public string? ValeurDefaut { get; set; }

    /// <summary>
    /// Valeurs possibles (pour les listes de choix, séparées par pipe "|")
    /// </summary>
    [MaxLength(2000)]
    public string? ValeursPossibles { get; set; }

    /// <summary>
    /// Valeur minimum (pour les numériques)
    /// </summary>
    public int? ValeurMin { get; set; }

    /// <summary>
    /// Valeur maximum (pour les numériques)
    /// </summary>
    public int? ValeurMax { get; set; }

    /// <summary>
    /// Unité de mesure (ex: "jours", "heures", "%")
    /// </summary>
    [MaxLength(20)]
    public string? Unite { get; set; }

    /// <summary>
    /// Ordre d'affichage dans la catégorie
    /// </summary>
    public int Ordre { get; set; } = 0;

    /// <summary>
    /// Indique si le paramètre nécessite un redémarrage de l'application
    /// </summary>
    public bool NecessiteRedemarrage { get; set; } = false;

    /// <summary>
    /// Indique si le paramètre est visible dans l'interface d'administration
    /// </summary>
    public bool EstVisible { get; set; } = true;

    /// <summary>
    /// Indique si le paramètre est modifiable (certains peuvent être en lecture seule)
    /// </summary>
    public bool EstModifiable { get; set; } = true;

    /// <summary>
    /// Indique si c'est un paramètre système critique
    /// </summary>
    public bool EstSysteme { get; set; } = false;

    /// <summary>
    /// Login de l'utilisateur qui a effectué la dernière modification
    /// </summary>
    [MaxLength(100)]
    public string? ModifieParLogin { get; set; }

    /// <summary>
    /// Date de dernière modification
    /// </summary>
    public DateTime? DateModification { get; set; }
}
