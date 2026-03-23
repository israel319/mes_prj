using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace KCCMaterialFlow.Infrastructure.Data;

/// <summary>
/// Factory pour créer le DbContext au moment du design (migrations EF).
/// Cette factory est utilisée automatiquement par les commandes EF CLI
/// comme 'dotnet ef migrations add' et 'dotnet ef database update'.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<KCCMaterialFlowDbContext>
{
    /// <summary>
    /// Crée une instance du DbContext pour les outils design-time d'EF Core.
    /// </summary>
    /// <param name="args">Arguments de ligne de commande (non utilisés)</param>
    /// <returns>Instance de KCCMaterialFlowDbContext configurée</returns>
    public KCCMaterialFlowDbContext CreateDbContext(string[] args)
    {
        // Charger les assemblies des modules pour les configurations EF
        var moduleAssemblies = LoadModuleAssemblies();
        
        // Configurer les assemblies AVANT de créer le DbContext
        KCCMaterialFlowDbContext.ConfiguredModuleAssemblies = moduleAssemblies;

        var optionsBuilder = new DbContextOptionsBuilder<KCCMaterialFlowDbContext>();
        
        // Connection string par défaut pour les migrations
        // En production, cette valeur est remplacée par celle de appsettings.json
        var connectionString = "Server=CDKTGNBK01127;Database=AppDev_KCCMaterialFlow_DB_Dev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
        
        optionsBuilder.UseSqlServer(connectionString, options =>
        {
            // Configurer les migrations pour utiliser l'assembly Infrastructure
            options.MigrationsAssembly("KCCMaterialFlow.Infrastructure");
            options.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });

        return new KCCMaterialFlowDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Charge dynamiquement les assemblies des modules pour les configurations EF.
    /// </summary>
    /// <returns>Liste des assemblies de modules chargées</returns>
    private static List<Assembly> LoadModuleAssemblies()
    {
        var assemblies = new List<Assembly>();
        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        // Pattern pour trouver les assemblies de modules
        var modulePattern = "KCCMaterialFlow.Module.*.dll";

        try
        {
            // Chercher les DLLs de modules dans le répertoire de base
            var moduleDlls = Directory.GetFiles(basePath, modulePattern, SearchOption.TopDirectoryOnly);

            foreach (var dll in moduleDlls)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    assemblies.Add(assembly);
                    Console.WriteLine($"[DesignTimeFactory] Loaded module assembly: {assembly.GetName().Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DesignTimeFactory] Failed to load {dll}: {ex.Message}");
                }
            }

            // Ajouter aussi l'assembly Module.Shared pour les entités partagées
            var sharedModuleAssembly = typeof(KCCMaterialFlow.Module.Shared.Entities.Compagnie).Assembly;
            assemblies.Add(sharedModuleAssembly);
            Console.WriteLine($"[DesignTimeFactory] Loaded Module.Shared assembly: {sharedModuleAssembly.GetName().Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DesignTimeFactory] Error loading module assemblies: {ex.Message}");
        }

        return assemblies;
    }
}
