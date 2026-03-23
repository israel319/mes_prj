using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Configuration d'une étape de workflow d'approbation, configurable par type de bon et motif (raison de sortie).
/// </summary>
public class WorkflowEtapeConfig
{
    [Key]
    public int IdWorkflowEtapeConfig { get; set; }

    [Required]
    [MaxLength(10)]
    public string BonType { get; set; } = "BSM";

    [MaxLength(50)]
    public string? RaisonSortieCode { get; set; }

    public int OrdreEtape { get; set; }

    [Required]
    [MaxLength(50)]
    public string RoleCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NomEtape { get; set; } = string.Empty;

    public bool EstActif { get; set; } = true;

    public DateTime DateCreation { get; set; } = DateTime.Now;

    public DateTime? DateModification { get; set; }

    [MaxLength(100)]
    public string? ModifieParLogin { get; set; }
}
