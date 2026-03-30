using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Solde disponible d'un matériel issu d'un Bon d'Entrée.
/// EF maps Id → IdSoldeMateriel column.
/// </summary>
public sealed class SoldeMateriel : BaseEntity
{
    public int MaterielEntreeId { get; set; }
    public int BonEntreeId { get; set; }

    [MaxLength(200)]
    public string CodeProduitSerial { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Designation { get; set; } = string.Empty;

    public decimal QuantiteInitiale { get; set; }
    public decimal QuantiteSortie { get; set; }

    public decimal QuantiteRestante => QuantiteInitiale - QuantiteSortie;
    public bool EstEpuise => QuantiteRestante <= 0;
    public bool EstPartiel => QuantiteSortie > 0 && QuantiteSortie < QuantiteInitiale;

    public DateTime DateDerniereMaj { get; set; } = DateTime.Now;

    [MaxLength(50)]
    public string? DernierBsmNumero { get; set; }
}
