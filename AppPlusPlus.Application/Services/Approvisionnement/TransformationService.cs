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

    public async Task<List<Transformation>> GetTransformationsByLocalisationsAsync(List<int> localisationIds)
    {
        var results = new List<Transformation>();
        foreach (var locId in localisationIds)
        {
            var transformations = await _approRepo.GetTransformationsByLocalisationAsync(locId);
            results.AddRange(transformations);
        }
        return results;
    }

    public async Task DeleteTransformationAsync(int transformationId)
        => await _approRepo.DeleteTransformationAsync(transformationId);

    public async Task<Transformation?> GetByIdAsync(int id)
        => await _approRepo.GetTransformationByIdAsync(id);

    public async Task AddAsync(Transformation transformation)
        => await _approRepo.AddTransformationAsync(transformation);

    public async Task UpdateAsync(Transformation transformation)
        => await _approRepo.UpdateTransformationAsync(transformation);
}
