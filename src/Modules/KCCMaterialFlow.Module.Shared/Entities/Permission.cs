using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente une permission système dans KCCMaterialFlow.
/// Chaque permission définit un droit d'accès spécifique (ex: créer un bon, approuver, scanner...).
/// </summary>
public class Permission
{
    /// <summary>
    /// Identifiant unique de la permission (clé primaire)
    /// </summary>
    [Key]
    public int IdPermission { get; set; }

    /// <summary>
    /// Code unique de la permission (ex: "CREATE_BON", "APPROVE_BON", "SCAN_BON")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string CodePermission { get; set; } = string.Empty;

    /// <summary>
    /// Nom affiché de la permission (ex: "Créer des bons d'entrée/sortie")
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string NomPermission { get; set; } = string.Empty;

    /// <summary>
    /// Description détaillée de la permission
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Catégorie/module auquel la permission appartient (ex: "Bons", "Sécurité", "Administration")
    /// </summary>
    [MaxLength(50)]
    public string? Categorie { get; set; }

    /// <summary>
    /// Indique si la permission est active
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Indique si c'est une permission système non modifiable
    /// </summary>
    public bool EstSysteme { get; set; } = false;

    /// <summary>
    /// Ordre d'affichage dans l'interface
    /// </summary>
    public int OrdreAffichage { get; set; } = 0;

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;

    /// <summary>
    /// Navigation vers les rôles ayant cette permission
    /// </summary>
    public virtual ICollection<RolePermission>? RolePermissions { get; set; }
}

/// <summary>
/// Table de liaison entre Role et Permission (relation many-to-many)
/// </summary>
public class RolePermission
{
    /// <summary>
    /// Identifiant unique (clé primaire)
    /// </summary>
    [Key]
    public int IdRolePermission { get; set; }

    /// <summary>
    /// ID du rôle
    /// </summary>
    public int IdRole { get; set; }

    /// <summary>
    /// ID de la permission
    /// </summary>
    public int IdPermission { get; set; }

    /// <summary>
    /// Date d'attribution de la permission au rôle
    /// </summary>
    public DateTime DateAttribution { get; set; } = DateTime.Now;

    /// <summary>
    /// Navigation vers le rôle
    /// </summary>
    public virtual Role? Role { get; set; }

    /// <summary>
    /// Navigation vers la permission
    /// </summary>
    public virtual Permission? Permission { get; set; }
}
