using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.Securite.Notifications;
using KCCMaterialFlow.Module.Securite.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KCCMaterialFlow.Module.Securite;

/// <summary>
/// Module de sécurité : gestion des scans QR, anomalies, investigations.
/// Utilisé par les agents de sécurité aux barrières pour contrôler les entrées/sorties.
/// </summary>
public class SecuriteModule : IModule
{
    /// <inheritdoc />
    public string ModuleId => "SEC";

    /// <inheritdoc />
    public string ModuleName => "Sécurité";

    /// <inheritdoc />
    public string RoutePrefix => "/securite";

    /// <inheritdoc />
    public string IconClass => "security";

    /// <inheritdoc />
    public int DisplayOrder => 4;

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        // Note: IScanRepository et IAnomalieRepository sont enregistrés par Infrastructure (Clean Architecture)

        // Services
        services.AddScoped<IScanService, ScanService>();
        services.AddScoped<IAnomalieService, AnomalieService>();

        // SEC-041 à SEC-045: Services d'email/notification
        services.AddSingleton<IEmailTemplateRenderer, InlineEmailTemplateRenderer>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<ISecuriteEmailService, SecuriteEmailService>();

        // Configuration des options email
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        if (configuration != null)
        {
            services.Configure<SecuriteEmailOptions>(configuration.GetSection(SecuriteEmailOptions.SectionName));
            services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        }
    }

    /// <inheritdoc />
    public IEnumerable<NavMenuItem> GetNavMenuItems()
    {
        return
        [
            new NavMenuItem
            {
                Id = "sec-scan",
                Label = "Scanner QR",
                Icon = "qr_code_scanner",
                Href = "/securite/scan",
                Order = 1,
                AllowedRoles = ["Security", "Admin"]
            },
            new NavMenuItem
            {
                Id = "sec-historique",
                Label = "Historique Scans",
                Icon = "history",
                Href = "/securite/historique",
                Order = 2,
                AllowedRoles = ["Security", "Admin", "Supervisor"]
            },
            new NavMenuItem
            {
                Id = "sec-anomalies",
                Label = "Anomalies",
                Icon = "warning",
                Href = "/securite/anomalies",
                Order = 3,
                AllowedRoles = ["Security", "Admin", "Supervisor"]
            },
            new NavMenuItem
            {
                Id = "sec-identification",
                Label = "Identification",
                Icon = "badge",
                Href = "/securite/identification",
                Order = 4,
                AllowedRoles = ["Security", "Admin"]
            }
        ];
    }
}
