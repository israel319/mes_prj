using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Application.DTOs.Finance;
using AppPlusPlus.Application.Services.Finance;
using AppPlusPlus.Domain.Entities.Approvisionnement;
using AppPlusPlus.Domain.Entities.Finance;
using AppPlusPlus.Infrastructure.Persistence;

namespace AppPlusPlus.Infrastructure.QueryServices.Finance;

public class ClotureQueryService : IClotureService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public ClotureQueryService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Versement>> GetCloturesByLocalisationsAsync(List<int> localisationIds)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var query = ctx.Versements
            .Include(v => v.Localisation)
            .AsQueryable();

        if (localisationIds.Any())
        {
            query = query.Where(v => localisationIds.Contains(v.LocalisationId));
        }

        return await query
            .OrderByDescending(v => v.DateCloture)
            .ThenByDescending(v => v.HeureCloture)
            .ToListAsync();
    }

    public async Task<bool> ClotureExistsAsync(DateOnly date, int localisationId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Versements.AnyAsync(v =>
            v.DateCloture == date && v.LocalisationId == localisationId);
    }

    public async Task<ClotureSummaryDto> GetClotureSummaryAsync(DateOnly date, int localisationId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var dto = new ClotureSummaryDto();

        // Check if cloture already exists
        dto.DejaClotureExiste = await ctx.Versements.AnyAsync(v =>
            v.DateCloture == date && v.LocalisationId == localisationId);

        // Paid factures (Status == 2) for the date, filtered via FactDetail.Localisationid
        // Include NULL Localisationid for retrocompatibility (old data before pre-selection fix)
        var factures = await ctx.Facts
            .Include(f => f.Details)
            .Where(f => f.Status == 2
                && f.Date == date
                && f.Details.Any(d => d.Localisationid == localisationId || !d.Localisationid.HasValue))
            .ToListAsync();

        dto.NbFactures = factures.Count;
        dto.TotalFactures = factures.Sum(f => f.TotalApresReduction ?? f.Total ?? 0);

        // Payments for the date, filtered through Fact -> FactDetail.Localisationid
        var paiements = await ctx.Payments
            .Include(p => p.Fact).ThenInclude(f => f!.Details)
            .Where(p => p.Date == date
                && p.Fact != null
                && p.Fact.Details.Any(d => d.Localisationid == localisationId || !d.Localisationid.HasValue))
            .ToListAsync();

        dto.NbPaiements = paiements.Count;
        dto.TotalPaiements = paiements.Sum(p => p.Amount);

        return dto;
    }

    public async Task CreateClotureAsync(Versement versement)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        // 1. Créer la clôture (Versement)
        ctx.Versements.Add(versement);
        await ctx.SaveChangesAsync();

        // 2. Résoudre ou créer la source "Clôture journalière" dans T_Expense_Source
        var source = await ctx.Set<ExpenseSource>()
            .FirstOrDefaultAsync(s => s.Sources == "Clôture journalière");
        if (source == null)
        {
            source = new ExpenseSource { Sources = "Clôture journalière" };
            ctx.Set<ExpenseSource>().Add(source);
            await ctx.SaveChangesAsync();
        }

        // 3. Créer automatiquement l'entrée en caisse liée
        var locName = await ctx.Localisations
            .Where(l => l.IdLocalisation == versement.LocalisationId)
            .Select(l => l.DescriptionLocalisation)
            .FirstOrDefaultAsync() ?? "";

        var expense = new ApproExpense
        {
            SourceId = source.Id,
            VersementId = versement.Id,
            AmountCDF = versement.Montant,
            Description = $"Clôture {locName} — {versement.DateCloture:dd/MM/yyyy}",
            Depositeur = versement.UserLogin,
            Comment = versement.Observation,
            UserLogin = versement.UserLogin,
            CreationDate = DateTime.Now,
        };
        ctx.ApproExpenses.Add(expense);
        await ctx.SaveChangesAsync();
    }
}
