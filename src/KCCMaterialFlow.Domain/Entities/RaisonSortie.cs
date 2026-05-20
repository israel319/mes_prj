using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Raison/motif spécifique de sortie (liée à une catégorie).
/// EF maps Id → IdRaisonSortie column.
/// </summary>
public sealed class RaisonSortie : BaseEntity
{
    public int CategorieId { get; set; }

    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Code { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool RequiertBonEntree { get; set; }
    public bool RequiertBarrieres { get; set; }
    public bool EstTemporaire { get; set; }
    public int? DureeMaxJours { get; set; }
    public bool ValidationSpeciale { get; set; }

    [MaxLength(100)]
    public string? TypeApprobateurSpecial { get; set; }

    public bool RequiertDetails { get; set; }

    public bool EstActif { get; set; } = true;
    public int OrdreAffichage { get; set; }

    [MaxLength(50)]
    public string? Icone { get; set; }

    [MaxLength(20)]
    public string? Couleur { get; set; }

    public CategorieSortie? Categorie { get; set; }
    public ICollection<RaisonEntreeRaisonSortie> RaisonsEntreeAutorisees { get; set; } = new List<RaisonEntreeRaisonSortie>();
}
