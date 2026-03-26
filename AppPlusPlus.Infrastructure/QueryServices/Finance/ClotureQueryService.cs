using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Application.Services.Finance;
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
}
