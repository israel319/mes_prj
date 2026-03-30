using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Activité/action métier assignable aux utilisateurs.
/// EF maps Id → IdActivite column.
/// </summary>
public sealed class Activite : BaseEntity
{
    [MaxLength(50)]
    public string CodeActivite { get; set; } = string.Empty;

    [MaxLength(200)]
    public string NomActivite { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Module { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Categorie { get; set; }

    public int OrdreAffichage { get; set; }
    public bool EstActif { get; set; } = true;
    public bool EstSysteme { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;

    public ICollection<UtilisateurActivite>? UtilisateurActivites { get; set; }
}

/// <summary>
/// Liaison Utilisateur-Activite (many-to-many).
/// EF maps Id → IdUtilisateurActivite column.
/// </summary>
public sealed class UtilisateurActivite : BaseEntity
{
    public int IdUtilisateur { get; set; }
    public int IdActivite { get; set; }
    public DateTime DateAttribution { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string? AttribueParLogin { get; set; }

    public bool EstActif { get; set; } = true;

    public Utilisateur? Utilisateur { get; set; }
    public Activite? Activite { get; set; }
}
