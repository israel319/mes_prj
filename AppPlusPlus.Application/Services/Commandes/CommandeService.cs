using AppPlusPlus.Application.DTOs.Commandes;
using AppPlusPlus.Domain.Entities.Commandes;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Commandes;

public class CommandeService : ICommandeService
{
    private readonly ICommandeRepository _commandeRepo;

    public CommandeService(ICommandeRepository commandeRepo)
    {
        _commandeRepo = commandeRepo;
    }

    public async Task<CommandeListResult> GetCommandesWithStatusUpdateAsync()
    {
        // 1. Load all commandes with Customer, Details, Livraisons, Factures
        var commandes = await _commandeRepo.GetAllWithFullDetailsAsync();

        // 2. Compute delivered quantities per CommandeDetail
        var allDetailIds = commandes
            .SelectMany(c => c.Details)
            .Select(d => d.Id)
            .ToList();

        var deliveredQty = await _commandeRepo.GetDeliveredQtyByDetailIdsAsync(allDetailIds);

        // 3. Compute paid amounts per Commande
        var cmdIds = commandes.Select(c => c.CommandeId).ToList();
        var paidByCommande = await _commandeRepo.GetPaidAmountByCommandeIdsAsync(cmdIds);

        // 4. Auto-correct payment amounts and statuses
        var commandesToUpdate = new List<Commande>();
        foreach (var cmd in commandes)
        {
            bool changed = false;

            // Reconcile payment amounts from Factures/Payments
            var paye = paidByCommande.GetValueOrDefault(cmd.CommandeId);
            if (cmd.MontantPaye != paye)
            {
                cmd.MontantPaye = paye;
                cmd.MontantRest = cmd.MontantTotal - paye;
                if (cmd.MontantRest < 0) cmd.MontantRest = 0;
                changed = true;
            }

            // Derive expected status from livraisons and payments
            bool hasLivraisons = cmd.Livraisons.Any();
            bool allDelivered = cmd.Details.Any() && hasLivraisons
                                && !HasRemainingToDeliver(cmd, deliveredQty);

            int expectedStatus;
            if (cmd.MontantPaye >= cmd.MontantTotal && cmd.MontantTotal > 0)
                expectedStatus = 3; // Fully paid (facturee)
            else if (allDelivered)
                expectedStatus = 2; // Fully delivered (livree)
            else if (hasLivraisons)
                expectedStatus = 1; // Partially delivered (en cours)
            else
                expectedStatus = 0; // New

            if (cmd.Status != expectedStatus)
            {
                cmd.Status = expectedStatus;
                changed = true;
            }

            if (changed)
                commandesToUpdate.Add(cmd);
        }

        // 5. Persist corrections
        if (commandesToUpdate.Any())
        {
            await _commandeRepo.UpdateCommandeStatusBatchAsync(commandesToUpdate);
        }

        return new CommandeListResult
        {
            Commandes = commandes,
            DeliveredQtyByDetailId = deliveredQty
        };
    }

    public async Task<List<Commande>> GetCommandesWithDetailsAsync()
    {
        return await _commandeRepo.GetAllWithFullDetailsAsync();
    }

    public async Task DeleteCommandeAsync(int commandeId)
    {
        var commande = await _commandeRepo.GetByIdAsync(commandeId);
        if (commande != null)
            await _commandeRepo.DeleteAsync(commande);
    }

    public bool HasRemainingToDeliver(Commande cmd, Dictionary<int, decimal> deliveredQty)
    {
        if (!cmd.Details.Any()) return false;
        foreach (var d in cmd.Details)
        {
            var livree = deliveredQty.GetValueOrDefault(d.Id);
            if ((d.Qte ?? 0) - livree > 0) return true;
        }
        return false;
    }
}
