using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <inheritdoc />
public sealed class MaterielCatalogService(
    IDbContextFactory<KCCMaterialFlowDbContext> ctxFactory)
    : IMaterielCatalogService
{
    public async Task<IReadOnlyList<MaterielSuggestion>> SearchAsync(
        string? fragment, string? departementCode, int maxResults = 20, CancellationToken ct = default)
    {
        await using var ctx = await ctxFactory.CreateDbContextAsync(ct);

        var q = ctx.Materiels.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(departementCode))
        {
            var dept = departementCode.Trim();
            q = q.Where(m => m.DepartementCode == dept || m.DepartementCode == null);
        }

        if (!string.IsNullOrWhiteSpace(fragment))
        {
            var f = fragment.Trim().ToLower();
            q = q.Where(m =>
                (m.CodeProduitSerial != null && m.CodeProduitSerial.ToLower().Contains(f)) ||
                (m.Designation != null && m.Designation.ToLower().Contains(f)));
        }

        var raw = await q
            .OrderByDescending(m => m.Id)
            .Select(m => new { m.CodeProduitSerial, m.Designation, m.DepartementCode })
            .Take(maxResults * 4)
            .ToListAsync(ct);

        return raw
            .Where(r => !string.IsNullOrWhiteSpace(r.CodeProduitSerial)
                     && !string.IsNullOrWhiteSpace(r.Designation))
            .GroupBy(r => (Code: r.CodeProduitSerial!.Trim(), Des: r.Designation!.Trim()), comparer: null)
            .Select(g => new MaterielSuggestion
            {
                CodeProduitSerial = g.Key.Code,
                Designation = g.Key.Des,
                DepartementCode = g.Select(x => x.DepartementCode).FirstOrDefault(d => !string.IsNullOrWhiteSpace(d))
            })
            .Take(maxResults)
            .ToList();
    }
}
