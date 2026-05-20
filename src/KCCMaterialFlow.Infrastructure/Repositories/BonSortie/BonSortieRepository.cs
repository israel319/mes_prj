using KCCMaterialFlow.Domain.Entities;
using DomainEnums = KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les Bons de Sortie.
/// Utilise IDbContextFactory pour éviter les problèmes de concurrence dans Blazor Server.
/// </summary>
public class BonSortieRepository : IBonSortieRepository
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;

    public BonSortieRepository(IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    #region CRUD de base

    public async Task<BonSortie?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Set<BonSortie>()
            .Include(b => b.Materiels)
            .Include(b => b.Approbations)
            .Include(b => b.Itineraires)
            .Include(b => b.Historiques)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<BonSortie?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Set<BonSortie>()
            .Include(b => b.Materiels)
            .Include(b => b.Approbations)
            .FirstOrDefaultAsync(b => b.NumeroReference == numeroReference, cancellationToken);
    }

    /// <summary>
    /// BSM-030: Récupère un bon par son hash QR Code
    /// </summary>
    public async Task<BonSortie?> GetByQRCodeHashAsync(string qrCodeHash, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Set<BonSortie>()
            .Include(b => b.Materiels)
            .Include(b => b.Itineraires)
            .FirstOrDefaultAsync(b => b.QRCodeHash == qrCodeHash, cancellationToken);
    }

    public async Task<BonSortie> AddAsync(BonSortie bonSortie, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        await context.Set<BonSortie>().AddAsync(bonSortie, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return bonSortie;
    }

    public async Task UpdateAsync(BonSortie bonSortie, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();

        // Recharger l'entité depuis la base avec tracking + collections enfants
        var existing = await context.Set<BonSortie>()
            .Include(b => b.Approbations)
            .FirstOrDefaultAsync(b => b.Id == bonSortie.Id, cancellationToken);

        if (existing == null)
        {
            throw new InvalidOperationException($"Bon de sortie {bonSortie.Id} non trouvé");
        }

        // Copier les propriétés scalaires (StatutActuel, QR, etc.)
        context.Entry(existing).CurrentValues.SetValues(bonSortie);

        // Synchroniser les approbations modifiées (Decision, DateAction, NomApprobateur, etc.)
        foreach (var approbation in bonSortie.Approbations)
        {
            var existingAppro = existing.Approbations.FirstOrDefault(a => a.Id == approbation.Id);
            if (existingAppro != null)
            {
                context.Entry(existingAppro).CurrentValues.SetValues(approbation);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var bon = await context.Set<BonSortie>().FindAsync([id], cancellationToken);
        if (bon != null)
        {
            context.Set<BonSortie>().Remove(bon);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Requêtes spécialisées

    public async Task<(IReadOnlyList<BonSortie> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        string? statut = null,
        string? typeSortie = null,
        string? departement = null,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        int skip = 0,
        int take = 20,
        string? userLogin = null,
        CancellationToken cancellationToken = default,
        string? demandeur = null)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var query = context.Set<BonSortie>()
            .Include(b => b.Materiels)
            .Include(b => b.Historiques)
            .AsQueryable();

        // Filtrer les brouillons : seul le propriétaire peut les voir
        // Pour compatibilité avec les anciens bons (CreatedByLogin vide), on les inclut aussi
        if (!string.IsNullOrEmpty(userLogin))
        {
            query = query.Where(b => b.StatutActuel != "Draft" || 
                                     b.CreatedByLogin == userLogin || 
                                     string.IsNullOrEmpty(b.CreatedByLogin));
        }
        else
        {
            // Si pas de userLogin, exclure tous les brouillons
            query = query.Where(b => b.StatutActuel != "Draft");
        }

        // Filtres
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(b => 
                b.NumeroReference.Contains(searchTerm) ||
                b.NomDemandeur.Contains(searchTerm) ||
                b.Description!.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(statut))
        {
            query = query.Where(b => b.StatutActuel == statut);
        }

        if (!string.IsNullOrWhiteSpace(typeSortie))
        {
            query = typeSortie switch
            {
                "Externe" => query.OfType<BonSortieExterne>(),
                "Interne" => query.OfType<BonSortieInterne>(),
                "Pret" => query.OfType<Pret>(),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(departement))
        {
            query = query.Where(b => b.DepartementDemandeur == departement);
        }

        if (dateDebut.HasValue)
        {
            query = query.Where(b => b.DateCreation >= dateDebut.Value);
        }

        if (dateFin.HasValue)
        {
            query = query.Where(b => b.DateCreation <= dateFin.Value);
        }

        if (!string.IsNullOrWhiteSpace(demandeur))
        {
            query = query.Where(b =>
                b.NomDemandeur == demandeur ||
                b.CreatedByLogin == demandeur);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(b => b.DateCreation)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<BonSortie> Items, int TotalCount)> GetByUserAsync(
        string userLogin,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        // Retourne les bons créés par l'utilisateur
        // Pour compatibilité avec les anciens bons (CreatedByLogin vide), on inclut aussi ceux où CreatedByLogin est vide
        using var context = _dbContextFactory.CreateDbContext();
        var query = context.Set<BonSortie>()
            .Include(b => b.Materiels)
            .Include(b => b.Historiques)
            .Where(b => b.CreatedByLogin == userLogin || string.IsNullOrEmpty(b.CreatedByLogin))
            .OrderByDescending(b => b.DateCreation);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<BonSortie>> GetPendingApprovalAsync(
        string role,
        CancellationToken cancellationToken = default)
    {
        var canonicalRole = CanonicalizeRole(role);

        using var context = _dbContextFactory.CreateDbContext();

        var query = context.Set<BonSortie>()
            .Include(b => b.Materiels)
            .Include(b => b.Historiques)
            .Include(b => b.Approbations);

        // ADMIN voit tous les bons en attente de n'importe quelle étape.
        if (canonicalRole == "ADMIN")
        {
            return await query
                .Where(b => b.Approbations.Any(a => a.Decision == "En attente"))
                .OrderBy(b => b.DateCreation)
                .ToListAsync(cancellationToken);
        }

        // Pour tout autre rôle : seule la PREMIÈRE étape encore "En attente" détermine
        // qui peut agir sur ce bon. Si ce n'est pas mon RoleCode, je ne le vois pas.
        // La comparaison se fait directement sur ApprobationSortie.RoleApprobateur (canonique en BD).
        return await query
            .Where(b => b.Approbations
                .Where(a => a.Decision == "En attente")
                .OrderBy(a => a.OrdreEtape)
                .Select(a => a.RoleApprobateur)
                .FirstOrDefault() == canonicalRole)
            .OrderBy(b => b.DateCreation)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// v2 — BSM en attente où l'étape COURANTE (première En attente) est assignée à l'employé (ApprobateurId).
    /// </summary>
    public async Task<IReadOnlyList<BonSortie>> GetPendingApprovalsByEmployeeAsync(int employeeId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var query = context.Set<BonSortie>()
            .AsNoTracking()
            .Include(b => b.Materiels)
            .Include(b => b.Historiques)
            .Include(b => b.Approbations);

        if (isAdmin)
        {
            return await query
                .Where(b => b.Approbations.Any(a => a.Decision == "En attente"))
                .OrderBy(b => b.DateCreation)
                .ToListAsync(cancellationToken);
        }

        var bons = await query
            .Where(b => b.Approbations.Any(a => a.Decision == "En attente"))
            .OrderBy(b => b.DateCreation)
            .ToListAsync(cancellationToken);

        return bons;  // Will be filtered by service with access to _currentUserService
    }

    public async Task<IReadOnlyList<BonSortie>> GetApprovedBonsWithApprobationsAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Set<BonSortie>()
            .AsNoTracking()
            .Where(b => b.StatutActuel == "Approved")
            .Include(b => b.Materiels)
            .Include(b => b.Approbations)
            .OrderByDescending(b => b.DateCreation)
            .ToListAsync(cancellationToken);
    }

    private static string BuildPendingStatusFromRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return "PendingUnknown";
        }

        var token = new string(role.Where(char.IsLetterOrDigit).ToArray());
        return string.IsNullOrWhiteSpace(token) ? "PendingUnknown" : $"Pending{token}";
    }

    private static string CanonicalizeRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return string.Empty;
        }

        var token = new string(role
            .Trim()
            .ToUpperInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray());

        if (token.Contains("ADMIN")) return "ADMIN";
        if (token.Contains("SUPERVISEUR")) return "SUPERVISEUR";
        if (token == "GM" || token.Contains("GENERALMANAGER")) return "GM";
        if (token == "IT" || token.Contains("INFORMATIQUE") || token.Contains("DEPARTEMENTIT") || token.Contains("EQUIPEIT")) return "IT";
        if (token.Contains("ENVIRONNEMENT")) return "ENVIRONNEMENT";
        if (token == "OPJ" || token.Contains("OFFICIERDEPOLICEJUDICIAIRE")) return "OPJ";
        if (token.Contains("IDENTIFICATION")) return "IDENTIFICATION";

        return token;
    }

    public async Task<IReadOnlyList<Pret>> GetActiveLoansAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Set<Pret>()
            .Include(p => p.Materiels)
            .Where(p => !p.EstRetourne && p.StatutActuel == "Approved")
            .OrderBy(p => p.DateRetourPrevue)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Pret>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var today = DateTime.Today;
        return await context.Set<Pret>()
            .Include(p => p.Materiels)
            .Where(p => !p.EstRetourne && p.DateRetourPrevue < today)
            .OrderBy(p => p.DateRetourPrevue)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// BSM-027: Récupère les prêts expirant dans N jours (pour alertes J-7)
    /// </summary>
    public async Task<IReadOnlyList<Pret>> GetLoansExpiringInDaysAsync(int days, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var today = DateTime.Today;
        var targetDate = today.AddDays(days);
        
        return await context.Set<Pret>()
            .Include(p => p.Materiels)
            .Where(p => !p.EstRetourne 
                && p.StatutActuel == "Approved"
                && p.DateRetourPrevue >= today 
                && p.DateRetourPrevue <= targetDate)
            .OrderBy(p => p.DateRetourPrevue)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BonSortieExterne>> GetByBonEntreeAsync(
        int bonEntreeId,
        CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Set<BonSortieExterne>()
            .Include(b => b.Materiels)
            .Where(b => b.BonEntreeAssocieId == bonEntreeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<string, int>> GetCountByStatutAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Set<BonSortie>()
            .GroupBy(b => b.StatutActuel)
            .Select(g => new { Statut = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Statut, x => x.Count, cancellationToken);
    }

    public async Task<string> GenerateNextNumeroAsync(string prefix, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var year = DateTime.Now.Year;
        var pattern = $"{prefix}-{year}-";

        var lastNumero = await context.Set<BonSortie>()
            .Where(b => b.NumeroReference.StartsWith(pattern))
            .OrderByDescending(b => b.NumeroReference)
            .Select(b => b.NumeroReference)
            .FirstOrDefaultAsync(cancellationToken);

        int nextNumber = 1;
        if (!string.IsNullOrEmpty(lastNumero))
        {
            var lastPart = lastNumero.Replace(pattern, "");
            if (int.TryParse(lastPart, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{pattern}{nextNumber:D6}";
    }

    #endregion

    #region Historique

    public async Task AddHistoryAsync(BonSortieHistory history, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        await context.Set<BonSortieHistory>().AddAsync(history, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BonSortieHistory>> GetHistoryAsync(int bonSortieId, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.Set<BonSortieHistory>()
            .Where(h => h.BonId == bonSortieId)
            .OrderByDescending(h => h.ActionDate)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Approbations

    public async Task AddApprobationAsync(ApprobationSortie approbation, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        await context.Set<ApprobationSortie>().AddAsync(approbation, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteApprobationsAsync(int bonSortieId, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var existing = await context.Set<ApprobationSortie>()
            .Where(a => a.BonId == bonSortieId)
            .ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            context.Set<ApprobationSortie>().RemoveRange(existing);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Données de référence

    public async Task<string?> GetRaisonSortieCodeByIdAsync(int raisonId, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var raison = await context.Set<RaisonSortie>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == raisonId, cancellationToken);
        return raison?.Code;
    }

    public async Task RemoveMaterielsAsync(int bonSortieId, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var materiels = await context.Set<MaterielSortie>()
            .Where(m => m.BonId == bonSortieId)
            .ToListAsync(cancellationToken);

        if (materiels.Count > 0)
        {
            context.Set<MaterielSortie>().RemoveRange(materiels);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion
}
