using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities.Staging;

/// <summary>
/// Table tampon pour l'import des contrats depuis DATA.xlsx (sheet "Contract").
/// </summary>
public sealed class StagingContract : BaseEntity
{
    [MaxLength(100)]
    public string? PoNumber { get; set; }

    [MaxLength(500)]
    public string? ContractDescription { get; set; }

    /// <summary>Code compagnie (lookup vers T_Compagnies.Code).</summary>
    [MaxLength(100)]
    public string? CompanyCode { get; set; }

    public bool? Actif { get; set; }

    public DateTime? DateSys { get; set; }

    [MaxLength(100)]
    public string? DateSysRaw { get; set; }

    public DateTime ImportBatchId { get; set; }
    public bool EstMerge { get; set; }

    [MaxLength(500)]
    public string? ErreurMessage { get; set; }
}
