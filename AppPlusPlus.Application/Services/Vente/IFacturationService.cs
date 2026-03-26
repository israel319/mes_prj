using AppPlusPlus.Application.DTOs.Vente;
using AppPlusPlus.Domain.Entities.Vente;

namespace AppPlusPlus.Application.Services.Vente;

public interface IFacturationService
{
    /// <summary>
    /// Loads facture rows for the grid, filtered by localisations and excluding cloture dates.
    /// </summary>
    Task<List<FactRowDto>> GetFactureRowsAsync(List<int> localisationIds, string login);

    /// <summary>
    /// Deletes a facture and all its detail lines.
    /// </summary>
    Task DeleteFactureAsync(int factId);

    /// <summary>
    /// Loads enriched facture views for the Paiements tab (status 1-2 only).
    /// </summary>
    Task<List<FactureViewDto>> GetPaiementsAsync(List<int> localisationIds, string login);
}
