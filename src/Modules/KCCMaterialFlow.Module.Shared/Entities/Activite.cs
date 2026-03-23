using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente une activité/action métier dans le système KCCMaterialFlow.
/// Chaque activité définit une action spécifique qu'un utilisateur peut être autorisé à effectuer.
/// Rôle = "qui vous êtes", Activité = "ce que vous pouvez faire".
/// </summary>
public class Activite
{
    /// <summary>
    /// Identifiant unique de l'activité (clé primaire)
    /// </summary>
    [Key]
    public int IdActivite { get; set; }

    /// <summary>
    /// Code unique de l'activité (ex: "BEM_CREER", "BSM_APPROUVER", "SEC_SCANNER")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string CodeActivite { get; set; } = string.Empty;

    /// <summary>
    /// Nom affiché de l'activité (ex: "Créer un Bon d'Entrée")
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string NomActivite { get; set; } = string.Empty;

    /// <summary>
    /// Description détaillée de l'activité
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Module auquel l'activité appartient (ex: "BonEntree", "BonSortie", "Securite", "Admin")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// Catégorie fonctionnelle (ex: "Création", "Approbation", "Scan", "Administration")
    /// </summary>
    [MaxLength(50)]
    public string? Categorie { get; set; }

    /// <summary>
    /// Ordre d'affichage dans l'interface
    /// </summary>
    public int OrdreAffichage { get; set; } = 0;

    /// <summary>
    /// Indique si l'activité est active
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Indique si c'est une activité système non modifiable
    /// </summary>
    public bool EstSysteme { get; set; } = false;

    /// <summary>
    /// Date de création de l'enregistrement
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;

    /// <summary>
    /// Navigation vers les utilisateurs ayant cette activité
    /// </summary>
    public virtual ICollection<UtilisateurActivite>? UtilisateurActivites { get; set; }
}

/// <summary>
/// Table de liaison entre Utilisateur et Activite (relation many-to-many).
/// Permet d'assigner des activités spécifiques à chaque utilisateur.
/// </summary>
public class UtilisateurActivite
{
    /// <summary>
    /// Identifiant unique (clé primaire)
    /// </summary>
    [Key]
    public int IdUtilisateurActivite { get; set; }

    /// <summary>
    /// ID de l'utilisateur
    /// </summary>
    public int IdUtilisateur { get; set; }

    /// <summary>
    /// ID de l'activité
    /// </summary>
    public int IdActivite { get; set; }

    /// <summary>
    /// Date d'attribution de l'activité à l'utilisateur
    /// </summary>
    public DateTime DateAttribution { get; set; } = DateTime.Now;

    /// <summary>
    /// Login de l'utilisateur qui a attribué l'activité
    /// </summary>
    [MaxLength(100)]
    public string? AttribueParLogin { get; set; }

    /// <summary>
    /// Indique si l'attribution est active
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Navigation vers l'utilisateur
    /// </summary>
    public virtual Utilisateur? Utilisateur { get; set; }

    /// <summary>
    /// Navigation vers l'activité
    /// </summary>
    public virtual Activite? Activite { get; set; }
}
