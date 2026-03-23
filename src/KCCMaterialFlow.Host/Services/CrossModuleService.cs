using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.BonSortie.Services;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// INT-005: Implémentation du service cross-module.
/// Implémenté dans le Host car il a accès à tous les modules.
/// </summary>
public class CrossModuleService : ICrossModuleService
{
    private readonly IBonSortieService _bonSortieService;

    public CrossModuleService(IBonSortieService bonSortieService)
    {
        _bonSortieService = bonSortieService;
    }

    /// <summary>
    /// Récupère les bons de sortie liés à un bon d'entrée
    /// </summary>
    public async Task<IReadOnlyList<CrossModuleBonSortieInfo>> GetBonsSortieByBonEntreeAsync(
        int bonEntreeId, 
        CancellationToken cancellationToken = default)
    {
        var bons = await _bonSortieService.GetBonsSortieByBonEntreeAsync(bonEntreeId, cancellationToken);

        return bons.Select(b => new CrossModuleBonSortieInfo
        {
            IdBonSortie = b.IdBonSortie,
            NumeroReference = b.NumeroReference,
            TypeBon = b.TypeBon,
            StatutActuel = b.StatutActuel,
            DateCreation = b.DateCreation,
            Destination = b.Destination,
            NomDemandeur = b.NomDemandeur,
            NombreMateriels = b.NombreMateriels
        }).ToList();
    }
}
