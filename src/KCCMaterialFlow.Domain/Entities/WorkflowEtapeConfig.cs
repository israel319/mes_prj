using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Étape de workflow d'approbation configurable en base de données.
/// Priorité de résolution : (BonType + DepartementCode) > (BonType + null) > défaut métier.
/// </summary>
public sealed class WorkflowEtapeConfig : BaseEntity
{
    /// <summary>Type de bon ciblé (ex. "BSM", "BEM").</summary>
    [MaxLength(10)]
    public string BonType { get; set; } = string.Empty;

    /// <summary>Code de raison de sortie (nullable → s'applique à toutes les raisons).</summary>
    [MaxLength(50)]
    public string? RaisonSortieCode { get; set; }

    /// <summary>
    /// Code du département Glencore (ex. "IT", "MINES", "ENV").
    /// NULL = s'applique à tous les départements.
    /// </summary>
    [MaxLength(100)]
    public string? DepartementCode { get; set; }

    /// <summary>Position dans la chaîne (1 = première approbation).</summary>
    public int OrdreEtape { get; set; }

    /// <summary>Code du rôle résolvable (ex. "SUPERINTENDENT", "GM", "OPJ", "IDENTIFICATION").</summary>
    [MaxLength(50)]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>Libellé affiché dans l'UI et les notifications.</summary>
    [MaxLength(100)]
    public string NomEtape { get; set; } = string.Empty;

    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime? DateModification { get; set; }

    [MaxLength(100)]
    public string? ModifieParLogin { get; set; }
}
