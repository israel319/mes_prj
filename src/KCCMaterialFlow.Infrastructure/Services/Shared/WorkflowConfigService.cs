using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using DomainTypeMateriel = KCCMaterialFlow.Domain.Enums.TypeMateriel;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

public class WorkflowConfigService : IWorkflowConfigService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private const string CachePrefix = "WorkflowConfig_";

    public WorkflowConfigService(IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory, IMemoryCache cache)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }

    // ─── Résolution du workflow effectif (sans écriture BD) ─────────────────
    public async Task<IReadOnlyList<WorkflowEtapeConfig>> GetResolvedWorkflowEtapesAsync(
        string bonType, string? raisonSortieCode, CancellationToken cancellationToken = default)
    {
        var normalizedBonType = NormalizeBonType(bonType);
        var normalizedRaison  = NormalizeRaison(raisonSortieCode);

        // 1) Config BD spécifique (BonType + Motif)
        var specific = await GetWorkflowEtapesForAdminAsync(normalizedBonType, normalizedRaison, cancellationToken);
        if (specific.Count > 0) return specific;

        // 2) Config BD générique (BonType sans motif)
        var generic = await GetWorkflowEtapesForAdminAsync(normalizedBonType, null, cancellationToken);
        if (generic.Count > 0) return generic;

        // 3) Défaut métier (calcul à la volée, sans écrire en BD)
        return await BuildBusinessDefaultWorkflowAsync(normalizedBonType, normalizedRaison, cancellationToken);
    }

    // ─── Résumé admin : quels motifs ont une config BD vs héritent ──────────
    public async Task<IReadOnlyList<WorkflowContextSummary>> GetWorkflowSummaryAsync(
        string bonType, CancellationToken cancellationToken = default)
    {
        var normalizedBonType = NormalizeBonType(bonType);

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Toutes les entrées BD pour ce BonType
        var allDbEntries = await context.Set<WorkflowEtapeConfig>()
            .Where(x => x.BonType == normalizedBonType && x.EstActif)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dbByRaison = allDbEntries
            .GroupBy(x => x.RaisonSortieCode ?? "")
            .ToDictionary(g => g.Key, g => g.OrderBy(x => x.OrdreEtape).ToList());

        // Toutes les raisons de sortie actives
        var raisons = await context.Set<RaisonSortie>()
            .Where(x => x.EstActif)
            .OrderBy(x => x.CategorieId).ThenBy(x => x.OrdreAffichage)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var genericCount = dbByRaison.TryGetValue("", out var genericList) ? genericList.Count : 0;
        var hasGeneric   = genericCount > 0;

        var result = new List<WorkflowContextSummary>();

        // Ligne "Workflow par défaut" (sans motif)
        result.Add(new WorkflowContextSummary
        {
            RaisonSortieCode  = null,
            RaisonSortieNom   = "(Workflow par défaut – sans motif spécifique)",
            EstPersonnalise   = hasGeneric,
            NombreEtapesBD    = genericCount,
            NombreEtapesResolu = hasGeneric
                ? genericCount
                : (await BuildBusinessDefaultWorkflowAsync(normalizedBonType, null, cancellationToken)).Count,
            Source = hasGeneric ? "BD générique" : "Défaut métier"
        });

        // Une ligne par motif
        foreach (var r in raisons)
        {
            var code = NormalizeRaison(r.Code);
            var hasSpecific = code != null && dbByRaison.TryGetValue(code, out var specificList) && specificList.Count > 0;
            var specificCount = hasSpecific ? dbByRaison[code!].Count : 0;

            var resolvedCount = hasSpecific
                ? specificCount
                : hasGeneric
                    ? genericCount
                    : (await BuildBusinessDefaultWorkflowAsync(normalizedBonType, code, cancellationToken)).Count;

            var source = hasSpecific
                ? "BD spécifique"
                : hasGeneric
                    ? "BD générique (hérité)"
                    : "Défaut métier";

            result.Add(new WorkflowContextSummary
            {
                RaisonSortieCode   = r.Code,
                RaisonSortieNom    = r.Nom,
                EstPersonnalise    = hasSpecific,
                NombreEtapesBD     = specificCount,
                NombreEtapesResolu = resolvedCount,
                Source             = source
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<WorkflowEtapeConfig>> GetWorkflowEtapesForAdminAsync(string bonType, string? raisonSortieCode, CancellationToken cancellationToken = default)
    {
        var normalizedBonType = NormalizeBonType(bonType);
        var normalizedRaison = NormalizeRaison(raisonSortieCode);
        var cacheKey = BuildCacheKey(normalizedBonType, normalizedRaison);

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;

            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var query = context.Set<WorkflowEtapeConfig>()
                .Where(x => x.BonType == normalizedBonType && x.EstActif);

            if (string.IsNullOrWhiteSpace(normalizedRaison))
            {
                query = query.Where(x => x.RaisonSortieCode == null || x.RaisonSortieCode == string.Empty);
            }
            else
            {
                query = query.Where(x => x.RaisonSortieCode == normalizedRaison);
            }

            return (IReadOnlyList<WorkflowEtapeConfig>)await query
                .OrderBy(x => x.OrdreEtape)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }) ?? [];
    }

    public async Task SaveWorkflowEtapesAsync(string bonType, string? raisonSortieCode, IEnumerable<WorkflowEtapeConfig> etapes, string? modifiedByLogin, CancellationToken cancellationToken = default)
    {
        var normalizedBonType = NormalizeBonType(bonType);
        var normalizedRaison = NormalizeRaison(raisonSortieCode);

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existing = await context.Set<WorkflowEtapeConfig>()
            .Where(x => x.BonType == normalizedBonType &&
                ((normalizedRaison == null && (x.RaisonSortieCode == null || x.RaisonSortieCode == string.Empty)) ||
                 (normalizedRaison != null && x.RaisonSortieCode == normalizedRaison)))
            .ToListAsync(cancellationToken);

        if (existing.Count > 0)
        {
            context.Set<WorkflowEtapeConfig>().RemoveRange(existing);
        }

        var now = DateTime.Now;
        var incoming = etapes
            .Where(x => !string.IsNullOrWhiteSpace(x.RoleCode) && !string.IsNullOrWhiteSpace(x.NomEtape))
            .OrderBy(x => x.OrdreEtape)
            .Select((x, index) => new WorkflowEtapeConfig
            {
                BonType = normalizedBonType,
                RaisonSortieCode = normalizedRaison,
                OrdreEtape = index + 1,
                RoleCode = x.RoleCode.Trim(),
                NomEtape = x.NomEtape.Trim(),
                EstActif = true,
                DateCreation = now,
                DateModification = now,
                ModifieParLogin = modifiedByLogin
            })
            .ToList();

        if (incoming.Count > 0)
        {
            await context.Set<WorkflowEtapeConfig>().AddRangeAsync(incoming, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        _cache.Remove(BuildCacheKey(normalizedBonType, normalizedRaison));
        _cache.Remove(BuildCacheKey(normalizedBonType, null));
    }

    // ─── Suppression d'une config spécifique (retour à l'héritage) ──────────
    public async Task DeleteWorkflowEtapesAsync(
        string bonType, string? raisonSortieCode, string? modifiedByLogin, CancellationToken cancellationToken = default)
    {
        var normalizedBonType = NormalizeBonType(bonType);
        var normalizedRaison  = NormalizeRaison(raisonSortieCode);

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existing = await context.Set<WorkflowEtapeConfig>()
            .Where(x => x.BonType == normalizedBonType &&
                ((normalizedRaison == null && (x.RaisonSortieCode == null || x.RaisonSortieCode == string.Empty)) ||
                 (normalizedRaison != null && x.RaisonSortieCode == normalizedRaison)))
            .ToListAsync(cancellationToken);

        if (existing.Count > 0)
        {
            context.Set<WorkflowEtapeConfig>().RemoveRange(existing);
            await context.SaveChangesAsync(cancellationToken);
        }

        _cache.Remove(BuildCacheKey(normalizedBonType, normalizedRaison));
        _cache.Remove(BuildCacheKey(normalizedBonType, null));
    }

    public async Task<IReadOnlyList<RaisonSortie>> GetRaisonsSortieAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<RaisonSortie>()
            .Where(x => x.EstActif)
            .OrderBy(x => x.CategorieId)
            .ThenBy(x => x.OrdreAffichage)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private static string NormalizeBonType(string bonType) =>
        string.IsNullOrWhiteSpace(bonType) ? "BSM" : bonType.Trim().ToUpperInvariant();

    private static string? NormalizeRaison(string? raisonSortieCode)
    {
        if (string.IsNullOrWhiteSpace(raisonSortieCode))
        {
            return null;
        }

        return raisonSortieCode.Trim().ToUpperInvariant();
    }

    private static string BuildCacheKey(string bonType, string? raisonSortieCode)
        => $"{CachePrefix}{bonType}_{raisonSortieCode ?? "DEFAULT"}";

    private async Task<IReadOnlyList<WorkflowEtapeConfig>> BuildBusinessDefaultWorkflowAsync(string bonType, string? raisonSortieCode, CancellationToken cancellationToken)
    {
        if (bonType == "BEM")
        {
            return
            [
                new WorkflowEtapeConfig { BonType = "BEM", RaisonSortieCode = raisonSortieCode, OrdreEtape = 1, RoleCode = "Superviseur", NomEtape = "Superviseur", EstActif = true },
                new WorkflowEtapeConfig { BonType = "BEM", RaisonSortieCode = raisonSortieCode, OrdreEtape = 2, RoleCode = "GM", NomEtape = "General Manager", EstActif = true },
                new WorkflowEtapeConfig { BonType = "BEM", RaisonSortieCode = raisonSortieCode, OrdreEtape = 3, RoleCode = "OPJ", NomEtape = "OPJ", EstActif = true },
                new WorkflowEtapeConfig { BonType = "BEM", RaisonSortieCode = raisonSortieCode, OrdreEtape = 4, RoleCode = "Identification", NomEtape = "Identification", EstActif = true }
            ];
        }

        var typeMateriel = await ResolveTypeMaterielDefautAsync(raisonSortieCode, cancellationToken);
        var etapes = new List<WorkflowEtapeConfig>();
        var ordre = 1;

        if (typeMateriel == DomainTypeMateriel.Informatique)
        {
            etapes.Add(new WorkflowEtapeConfig { BonType = "BSM", RaisonSortieCode = raisonSortieCode, OrdreEtape = ordre++, RoleCode = "IT", NomEtape = "IT", EstActif = true });
        }

        if (typeMateriel is DomainTypeMateriel.Residu or DomainTypeMateriel.Radioprotection or DomainTypeMateriel.Modification)
        {
            etapes.Add(new WorkflowEtapeConfig { BonType = "BSM", RaisonSortieCode = raisonSortieCode, OrdreEtape = ordre++, RoleCode = "Environnement", NomEtape = "Environnement", EstActif = true });
        }

        etapes.Add(new WorkflowEtapeConfig { BonType = "BSM", RaisonSortieCode = raisonSortieCode, OrdreEtape = ordre++, RoleCode = "Superviseur", NomEtape = "Superviseur", EstActif = true });
        etapes.Add(new WorkflowEtapeConfig { BonType = "BSM", RaisonSortieCode = raisonSortieCode, OrdreEtape = ordre++, RoleCode = "GM", NomEtape = "General Manager", EstActif = true });
        etapes.Add(new WorkflowEtapeConfig { BonType = "BSM", RaisonSortieCode = raisonSortieCode, OrdreEtape = ordre++, RoleCode = "OPJ", NomEtape = "OPJ", EstActif = true });
        etapes.Add(new WorkflowEtapeConfig { BonType = "BSM", RaisonSortieCode = raisonSortieCode, OrdreEtape = ordre++, RoleCode = "Identification", NomEtape = "Identification", EstActif = true });

        return etapes;
    }

    private async Task<DomainTypeMateriel?> ResolveTypeMaterielDefautAsync(string? raisonSortieCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(raisonSortieCode))
        {
            return null;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<RaisonSortie>()
            .Where(x => x.EstActif && x.Code != null && x.Code.ToUpper() == raisonSortieCode)
            .Select(x => x.TypeMaterielDefaut)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
