using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service de gestion des activités.
/// Les activités sont DÉRIVÉES automatiquement des rôles via le mapping statique RoleDefaultActivites.
/// Plus de gestion BD de la table UtilisateurActivite.
/// </summary>
public class ActiviteService : IActiviteService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ActiviteService> _logger;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private const string CacheKeyAllActivites = "Activites_All";

    /// <summary>
    /// Mapping statique : chaque rôle -> ses activités par défaut.
    /// C'est la source unique qui détermine ce que chaque rôle peut faire.
    /// </summary>
    private static readonly Dictionary<string, string[]> RoleDefaultActivites = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ADMIN"] = [
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_APPROUVER", "BEM_REJETER", "BEM_RETOURNER", "BEM_SUPPRIMER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_APPROUVER", "BSM_REJETER", "BSM_RETOURNER", "BSM_SUPPRIMER", "BSM_IMPRIMER",
            "PRET_RETOUR", "PRET_EXTENSION",
            "SEC_SCANNER", "SEC_CONFIRMER_PASSAGE", "SEC_SIGNALER_ANOMALIE", "SEC_TRAITER_ANOMALIE", "SEC_REOUVRIR_ANOMALIE", "SEC_VOIR_HISTORIQUE",
            "SEC_GERER_BARRIERES", "SEC_GERER_ITINERAIRES", "SEC_GERER_AGENTS",
            "ADMIN_UTILISATEURS", "ADMIN_ROLES", "ADMIN_DEPARTEMENTS", "ADMIN_TYPES_MATERIELS", "ADMIN_STATUTS", "ADMIN_PARAMETRES", "ADMIN_AUDIT", "ADMIN_ACTIVITES",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "EXPORT_EXCEL", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD"
        ],
        ["APPROBATEUR"] = [
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["SUPERVISEUR"] = [
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["GM"] = [
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["OPJ"] = [
            "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "SEC_SIGNALER_ANOMALIE", "SEC_TRAITER_ANOMALIE", "SEC_REOUVRIR_ANOMALIE", "SEC_VOIR_HISTORIQUE",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["IT"] = [
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "ADMIN_UTILISATEURS", "ADMIN_ROLES", "ADMIN_PARAMETRES", "ADMIN_AUDIT", "ADMIN_ACTIVITES",
            "VOIR_TOUS_BONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["AGENT_SECURITE"] = [
            "SEC_SCANNER", "SEC_CONFIRMER_PASSAGE", "SEC_SIGNALER_ANOMALIE", "SEC_VOIR_HISTORIQUE"
        ],
        ["BARRIERE"] = [
            "SEC_SCANNER", "SEC_CONFIRMER_PASSAGE", "SEC_SIGNALER_ANOMALIE", "SEC_VOIR_HISTORIQUE",
            "SEC_GERER_BARRIERES", "SEC_GERER_AGENTS"
        ],
        ["IDENTIFICATION"] = [
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER",
            "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER",
            "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "SEC_SCANNER", "SEC_CONFIRMER_PASSAGE", "SEC_VOIR_HISTORIQUE",
            "VOIR_TOUS_BONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD"
        ],
        ["INVESTIGATION"] = [
            "SEC_SIGNALER_ANOMALIE", "SEC_TRAITER_ANOMALIE", "SEC_REOUVRIR_ANOMALIE", "SEC_VOIR_HISTORIQUE",
            "VOIR_TOUS_BONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD", "EXPORT_EXCEL"
        ],
        ["ENVIRONNEMENT"] = [
            "BEM_APPROUVER", "BEM_REJETER", "BEM_IMPRIMER",
            "BSM_APPROUVER", "BSM_REJETER", "BSM_IMPRIMER",
            "VOIR_TOUS_BONS", "VOIR_APPROBATIONS", "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD"
        ],
        ["UTILISATEUR"] = [
            "BEM_CREER", "BEM_MODIFIER", "BEM_SOUMETTRE", "BEM_SUPPRIMER", "BEM_IMPRIMER",
            "BSM_CREER", "BSM_MODIFIER", "BSM_SOUMETTRE", "BSM_SUPPRIMER", "BSM_IMPRIMER",
            "PRET_RETOUR", "PRET_EXTENSION",
            "VOIR_HISTORIQUE", "VOIR_TABLEAU_BORD"
        ],
        ["DEMANDEUR"] = [
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

    public IReadOnlyList<string> GetDefaultActiviteCodesForRole(string roleCode)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
            return [];

        return RoleDefaultActivites.TryGetValue(roleCode, out var codes)
            ? codes
            : [];
    }
}
