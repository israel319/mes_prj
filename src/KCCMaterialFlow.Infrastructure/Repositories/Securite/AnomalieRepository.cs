using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Repositories;

/// <summary>
/// SEC-010: Implémentation du repository pour les anomalies.
/// Utilise IDbContextFactory pour éviter les problèmes de concurrence dans Blazor Server.
/// </summary>
public class AnomalieRepository : IAnomalieRepository
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;

    /// <summary>
    /// Propriété pour compatibilité avec le code existant - crée un nouveau DbContext pour chaque accès
    /// </summary>
    private KCCMaterialFlowDbContext _context => _dbContextFactory.CreateDbContext();

    public AnomalieRepository(IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    #region CRUD de base

    public async Task<Anomalie?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Anomalie>()
            .Include(a => a.ScanEvenement)
            .Include(a => a.Barriere)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Anomalie> CreateAsync(Anomalie anomalie, CancellationToken cancellationToken = default)
    {
        await _context.Set<Anomalie>().AddAsync(anomalie, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return anomalie;
    }

    public async Task UpdateAsync(Anomalie anomalie, CancellationToken cancellationToken = default)
    {
        _context.Set<Anomalie>().Update(anomalie);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var anomalie = await _context.Set<Anomalie>().FindAsync([id], cancellationToken);
        if (anomalie != null)
        {
            _context.Set<Anomalie>().Remove(anomalie);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Requêtes par Bon

    public async Task<IReadOnlyList<Anomalie>> GetByBonAsync(
        int bonId, 
        string typeBon, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Anomalie>()
            .Include(a => a.ScanEvenement)
            .Include(a => a.Barriere)
            .Where(a => a.BonId == bonId && a.TypeBon == typeBon)
            .OrderByDescending(a => a.DateSignalement)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasAnomaliesNonTraiteesAsync(
        int bonId, 
        string typeBon, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Anomalie>()
            .AnyAsync(a => 
                a.BonId == bonId && 
                a.TypeBon == typeBon && 
                !a.EstTraitee, 
                cancellationToken);
    }

    #endregion

    #region Anomalies Non Traitées

    public async Task<IReadOnlyList<Anomalie>> GetNonTraiteesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Anomalie>()
            .Include(a => a.ScanEvenement)
            .Include(a => a.Barriere)
            .Where(a => !a.EstTraitee)
            .OrderByDescending(a => a.NiveauGravite == NiveauGraviteValues.Critique)
            .ThenByDescending(a => a.NiveauGravite == NiveauGraviteValues.Eleve)
            .ThenByDescending(a => a.DateSignalement)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Anomalie>> GetNonTraiteesAsync(
        string? niveauGravite = null,
        string? typeAnomalie = null,
        int? barriereId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Anomalie>()
            .Include(a => a.ScanEvenement)
            .Include(a => a.Barriere)
            .Where(a => !a.EstTraitee);

        if (!string.IsNullOrWhiteSpace(niveauGravite))
            query = query.Where(a => a.NiveauGravite == niveauGravite);

        if (!string.IsNullOrWhiteSpace(typeAnomalie))
            query = query.Where(a => a.TypeAnomalie == typeAnomalie);

        if (barriereId.HasValue)
            query = query.Where(a => a.BarriereId == barriereId.Value);

        return await query
            .OrderByDescending(a => a.NiveauGravite == NiveauGraviteValues.Critique)
            .ThenByDescending(a => a.NiveauGravite == NiveauGraviteValues.Eleve)
            .ThenByDescending(a => a.DateSignalement)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetCountByGraviteAsync(
        bool nonTraiteesSeulementNot = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Anomalie>().AsQueryable();

        if (nonTraiteesSeulementNot)
            query = query.Where(a => !a.EstTraitee);

        return await query
            .GroupBy(a => a.NiveauGravite)
            .Select(g => new { Gravite = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Gravite, x => x.Count, cancellationToken);
    }

    #endregion

    #region Requêtes par Scan

    public async Task<IReadOnlyList<Anomalie>> GetByScanAsync(
        int scanId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Anomalie>()
            .Include(a => a.Barriere)
            .Where(a => a.ScanId == scanId)
            .OrderByDescending(a => a.DateSignalement)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Traitement des Anomalies

    public async Task MarquerCommeTraiteeAsync(
        int id,
        string traitePar,
        string resolution,
        string? actionsCorrectives = null,
        CancellationToken cancellationToken = default)
    {
        var anomalie = await _context.Set<Anomalie>().FindAsync([id], cancellationToken);
        if (anomalie != null)
        {
            anomalie.EstTraitee = true;
            anomalie.DateTraitement = DateTime.Now;
            anomalie.TraitePar = traitePar;
            anomalie.Resolution = resolution;
            anomalie.ActionsCorrectives = actionsCorrectives;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Recherche et Statistiques

    public async Task<(IReadOnlyList<Anomalie> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        string? typeAnomalie = null,
        string? niveauGravite = null,
        bool? estTraitee = null,
        int? barriereId = null,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Anomalie>()
            .Include(a => a.ScanEvenement)
            .Include(a => a.Barriere)
            .AsQueryable();

        // Filtres
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.Description.Contains(searchTerm) ||
                (a.NumeroReferenceBon != null && a.NumeroReferenceBon.Contains(searchTerm)) ||
                (a.SignaleParNom != null && a.SignaleParNom.Contains(searchTerm)) ||
                a.SignalePar.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(typeAnomalie))
            query = query.Where(a => a.TypeAnomalie == typeAnomalie);

        if (!string.IsNullOrWhiteSpace(niveauGravite))
            query = query.Where(a => a.NiveauGravite == niveauGravite);

        if (estTraitee.HasValue)
            query = query.Where(a => a.EstTraitee == estTraitee.Value);

        if (barriereId.HasValue)
            query = query.Where(a => a.BarriereId == barriereId.Value);

        if (dateDebut.HasValue)
            query = query.Where(a => a.DateSignalement >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(a => a.DateSignalement <= dateFin.Value);

        // Pagination
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.DateSignalement)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<AnomalieStats> GetStatistiquesAsync(
        DateTime dateDebut,
        DateTime dateFin,
        CancellationToken cancellationToken = default)
    {
        var anomalies = await _context.Set<Anomalie>()
            .Where(a => a.DateSignalement >= dateDebut && a.DateSignalement <= dateFin)
            .ToListAsync(cancellationToken);

        var stats = new AnomalieStats
        {
            TotalAnomalies = anomalies.Count,
            AnomaliesTraitees = anomalies.Count(a => a.EstTraitee),
            AnomaliesNonTraitees = anomalies.Count(a => !a.EstTraitee),
            AnomaliesCritiques = anomalies.Count(a => a.NiveauGravite == NiveauGraviteValues.Critique),
            ParType = anomalies.GroupBy(a => a.TypeAnomalie).ToDictionary(g => g.Key, g => g.Count()),
            ParGravite = anomalies.GroupBy(a => a.NiveauGravite).ToDictionary(g => g.Key, g => g.Count())
        };

        // Taux de résolution
        stats.TauxResolution = stats.TotalAnomalies > 0 
            ? (double)stats.AnomaliesTraitees / stats.TotalAnomalies * 100 
            : 0;

        // Délai moyen de résolution
        var anomaliesResolues = anomalies.Where(a => a.EstTraitee && a.DateTraitement.HasValue).ToList();
        if (anomaliesResolues.Any())
        {
            var delaisEnMinutes = anomaliesResolues
                .Select(a => (a.DateTraitement!.Value - a.DateSignalement).TotalMinutes)
                .Average();
            stats.DelaiMoyenResolution = TimeSpan.FromMinutes(delaisEnMinutes);
        }

        return stats;
    }

    #endregion
}
