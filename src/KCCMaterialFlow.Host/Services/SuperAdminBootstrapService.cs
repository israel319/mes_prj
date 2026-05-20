using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Au démarrage, parcourt la liste <c>InitialSuperAdminMatricules</c> dans la configuration
/// et positionne <see cref="AppUser.NiveauAdmin"/> = SuperAdmin
/// pour les AppUsers correspondants (liés via Employee.Matricule). Idempotent.
/// </summary>
public sealed class SuperAdminBootstrapService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SuperAdminBootstrapService> _logger;

    public SuperAdminBootstrapService(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger<SuperAdminBootstrapService> logger)
    {
        _services = services;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var matricules = _configuration.GetSection("InitialSuperAdminMatricules").Get<string[]>()
                         ?? Array.Empty<string>();

        if (matricules.Length == 0)
        {
            _logger.LogInformation("Aucun matricule SuperAdmin configuré (InitialSuperAdminMatricules).");
            return;
        }

        try
        {
            using var scope = _services.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<KCCMaterialFlowDbContext>>();
            await using var ctx = await dbFactory.CreateDbContextAsync(cancellationToken);

            var normalized = matricules
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(m => m.Trim().ToUpperInvariant())
                .Distinct()
                .ToList();

            var employees = await ctx.Employees
                .Where(e => e.Matricule != null && normalized.Contains(e.Matricule.ToUpper()))
                .ToListAsync(cancellationToken);

            var employeeIds = employees.Select(e => e.Id).ToList();
            var appUsers = await ctx.AppUsers
                .Where(u => u.EmployeeId.HasValue && employeeIds.Contains(u.EmployeeId!.Value))
                .ToListAsync(cancellationToken);

            int promoted = 0;
            foreach (var emp in employees)
            {
                var appUser = appUsers.FirstOrDefault(u => u.EmployeeId == emp.Id);
                if (appUser != null)
                {
                    if (appUser.NiveauAdmin != NiveauAdmin.SuperAdmin)
                    {
                        appUser.NiveauAdmin = NiveauAdmin.SuperAdmin;
                        promoted++;
                    }
                }
                else
                {
                    _logger.LogWarning(
                        "Bootstrap SuperAdmin : pas d'AppUser pour le matricule {M} (Employee.Id={Id}) — connexion requise pour créer AppUser.",
                        emp.Matricule, emp.Id);
                }
            }

            if (promoted > 0)
            {
                await ctx.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Bootstrap SuperAdmin : {Count} Employee(s) promu(s) à SuperAdmin.", promoted);
            }

            var missing = normalized.Except(employees.Select(e => e.Matricule!.ToUpper())).ToList();
            if (missing.Count > 0)
            {
                _logger.LogWarning("Bootstrap SuperAdmin : matricule(s) introuvable(s) en BD : {Matricules}",
                    string.Join(", ", missing));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bootstrap SuperAdmin : échec de l'initialisation.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
