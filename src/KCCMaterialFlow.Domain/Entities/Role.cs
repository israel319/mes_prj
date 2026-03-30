using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Rôle utilisateur. EF maps Id → IdRole column.
/// </summary>
public sealed class Role : BaseEntity
{
    [MaxLength(50)]
    public string CodeRole { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NomRole { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int NiveauPriorite { get; set; }
    public bool EstActif { get; set; } = true;
    public bool EstSysteme { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DateModification { get; set; }

    public ICollection<UtilisateurRole>? UtilisateurRoles { get; set; }
    public ICollection<RolePermission>? RolePermissions { get; set; }
}

/// <summary>
/// Liaison Utilisateur-Role (many-to-many).
/// EF maps Id → IdUtilisateurRole column.
/// </summary>
public sealed class UtilisateurRole : BaseEntity
{
    public int IdUtilisateur { get; set; }
    public int IdRole { get; set; }
    public DateTime DateAttribution { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string? AttribueParLogin { get; set; }

    public Utilisateur? Utilisateur { get; set; }
    public Role? Role { get; set; }
}
