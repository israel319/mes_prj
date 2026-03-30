using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Configuration d'une étape de workflow d'approbation.
/// EF maps Id → IdWorkflowEtapeConfig column.
/// </summary>
public sealed class WorkflowEtapeConfig : BaseEntity
{
    [MaxLength(10)]
    public string BonType { get; set; } = "BSM";

    [MaxLength(50)]
    public string? RaisonSortieCode { get; set; }

    public int OrdreEtape { get; set; }

    [MaxLength(50)]
    public string RoleCode { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NomEtape { get; set; } = string.Empty;

    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DateModification { get; set; }

    [MaxLength(100)]
    public string? ModifieParLogin { get; set; }
}
