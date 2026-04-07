using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les Bons d'Entrée.
/// Utilise IDbContextFactory pour éviter les problèmes de concurrence dans Blazor Server.
/// </summary>
public class BonEntreeRepository : IBonEntreeRepository
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly ILogger<BonEntreeRepository> _logger;

    public BonEntreeRepository(IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory, ILogger<BonEntreeRepository> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Propriété pour compatibilité avec le code existant - crée un nouveau DbContext pour chaque accès
    /// </summary>
    private KCCMaterialFlowDbContext _dbContext => _dbContextFactory.CreateDbContext();

    #region CRUD Operations

    public async Task<BonEntree?> GetByIdAsync(
        int id,
        bool includeMateriels = true,
        bool includeApprobations = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<BonEntree>().AsQueryable();

        if (includeMateriels)
        {
            query = query.Include(b => b.Materiels);
        }

        if (includeApprobations)
        {
            query = query.Include(b => b.Approbations);
        }

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<BonEntree?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(numeroReference))
            return null;

        return await _dbContext.Set<BonEntree>()
            .AsNoTracking()
            .Include(b => b.Materiels)
            .FirstOrDefaultAsync(b => b.NumeroReference == numeroReference, cancellationToken);
    }

    public async Task<BonEntree> CreateAsync(BonEntree bonEntree, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        bonEntree.DateCreation = DateTime.Now;

        if (string.IsNullOrEmpty(bonEntree.NumeroReference))
        {
            bonEntree.NumeroReference = await GenerateNextNumeroReferenceAsync(cancellationToken);
        }

        dbContext.Set<BonEntree>().Add(bonEntree);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Bon d'entrée {Numero} créé avec ID {Id}", bonEntree.NumeroReference, bonEntree.Id);

        return bonEntree;
    }

    public async Task<BonEntree> UpdateAsync(BonEntree bonEntree, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        // Recharger l'entité depuis la base avec tracking
        var existing = await dbContext.Set<BonEntree>()
            .FirstOrDefaultAsync(b => b.Id == bonEntree.Id, cancellationToken);
        
        if (existing == null)
        {
            throw new InvalidOperationException($"Bon d'entrée {bonEntree.Id} non trouvé");
        }
        
        // Copier les propriétés modifiées
        existing.StatutActuel = bonEntree.StatutActuel;
        existing.Provenance = bonEntree.Provenance;
        existing.Destination = bonEntree.Destination;
        existing.Description = bonEntree.Description;
        existing.DateExpiration = bonEntree.DateExpiration;
        existing.ContratId = bonEntree.ContratId;
        existing.NumeroContrat = bonEntree.NumeroContrat;
        existing.NomCompagnie = bonEntree.NomCompagnie;
        existing.EmailContractant = bonEntree.EmailContractant;
        existing.SiteManager = bonEntree.SiteManager;
        existing.HostDepartment = bonEntree.HostDepartment;
        existing.ReasonOnSite = bonEntree.ReasonOnSite;
        existing.NomEscorteur = bonEntree.NomEscorteur;
        existing.FonctionEscorteur = bonEntree.FonctionEscorteur;
        existing.QRCodeBase64 = bonEntree.QRCodeBase64;
        existing.QRCodeHash = bonEntree.QRCodeHash;
        existing.QRCodeData = bonEntree.QRCodeData;
        existing.DateGenerationQR = bonEntree.DateGenerationQR;
        existing.Quantite = bonEntree.Quantite;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Bon d'entrée {Numero} mis à jour (statut: {Statut})", existing.NumeroReference, existing.StatutActuel);

        return existing;
    }

    public async Task DeleteAsync(int id, string motif, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        var bonEntree = await dbContext.Set<BonEntree>()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (bonEntree != null)
        {
            // Marquer comme annulé via le statut
            bonEntree.StatutActuel = "Cancelled";

            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Bon d'entrée {Numero} annulé: {Motif}", bonEntree.NumeroReference, motif);
        }
    }

    #endregion

    #region Search & Filter

    public async Task<BonEntreeSearchResult> SearchAsync(BonEntreeFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<BonEntree>().AsQueryable();

        // Exclure les annulés
        query = query.Where(b => b.StatutActuel != "Cancelled");

        // Appliquer les filtres
        query = ApplyFilters(query, filter);

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, filter.SortBy, filter.SortDescending);

        var items = await query
            .AsNoTracking()
            .Skip(filter.Skip)
            .Take(filter.Take)
            .Include(b => b.Materiels)
            .Include(b => b.Historiques)
            .ToListAsync(cancellationToken);

        return new BonEntreeSearchResult
        {
            Items = items,
            TotalCount = totalCount,
            Skip = filter.Skip,
            Take = filter.Take
        };
    }

    public async Task<BonEntreeSearchResult> GetByCreateurAsync(string login, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
    {
        var filter = new BonEntreeFilter
        {
            Demandeur = login,
            Skip = skip,
            Take = take
        };

        return await SearchAsync(filter, cancellationToken);
    }

    public async Task<IReadOnlyList<BonEntree>> GetPendingApprovalsAsync(IEnumerable<string> userRoles, CancellationToken cancellationToken = default)
    {
        var roles = userRoles.ToList();
        var isAdmin = roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));
        var rolesLower = roles.Select(r => r.ToLowerInvariant()).ToHashSet();

        _logger.LogDebug("GetPendingApprovalsAsync - Rôles: {Roles}, IsAdmin: {IsAdmin}",
            string.Join(", ", roles), isAdmin);

        if (!isAdmin && rolesLower.Count == 0)
            return new List<BonEntree>();

        // Récupérer tous les bons en attente d'approbation (pas Draft, Cancelled, Approved, Rejected)
        var bons = await _dbContext.Set<BonEntree>()
            .AsNoTracking()
            .Where(b => b.StatutActuel != "Cancelled" && b.StatutActuel != "Draft"
                      && b.StatutActuel != "Approved" && b.StatutActuel != "Rejected")
            .Include(b => b.Materiels)
            .Include(b => b.Approbations)
            .Include(b => b.Historiques)
            .OrderByDescending(b => b.DateCreation)
            .ToListAsync(cancellationToken);

        if (isAdmin)
        {
            _logger.LogInformation("GetPendingApprovalsAsync - Admin: {Count} bons en attente", bons.Count);
            return bons;
        }

        // Filtrer: ne garder que les bons dont l'étape courante (première "En attente") correspond au rôle de l'utilisateur
        var filtered = bons.Where(b =>
        {
            var currentStep = b.Approbations
                .OrderBy(a => a.OrdreEtape)
                .FirstOrDefault(a => a.Decision == "En attente");

            if (currentStep?.NomEtape == null) return false;

            // Convertir le NomEtape (display name) en code rôle pour matching correct
            // Ex: "General Manager" → "gm", "Superviseur" → "superviseur", "Département IT" → "it"
            var stepRole = MapNomEtapeToRoleCode(currentStep.NomEtape).ToLowerInvariant();
            var stepNameLower = currentStep.NomEtape.ToLowerInvariant();

            return rolesLower.Any(r => stepRole == r || stepNameLower == r || stepNameLower.Contains(r));
        }).ToList();

        _logger.LogInformation("GetPendingApprovalsAsync - {Count} bons filtrés pour les rôles {Roles}", filtered.Count, string.Join(", ", roles));

        return filtered;
    }

    /// <summary>
    /// Mappe un NomEtape (display name) vers le code rôle correspondant.
    /// Nécessaire car les BEM stockent le nom d'affichage dans Approbation.NomEtape.
    /// </summary>
    private static string MapNomEtapeToRoleCode(string nomEtape)
    {
        return nomEtape.ToLowerInvariant() switch
        {
            "superviseur" => "Superviseur",
            "general manager" => "GM",
            "opj" => "OPJ",
            "identification" => "Identification",
            "département it" or "departement it" => "IT",
            "département environnement" or "departement environnement" => "Environnement",
            "investigation" => "Investigation",
            "barrière" or "barriere" => "Barriere",
            _ => nomEtape
        };
    }

    #endregion

    #region Statistics

    public async Task<Dictionary<string, int>> GetCountByStatutAsync(CancellationToken cancellationToken = default)
    {
        var counts = await _dbContext.Set<BonEntree>()
            .Where(b => b.StatutActuel != "Cancelled")
            .GroupBy(b => b.StatutActuel)
            .Select(g => new { Statut = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return counts.ToDictionary(x => x.Statut, x => x.Count);
    }

    public async Task<int> GetTodayCountAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        return await _dbContext.Set<BonEntree>()
            .CountAsync(b => b.DateCreation >= today && b.DateCreation < tomorrow, cancellationToken);
    }

    public async Task<string> GenerateNextNumeroReferenceAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.Now.Year;
        var prefix = $"BEM-{year}-";

        var lastNumero = await _dbContext.Set<BonEntree>()
            .Where(b => b.NumeroReference.StartsWith(prefix))
            .OrderByDescending(b => b.NumeroReference)
            .Select(b => b.NumeroReference)
            .FirstOrDefaultAsync(cancellationToken);

        int nextSequence = 1;
        if (!string.IsNullOrEmpty(lastNumero))
        {
            var sequencePart = lastNumero.Replace(prefix, "");
            if (int.TryParse(sequencePart, out int lastSequence))
            {
                nextSequence = lastSequence + 1;
            }
        }

        return $"{prefix}{nextSequence:D6}";
    }

    #endregion

    #region Related Entities

    public async Task<Materiel> AddMaterielAsync(int bonId, Materiel materiel, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        materiel.BonId = bonId;

        dbContext.Set<Materiel>().Add(materiel);
        await dbContext.SaveChangesAsync(cancellationToken);

        await UpdateBonQuantiteAsync(bonId, cancellationToken);

        return materiel;
    }

    public async Task<Materiel> UpdateMaterielAsync(Materiel materiel, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        dbContext.Set<Materiel>().Update(materiel);
        await dbContext.SaveChangesAsync(cancellationToken);

        await UpdateBonQuantiteAsync(materiel.BonId, cancellationToken);

        return materiel;
    }

    public async Task DeleteMaterielAsync(int materielId, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        var materiel = await dbContext.Set<Materiel>()
            .FirstOrDefaultAsync(m => m.Id == materielId, cancellationToken);

        if (materiel != null)
        {
            var bonId = materiel.BonId;
            dbContext.Set<Materiel>().Remove(materiel);
            await dbContext.SaveChangesAsync(cancellationToken);

            await UpdateBonQuantiteAsync(bonId, cancellationToken);
        }
    }

    public async Task<Approbation> UpsertApprobationAsync(Approbation approbation, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        var existing = await dbContext.Set<Approbation>()
            .FirstOrDefaultAsync(a => a.BonId == approbation.BonId && a.OrdreEtape == approbation.OrdreEtape, cancellationToken);

        if (existing != null)
        {
            existing.Decision = approbation.Decision;
            existing.DateAction = approbation.DateAction;
            existing.ReservesEventuelles = approbation.ReservesEventuelles;
            existing.NomApprobateur = approbation.NomApprobateur;
            existing.RoleApprobateur = approbation.RoleApprobateur;

            await dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
        else
        {
            dbContext.Set<Approbation>().Add(approbation);
            await dbContext.SaveChangesAsync(cancellationToken);
            return approbation;
        }
    }

    public async Task<IReadOnlyList<Approbation>> GetApprobationsAsync(int bonId, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return await dbContext.Set<Approbation>()
            .AsNoTracking()
            .Where(a => a.BonId == bonId)
            .OrderBy(a => a.OrdreEtape)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteApprobationsAsync(int bonId, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var existing = await dbContext.Set<Approbation>()
            .Where(a => a.BonId == bonId)
            .ToListAsync(cancellationToken);

        if (existing.Count > 0)
        {
            dbContext.Set<Approbation>().RemoveRange(existing);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Liaison Entrée-Sortie (BSM-031)

    /// <summary>
    /// BSM-031: Associe un bon de sortie à un bon d'entrée (relation 1:N - un BEM peut avoir plusieurs BSM)
    /// Le flag EstVerrouillePourSortie sera mis à true uniquement quand toute la quantité sera épuisée
    /// </summary>
    public async Task LockForSortieAsync(int bonEntreeId, int bonSortieId, string bonSortieNumero, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        var bonEntree = await dbContext.Set<BonEntree>()
            .Include(b => b.Materiels)
            .FirstOrDefaultAsync(b => b.Id == bonEntreeId, cancellationToken);

        if (bonEntree == null)
            throw new InvalidOperationException($"Bon d'entrée {bonEntreeId} non trouvé");

        // Mettre à jour les informations du dernier BSM associé
        bonEntree.DateVerrouillage = DateTime.Now;
        bonEntree.BonSortieAssocieId = bonSortieId;
        bonEntree.BonSortieAssocieNumero = bonSortieNumero;
        
        // Vérifier si toute la quantité est épuisée
        var quantiteDisponible = bonEntree.Materiels?.Sum(m => m.QuantiteDisponible) ?? 0;
        if (quantiteDisponible <= 0)
        {
            bonEntree.EstVerrouillePourSortie = true;
            _logger.LogInformation("BEM {BemNumero} complètement utilisé, verrouillé", bonEntree.NumeroReference);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("BEM {BemNumero} associé à BSM {BsmNumero}",
            bonEntree.NumeroReference, bonSortieNumero);
    }

    /// <summary>
    /// BSM-031: Déverrouille un bon d'entrée
    /// </summary>
    public async Task UnlockFromSortieAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        var bonEntree = await dbContext.Set<BonEntree>()
            .FirstOrDefaultAsync(b => b.Id == bonEntreeId, cancellationToken);

        if (bonEntree == null)
            return;

        var oldBsmNumero = bonEntree.BonSortieAssocieNumero;

        bonEntree.EstVerrouillePourSortie = false;
        bonEntree.DateVerrouillage = null;
        bonEntree.BonSortieAssocieId = null;
        bonEntree.BonSortieAssocieNumero = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("BEM {BemNumero} déverrouillé (était lié à {BsmNumero})",
            bonEntree.NumeroReference, oldBsmNumero);
    }

    /// <summary>
    /// BSM-031: Vérifie si un bon d'entrée est disponible pour une sortie
    /// (disponible si approuvé, non expiré et avec quantité disponible > 0)
    /// </summary>
    public async Task<bool> IsAvailableForSortieAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        var bonEntree = await _dbContext.Set<BonEntree>()
            .AsNoTracking()
            .Include(b => b.Materiels)
            .FirstOrDefaultAsync(b => b.Id == bonEntreeId, cancellationToken);

        if (bonEntree == null)
            return false;

        // Disponible si: approuvé ET non expiré ET quantité disponible > 0
        var quantiteDisponible = bonEntree.Materiels?.Sum(m => m.QuantiteDisponible) ?? 0;
        return bonEntree.StatutActuel == "Approved"
            && bonEntree.DateExpiration >= DateTime.Today
            && quantiteDisponible > 0;
    }

    /// <summary>
    /// INT-006: Archive un bon d'entrée après complétion du bon de sortie associé
    /// </summary>
    public async Task ArchiveAsync(int bonEntreeId, string motif, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        var bonEntree = await dbContext.Set<BonEntree>()
            .FirstOrDefaultAsync(b => b.Id == bonEntreeId, cancellationToken);

        if (bonEntree == null)
        {
            _logger.LogWarning("INT-006: BEM {Id} non trouvé pour archivage", bonEntreeId);
            return;
        }

        var oldStatut = bonEntree.StatutActuel;
        bonEntree.StatutActuel = "Archived";

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("INT-006: BEM {Numero} archivé (ancien statut: {OldStatut}). Motif: {Motif}",
            bonEntree.NumeroReference, oldStatut, motif);
    }

    #endregion

    #region Stock & Disponibilité (BSM-031)

    /// <summary>
    /// Décrémente la quantité disponible des matériels lors de l'approbation finale d'un BSM.
    /// Utilise un seul DbContext pour toutes les opérations (tracking correct).
    /// </summary>
    public async Task<StockUpdateResult> DecrementMaterielStockAsync(IEnumerable<MaterielStockDecrement> materielsASortir, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        var materielsList = materielsASortir.ToList();
        var warnings = new List<string>();

        foreach (var item in materielsList)
        {
            _logger.LogInformation("Traitement du matériel MaterielEntreeId={MaterielId}, QuantiteASortir={Qte}", 
                item.MaterielEntreeId, item.QuantiteASortir);

            var materiel = await dbContext.Set<Materiel>()
                .FirstOrDefaultAsync(m => m.Id == item.MaterielEntreeId, cancellationToken);

            if (materiel == null)
            {
                _logger.LogWarning("Matériel {Id} non trouvé dans bem.Materiels", item.MaterielEntreeId);
                warnings.Add($"Matériel {item.MaterielEntreeId} non trouvé");
                continue;
            }

            var oldQty = materiel.QuantiteDisponible;

            if (materiel.QuantiteDisponible < item.QuantiteASortir)
            {
                return StockUpdateResult.Fail(
                    $"Stock insuffisant pour {materiel.Designation}: disponible={materiel.QuantiteDisponible}, demandé={item.QuantiteASortir}");
            }

            materiel.QuantiteDisponible -= item.QuantiteASortir;

            _logger.LogInformation("Stock décrémenté pour matériel {Id} ({Designation}): {OldQty} -> {NewQty}",
                materiel.Id, materiel.Designation, oldQty, materiel.QuantiteDisponible);
        }

        var changedEntries = await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("SaveChangesAsync terminé: {ChangedCount} entrées modifiées", changedEntries);

        var result = StockUpdateResult.Ok();
        result.Warnings = warnings;
        return result;
    }

    /// <summary>
    /// Ré-incrémente la quantité disponible des matériels lors du retour d'un prêt.
    /// </summary>
    public async Task<StockUpdateResult> IncrementMaterielStockAsync(IEnumerable<MaterielStockDecrement> materielsARestituer, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var materielsList = materielsARestituer.ToList();
        var warnings = new List<string>();

        foreach (var item in materielsList)
        {
            var materiel = await dbContext.Set<Materiel>()
                .FirstOrDefaultAsync(m => m.Id == item.MaterielEntreeId, cancellationToken);

            if (materiel == null)
            {
                _logger.LogWarning("Matériel {Id} non trouvé pour restitution", item.MaterielEntreeId);
                warnings.Add($"Matériel {item.MaterielEntreeId} non trouvé");
                continue;
            }

            var oldQty = materiel.QuantiteDisponible;
            materiel.QuantiteDisponible += item.QuantiteASortir;

            // Ne pas dépasser la quantité initiale
            if (materiel.QuantiteDisponible > materiel.Quantite)
                materiel.QuantiteDisponible = materiel.Quantite;

            _logger.LogInformation("Stock restauré pour matériel {Id} ({Designation}): {OldQty} -> {NewQty}",
                materiel.Id, materiel.Designation, oldQty, materiel.QuantiteDisponible);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var result = StockUpdateResult.Ok();
        result.Warnings = warnings;
        return result;
    }

    /// <summary>
    /// Récupère tous les bons d'entrée disponibles pour sortie (approuvés, non expirés, avec quantité > 0).
    /// </summary>
    public async Task<IReadOnlyList<BonEntreeForDropdown>> GetAllAvailableForSortieAsync(string? hostDepartmentFilter = null, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        var today = DateTime.Today;
        
        var query = dbContext.Set<BonEntree>()
            .AsNoTracking()
            .Include(b => b.Materiels)
            .Where(b => b.StatutActuel == "Approved"
                     && b.DateExpiration >= today
                     && !b.EstVerrouillePourSortie);

        if (!string.IsNullOrWhiteSpace(hostDepartmentFilter))
        {
            var normalizedFilter = hostDepartmentFilter.Trim().ToUpperInvariant();
            query = query.Where(b => b.HostDepartment != null && b.HostDepartment.ToUpper().Contains(normalizedFilter));
        }

        var bonsDisponibles = await query
            .OrderByDescending(b => b.DateCreation)
            .ToListAsync(cancellationToken);

        return bonsDisponibles
            .Where(b => b.Materiels != null && b.Materiels.Sum(m => m.QuantiteDisponible) > 0)
            .Select(b => new BonEntreeForDropdown
            {
                IdBon = b.Id,
                NumeroReference = b.NumeroReference,
                NomCompagnie = b.NomCompagnie ?? string.Empty,
                HostDepartment = b.HostDepartment ?? string.Empty,
                ReasonOnSite = b.ReasonOnSite ?? string.Empty,
                Destination = b.Destination ?? string.Empty,
                DateExpiration = b.DateExpiration,
                NombreMateriels = b.Materiels?.Count ?? 0,
                QuantiteTotaleDisponible = b.Materiels?.Sum(m => m.QuantiteDisponible) ?? 0
            })
            .ToList();
    }

    #endregion

    #region Private Helpers

    private static IQueryable<BonEntree> ApplyFilters(IQueryable<BonEntree> query, BonEntreeFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToUpper();
            query = query.Where(b =>
                b.NumeroReference.ToUpper().Contains(term) ||
                b.NomCompagnie.ToUpper().Contains(term) ||
                (b.Description != null && b.Description.ToUpper().Contains(term)) ||
                (b.NumeroContrat != null && b.NumeroContrat.ToUpper().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Statut))
        {
            query = query.Where(b => b.StatutActuel == filter.Statut);
        }

        if (filter.Statuts != null && filter.Statuts.Count > 0)
        {
            query = query.Where(b => filter.Statuts.Contains(b.StatutActuel));
        }

        if (filter.DateDebut.HasValue)
        {
            query = query.Where(b => b.DateCreation >= filter.DateDebut.Value);
        }

        if (filter.DateFin.HasValue)
        {
            var dateFin = filter.DateFin.Value.AddDays(1);
            query = query.Where(b => b.DateCreation < dateFin);
        }

        if (!string.IsNullOrWhiteSpace(filter.Compagnie))
        {
            var compagnie = filter.Compagnie.ToUpper();
            query = query.Where(b => b.NomCompagnie.ToUpper().Contains(compagnie));
        }

        if (!string.IsNullOrWhiteSpace(filter.Departement))
        {
            var dept = filter.Departement.ToUpper();
            query = query.Where(b => b.HostDepartment.ToUpper() == dept);
        }

        if (!string.IsNullOrWhiteSpace(filter.Demandeur))
        {
            var demandeur = filter.Demandeur.ToUpper();
            query = query.Where(b => b.NomDemandeur.ToUpper() == demandeur);
        }

        return query;
    }

    private static IQueryable<BonEntree> ApplySorting(IQueryable<BonEntree> query, string sortBy, bool descending)
    {
        return sortBy.ToLower() switch
        {
            "numeroreference" => descending 
                ? query.OrderByDescending(b => b.NumeroReference) 
                : query.OrderBy(b => b.NumeroReference),
            "nomcompagnie" => descending 
                ? query.OrderByDescending(b => b.NomCompagnie) 
                : query.OrderBy(b => b.NomCompagnie),
            "statutactuel" => descending 
                ? query.OrderByDescending(b => b.StatutActuel) 
                : query.OrderBy(b => b.StatutActuel),
            "dateexpiration" => descending 
                ? query.OrderByDescending(b => b.DateExpiration) 
                : query.OrderBy(b => b.DateExpiration),
            _ => descending 
                ? query.OrderByDescending(b => b.DateCreation) 
                : query.OrderBy(b => b.DateCreation)
        };
    }

    private async Task UpdateBonQuantiteAsync(int bonId, CancellationToken cancellationToken)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        
        var totalQuantite = await dbContext.Set<Materiel>()
            .Where(m => m.BonId == bonId)
            .SumAsync(m => m.Quantite, cancellationToken);

        var bon = await dbContext.Set<BonEntree>()
            .FirstOrDefaultAsync(b => b.Id == bonId, cancellationToken);

        if (bon != null)
        {
            bon.Quantite = (int)totalQuantite;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Historique

    public async Task AddHistoryAsync(BonEntreeHistory history, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.Set<BonEntreeHistory>().AddAsync(history, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BonEntreeHistory>> GetHistoryAsync(int bonId, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Set<BonEntreeHistory>()
            .Where(h => h.BonId == bonId)
            .OrderByDescending(h => h.ActionDate)
            .ToListAsync(cancellationToken);
    }

    #endregion
}
