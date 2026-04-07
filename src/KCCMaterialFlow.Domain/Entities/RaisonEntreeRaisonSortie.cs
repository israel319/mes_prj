using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Table de jonction : RaisonEntree → RaisonSortie autorisée.
/// Permet de déterminer automatiquement le motif de sortie à partir du motif d'entrée.
/// </summary>
public sealed class RaisonEntreeRaisonSortie : BaseEntity
{
    public int RaisonEntreeId { get; set; }
    public int RaisonSortieId { get; set; }

    /// <summary>
    /// Si true et qu'un seul motif de sortie est mappé, il est auto-sélectionné et verrouillé.
    /// </summary>
    public bool AutoSelection { get; set; }

    public int OrdreAffichage { get; set; }

    // Navigations
    public RaisonEntree RaisonEntree { get; set; } = null!;
    public RaisonSortie RaisonSortie { get; set; } = null!;
}
