using AppPlusPlus.Application.DTOs.Vente;
using AppPlusPlus.Domain.Entities.Vente;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Vente;

/// <summary>
/// Stub Application-layer implementation of IFacturationService.
/// The real implementation is FacturationQueryService in Infrastructure
/// (registered via DependencyInjection).
/// This class is kept as a fallback / reference.
/// </summary>
public class FacturationService : IFacturationService
{
    private readonly IFactureRepository _factureRepo;

    public FacturationService(IFactureRepository factureRepo)
    {
        _factureRepo = factureRepo;
    }

    public async Task<List<FactRowDto>> GetFactureRowsAsync(List<int> localisationIds, string login)
    {
        // Simplified fallback: no cloture-date exclusion, no payment join.
        var facts = await _factureRepo.GetByLocalisationIdsAsync(localisationIds);
        return facts.Select(f => new FactRowDto
        {
            Id = f.Id,
            Client = f.DescriptionName,
            Articles = f.DescriptionArticle ?? "",
            TotalQte = f.Details.Sum(d => d.Qte ?? 0),
            Total = (double)(f.TotalApresReduction ?? f.Total ?? 0),
            Date = f.Date,
            Status = f.Status,
            Login = f.User,
        }).ToList();
    }

    public async Task DeleteFactureAsync(int factId)
    {
        var facture = await _factureRepo.GetByIdAsync(factId);
        if (facture != null)
            await _factureRepo.DeleteAsync(facture);
    }

    public Task<List<FactureViewDto>> GetPaiementsAsync(List<int> localisationIds, string login)
    {
        // Stub: real implementation is in Infrastructure.
        return Task.FromResult(new List<FactureViewDto>());
    }

    public async Task<Fact?> GetFactureWithDetailsAsync(int factId)
    {
        // Stub: delegates to repository. Real implementation is in Infrastructure.
        return await _factureRepo.GetWithDetailsAsync(factId);
    }
}
