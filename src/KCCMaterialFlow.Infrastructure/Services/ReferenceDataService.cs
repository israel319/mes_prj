using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Service pour accéder aux données de référence
/// Utilise IDbContextFactory pour éviter les problèmes de concurrence dans Blazor Server.
/// </summary>
public class ReferenceDataService : IReferenceDataService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly ILogger<ReferenceDataService> _logger;

    /// <summary>
    /// Propriété pour compatibilité avec le code existant - crée un nouveau DbContext pour chaque accès
    /// </summary>
    private KCCMaterialFlowDbContext _context => _dbContextFactory.CreateDbContext();

    public ReferenceDataService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        ILogger<ReferenceDataService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    #region Compagnies

    public async Task<IEnumerable<Compagnie>> GetCompagniesAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Compagnies.AsQueryable();
        
        if (activeOnly)
            query = query.Where(c => c.EstActif);

        return await query
            .OrderBy(c => c.Nom)
            .ToListAsync(cancellationToken);
    }

    public async Task<Compagnie?> GetCompagnieByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Compagnies
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Compagnie?> GetCompagnieByNomAsync(string nom, CancellationToken cancellationToken = default)
    {
        return await _context.Compagnies
            .FirstOrDefaultAsync(c => c.Nom == nom, cancellationToken);
    }

    #endregion

    #region Contrats

    public async Task<IEnumerable<Contrat>> GetContratsAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Contrats.Include(c => c.Compagnie).AsQueryable();

        if (activeOnly)
            query = query.Where(c => c.EstActif);

        return await query
            .OrderBy(c => c.PoNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Contrat>> GetContratsByCompagnieAsync(int compagnieId, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Contrats
            .Where(c => c.CompagnieId == compagnieId);

        if (activeOnly)
            query = query.Where(c => c.EstActif);

        return await query
            .OrderBy(c => c.PoNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<Contrat?> GetContratByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Contrats
            .Include(c => c.Compagnie)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    #endregion

    #region Sites

    public async Task<IEnumerable<Site>> GetSitesAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Sites.AsQueryable();
        
        if (activeOnly)
            query = query.Where(s => s.EstActif);

        return await query
            .OrderBy(s => s.OrdreAffichage)
            .ThenBy(s => s.Nom)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Site>> GetSitesInternesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sites
            .Where(s => s.EstActif && s.EstInterne)
            .OrderBy(s => s.OrdreAffichage)
            .ThenBy(s => s.Nom)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Site>> GetSitesExternesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sites
            .Where(s => s.EstActif && !s.EstInterne)
            .OrderBy(s => s.OrdreAffichage)
            .ThenBy(s => s.Nom)
            .ToListAsync(cancellationToken);
    }

    public async Task<Site?> GetSiteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sites
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    #endregion

    #region Employés

    public async Task<IEnumerable<Employee>> GetEmployeesAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Employees
            .Include(e => e.Compagnie)
            .AsQueryable();
        
        return await query
            .OrderBy(e => e.NomComplet)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetEscorteursAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(e => e.PeutEtreEscorteur)
            .OrderBy(e => e.NomComplet)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByCompagnieAsync(int compagnieId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(e => e.CompagnieId == compagnieId)
            .OrderBy(e => e.NomComplet)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Compagnie)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        using var ctx = _dbContextFactory.CreateDbContext();
        var empId = await ctx.AppUsers.AsNoTracking()
            .Where(u => u.Login == login)
            .Select(u => u.EmployeeId)
            .FirstOrDefaultAsync(cancellationToken);
        if (empId == null) return null;
        return await ctx.Employees
            .Include(e => e.Compagnie)
            .FirstOrDefaultAsync(e => e.Id == empId.Value, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByMatriculeAsync(string matricule, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Compagnie)
            .FirstOrDefaultAsync(e => e.Matricule == matricule, cancellationToken);
    }

    #endregion

    #region Recherche

    public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm, int maxResults = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<Employee>();

        var term = searchTerm.ToLower();

        return await _context.Employees
            .Include(e => e.Compagnie)
            .Where(e =>
                (e.NomComplet.ToLower().Contains(term) ||
                 (e.Matricule != null && e.Matricule.ToLower().Contains(term)) ||
                 (e.Email != null && e.Email.ToLower().Contains(term))))
            .OrderBy(e => e.NomComplet)
            .Take(maxResults)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Compagnie>> SearchCompagniesAsync(string searchTerm, int maxResults = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<Compagnie>();

        var term = searchTerm.ToLower();

        return await _context.Compagnies
            .Where(c => c.EstActif && 
                (c.Nom.ToLower().Contains(term) ||
                 (c.Code != null && c.Code.ToLower().Contains(term))))
            .OrderBy(c => c.Nom)
            .Take(maxResults)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Catégories de Sortie

    public async Task<IEnumerable<CategorieSortie>> GetCategoriesSortieAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.CategoriesSortie.AsQueryable();
        
        if (activeOnly)
            query = query.Where(c => c.EstActif);

        return await query
            .OrderBy(c => c.OrdreAffichage)
            .ThenBy(c => c.Nom)
            .ToListAsync(cancellationToken);
    }

    public async Task<CategorieSortie?> GetCategorieSortieByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.CategoriesSortie
            .Include(c => c.Raisons.Where(r => r.EstActif))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<CategorieSortie?> GetCategorieSortieByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.CategoriesSortie
            .Include(c => c.Raisons.Where(r => r.EstActif))
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
    }

    #endregion

    #region Raisons de Sortie

    public async Task<IEnumerable<RaisonSortie>> GetRaisonsSortieAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.RaisonsSortie
            .Include(r => r.Categorie)
            .AsQueryable();
        
        if (activeOnly)
            query = query.Where(r => r.EstActif);

        return await query
            .OrderBy(r => r.CategorieId)
            .ThenBy(r => r.OrdreAffichage)
            .ThenBy(r => r.Nom)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RaisonSortie>> GetRaisonsSortieByCategorieAsync(int categorieId, CancellationToken cancellationToken = default)
    {
        return await _context.RaisonsSortie
            .Where(r => r.EstActif && r.CategorieId == categorieId)
            .OrderBy(r => r.OrdreAffichage)
            .ThenBy(r => r.Nom)
            .ToListAsync(cancellationToken);
    }

    public async Task<RaisonSortie?> GetRaisonSortieByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RaisonsSortie
            .Include(r => r.Categorie)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    #endregion

    #region Checkpoints

    public async Task<IEnumerable<Checkpoint>> GetCheckpointsAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Checkpoints
            .Include(c => c.Site)
            .AsQueryable();
        
        if (activeOnly)
            query = query.Where(c => c.EstActif);

        return await query
            .OrderBy(c => c.OrdreDefaut)
            .ThenBy(c => c.Nom)
            .ToListAsync(cancellationToken);
    }

    public async Task<Checkpoint?> GetCheckpointByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Checkpoints
            .Include(c => c.Site)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Checkpoint?> GetCheckpointBySiteIdAsync(int siteId, CancellationToken cancellationToken = default)
    {
        return await _context.Checkpoints
            .Include(c => c.Site)
            .FirstOrDefaultAsync(c => c.SiteId == siteId && c.EstActif, cancellationToken);
    }

    #endregion
}
