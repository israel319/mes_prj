using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Utilisateur du système (synchronisé depuis AD).
/// EF maps Id → IdUtilisateur column.
/// </summary>
public sealed class Utilisateur : BaseEntity
{
    [MaxLength(100)]
    public string Login { get; set; } = string.Empty;

    [MaxLength(200)]
    public string NomComplet { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Fonction { get; set; }

    [MaxLength(100)]
    public string? Departement { get; set; }

    public int IdRole { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Telephone { get; set; }

    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DateModification { get; set; }
    public DateTime? DerniereConnexion { get; set; }

    // Navigation properties (configured via EF)
    public Role? RolePrincipal { get; set; }
    public ICollection<UtilisateurRole>? UtilisateurRoles { get; set; }
    public ICollection<UtilisateurActivite>? UtilisateurActivites { get; set; }
}
