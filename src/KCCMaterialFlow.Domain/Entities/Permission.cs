using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Permission système.
/// EF maps Id → IdPermission column.
/// </summary>
public sealed class Permission : BaseEntity
{
    [MaxLength(50)]
    public string CodePermission { get; set; } = string.Empty;

    [MaxLength(150)]
    public string NomPermission { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Categorie { get; set; }

    public bool EstActif { get; set; } = true;
    public bool EstSysteme { get; set; }
    public int OrdreAffichage { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;

    public ICollection<RolePermission>? RolePermissions { get; set; }
}

/// <summary>
/// Liaison Role-Permission (many-to-many).
/// EF maps Id → IdRolePermission column.
/// </summary>
public sealed class RolePermission : BaseEntity
{
    public int IdRole { get; set; }
    public int IdPermission { get; set; }
    public DateTime DateAttribution { get; set; } = DateTime.Now;

    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
