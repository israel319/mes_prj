using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de journalisation d'audit.
/// Implémentation Infrastructure utilisant IDbContextFactory pour un cycle de vie correct des DbContext.
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        ILogger<AuditLogService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task LogAsync(
        string utilisateurLogin,
        string? utilisateurNom,
        string typeAction,
        string categorie,
        string description,
        string? entiteId = null,
        string? entiteType = null,
        string? details = null,
        string? ancienneValeur = null,
        string? nouvelleValeur = null,
        string resultat = "Succes",
        string niveau = "Info",
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var auditLog = new AuditLog
        {
            DateAction = DateTime.Now,
            UtilisateurLogin = utilisateurLogin,
            UtilisateurNom = utilisateurNom,
            TypeAction = typeAction,
            Categorie = categorie,
            Description = description,
            EntiteId = entiteId,
            EntiteType = entiteType,
            Details = details,
            AncienneValeur = ancienneValeur,
            NouvelleValeur = nouvelleValeur,
            Resultat = resultat,
            Niveau = niveau
        };

        context.Set<AuditLog>().Add(auditLog);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Audit: {TypeAction} - {Categorie} - {Description} par {User}",
            typeAction, categorie, description, utilisateurLogin);
    }

    public async Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        string? utilisateurLogin = null,
        string? typeAction = null,
        string? categorie = null,
        string? niveau = null,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var query = context.Set<AuditLog>().AsNoTracking();

        if (dateDebut.HasValue)
            query = query.Where(a => a.DateAction >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(a => a.DateAction <= dateFin.Value.AddDays(1));

        if (!string.IsNullOrWhiteSpace(utilisateurLogin))
            query = query.Where(a => a.UtilisateurLogin.ToUpper().Contains(utilisateurLogin.ToUpper()));

        if (!string.IsNullOrWhiteSpace(typeAction))
            query = query.Where(a => a.TypeAction == typeAction);

        if (!string.IsNullOrWhiteSpace(categorie))
            query = query.Where(a => a.Categorie == categorie);

        if (!string.IsNullOrWhiteSpace(niveau))
            query = query.Where(a => a.Niveau == niveau);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.DateAction)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(
        string entiteType,
        string entiteId,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<AuditLog>()
            .AsNoTracking()
            .Where(a => a.EntiteType == entiteType && a.EntiteId == entiteId)
            .OrderByDescending(a => a.DateAction)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByUserAsync(
        string utilisateurLogin,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<AuditLog>()
            .AsNoTracking()
            .Where(a => a.UtilisateurLogin.ToUpper() == utilisateurLogin.ToUpper())
            .OrderByDescending(a => a.DateAction)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetActionTypesAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<AuditLog>()
            .AsNoTracking()
            .Select(a => a.TypeAction)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<AuditLog>()
            .AsNoTracking()
            .Select(a => a.Categorie)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    public async Task<AuditStats> GetStatsAsync(
        DateTime dateDebut,
        DateTime dateFin,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var query = context.Set<AuditLog>()
            .AsNoTracking()
            .Where(a => a.DateAction >= dateDebut && a.DateAction <= dateFin.AddDays(1));

        var logs = await query.ToListAsync(cancellationToken);

        var stats = new AuditStats
        {
            TotalActions = logs.Count,
            TotalUsers = logs.Select(l => l.UtilisateurLogin).Distinct().Count(),
            TotalErrors = logs.Count(l => l.Resultat == "Echec"),
            ActionsByType = logs
                .GroupBy(l => l.TypeAction)
                .ToDictionary(g => g.Key, g => g.Count()),
            ActionsByCategory = logs
                .GroupBy(l => l.Categorie)
                .ToDictionary(g => g.Key, g => g.Count()),
            ActionsByUser = logs
                .GroupBy(l => l.UtilisateurLogin)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToDictionary(g => g.Key, g => g.Count()),
            ActionsPerDay = logs
                .GroupBy(l => l.DateAction.Date)
                .Select(g => (g.Key, g.Count()))
                .OrderBy(x => x.Key)
                .ToList()
        };

        return stats;
    }
}
