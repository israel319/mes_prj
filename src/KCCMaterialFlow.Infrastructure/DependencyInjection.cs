using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Infrastructure.Repositories;
using KCCMaterialFlow.Infrastructure.Services;
using KCCMaterialFlow.Infrastructure.Services.Shared;
using KCCMaterialFlow.Infrastructure.Services.Securite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using InfraBonEntree = KCCMaterialFlow.Infrastructure.Services.BonEntree;
using InfraBonSortie = KCCMaterialFlow.Infrastructure.Services.BonSortie;

namespace KCCMaterialFlow.Infrastructure;

/// <summary>
/// Extension method pour l'enregistrement DI de la couche Infrastructure.
/// Appelé depuis Program.cs : services.AddInfrastructure(configuration);
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // ── DbContext Factory + scoped DbContext ──────────────────────────
        services.AddDbContextFactory<KCCMaterialFlowDbContext>((sp, optionsBuilder) =>
        {
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(30);
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        });

        services.AddScoped(sp =>
            sp.GetRequiredService<IDbContextFactory<KCCMaterialFlowDbContext>>().CreateDbContext());

        // IApplicationDbContext → KCCMaterialFlowDbContext (utilisé par tous les CQRS handlers)
        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<KCCMaterialFlowDbContext>());

        // ── Repositories ─────────────────────────────────────────────────
        services.AddScoped<IBonEntreeRepository, BonEntreeRepository>();
        services.AddScoped<IBonSortieRepository, BonSortieRepository>();
        services.AddScoped<IScanRepository, ScanRepository>();
        services.AddScoped<IAnomalieRepository, AnomalieRepository>();

        // ── Services Core ────────────────────────────────────────────────
        // Note: ICurrentUserService et IIdentityProvider sont enregistrés
        // dans Program.cs car ils nécessitent le décorateur DatabaseRoleEnricherService
        // et la vérification OperatingSystem.IsWindows() (CA1416).
        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<EmailNotificationService>();
        services.AddScoped<IEmailNotificationService>(sp => sp.GetRequiredService<EmailNotificationService>());
        services.AddScoped<IEmailService>(sp => sp.GetRequiredService<EmailNotificationService>());
        services.AddScoped<INotificationRejetService, NotificationRejetService>();
        services.AddScoped<IReferenceDataService, ReferenceDataService>();
        services.AddScoped<IQRCodeService, QRCodeService>();

        // ── Services Shared ─────────────────────────────────────────────
        services.AddScoped<IBarriereService, BarriereService>();
        services.AddScoped<IStatutService, StatutService>();
        services.AddScoped<ITypeMaterielService, TypeMaterielService>();
        services.AddScoped<IParametreSystemeService, ParametreSystemeService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<ICategorieSortieService, CategorieSortieService>();
        services.AddScoped<IRaisonEntreeService, RaisonEntreeService>();
        services.AddScoped<IWorkflowConfigService, WorkflowConfigService>();
        services.AddScoped<IDataImportService, Services.Shared.DataImportService>();
        services.AddScoped<IAllEmployeeImportService, Services.Shared.AllEmployeeImportService>();
        services.AddScoped<IWorkflowApprobateurSpecialService, WorkflowApprobateurSpecialService>();
        services.AddScoped<IChaineApprobationService, ChaineApprobationService>();
        services.AddScoped<IAllEmployeeSearchService, Services.Shared.AllEmployeeSearchService>();
        services.AddScoped<IMaterielCatalogService, Services.Shared.MaterielCatalogService>();

        // ── Services BonEntree ───────────────────────────────────────────
        services.AddScoped<IBonEntreeService, InfraBonEntree.BonEntreeService>();
        services.AddScoped<IBonEntreeLockService, InfraBonEntree.BonEntreeLockService>();

        // ── Services BonSortie ───────────────────────────────────────────
        services.AddScoped<IBonSortieService, InfraBonSortie.BonSortieService>();
        services.AddHostedService<InfraBonSortie.PretExpirationAlertService>();
        services.AddHostedService<InfraBonSortie.PretInvestigationService>();

        // ── Services Securite ────────────────────────────────────────────
        services.AddScoped<IScanService, ScanService>();
        services.AddScoped<IAnomalieService, AnomalieService>();
        services.AddSingleton<IEmailTemplateRenderer, InlineEmailTemplateRenderer>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<ISecuriteEmailService, SecuriteEmailService>();
        services.AddHostedService<PretExpirantAlertService>();

        // ── Options ──────────────────────────────────────────────────────
        services.Configure<SecuriteEmailOptions>(
            configuration.GetSection(SecuriteEmailOptions.SectionName));
        services.Configure<SmtpOptions>(
            configuration.GetSection(SmtpOptions.SectionName));

        return services;
    }
}
