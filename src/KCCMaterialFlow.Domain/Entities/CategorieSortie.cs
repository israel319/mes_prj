using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Catégorie principale de sortie de matériel (INTERNE/EXTERNE).
/// EF maps Id → IdCategorieSortie column.
/// </summary>
public sealed class CategorieSortie : BaseEntity
{
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Code { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool RequiertBarrieres { get; set; }
    public bool RequiertBonEntree { get; set; }

    [MaxLength(50)]
    public string? TypeEntite { get; set; }

    public bool EstActif { get; set; } = true;
    public int OrdreAffichage { get; set; }

    public ICollection<RaisonSortie> Raisons { get; set; } = new List<RaisonSortie>();
}
