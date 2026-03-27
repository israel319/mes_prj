using AppPlusPlus.Application.DTOs.Commandes;
using AppPlusPlus.Domain.Entities.Commandes;
using AppPlusPlus.Application.Interfaces.Repositories;

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

    public async Task<Commande?> GetCommandeWithDetailsAsync(int commandeId)
    {
        return await _commandeRepo.GetWithDetailsAsync(commandeId);
    }

    public async Task SaveCommandeAsync(Commande commande, List<CommandeDetail> details)
    {
        await _commandeRepo.SaveCommandeWithDetailsAsync(commande, details);
    }

    public async Task<CommandeDetailViewDto?> GetCommandeDetailViewAsync(int commandeId)
    {
        var cmd = await _commandeRepo.GetWithAllNavigationsAsync(commandeId);
        if (cmd is null)
            return null;

        // Compute delivered quantities
        var detailIds = cmd.Details.Select(d => d.Id).ToList();
        var deliveredQty = detailIds.Any()
            ? await _commandeRepo.GetDeliveredQtyByDetailIdsAsync(detailIds)
            : new Dictionary<int, decimal>();

        // Map article rows
        var articleRows = cmd.Details.Select(d =>
        {
            var qte = d.Qte ?? 0;
            var livrQte = deliveredQty.GetValueOrDefault(d.Id);
            return new CommandeArticleRow
            {
                ArticleName = d.Article?.Description ?? d.ArticleId ?? "—",
                Qte = qte,
                PU = d.Pu ?? 0,
                Montant = qte * (d.Pu ?? 0),
                QteLivree = livrQte
            };
        }).ToList();

        // Map livraison rows
        var livraisonRows = cmd.Livraisons
            .OrderByDescending(l => l.Date)
            .Select(l =>
            {
                var fact = l.Fact;
                var montant = l.Details.Sum(d => d.MontantPaye ?? 0);
                int? factureId = fact?.Id;
                decimal totalPaye = fact?.Payments.Sum(p => p.Amount) ?? 0;

                string statusLabel;
                string statusStyle;

                if (fact != null)
                {
                    if (totalPaye >= montant && montant > 0)
                    { statusLabel = "Payée"; statusStyle = "bg-success"; }
                    else if (totalPaye > 0)
                    { statusLabel = "Partiel"; statusStyle = "bg-warning text-dark"; }
                    else
                    { statusLabel = "Non payée"; statusStyle = "bg-danger"; }
                }
                else
                {
                    var s = l.Status ?? 0;
                    (statusLabel, statusStyle) = s switch
                    {
                        0 => ("En attente", "bg-light text-dark"),
                        1 => ("Livrée — Non facturée", "bg-info"),
                        _ => ("—", "bg-secondary")
                    };
                }

                return new CommandeLivraisonRow
                {
                    LivraisonId = l.LivraisonId,
                    Porteur = l.Porteur ?? "—",
                    Montant = montant,
                    Date = l.Date,
                    FactureId = factureId,
                    TotalPaye = totalPaye,
                    StatusLabel = statusLabel,
                    StatusStyle = statusStyle
                };
            }).ToList();

        return new CommandeDetailViewDto
        {
            Commande = cmd,
            ArticleRows = articleRows,
            LivraisonRows = livraisonRows
        };
    }
}
