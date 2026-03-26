using AppPlusPlus.Domain.Entities.Approvisionnement;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Approvisionnement;

public class ApproService : IApproService
{
    private readonly IApproRepository _approRepo;

    public ApproService(IApproRepository approRepo)
    {
        _approRepo = approRepo;
    }

    public async Task<List<Appro>> GetApprosByLocalisationsAsync(List<int> localisationIds)
    {
        // Phase 3: Aggregate results from multiple localisations
        var results = new List<Appro>();
        foreach (var locId in localisationIds)
        {
            var appros = await _approRepo.GetByLocalisationAsync(locId);
            results.AddRange(appros);
        }
        return results;
    }

    public async Task DeleteApproAsync(int approId)
    {
        var appro = await _approRepo.GetByIdAsync(approId);
        if (appro != null)
            await _approRepo.DeleteAsync(appro);
    }
}
