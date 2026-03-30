using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Repositories;

/// <summary>
/// SEC-008: Implémentation du repository pour les événements de scan.
/// Utilise IDbContextFactory pour éviter les problèmes de concurrence dans Blazor Server.
/// </summary>
public class ScanRepository : IScanRepository
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;

    /// <summary>
    /// Propriété pour compatibilité avec le code existant - crée un nouveau DbContext pour chaque accès
    /// </summary>
    private KCCMaterialFlowDbContext _context => _dbContextFactory.CreateDbContext();

    public ScanRepository(IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    #region CRUD de base

    public async Task<ScanEvenement?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ScanEvenement>()
            .Include(s => s.Barriere)
            .Include(s => s.Anomalies)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<ScanEvenement> CreateScanAsync(ScanEvenement scan, CancellationToken cancellationToken = default)
    {
        await _context.Set<ScanEvenement>().AddAsync(scan, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return scan;
    }

    public async Task UpdateAsync(ScanEvenement scan, CancellationToken cancellationToken = default)
    {
        _context.Set<ScanEvenement>().Update(scan);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var scan = await _context.Set<ScanEvenement>().FindAsync([id], cancellationToken);
        if (scan != null)
        {
            _context.Set<ScanEvenement>().Remove(scan);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Requêtes par Bon

    public async Task<IReadOnlyList<ScanEvenement>> GetScansByBonAsync(
        int bonId, 
        string typeBon, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<ScanEvenement>()
            .Include(s => s.Barriere)
            .Where(s => s.BonId == bonId && s.TypeBon == typeBon)
            .OrderByDescending(s => s.DateHeureScan)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasBeenScannedAtBarriereAsync(
        int bonId, 
        string typeBon, 
        int barriereId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<ScanEvenement>()
            .AnyAsync(s => 
                s.BonId == bonId && 
                s.TypeBon == typeBon && 
                s.BarriereId == barriereId &&
                s.StatutScan == StatutScanValues.Valid, 
                cancellationToken);
    }

    public async Task<ScanEvenement?> GetLastScanForBonAsync(
        int bonId, 
        string typeBon, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<ScanEvenement>()
            .Include(s => s.Barriere)
            .Where(s => s.BonId == bonId && s.TypeBon == typeBon)
            .OrderByDescending(s => s.DateHeureScan)
            .FirstOrDefaultAsync(cancellationToken);
    }

    #endregion

    #region Requêtes par Barrière

    public async Task<IReadOnlyList<ScanEvenement>> GetScansParBarriereAsync(
        int barriereId,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<ScanEvenement>()
            .Include(s => s.Barriere)
            .Where(s => s.BarriereId == barriereId);

        if (dateDebut.HasValue)
            query = query.Where(s => s.DateHeureScan >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(s => s.DateHeureScan <= dateFin.Value);

        return await query
            .OrderByDescending(s => s.DateHeureScan)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<ScanEvenement> Items, int TotalCount)> GetScansParBarrierePagedAsync(
        int barriereId,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        string? statutScan = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<ScanEvenement>()
            .Include(s => s.Barriere)
            .Where(s => s.BarriereId == barriereId);

        if (dateDebut.HasValue)
            query = query.Where(s => s.DateHeureScan >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(s => s.DateHeureScan <= dateFin.Value);

        if (!string.IsNullOrWhiteSpace(statutScan))
            query = query.Where(s => s.StatutScan == statutScan);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.DateHeureScan)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    #endregion

    #region Requêtes par Agent

    public async Task<IReadOnlyList<ScanEvenement>> GetScansByAgentAsync(
        string agentLogin,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<ScanEvenement>()
            .Include(s => s.Barriere)
            .Where(s => s.AgentLogin == agentLogin);

        if (dateDebut.HasValue)
            query = query.Where(s => s.DateHeureScan >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(s => s.DateHeureScan <= dateFin.Value);

        return await query
            .OrderByDescending(s => s.DateHeureScan)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Recherche et Statistiques

    public async Task<(IReadOnlyList<ScanEvenement> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        int? barriereId = null,
        string? statutScan = null,
        string? typeMouvement = null,
        string? agentLogin = null,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<ScanEvenement>()
            .Include(s => s.Barriere)
            .AsQueryable();

        // Filtres
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s =>
                (s.NumeroReferenceBon != null && s.NumeroReferenceBon.Contains(searchTerm)) ||
                (s.AgentNom != null && s.AgentNom.Contains(searchTerm)) ||
                s.AgentLogin.Contains(searchTerm) ||
                (s.QRCodeData != null && s.QRCodeData.Contains(searchTerm)));
        }

        if (barriereId.HasValue)
            query = query.Where(s => s.BarriereId == barriereId.Value);

        if (!string.IsNullOrWhiteSpace(statutScan))
            query = query.Where(s => s.StatutScan == statutScan);

        if (!string.IsNullOrWhiteSpace(typeMouvement))
            query = query.Where(s => s.TypeMouvement == typeMouvement);

        if (!string.IsNullOrWhiteSpace(agentLogin))
            query = query.Where(s => s.AgentLogin == agentLogin);

        if (dateDebut.HasValue)
            query = query.Where(s => s.DateHeureScan >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(s => s.DateHeureScan <= dateFin.Value);

        // Pagination
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.DateHeureScan)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Dictionary<string, int>> GetScanCountByStatutAsync(
        DateTime dateDebut,
        DateTime dateFin,
        int? barriereId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<ScanEvenement>()
            .Where(s => s.DateHeureScan >= dateDebut && s.DateHeureScan <= dateFin);

        if (barriereId.HasValue)
            query = query.Where(s => s.BarriereId == barriereId.Value);

        return await query
            .GroupBy(s => s.StatutScan)
            .Select(g => new { Statut = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Statut, x => x.Count, cancellationToken);
    }

    public async Task<IReadOnlyList<ScanEvenement>> GetScansAvecAnomaliesAsync(
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<ScanEvenement>()
            .Include(s => s.Barriere)
            .Include(s => s.Anomalies)
            .Where(s => s.AnomalieSignalee);

        if (dateDebut.HasValue)
            query = query.Where(s => s.DateHeureScan >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(s => s.DateHeureScan <= dateFin.Value);

        return await query
            .OrderByDescending(s => s.DateHeureScan)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Itinéraire & Historique

    public async Task<ItineraireInfo?> GetItinerairePrevuAsync(int bonId, int barriereId, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var itineraire = await context.Set<ItinerairePrevu>()
            .AsNoTracking()
            .Where(i => i.BonId == bonId && i.BarriereId == barriereId)
            .FirstOrDefaultAsync(cancellationToken);

        return itineraire == null ? null : new ItineraireInfo { OrdrePassage = itineraire.OrdrePassage };
    }

    public async Task<ItineraireInfo?> GetItineraireSortieAsync(int bonSortieId, int barriereId, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var itineraire = await context.Set<ItineraireSortie>()
            .AsNoTracking()
            .Where(i => i.BonId == bonSortieId && i.BarriereId == barriereId)
            .FirstOrDefaultAsync(cancellationToken);

        return itineraire == null ? null : new ItineraireInfo { OrdrePassage = itineraire.OrdrePassage };
    }

    public async Task<ScanBonInfo?> GetBonInfoAsync(int bonId, string typeBon, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();

        if (typeBon == "BEM")
        {
            var bon = await context.Set<BonEntree>()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bonId, cancellationToken);

            return bon == null ? null : new ScanBonInfo
            {
                IdBon = bon.Id,
                NumeroReference = bon.NumeroReference,
                Provenance = bon.Provenance,
                Destination = bon.Destination,
                DateExpiration = bon.DateExpiration,
                Quantite = bon.Quantite
            };
        }
        else
        {
            var bon = await context.Set<BonSortie>()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bonId, cancellationToken);

            return bon == null ? null : new ScanBonInfo
            {
                IdBon = bon.Id,
                NumeroReference = bon.NumeroReference,
                Provenance = bon.Provenance,
                Destination = bon.Destination,
                DateExpiration = bon.DateExpiration,
                Quantite = bon.Quantite
            };
        }
    }

    public async Task UpdateItineraireSortiePassageAsync(int bonSortieId, int barriereId, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var itineraire = await context.Set<ItineraireSortie>()
            .FirstOrDefaultAsync(i => i.BonId == bonSortieId && i.BarriereId == barriereId, cancellationToken);

        if (itineraire != null)
        {
            itineraire.DatePassageEffective = DateTime.Now;
            itineraire.StatutPassage = "Passé";
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task CreateHistoriqueScanAsync(HistoriqueScan historique, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        await context.Set<HistoriqueScan>().AddAsync(historique, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
