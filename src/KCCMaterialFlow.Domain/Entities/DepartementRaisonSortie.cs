using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Table de jonction : Département → RaisonSortie autorisée.
/// DepartementId = NULL signifie "défaut pour tous les départements non-mappés".
/// </summary>
public sealed class DepartementRaisonSortie : BaseEntity
{
    public int? DepartementId { get; set; }
    public int RaisonSortieId { get; set; }

    /// <summary>
    /// Si true et qu'un seul motif est autorisé, il est auto-sélectionné et verrouillé.
    /// </summary>
    public bool AutoSelection { get; set; }

    public int OrdreAffichage { get; set; }

    // Navigations
    public Departement? Departement { get; set; }
    public RaisonSortie RaisonSortie { get; set; } = null!;
}
