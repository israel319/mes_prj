using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente un rôle utilisateur dans le système.
/// Les rôles définissent les permissions et accès des utilisateurs.
/// </summary>
public class Role
{
    /// <summary>
    /// Identifiant unique du rôle (clé primaire)
    /// </summary>
    [Key]
    public int IdRole { get; set; }

    /// <summary>
    /// Code unique du rôle (ex: "ADMIN", "APPROBATEUR", "AGENT_SECURITE")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string CodeRole { get; set; } = string.Empty;

    /// <summary>
    /// Nom affiché du rôle (ex: "Administrateur", "Approbateur", "Agent de sécurité")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string NomRole { get; set; } = string.Empty;

    /// <summary>
    /// Description du rôle et de ses responsabilités
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Niveau de priorité du rôle (plus élevé = plus de privilèges)
    /// Utilisé pour déterminer le rôle effectif en cas de multiples rôles
    /// </summary>
    public int NiveauPriorite { get; set; } = 0;

    /// <summary>
    /// Indique si le rôle est actif
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Indique si c'est un rôle système non modifiable
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

    /// <summary>
    /// Collection des utilisateurs ayant ce rôle
    /// </summary>
    public virtual ICollection<UtilisateurRole>? UtilisateurRoles { get; set; }

    /// <summary>
    /// Collection des permissions associées à ce rôle
    /// </summary>
    public virtual ICollection<RolePermission>? RolePermissions { get; set; }
}

/// <summary>
/// Table de liaison entre Utilisateur et Role (relation many-to-many)
/// </summary>
public class UtilisateurRole
{
    [Key]
    public int IdUtilisateurRole { get; set; }

    /// <summary>
    /// ID de l'utilisateur
    /// </summary>
    public int IdUtilisateur { get; set; }

    /// <summary>
    /// ID du rôle
    /// </summary>
    public int IdRole { get; set; }

    /// <summary>
    /// Date d'attribution du rôle
    /// </summary>
    public DateTime DateAttribution { get; set; } = DateTime.Now;

    /// <summary>
    /// Utilisateur qui a attribué le rôle
    /// </summary>
    [MaxLength(100)]
    public string? AttribueParLogin { get; set; }

    /// <summary>
    /// Navigation vers l'utilisateur
    /// </summary>
    public virtual Utilisateur? Utilisateur { get; set; }

    /// <summary>
    /// Navigation vers le rôle
    /// </summary>
    public virtual Role? Role { get; set; }
}
