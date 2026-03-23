using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente un utilisateur du système KCCMaterialFlow.
/// Les utilisateurs sont synchronisés depuis Active Directory via leur login Windows.
/// </summary>
public class Utilisateur
{
    /// <summary>
    /// Identifiant unique de l'utilisateur (clé primaire)
    /// </summary>
    [Key]
    public int IdUtilisateur { get; set; }

    /// <summary>
    /// Login Windows de l'utilisateur (ex: DOMAIN\username)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet de l'utilisateur (ex: "Jean Dupont")
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string NomComplet { get; set; } = string.Empty;

    /// <summary>
    /// Fonction/titre de l'utilisateur dans l'entreprise (ex: "Ingénieur", "Technicien")
    /// </summary>
    [MaxLength(150)]
    public string? Fonction { get; set; }

    /// <summary>
    /// Département auquel l'utilisateur appartient
    /// </summary>
    [MaxLength(100)]
    public string? Departement { get; set; }

    /// <summary>
    /// ID du rôle principal de l'utilisateur (clé étrangère vers T_Roles)
    /// </summary>
    [Required]
    public int IdRole { get; set; }

    /// <summary>
    /// Adresse email de l'utilisateur pour les notifications
    /// </summary>
    [MaxLength(200)]
    public string? Email { get; set; }

    /// <summary>
    /// Numéro de téléphone de l'utilisateur
    /// </summary>
    [MaxLength(50)]
    public string? Telephone { get; set; }

    /// <summary>
    /// Indique si l'utilisateur est actif dans le système
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Date de création de l'enregistrement
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.Now;

    /// <summary>
    /// Date de dernière modification de l'enregistrement
    /// </summary>
    public DateTime? DateModification { get; set; }

    /// <summary>
    /// Date de dernière connexion de l'utilisateur
    /// </summary>
    public DateTime? DerniereConnexion { get; set; }

    /// <summary>
    /// Collection des rôles de l'utilisateur
    /// </summary>
    public virtual ICollection<UtilisateurRole>? UtilisateurRoles { get; set; }

    /// <summary>
    /// Navigation vers le rôle principal
    /// </summary>
    public virtual Role? RolePrincipal { get; set; }

    /// <summary>
    /// Collection des activités assignées à l'utilisateur
    /// </summary>
    public virtual ICollection<UtilisateurActivite>? UtilisateurActivites { get; set; }
}
