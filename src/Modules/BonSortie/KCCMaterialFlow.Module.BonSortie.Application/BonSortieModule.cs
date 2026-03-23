using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.BonSortie.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KCCMaterialFlow.Module.BonSortie;

/// <summary>
/// Module de gestion des Bons de Sortie Matériel (BSM).
/// Gère le processus de sortie de matériels du site avec workflow d'approbation.
/// Supporte les sorties externes (vers l'extérieur) et internes (entre départements).
/// </summary>
public class BonSortieModule : IModule
{
    /// <inheritdoc />
    public string ModuleId => "BSM";

    /// <inheritdoc />
    public string ModuleName => "Bon de Sortie Matériel";

    /// <inheritdoc />
    public string RoutePrefix => "/bon-sortie";

    /// <inheritdoc />
    public string IconClass => "output";

    /// <inheritdoc />
    public int DisplayOrder => 2;

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        // Note: IBonSortieRepository est enregistré par Infrastructure (Clean Architecture)
        // Enregistrement du service
        services.AddScoped<IBonSortieService, BonSortieService>();

        // BSM-064: Service de background pour les alertes d'expiration de prêts (J-7)
        services.AddHostedService<PretExpirationAlertService>();

        // BSM-065: Service de background pour investigation des prêts expirés
        services.AddHostedService<PretInvestigationService>();
    }

    /// <inheritdoc />
    public IEnumerable<NavMenuItem> GetNavMenuItems()
    {
        return
        [
            new NavMenuItem
            {
                Id = "bsm-list",
                Label = "Bons de Sortie",
                Icon = "output",
                Href = "/bon-sortie",
                Order = 1,
                AllowedRoles = [] // Accessible à tous les utilisateurs authentifiés
            },
            new NavMenuItem
            {
                Id = "bsm-new-externe",
                Label = "Nouvelle Sortie Externe",
                Icon = "logout",
                Href = "/bon-sortie/externe/nouveau",
                Order = 2,
                AllowedRoles = []
            },
            new NavMenuItem
            {
                Id = "bsm-new-interne",
                Label = "Nouvelle Sortie Interne",
                Icon = "swap_horiz",
                Href = "/bon-sortie/interne/nouveau",
                Order = 3,
                AllowedRoles = []
            },
            new NavMenuItem
            {
                Id = "bsm-my",
                Label = "Mes Demandes",
                Icon = "list_alt",
                Href = "/bon-sortie/mes-demandes",
                Order = 4,
                AllowedRoles = []
            },
            new NavMenuItem
            {
                Id = "bsm-prets",
                Label = "Prêts en cours",
                Icon = "schedule",
                Href = "/bon-sortie/prets",
                Order = 5,
                AllowedRoles = []
            },
            new NavMenuItem
            {
                Id = "bsm-approve",
                Label = "À Approuver",
                Icon = "approval",
                Href = "/bon-sortie/approbations",
                Order = 6,
                AllowedRoles = ["Approbateur", "Superviseur", "Admin"]
            }
        ];
    }
}
