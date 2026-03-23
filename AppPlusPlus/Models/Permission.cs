using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

/// <summary>
/// T_Permissions — Droits d'accès d'un rôle sur une fonction.
/// Combinaison unique (RoleId, FonctionId).
/// </summary>
[Table("T_Permissions")]
public class Permission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PermissionId { get; set; }

    [Required]
    public int RoleId { get; set; }

    [Required]
    public int FonctionId { get; set; }

    /// <summary>Droit de lecture / consultation.</summary>
    public bool CanRead { get; set; } = true;

    /// <summary>Droit d'écriture / modification / création.</summary>
    public bool CanWrite { get; set; }

    /// <summary>Droit de suppression.</summary>
    public bool CanDelete { get; set; }

    // Navigation
    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }

    [ForeignKey(nameof(FonctionId))]
    public Fonction? Fonction { get; set; }
}
