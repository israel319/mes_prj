using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

public sealed class WorkflowApprobateurSpecialService : IWorkflowApprobateurSpecialService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _factory;

    public WorkflowApprobateurSpecialService(IDbContextFactory<KCCMaterialFlowDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<WorkflowApprobateurSpecial>> GetAllAsync(CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct);
        return await ctx.WorkflowApprobateursSpeciaux
            .Include(x => x.Employee)
            .Include(x => x.Site)
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Ordre)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<WorkflowApprobateurSpecial>> GetByTypeAsync(
        TypeApprobateurSpecial type, bool actifsOnly = true, CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct);
        var q = ctx.WorkflowApprobateursSpeciaux.Include(x => x.Employee).Where(x => x.Type == type);
        if (actifsOnly) q = q.Where(x => x.EstActif);
        return await q.OrderBy(x => x.Ordre).AsNoTracking().ToListAsync(ct);
    }

    public async Task<WorkflowApprobateurSpecial> AddAsync(
        TypeApprobateurSpecial type, int employeeId, int ordre = 1, int? siteId = null, CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct);

        var exists = await ctx.WorkflowApprobateursSpeciaux
            .AnyAsync(x => x.Type == type && x.EmployeeId == employeeId && x.SiteId == siteId, ct);
        if (exists)
            throw new InvalidOperationException($"L'employé est déjà enregistré comme {type} pour ce site.");

        var entity = new WorkflowApprobateurSpecial
        {
            Type = type,
            EmployeeId = employeeId,
            Ordre = ordre,
            SiteId = siteId,
            EstActif = true,
            DateCreation = DateTime.Now
        };
        ctx.WorkflowApprobateursSpeciaux.Add(entity);
        await ctx.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(int id, int ordre, bool estActif, CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct);
        var entity = await ctx.WorkflowApprobateursSpeciaux.FirstOrDefaultAsync(x => x.Id == id, ct)
                     ?? throw new KeyNotFoundException($"Approbateur spécial {id} introuvable");
        entity.Ordre = ordre;
        entity.EstActif = estActif;
        await ctx.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await using var ctx = await _factory.CreateDbContextAsync(ct);
        await ctx.WorkflowApprobateursSpeciaux.Where(x => x.Id == id).ExecuteDeleteAsync(ct);
    }
}
