using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des activités utilisateur avec mise en cache.
/// Permet d'assigner, retirer et vérifier les activités autorisées pour chaque utilisateur.
/// </summary>
public class ActiviteService : IActiviteService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ActiviteService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private const string CacheKeyAllActivites = "Activites_All";
    private const string CacheKeyUserPrefix = "Activites_User_";

    /// <summary>
    /// Mapping statique : chaque rôle -> ses activités par défaut.
    /// Quand un rôle est assigné à un utilisateur, ces activités sont automatiquement ajoutées.
    /// </summary>
    private static readonly Dictionary<string, string[]> RoleDefaultActivites = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ADMIN"] = [
            // Toutes les activités
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_APPROUVER", "BEM_REJETER", "BEM_RETOURNER", "BEM_SUPPRIMER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_APPROUVER", "BSM_REJETER", "BSM_RETOURNER", "BSM_SUPPRIMER", "BSM_IMPRIMER",
            "PRET_RETOUR", "PRET_EXTENSION",
            "SEC_SCANNER", "SEC_CONFIRMER_PASSAGE", "SEC_SIGNALER_ANOMALIE", "SEC_TRAITER_ANOMALIE", "SEC_REOUVRIR_ANOMALIE", "SEC_VOIR_HISTORIQUE",
            "SEC_GERER_BARRIERES", "SEC_GERER_ITINERAIRES", "SEC_GERER_AGENTS",
            "ADMIN_UTILISATEURS", "ADMIN_ROLES", "ADMIN_DEPARTEMENTS", "ADMIN_TYPES_MATERIELS", "ADMIN_STATUTS", "ADMIN_PARAMETRES", "ADMIN_AUDIT", "ADMIN_ACTIVITES",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "EXPORT_EXCEL", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD"
        ],
        ["APPROBATEUR"] = [
            // Création bons + Approuver/Rejeter + Consultation
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["SUPERVISEUR"] = [
            // Idem Approbateur
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["GM"] = [
            // Idem Approbateur
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["OPJ"] = [
            // Approbation BEM+BSM + Anomalies sécurité + Consultation + Voir tous bons
            "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "SEC_SIGNALER_ANOMALIE", "SEC_TRAITER_ANOMALIE", "SEC_REOUVRIR_ANOMALIE", "SEC_VOIR_HISTORIQUE",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["IT"] = [
            // Création bons + Approuver/Rejeter + Administration + Consultation + Voir tous bons
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "ADMIN_UTILISATEURS", "ADMIN_ROLES", "ADMIN_PARAMETRES", "ADMIN_AUDIT", "ADMIN_ACTIVITES",
            "VOIR_TOUS_BONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["AGENT_SECURITE"] = [
            // Scanner, Confirmer passage, Signaler anomalie, Historique scans
            "SEC_SCANNER", "SEC_CONFIRMER_PASSAGE", "SEC_SIGNALER_ANOMALIE", "SEC_VOIR_HISTORIQUE"
        ],
        ["BARRIERE"] = [
            // Scanner + Gestion barrières/agents
            "SEC_SCANNER", "SEC_CONFIRMER_PASSAGE", "SEC_SIGNALER_ANOMALIE", "SEC_VOIR_HISTORIQUE",
            "SEC_GERER_BARRIERES", "SEC_GERER_AGENTS"
        ],
        ["IDENTIFICATION"] = [
            // Imprimer + Scanner + Consultation + Approuver/Rejeter BEM+BSM + Voir tous bons + Historique
            "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "SEC_SCANNER", "SEC_CONFIRMER_PASSAGE", "SEC_VOIR_HISTORIQUE",
            "VOIR_TOUS_BONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD"
        ],
        ["INVESTIGATION"] = [
            // Anomalies (signaler/traiter/réouvrir) + Consultation + Export
            "SEC_SIGNALER_ANOMALIE", "SEC_TRAITER_ANOMALIE", "SEC_REOUVRIR_ANOMALIE", "SEC_VOIR_HISTORIQUE",
            "VOIR_TOUS_BONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["ENVIRONNEMENT"] = [
            // Approbation BEM+BSM + Consultation
            "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD"
        ],
        ["UTILISATEUR"] = [
            // Demandeur standard : créer, modifier, soumettre, supprimer, imprimer ses propres bons + Prêts + Historique
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_IMPRIMER",
            "PRET_RETOUR", "PRET_EXTENSION",
            "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD"
        ],
        ["DEMANDEUR"] = [
            // Idem UTILISATEUR
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_IMPRIMER",
            "PRET_RETOUR", "PRET_EXTENSION",
            "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD"
        ]
    };

    public ActiviteService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        IMemoryCache cache,
        ILogger<ActiviteService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
        _logger = logger;
    }

    // ===== ACTIVITÉS =====

    public async Task<IReadOnlyList<Activite>> GetAllActivitesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeyAllActivites, out IReadOnlyList<Activite>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var activites = await context.Set<Activite>()
            .AsNoTracking()
            .Where(a => a.EstActif)
            .OrderBy(a => a.Module)
            .ThenBy(a => a.OrdreAffichage)
            .ToListAsync(cancellationToken);

        _cache.Set(CacheKeyAllActivites, (IReadOnlyList<Activite>)activites, CacheDuration);
        _logger.LogDebug("Cache activités chargé : {Count} activités", activites.Count);

        return activites;
    }

    public async Task<Activite?> GetActiviteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var all = await GetAllActivitesAsync(cancellationToken);
        return all.FirstOrDefault(a => a.Id == id);
    }

    public async Task<Activite?> GetActiviteByCodeAsync(string codeActivite, CancellationToken cancellationToken = default)
    {
        var all = await GetAllActivitesAsync(cancellationToken);
        return all.FirstOrDefault(a => a.CodeActivite.Equals(codeActivite, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Dictionary<string, List<Activite>>> GetActivitesGroupedByModuleAsync(CancellationToken cancellationToken = default)
    {
        var all = await GetAllActivitesAsync(cancellationToken);
        return all
            .GroupBy(a => a.Module)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.OrderBy(a => a.OrdreAffichage).ToList());
    }

    // ===== ACTIVITÉS UTILISATEUR =====

    public async Task<IReadOnlyList<Activite>> GetActivitesForUserAsync(int idUtilisateur, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyUserPrefix}{idUtilisateur}";
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<Activite>? cached) && cached != null)
            return cached;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var activites = await context.Set<UtilisateurActivite>()
            .AsNoTracking()
            .Where(ua => ua.Id == idUtilisateur && ua.EstActif)
            .Include(ua => ua.Activite)
            .Where(ua => ua.Activite != null && ua.Activite.EstActif)
            .Select(ua => ua.Activite!)
            .OrderBy(a => a.Module)
            .ThenBy(a => a.OrdreAffichage)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, (IReadOnlyList<Activite>)activites, CacheDuration);
        _logger.LogDebug("Cache activités utilisateur {IdUtilisateur} chargé : {Count} activités", idUtilisateur, activites.Count);

        return activites;
    }

    public async Task<IReadOnlyList<int>> GetActiviteIdsForUserAsync(int idUtilisateur, CancellationToken cancellationToken = default)
    {
        var activites = await GetActivitesForUserAsync(idUtilisateur, cancellationToken);
        return activites.Select(a => a.Id).ToList();
    }

    public async Task UpdateUserActivitesAsync(int idUtilisateur, IEnumerable<int> activiteIds, string? attribueParLogin = null, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var newIds = activiteIds.ToHashSet();

        // Récupérer les assignations existantes
        var existing = await context.Set<UtilisateurActivite>()
            .Where(ua => ua.Id == idUtilisateur)
            .ToListAsync(cancellationToken);

        var existingIds = existing.Select(ua => ua.Id).ToHashSet();

        // Supprimer celles qui ne sont plus sélectionnées
        var toRemove = existing.Where(ua => !newIds.Contains(ua.Id)).ToList();
        if (toRemove.Count > 0)
        {
            context.Set<UtilisateurActivite>().RemoveRange(toRemove);
            _logger.LogInformation("Retrait de {Count} activités pour l'utilisateur {IdUtilisateur}", toRemove.Count, idUtilisateur);
        }

        // Ajouter les nouvelles
        var toAdd = newIds.Except(existingIds).ToList();
        if (toAdd.Count > 0)
        {
            foreach (var activiteId in toAdd)
            {
                context.Set<UtilisateurActivite>().Add(new UtilisateurActivite
                {
                    IdUtilisateur = idUtilisateur,
                    IdActivite = activiteId,
                    DateAttribution = DateTime.Now,
                    AttribueParLogin = attribueParLogin,
                    EstActif = true
                });
            }
            _logger.LogInformation("Ajout de {Count} activités pour l'utilisateur {IdUtilisateur}", toAdd.Count, idUtilisateur);
        }

        await context.SaveChangesAsync(cancellationToken);
        InvalidateUserCache(idUtilisateur);
    }

    public async Task<bool> AssignActiviteToUserAsync(int idUtilisateur, int idActivite, string? attribueParLogin = null, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Vérifier si l'assignation existe déjà
        var exists = await context.Set<UtilisateurActivite>()
            .AnyAsync(ua => ua.Id == idUtilisateur && ua.Id == idActivite, cancellationToken);

        if (exists)
        {
            _logger.LogDebug("Activité {IdActivite} déjà assignée à l'utilisateur {IdUtilisateur}", idActivite, idUtilisateur);
            return false;
        }

        context.Set<UtilisateurActivite>().Add(new UtilisateurActivite
        {
            IdUtilisateur = idUtilisateur,
            IdActivite = idActivite,
            DateAttribution = DateTime.Now,
            AttribueParLogin = attribueParLogin,
            EstActif = true
        });

        await context.SaveChangesAsync(cancellationToken);
        InvalidateUserCache(idUtilisateur);

        _logger.LogInformation("Activité {IdActivite} assignée à l'utilisateur {IdUtilisateur}", idActivite, idUtilisateur);
        return true;
    }

    public async Task<bool> RemoveActiviteFromUserAsync(int idUtilisateur, int idActivite, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entry = await context.Set<UtilisateurActivite>()
            .FirstOrDefaultAsync(ua => ua.Id == idUtilisateur && ua.Id == idActivite, cancellationToken);

        if (entry == null)
        {
            _logger.LogDebug("Activité {IdActivite} non trouvée pour l'utilisateur {IdUtilisateur}", idActivite, idUtilisateur);
            return false;
        }

        context.Set<UtilisateurActivite>().Remove(entry);
        await context.SaveChangesAsync(cancellationToken);
        InvalidateUserCache(idUtilisateur);

        _logger.LogInformation("Activité {IdActivite} retirée de l'utilisateur {IdUtilisateur}", idActivite, idUtilisateur);
        return true;
    }

    public async Task<bool> UserHasActiviteAsync(int idUtilisateur, string codeActivite, CancellationToken cancellationToken = default)
    {
        var activites = await GetActivitesForUserAsync(idUtilisateur, cancellationToken);
        return activites.Any(a => a.CodeActivite.Equals(codeActivite, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> UserHasAnyActiviteAsync(int idUtilisateur, IEnumerable<string> codeActivites, CancellationToken cancellationToken = default)
    {
        var activites = await GetActivitesForUserAsync(idUtilisateur, cancellationToken);
        var codes = new HashSet<string>(codeActivites, StringComparer.OrdinalIgnoreCase);
        return activites.Any(a => codes.Contains(a.CodeActivite));
    }

    public void InvalidateUserCache(int idUtilisateur)
    {
        var cacheKey = $"{CacheKeyUserPrefix}{idUtilisateur}";
        _cache.Remove(cacheKey);
        _logger.LogDebug("Cache activités invalidé pour l'utilisateur {IdUtilisateur}", idUtilisateur);
    }

    public IReadOnlyList<string> GetDefaultActiviteCodesForRole(string roleCode)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
            return [];

        return RoleDefaultActivites.TryGetValue(roleCode, out var codes)
            ? codes
            : [];
    }

    public async Task<int> SyncActivitesFromRolesAsync(int idUtilisateur, IEnumerable<string> roleCodes, string? attribueParLogin = null, CancellationToken cancellationToken = default)
    {
        // Collecter toutes les activités par défaut de tous les rôles
        var requiredCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var roleCode in roleCodes)
        {
            if (RoleDefaultActivites.TryGetValue(roleCode, out var defaults))
            {
                foreach (var code in defaults)
                    requiredCodes.Add(code);
            }
        }

        if (requiredCodes.Count == 0)
        {
            _logger.LogDebug("Aucune activité par défaut pour les rôles de l'utilisateur {IdUtilisateur}", idUtilisateur);
            return 0;
        }

        // Récupérer les IDs des activités requises
        var allActivites = await GetAllActivitesAsync(cancellationToken);
        var requiredIds = allActivites
            .Where(a => requiredCodes.Contains(a.CodeActivite))
            .Select(a => a.Id)
            .ToHashSet();

        // Récupérer les activités actuelles de l'utilisateur
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existingIds = await context.Set<UtilisateurActivite>()
            .Where(ua => ua.Id == idUtilisateur)
            .Select(ua => ua.Id)
            .ToHashSetAsync(cancellationToken);

        // Ajouter seulement celles qui manquent (ne retire rien)
        var toAdd = requiredIds.Except(existingIds).ToList();
        if (toAdd.Count == 0)
        {
            _logger.LogDebug("Toutes les activités par défaut déjà assignées à l'utilisateur {IdUtilisateur}", idUtilisateur);
            return 0;
        }

        foreach (var activiteId in toAdd)
        {
            context.Set<UtilisateurActivite>().Add(new UtilisateurActivite
            {
                IdUtilisateur = idUtilisateur,
                IdActivite = activiteId,
                DateAttribution = DateTime.Now,
                AttribueParLogin = attribueParLogin ?? "SYSTEM_ROLE_SYNC",
                EstActif = true
            });
        }

        await context.SaveChangesAsync(cancellationToken);
        InvalidateUserCache(idUtilisateur);

        _logger.LogInformation(
            "Synchronisation rôles->activités : {Count} activités ajoutées pour l'utilisateur {IdUtilisateur}",
            toAdd.Count, idUtilisateur);

        return toAdd.Count;
    }
}
