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
}
