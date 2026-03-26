using AppPlusPlus.Application.DTOs.Commandes;
using AppPlusPlus.Domain.Entities.Commandes;

namespace AppPlusPlus.Application.Services.Commandes;

public interface ICommandeService
{
    /// <summary>
    /// Returns all client commandes with full navigation properties loaded,
    /// auto-corrected statuses and payment amounts (persisted if changed),
    /// plus a delivered-quantity lookup.
    /// </summary>
    Task<CommandeListResult> GetCommandesWithStatusUpdateAsync();

    Task<List<Commande>> GetCommandesWithDetailsAsync();

    Task DeleteCommandeAsync(int commandeId);

    /// <summary>
    /// Determines whether the given commande still has at least one detail line
    /// where the ordered quantity exceeds the delivered quantity.
    /// </summary>
    bool HasRemainingToDeliver(Commande cmd, Dictionary<int, decimal> deliveredQty);
}
