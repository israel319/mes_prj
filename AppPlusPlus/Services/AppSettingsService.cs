using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Data;

namespace AppPlusPlus.Services;

/// <summary>
/// Service singleton qui met en cache les paramètres globaux de l'application (dont le nom).
/// Chargé au démarrage, rafraîchissable à tout moment.
/// </summary>
public class AppSettingsService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private Dictionary<string, string> _cache = new(StringComparer.OrdinalIgnoreCase);

    public const string KEY_APP_NAME = "AppName";
    private const string DEFAULT_APP_NAME = "App++";

    public event Action? OnChange;

    public AppSettingsService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>Nom de l'application (depuis la BD ou valeur par défaut).</summary>
    public string AppName => Get(KEY_APP_NAME) ?? DEFAULT_APP_NAME;

    /// <summary>Charge tous les paramètres depuis la BD.</summary>
    public async Task LoadAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        try
        {
            await EnsureTableAsync(ctx);

            _cache = await ctx.AppSettings
                .ToDictionaryAsync(s => s.Key, s => s.Value ?? "", StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            _cache = new(StringComparer.OrdinalIgnoreCase);
        }
    }

    /// <summary>Obtient la valeur d'un paramètre par sa clé.</summary>
    public string? Get(string key)
        => _cache.TryGetValue(key, out var val) ? val : null;

    /// <summary>Enregistre un paramètre dans la BD et met à jour le cache.</summary>
    public async Task SetAsync(string key, string value)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        // S'assurer que la table existe
        await EnsureTableAsync(ctx);

        var existing = await ctx.AppSettings.FindAsync(key);
        if (existing != null)
        {
            existing.Value = value;
        }
        else
        {
            ctx.AppSettings.Add(new Models.AppSetting { Key = key, Value = value });
        }
        await ctx.SaveChangesAsync();
        _cache[key] = value;
        OnChange?.Invoke();
    }

    /// <summary>Crée la table T_AppSettings si elle n'existe pas.</summary>
    private static async Task EnsureTableAsync(AppDbContext ctx)
    {
        await ctx.Database.ExecuteSqlRawAsync("""
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_AppSettings]') AND type = 'U')
            CREATE TABLE [dbo].[T_AppSettings](
                [Key] [varchar](100) NOT NULL,
                [Value] [varchar](500) NULL,
                CONSTRAINT [PK_T_AppSettings] PRIMARY KEY CLUSTERED ([Key] ASC)
            )
            """);
    }
}
