namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// INT-005: Interface pour les opérations cross-module.
/// Permet au module BonEntree de récupérer les bons de sortie liés
/// sans référencer directement le module BonSortie.
/// </summary>
public interface ICrossModuleService
{
    /// <summary>
    /// Récupère les bons de sortie liés à un bon d'entrée.
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des bons de sortie liés</returns>
    Task<IReadOnlyList<CrossModuleBonSortieInfo>> GetBonsSortieByBonEntreeAsync(int bonEntreeId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Informations de base d'un bon de sortie pour affichage cross-module
/// </summary>
public class CrossModuleBonSortieInfo
{
    public int IdBonSortie { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string TypeBon { get; set; } = string.Empty;
    public string StatutActuel { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public string Destination { get; set; } = string.Empty;
    public string NomDemandeur { get; set; } = string.Empty;
    public int NombreMateriels { get; set; }
    public string Url => $"/bon-sortie/{IdBonSortie}";
}
