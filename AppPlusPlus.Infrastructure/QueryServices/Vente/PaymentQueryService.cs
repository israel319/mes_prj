using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Application.Services.Vente;
using AppPlusPlus.Domain.Entities.Vente;
using AppPlusPlus.Infrastructure.Persistence;

namespace AppPlusPlus.Infrastructure.QueryServices.Vente;

public class PaymentQueryService : IPaymentService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public PaymentQueryService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Payment>> GetPaymentsByFactureAsync(int factId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Payments.Where(p => p.IdFact == factId).ToListAsync();
    }

    public async Task RecordPaymentAsync(
        int factId, int? commandeId, decimal amount, string mode, string? note, string userLogin)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        await using var tx = await ctx.Database.BeginTransactionAsync();

        try
        {
            // 1) Create the payment
            var payment = new Payment
            {
                IdFact = factId,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Amount = amount,
                Note = mode + (string.IsNullOrWhiteSpace(note) ? "" : $" — {note}"),
                DateSys = DateTime.Now,
                User = userLogin
            };
            ctx.Payments.Add(payment);
            await ctx.SaveChangesAsync();

            // 2) If commandeId is set, update Commande payment fields
            if (commandeId.HasValue)
            {
                var commande = await ctx.Commandes.FindAsync(commandeId.Value);
                if (commande != null)
                {
                    commande.MontantPaye += amount;
                    commande.MontantRest = commande.MontantTotal - commande.MontantPaye;
                    if (commande.MontantRest < 0) commande.MontantRest = 0;
                }
            }

            // 3) Recalculate facture status based on total payments
            var facture = await ctx.Facts.FindAsync(factId);
            if (facture != null)
            {
                var totalPaye = await ctx.Payments
                    .Where(p => p.IdFact == factId)
                    .SumAsync(p => p.Amount);

                if (totalPaye >= (facture.TotalApresReduction ?? facture.Total ?? 0))
                    facture.Status = 2; // Fully paid
                else
                    facture.Status = 1; // Partial payment
            }

            await ctx.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
