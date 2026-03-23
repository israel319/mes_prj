using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente un type de matériel avec ses workflows et paramètres spécifiques.
/// Définit les catégories de matériel et leurs règles de traitement.
/// </summary>
public class TypeMateriel
{
    /// <summary>
    /// Identifiant unique du type de matériel (clé primaire)
    /// </summary>
    [Key]
    public int IdTypeMateriel { get; set; }

    /// <summary>
    /// Code unique du type (ex: "VEHICULE", "EQUIPEMENT", "DOCUMENT")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string CodeType { get; set; } = string.Empty;

    /// <summary>
    /// Nom affiché du type de matériel (ex: "Véhicule", "Équipement informatique")
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string NomType { get; set; } = string.Empty;

    /// <summary>
    /// Description du type de matériel
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Catégorie parente (ex: "Matériel roulant", "Bureautique")
    /// </summary>
    [MaxLength(100)]
    public string? Categorie { get; set; }

    /// <summary>
    /// Icône pour l'affichage (classe CSS ou emoji)
    /// </summary>
    [MaxLength(50)]
    public string? Icone { get; set; }

    /// <summary>
    /// Couleur d'identification (format hex)
    /// </summary>
    [MaxLength(20)]
    public string? Couleur { get; set; }

    /// <summary>
    /// Indique si ce type nécessite une approbation de département
    /// </summary>
    public bool RequiertApprobationDepartement { get; set; } = true;

    /// <summary>
    /// Indique si ce type nécessite une approbation de la direction
    /// </summary>
    public bool RequiertApprobationDirection { get; set; } = false;

    /// <summary>
    /// Nombre de niveaux d'approbation requis
    /// </summary>
    public int NiveauxApprobation { get; set; } = 1;

    /// <summary>
    /// Durée de validité par défaut en jours (pour les bons de sortie)
    /// </summary>
    public int DureeValiditeDefautJours { get; set; } = 30;

    /// <summary>
    /// Durée maximum autorisée en jours
    /// </summary>
    public int DureeMaximumJours { get; set; } = 365;

    /// <summary>
    /// Indique si le numéro de série est obligatoire
    /// </summary>
    public bool NumeroSerieObligatoire { get; set; } = false;

    /// <summary>
    /// Indique si une photo est obligatoire
    /// </summary>
    public bool PhotoObligatoire { get; set; } = false;

    /// <summary>
    /// Champs personnalisés supplémentaires requis (format JSON)
    /// </summary>
    [MaxLength(2000)]
    public string? ChampsPersonnalises { get; set; }

    /// <summary>
    /// Workflow spécifique (format JSON avec étapes et transitions)
    /// </summary>
    [MaxLength(4000)]
    public string? WorkflowConfig { get; set; }

    /// <summary>
    /// Ordre d'affichage dans les listes
    /// </summary>
    public int Ordre { get; set; } = 0;

    /// <summary>
    /// Indique si le type est actif
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Date de création de l'enregistrement
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;

    /// <summary>
    /// Date de dernière modification
    /// </summary>
    public DateTime? DateModification { get; set; }
}
