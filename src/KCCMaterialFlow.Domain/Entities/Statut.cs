using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Statut configurable pour les bons.
/// EF maps Id → IdStatut column.
/// </summary>
public sealed class Statut : BaseEntity
{
    [MaxLength(50)]
    public string CodeStatut { get; set; } = string.Empty;

    [MaxLength(100)]
    public string LibelleStatut { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string TypeBon { get; set; } = "Tous";

    [MaxLength(20)]
    public string CouleurFond { get; set; } = "#6c757d";

    [MaxLength(20)]
    public string CouleurTexte { get; set; } = "#ffffff";

    [MaxLength(50)]
    public string? Icone { get; set; }

    public int Ordre { get; set; }
    public bool EstFinal { get; set; }
    public bool RequiertAction { get; set; }

    [MaxLength(200)]
    public string? StatutsSuivants { get; set; }

    public bool EstActif { get; set; } = true;
    public bool EstSysteme { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DateModification { get; set; }
}
