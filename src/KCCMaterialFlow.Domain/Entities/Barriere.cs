using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Barrière de sécurité (point de contrôle).
/// EF maps Id → IdBarriere column.
/// </summary>
public sealed class Barriere : BaseEntity
{
    [MaxLength(20)]
    public string CodeBarriere { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NomBarriere { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Localisation { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string TypeBarriere { get; set; } = "Mixte";

    public bool EstActive { get; set; } = true;
    public int OrdreAffichage { get; set; }

    [MaxLength(100)]
    public string? HorairesOuverture { get; set; }

    [MaxLength(50)]
    public string? Telephone { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DateModification { get; set; }
}
