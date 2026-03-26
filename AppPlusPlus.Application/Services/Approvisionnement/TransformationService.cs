using AppPlusPlus.Domain.Entities.Approvisionnement;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Approvisionnement;

public class TransformationService : ITransformationService
{
    private readonly IApproRepository _approRepo;

    public TransformationService(IApproRepository approRepo)
    {
        _approRepo = approRepo;
    }

    public Task<List<Transformation>> GetTransformationsByLocalisationsAsync(List<int> localisationIds)
    {
        // Phase 3: Implement transformation retrieval filtered by localisations.
        // Note: May require a dedicated ITransformationRepository.
        return Task.FromResult(new List<Transformation>());
    }

    public Task DeleteTransformationAsync(int transformationId)
    {
        // Phase 3: Implement transformation deletion.
        return Task.CompletedTask;
    }
}
