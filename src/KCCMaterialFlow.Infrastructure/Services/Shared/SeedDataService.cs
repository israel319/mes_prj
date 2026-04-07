using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Service pour initialiser les données de référence dans la base de données.
/// NOTE: Les catégories/raisons de sortie sont gérées via Scripts/InsertCategoriesCheckpoints.sql
/// Ce service ne seed que les barrières si nécessaire.
/// </summary>
public static class SeedDataService
{
    /// <summary>
    /// Initialise les données de référence (barrières uniquement)
    /// Les catégories et raisons sont gérées via le script SQL InsertCategoriesCheckpoints.sql
    /// </summary>
    public static async Task SeedReferenceDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<KCCMaterialFlowDbContext>>();
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var logger = scope.ServiceProvider.GetService<ILogger<KCCMaterialFlowDbContext>>();

        try
        {
            await SeedBarrieresAsync(context, logger);
            await SeedDepartementRaisonMappingsAsync(context, logger);
            await SeedRaisonsEntreeAsync(context, logger);
            await SeedWorkflowEtapesAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Erreur lors du seeding des données de référence");
            throw;
        }
    }

    /// <summary>
    /// Initialise les barrières (checkpoints) si elles n'existent pas
    /// </summary>
    private static async Task SeedBarrieresAsync(KCCMaterialFlowDbContext context, ILogger? logger)
    {
        var barriereSet = context.Set<Barriere>();

        if (await barriereSet.AnyAsync())
        {
            logger?.LogInformation("Les barrières existent déjà, skip du seeding");
            return;
        }

        logger?.LogInformation("Création des barrières...");

        var barrieres = new List<Barriere>
        {
            new() { CodeBarriere = "B1-KTO", NomBarriere = "Barrière KTO", Description = "Entrée principale KTO", Localisation = "KTO", EstActive = true, OrdreAffichage = 1 },
            new() { CodeBarriere = "B2-LUILU", NomBarriere = "Barrière LUILU", Description = "Entrée site LUILU", Localisation = "LUILU", EstActive = true, OrdreAffichage = 2 },
            new() { CodeBarriere = "B3-SKM", NomBarriere = "Barrière SKM", Description = "Entrée site SKM", Localisation = "SKM", EstActive = true, OrdreAffichage = 3 },
            new() { CodeBarriere = "B4-LUSANGA", NomBarriere = "Barrière LUSANGA", Description = "Entrée site LUSANGA", Localisation = "LUSANGA", EstActive = true, OrdreAffichage = 4 },
            new() { CodeBarriere = "B5-KOV", NomBarriere = "Barrière KOV", Description = "Entrée site KOV", Localisation = "KOV", EstActive = true, OrdreAffichage = 5 },
            new() { CodeBarriere = "B6-MV", NomBarriere = "Barrière MV", Description = "Entrée site MV", Localisation = "MV", EstActive = true, OrdreAffichage = 6 },
            new() { CodeBarriere = "B7-MASHAMBA", NomBarriere = "Barrière MASHAMBA", Description = "Entrée site MASHAMBA", Localisation = "MASHAMBA", EstActive = true, OrdreAffichage = 7 },
            new() { CodeBarriere = "B8-KTC", NomBarriere = "Barrière KTC", Description = "Entrée site KTC", Localisation = "KTC", EstActive = true, OrdreAffichage = 8 }
        };

        await barriereSet.AddRangeAsync(barrieres);
        await context.SaveChangesAsync();

        logger?.LogInformation("8 barrières créées avec succès");
    }

    /// <summary>
    /// Seed les mappings Département → RaisonSortie autorisées.
    /// DepartementId = null signifie "défaut pour tous les départements non-mappés spécifiquement".
    /// </summary>
    private static async Task SeedDepartementRaisonMappingsAsync(KCCMaterialFlowDbContext context, ILogger? logger)
    {
        var mappingSet = context.Set<DepartementRaisonSortie>();
        if (await mappingSet.AnyAsync())
        {
            logger?.LogInformation("Les mappings Département-RaisonSortie existent déjà, skip");
            return;
        }

        // Charger les départements et raisons existants
        var departements = await context.Departements.ToListAsync();
        var raisons = await context.Set<RaisonSortie>().ToListAsync();

        if (!raisons.Any())
        {
            logger?.LogWarning("Aucune RaisonSortie trouvée — seeding des mappings ignoré. Exécutez d'abord le script InsertCategoriesCheckpoints.sql");
            return;
        }

        var mappings = new List<DepartementRaisonSortie>();

        // Helper pour trouver un département par code (case-insensitive)
        int? FindDeptId(string code) => departements
            .FirstOrDefault(d => d.CodeDepartement.Equals(code, StringComparison.OrdinalIgnoreCase))?.Id;

        // Helper pour trouver une raison par code
        int? FindRaisonId(string code) => raisons
            .FirstOrDefault(r => r.Code != null && r.Code.Equals(code, StringComparison.OrdinalIgnoreCase))?.Id;

        // ── Département IT → Informatique uniquement ──
        var deptIT = FindDeptId("IT");
        var raisonINFO = FindRaisonId("INFO");
        if (deptIT.HasValue && raisonINFO.HasValue)
        {
            mappings.Add(new DepartementRaisonSortie
            {
                DepartementId = deptIT.Value,
                RaisonSortieId = raisonINFO.Value,
                AutoSelection = true,
                OrdreAffichage = 1
            });
        }

        // ── Département Environnement → Résidu, Radio Protection, Modification ──
        var deptENV = FindDeptId("ENV") ?? FindDeptId("HSE");
        var envRaisons = new[] { ("RESIDU", 1), ("RADIO_PROT", 2), ("MODIF", 3) };
        foreach (var (code, ordre) in envRaisons)
        {
            var raisonId = FindRaisonId(code);
            if (deptENV.HasValue && raisonId.HasValue)
            {
                mappings.Add(new DepartementRaisonSortie
                {
                    DepartementId = deptENV.Value,
                    RaisonSortieId = raisonId.Value,
                    AutoSelection = false,
                    OrdreAffichage = ordre
                });
            }
        }

        // ── Défaut (DepartementId = NULL) → Fin chantier, Circulaire, Prêt, Autre ──
        var defaultRaisons = new[] { ("FIN_CHANTIER", 1), ("CIRC", 2), ("PRET", 3), ("AUTRE", 4) };
        foreach (var (code, ordre) in defaultRaisons)
        {
            var raisonId = FindRaisonId(code);
            if (raisonId.HasValue)
            {
                mappings.Add(new DepartementRaisonSortie
                {
                    DepartementId = null,
                    RaisonSortieId = raisonId.Value,
                    AutoSelection = false,
                    OrdreAffichage = ordre
                });
            }
        }

        if (mappings.Any())
        {
            await mappingSet.AddRangeAsync(mappings);
            await context.SaveChangesAsync();
            logger?.LogInformation("{Count} mappings Département-RaisonSortie créés", mappings.Count);
        }
        else
        {
            logger?.LogWarning("Aucun mapping créé — vérifiez que les départements (IT, ENV/HSE) et raisons (INFO, RESIDU, etc.) existent");
        }
    }

    /// <summary>
    /// Seed les motifs d'entrée structurés et leurs mappings vers les motifs de sortie.
    /// </summary>
    private static async Task SeedRaisonsEntreeAsync(KCCMaterialFlowDbContext context, ILogger? logger)
    {
        var raisonEntreeSet = context.Set<RaisonEntree>();
        if (await raisonEntreeSet.AnyAsync())
        {
            logger?.LogInformation("Les RaisonsEntree existent déjà, skip");
            return;
        }

        // Définir les motifs d'entrée
        var raisonsEntree = new List<RaisonEntree>
        {
            new() { Nom = "Contrat / Chantier", Code = "CONTRAT", Description = "Matériel entrant dans le cadre d'un contrat ou chantier", EstActif = true, OrdreAffichage = 1, Icone = "assignment", Couleur = "#1976D2" },
            new() { Nom = "Maintenance / Réparation", Code = "MAINTENANCE", Description = "Matériel entrant pour maintenance ou réparation", EstActif = true, OrdreAffichage = 2, Icone = "build", Couleur = "#FF9800" },
            new() { Nom = "Stockage temporaire", Code = "STOCKAGE_TEMP", Description = "Matériel entrant pour stockage temporaire", EstActif = true, OrdreAffichage = 3, Icone = "inventory", Couleur = "#607D8B" },
            new() { Nom = "Radio protection", Code = "RADIO", Description = "Matériel lié à la radio protection", EstActif = true, OrdreAffichage = 4, Icone = "warning", Couleur = "#F44336" },
            new() { Nom = "Modification / Transformation", Code = "MODIF", Description = "Matériel entrant pour modification ou transformation", EstActif = true, OrdreAffichage = 5, Icone = "settings", Couleur = "#9C27B0" },
            new() { Nom = "Autre", Code = "AUTRE", Description = "Autre motif (commentaire requis)", EstActif = true, OrdreAffichage = 6, Icone = "help_outline", Couleur = "#757575" },
        };

        await raisonEntreeSet.AddRangeAsync(raisonsEntree);
        await context.SaveChangesAsync();
        logger?.LogInformation("{Count} RaisonsEntree créées", raisonsEntree.Count);

        // Créer les mappings Entrée → Sortie (seulement si les raisons de sortie existent)
        var raisonsSortie = await context.Set<RaisonSortie>().ToListAsync();
        if (!raisonsSortie.Any())
        {
            logger?.LogWarning("Aucune RaisonSortie trouvée — les mappings Entrée→Sortie seront créés plus tard");
            return;
        }

        int? FindRaisonSortieId(string code) => raisonsSortie
            .FirstOrDefault(r => r.Code != null && r.Code.Equals(code, StringComparison.OrdinalIgnoreCase))?.Id;

        // Créer les mappings Entrée → Sortie
        var mappingSet = context.Set<RaisonEntreeRaisonSortie>();
        var mappings = new List<RaisonEntreeRaisonSortie>();

        // Recharger les raisons d'entrée pour avoir leurs IDs
        var savedRaisonsEntree = await raisonEntreeSet.ToListAsync();

        var entreeToSortie = new[]
        {
            ("CONTRAT", "FIN_CHANTIER"),
            ("MAINTENANCE", "FIN_CHANTIER"),
            ("STOCKAGE_TEMP", "RESIDU"),
            ("RADIO", "RADIO_PROT"),
            ("MODIF", "MODIF"),
            ("AUTRE", "AUTRE"),
        };

        foreach (var (entreeCode, sortieCode) in entreeToSortie)
        {
            var entreeId = savedRaisonsEntree.FirstOrDefault(r => r.Code == entreeCode)?.Id;
            var sortieId = FindRaisonSortieId(sortieCode);

            if (entreeId.HasValue && sortieId.HasValue)
            {
                mappings.Add(new RaisonEntreeRaisonSortie
                {
                    RaisonEntreeId = entreeId.Value,
                    RaisonSortieId = sortieId.Value,
                    AutoSelection = true,
                    OrdreAffichage = 1
                });
            }
            else
            {
                logger?.LogWarning("Mapping ignoré: {Entree} → {Sortie} (entrée={EntreeId}, sortie={SortieId})",
                    entreeCode, sortieCode, entreeId, sortieId);
            }
        }

        if (mappings.Any())
        {
            await mappingSet.AddRangeAsync(mappings);
            await context.SaveChangesAsync();
            logger?.LogInformation("{Count} mappings RaisonEntree→RaisonSortie créés", mappings.Count);
        }
    }

    /// <summary>
    /// Seed les chaînes d'approbation par défaut dans T_WorkflowEtapesConfig.
    /// </summary>
    private static async Task SeedWorkflowEtapesAsync(KCCMaterialFlowDbContext context, ILogger? logger)
    {
        var configSet = context.Set<WorkflowEtapeConfig>();
        if (await configSet.AnyAsync())
        {
            logger?.LogInformation("Les WorkflowEtapeConfig existent déjà, skip");
            return;
        }

        logger?.LogInformation("Création des chaînes d'approbation par défaut...");

        var now = DateTime.Now;
        var etapes = new List<WorkflowEtapeConfig>();

        void AddChain(string bonType, string? raisonCode, params (string role, string nom)[] steps)
        {
            for (int i = 0; i < steps.Length; i++)
            {
                etapes.Add(new WorkflowEtapeConfig
                {
                    BonType = bonType,
                    RaisonSortieCode = raisonCode,
                    OrdreEtape = i + 1,
                    RoleCode = steps[i].role,
                    NomEtape = steps[i].nom,
                    EstActif = true,
                    DateCreation = now
                });
            }
        }

        // ── BSM : Circulaire ──
        AddChain("BSM", "CIRC",
            ("SUPERVISEUR", "Superviseur"),
            ("GM", "General Manager"),
            ("OPJ", "OPJ"),
            ("IDENTIFICATION", "Identification"));

        // ── BSM : Informatique (IT en premier) ──
        AddChain("BSM", "INFO",
            ("IT", "Département IT"),
            ("SUPERVISEUR", "Superviseur"),
            ("GM", "General Manager"),
            ("OPJ", "OPJ"),
            ("IDENTIFICATION", "Identification"));

        // ── BSM : Fin de Chantier (OPJ en premier) ──
        AddChain("BSM", "FIN_CHANTIER",
            ("OPJ", "OPJ"),
            ("SUPERVISEUR", "Superviseur"),
            ("GM", "General Manager"),
            ("IDENTIFICATION", "Identification"));

        // ── BSM : Résidu (Environnement en premier) ──
        AddChain("BSM", "RESIDU",
            ("ENVIRONNEMENT", "Département Environnement"),
            ("SUPERVISEUR", "Superviseur"),
            ("GM", "General Manager"),
            ("OPJ", "OPJ"),
            ("IDENTIFICATION", "Identification"));

        // ── BSM : Radio Protection (Environnement en premier) ──
        AddChain("BSM", "RADIO_PROT",
            ("ENVIRONNEMENT", "Département Environnement"),
            ("SUPERVISEUR", "Superviseur"),
            ("GM", "General Manager"),
            ("OPJ", "OPJ"),
            ("IDENTIFICATION", "Identification"));

        // ── BSM : Modification (Environnement en premier) ──
        AddChain("BSM", "MODIF",
            ("ENVIRONNEMENT", "Département Environnement"),
            ("SUPERVISEUR", "Superviseur"),
            ("GM", "General Manager"),
            ("OPJ", "OPJ"),
            ("IDENTIFICATION", "Identification"));

        // ── BSM : Prêt ──
        AddChain("BSM", "PRET",
            ("SUPERVISEUR", "Superviseur"),
            ("GM", "General Manager"),
            ("OPJ", "OPJ"),
            ("IDENTIFICATION", "Identification"));

        // ── BEM : Chaîne unique (null = toutes les entrées) ──
        AddChain("BEM", null,
            ("SUPERVISEUR", "Superviseur"),
            ("GM", "General Manager"),
            ("OPJ", "OPJ"),
            ("IDENTIFICATION", "Identification"));

        await configSet.AddRangeAsync(etapes);
        await context.SaveChangesAsync();
        logger?.LogInformation("{Count} étapes de workflow créées", etapes.Count);
    }
}
