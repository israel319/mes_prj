using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Application.DTOs.Vente;
using AppPlusPlus.Application.Services.Vente;
using AppPlusPlus.Infrastructure.Persistence;

namespace AppPlusPlus.Infrastructure.QueryServices.Vente;

public class FacturationQueryService : IFacturationService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public FacturationQueryService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<FactRowDto>> GetFactureRowsAsync(List<int> localisationIds, string login)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var articleNames = await ctx.Articles
            .ToDictionaryAsync(a => a.IdArticle, a => a.Description ?? a.IdArticle);

        var factsQuery = ctx.Facts
            .Include(f => f.Details)
            .AsQueryable();

        // Filter by user's localisations if not admin (non-empty list = non-admin)
        if (localisationIds.Any())
        {
            // Exclude factures on already-closed dates for the user's localisations
            var cloturedDates = await ctx.Versements
                .Where(v => localisationIds.Contains(v.LocalisationId))
                .Select(v => v.DateCloture)
                .Distinct()
                .ToListAsync();

            factsQuery = factsQuery.Where(f =>
                (f.User.ToLower() == login.ToLower()
                || f.Details.Any(d => d.Localisationid.HasValue && localisationIds.Contains(d.Localisationid.Value)))
                && !cloturedDates.Contains(f.Date));
        }

        var facts = await factsQuery
            .OrderByDescending(f => f.Date)
            .ThenByDescending(f => f.Id)
            .ToListAsync();

        var paymentsByFact = await ctx.Payments
            .GroupBy(p => p.IdFact)
            .Select(g => new { IdFact = g.Key, Total = g.Sum(x => x.Amount) })
            .ToDictionaryAsync(x => x.IdFact, x => (double)x.Total);

        return facts.Select(f =>
        {
            var articlesStr = f.Details.Any()
                ? string.Join(", ", f.Details
                    .Where(d => d.IdArticle != null)
                    .Select(d => articleNames.GetValueOrDefault(d.IdArticle!, d.IdArticle!)))
                : f.DescriptionArticle ?? "";

            var total = (double)(f.TotalApresReduction ?? f.Total ?? 0);
            var paye = paymentsByFact.GetValueOrDefault(f.Id);

            return new FactRowDto
            {
                Id = f.Id,
                Client = f.DescriptionName,
                Articles = articlesStr,
                TotalQte = f.Details.Sum(d => d.Qte ?? 0),
                Total = total,
                Paye = paye,
                Solde = total - paye,
                Date = f.Date,
                Status = f.Status,
                Login = f.User,
            };
        }).ToList();
    }

    public async Task DeleteFactureAsync(int factId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        var fact = await ctx.Facts.Include(f => f.Details).FirstOrDefaultAsync(f => f.Id == factId);
        if (fact != null)
        {
            ctx.FactDetails.RemoveRange(fact.Details);
            ctx.Facts.Remove(fact);
            await ctx.SaveChangesAsync();
        }
    }

    public async Task<List<FactureViewDto>> GetPaiementsAsync(List<int> localisationIds, string login)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var facturesQuery = ctx.Facts
            .Include(f => f.Customer)
            .Include(f => f.Payments)
            .Include(f => f.Details)
            .Where(f => f.Status >= 1 && f.Status <= 2)
            .AsQueryable();

        // Filter by user's localisations if not admin
        if (localisationIds.Any())
        {
            facturesQuery = facturesQuery.Where(f =>
                f.User.ToLower() == login.ToLower()
                || f.Details.Any(d => d.Localisationid.HasValue && localisationIds.Contains(d.Localisationid.Value)));
        }

        var factures = await facturesQuery
            .OrderByDescending(f => f.DateSys)
            .ToListAsync();

        return factures.Select(f =>
        {
            var totalPaye = f.Payments?.Sum(p => p.Amount) ?? 0;
            var totalFacture = f.TotalApresReduction ?? f.Total ?? 0;
            return new FactureViewDto
            {
                Facture = f,
                ClientName = f.Customer?.CustomerName ?? f.DescriptionName,
                TotalPaye = totalPaye,
                Reste = totalFacture - totalPaye,
                Payments = f.Payments?.OrderByDescending(p => p.Date).ToList() ?? new()
            };
        }).ToList();
    }
}
