using AppPlusPlus.Application.Common;
using AppPlusPlus.Domain.Entities.Commandes;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Commandes;

/// <summary>
/// Stub Application-layer implementation of ILivraisonService.
/// The real implementation is LivraisonQueryService in Infrastructure
/// (registered via DependencyInjection).
/// This class is kept as a fallback / reference.
/// </summary>
public class LivraisonService : ILivraisonService
{
    private readonly ICommandeRepository _commandeRepo;

    public LivraisonService(ICommandeRepository commandeRepo)
    {
        _commandeRepo = commandeRepo;
    }

    public Task<List<Livraison>> GetLivraisonsByLocalisationsAsync(List<int> localisationIds, string login)
    {
        // Stub: real implementation is in Infrastructure.
        return Task.FromResult(new List<Livraison>());
    }

    public async Task DeleteLivraisonAsync(int livraisonId)
    {
        // Stub: real implementation is in Infrastructure.
        var livraison = await _commandeRepo.GetLivraisonWithDetailsAsync(livraisonId);
        if (livraison != null)
        {
            // Placeholder -- actual deletion in Infrastructure's LivraisonQueryService.
        }
    }

    public Task MarkLivraisonDeliveredAsync(int livraisonId)
    {
        // Stub: real implementation is in Infrastructure.
        return Task.CompletedTask;
    }

    public Task<ServiceResult<int>> PayerLivraisonAsync(int livraisonId, string login)
    {
        // Stub: real implementation is in Infrastructure.
        return Task.FromResult(ServiceResult.Fail<int>("Not implemented in Application layer stub."));
    }
}
