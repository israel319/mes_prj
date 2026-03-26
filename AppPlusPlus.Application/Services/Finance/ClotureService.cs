using AppPlusPlus.Domain.Entities.Finance;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Finance;

public class ClotureService : IClotureService
{
    private readonly IFinanceRepository _financeRepo;

    public ClotureService(IFinanceRepository financeRepo)
    {
        _financeRepo = financeRepo;
    }

    public async Task<List<Versement>> GetCloturesByLocalisationsAsync(List<int> localisationIds)
    {
        // Phase 3: Implement cloture retrieval filtered by localisations.
        // Clotures map to Versements grouped by date/localisation.
        var results = new List<Versement>();
        foreach (var locId in localisationIds)
        {
            var versements = await _financeRepo.GetVersementsByLocalisationAsync(locId);
            results.AddRange(versements);
        }
        return results;
    }
}
