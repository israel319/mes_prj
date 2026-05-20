using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <inheritdoc />
public sealed class AllEmployeeSearchService(
    IDbContextFactory<KCCMaterialFlowDbContext> ctxFactory)
    : IAllEmployeeSearchService
{
    public async Task<IReadOnlyList<AllEmployee>> SearchAsync(
        string? fragment, int maxResults = 25, CancellationToken ct = default)
    {
        await using var ctx = await ctxFactory.CreateDbContextAsync(ct);
        var query = ctx.AllEmployees.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(fragment))
        {
            var f = fragment.Trim().ToLower();
            query = query.Where(g =>
                (g.EmployeeCode != null && g.EmployeeCode.ToLower().Contains(f)) ||
                (g.FirstName != null && g.FirstName.ToLower().Contains(f)) ||
                (g.LastName != null && g.LastName.ToLower().Contains(f)) ||
                (g.UserName != null && g.UserName.ToLower().Contains(f)) ||
                (g.Departement != null && g.Departement.ToLower().Contains(f)) ||
                (g.Mail != null && g.Mail.ToLower().Contains(f)));
        }

        return await query
            .OrderBy(g => g.LastName)
            .ThenBy(g => g.FirstName)
            .Take(maxResults)
            .ToListAsync(ct);
    }

    public async Task<AllEmployee?> GetByCodeAsync(string employeeCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(employeeCode)) return null;
        await using var ctx = await ctxFactory.CreateDbContextAsync(ct);
        return await ctx.AllEmployees.AsNoTracking()
            .FirstOrDefaultAsync(g => g.EmployeeCode == employeeCode, ct);
    }

    public async Task<string?> GetSiteManagerForAsync(string requestedForEmployeeCode, CancellationToken ct = default)
    {
        var demandeur = await GetByCodeAsync(requestedForEmployeeCode, ct);
        if (demandeur is null) return null;

        // Privilégier le display direct s'il est rempli, sinon résoudre via ManagerHodEmployeeCode
        if (!string.IsNullOrWhiteSpace(demandeur.ManagerHodEmployeeDisplay))
            return demandeur.ManagerHodEmployeeDisplay;

        if (string.IsNullOrWhiteSpace(demandeur.ManagerHodEmployeeCode)) return null;
        var hod = await GetByCodeAsync(demandeur.ManagerHodEmployeeCode, ct);
        if (hod is null) return null;
        return $"{hod.FirstName} {hod.LastName}".Trim();
    }

    public async Task<IReadOnlyList<string>> GetDistinctDepartementsAsync(CancellationToken ct = default)
    {
        await using var ctx = await ctxFactory.CreateDbContextAsync(ct);
        return await ctx.AllEmployees
            .AsNoTracking()
            .Where(e => e.Departement != null && e.Departement != "")
            .Select(e => e.Departement!)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync(ct);
    }
}
