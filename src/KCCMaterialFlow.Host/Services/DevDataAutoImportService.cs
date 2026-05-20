using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Service hosted DEV-ONLY : importe Book2.xlsx au démarrage vers
/// T_AllEmployees, et synchronise Employee.Login depuis UserName.
/// La table T_Employees N'EST PAS écrasée.
///
/// Configuration (appsettings.Development.json) :
///   "DevAutoImport": {
///     "Enabled": true,
///     "DataFile": "C:/Users/ikasa/Downloads/Book2.xlsx",
///     "MinEmployeesThreshold": 50,    // déclenche l'import si T_AllEmployees < seuil
///     "Force": false                   // force l'import même si seuil atteint
///   }
/// </summary>
public sealed class DevDataAutoImportService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _env;
    private readonly ILogger<DevDataAutoImportService> _logger;

    public DevDataAutoImportService(
        IServiceProvider services,
        IConfiguration configuration,
        IHostEnvironment env,
        ILogger<DevDataAutoImportService> logger)
    {
        _services = services;
        _configuration = configuration;
        _env = env;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_env.IsDevelopment())
            return;

        var section = _configuration.GetSection("DevAutoImport");
        if (!section.Exists() || !section.GetValue("Enabled", false))
            return;

        var dataFile = section["DataFile"];
        if (string.IsNullOrWhiteSpace(dataFile))
        {
            _logger.LogWarning("DevAutoImport activé mais DataFile non configuré.");
            return;
        }

        if (!File.Exists(dataFile))
        {
            _logger.LogWarning("DevAutoImport : fichier introuvable {File}", dataFile);
            return;
        }

        var threshold = section.GetValue("MinEmployeesThreshold", 50);
        var force = section.GetValue("Force", false);

        try
        {
            using var scope = _services.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<KCCMaterialFlowDbContext>>();
            var glencoreImport = scope.ServiceProvider.GetRequiredService<IAllEmployeeImportService>();

            await using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);
            var glencoreCount = await ctx.AllEmployees.CountAsync(cancellationToken);

            if (!force && glencoreCount >= threshold)
            {
                _logger.LogInformation(
                    "DevAutoImport : {Count} GlencoreEmployees en BD (>= {Threshold}), import ignoré (Force=false).",
                    glencoreCount, threshold);
                return;
            }

            _logger.LogInformation(
                "DevAutoImport : démarrage import Book2.xlsx ({File}) — actuels={Count}, force={Force}",
                dataFile, glencoreCount, force);

            // Copie locale pour éviter le verrou si le fichier est ouvert dans Excel
            var tmp = Path.Combine(Path.GetTempPath(), $"book2-{Guid.NewGuid():N}.xlsx");
            File.Copy(dataFile, tmp, overwrite: true);
            try
            {
                await using var fs = File.OpenRead(tmp);
                var result = await glencoreImport.ImportFromXlsxAsync(fs, cancellationToken);

                _logger.LogInformation(
                    "DevAutoImport terminé : {Read} lignes, {Ins} insertions, {Upd} maj, {Skip} ignorées, {Login} Employee.Login synchronisés.",
                    result.RowsRead, result.Inserted, result.Updated, result.Skipped, result.AppUsersUpserted);
            }
            finally
            {
                try { File.Delete(tmp); } catch { /* best-effort */ }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DevAutoImport : échec de l'import.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
