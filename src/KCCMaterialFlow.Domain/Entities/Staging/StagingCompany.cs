using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities.Staging;

/// <summary>
/// Table tampon pour l'import des compagnies depuis DATA.xlsx (sheet "Company").
/// Toutes les colonnes restent brutes (string) avant validation/merge.
/// </summary>
public sealed class StagingCompany : BaseEntity
{
    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(100)]
    public string? CompanyCode { get; set; }

    /// <summary>Colonne 3 (sans en-tête) — flag actif (0/1).</summary>
    public bool? Actif { get; set; }

    /// <summary>Colonne 4 (sans en-tête) — DateSys brute.</summary>
    public DateTime? DateSys { get; set; }

    /// <summary>Valeur originale de DateSys lue dans Excel (debug si parse échoue).</summary>
    [MaxLength(100)]
    public string? DateSysRaw { get; set; }

    /// <summary>Lot d'import (timestamp UTC) — permet de purger un import donné.</summary>
    public DateTime ImportBatchId { get; set; }

    /// <summary>True une fois la ligne mergée vers T_Compagnies.</summary>
    public bool EstMerge { get; set; }

    [MaxLength(500)]
    public string? ErreurMessage { get; set; }
}
