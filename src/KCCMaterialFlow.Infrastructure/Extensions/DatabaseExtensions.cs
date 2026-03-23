using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Extensions;

/// <summary>
/// Extensions pour la vérification de la connectivité base de données
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Vérifie si la connexion à la base de données est fonctionnelle
    /// </summary>
    /// <param name="context">DbContext à tester</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>True si la connexion est établie</returns>
    public static async Task<bool> CanConnectAsync(this KCCMaterialFlowDbContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            return await context.Database.CanConnectAsync(cancellationToken);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Teste la connexion et retourne les informations détaillées
    /// </summary>
    /// <param name="context">DbContext à tester</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat du test avec détails</returns>
    public static async Task<DatabaseConnectionResult> TestConnectionAsync(
        this KCCMaterialFlowDbContext context,
        CancellationToken cancellationToken = default)
    {
        var result = new DatabaseConnectionResult();
        var startTime = DateTime.UtcNow;

        try
        {
            result.CanConnect = await context.Database.CanConnectAsync(cancellationToken);
            result.ResponseTime = DateTime.UtcNow - startTime;

            if (result.CanConnect)
            {
                // Récupérer le nom du serveur et de la base
                var connection = context.Database.GetDbConnection();
                result.ServerName = connection.DataSource;
                result.DatabaseName = connection.Database;
                result.Message = "Connexion établie avec succès";
            }
            else
            {
                result.Message = "Impossible de se connecter à la base de données";
            }
        }
        catch (Exception ex)
        {
            result.CanConnect = false;
            result.ResponseTime = DateTime.UtcNow - startTime;
            result.Message = $"Erreur de connexion: {ex.Message}";
            result.Exception = ex;
        }

        return result;
    }

    /// <summary>
    /// Applique les migrations en attente
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    public static async Task ApplyMigrationsAsync(
        this KCCMaterialFlowDbContext context,
        CancellationToken cancellationToken = default)
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Obtient la liste des migrations appliquées et en attente
    /// </summary>
    public static async Task<MigrationStatus> GetMigrationStatusAsync(
        this KCCMaterialFlowDbContext context,
        CancellationToken cancellationToken = default)
    {
        return new MigrationStatus
        {
            AppliedMigrations = (await context.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList(),
            PendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList()
        };
    }
}

/// <summary>
/// Résultat d'un test de connexion à la base de données
/// </summary>
public class DatabaseConnectionResult
{
    public bool CanConnect { get; set; }
    public string? ServerName { get; set; }
    public string? DatabaseName { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
}

/// <summary>
/// Statut des migrations
/// </summary>
public class MigrationStatus
{
    public List<string> AppliedMigrations { get; set; } = new();
    public List<string> PendingMigrations { get; set; } = new();
    public bool HasPendingMigrations => PendingMigrations.Count > 0;
}
