using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        var optionsBuilder = new DbContextOptionsBuilder<KCCMaterialFlowDbContext>();

        // Connection string par défaut pour les migrations
        // En production, cette valeur est remplacée par celle de appsettings.json
        var connectionString = "Server=CDKTGSVC9DB0034;Database=AppDev_SecurityPortal_DB;User Id=svc-cd-ktg-devdbt;Password=Bdoj821_;TrustServerCertificate=True;MultipleActiveResultSets=true";

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
}
