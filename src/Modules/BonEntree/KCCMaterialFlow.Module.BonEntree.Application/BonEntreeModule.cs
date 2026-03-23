using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.BonEntree.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KCCMaterialFlow.Module.BonEntree;

/// <summary>
/// Module de gestion des Bons d'Entrée Matériel (BEM).
/// Gère le processus d'entrée de matériels sur le site avec workflow d'approbation.
/// </summary>
public class BonEntreeModule : IModule
{
    /// <inheritdoc />
    public string ModuleId => "BEM";

    /// <inheritdoc />
    public string ModuleName => "Bon d'Entrée Matériel";

    /// <inheritdoc />
    public string RoutePrefix => "/bon-entree";

    /// <inheritdoc />
    public string IconClass => "input";

    /// <inheritdoc />
    public int DisplayOrder => 1;

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        // Note: IBonEntreeRepository est enregistré par Infrastructure (Clean Architecture)
        // Enregistrement du service
        services.AddScoped<IBonEntreeService, BonEntreeService>();

        // BSM-031: Service de verrouillage pour la liaison Entrée-Sortie
        services.AddScoped<IBonEntreeLockService, BonEntreeLockService>();
    }

    /// <inheritdoc />
    public IEnumerable<NavMenuItem> GetNavMenuItems()
    {
        return
        [
            new NavMenuItem
            {
                Id = "bem-list",
                Label = "Bons d'Entrée",
                Icon = "input",
                Href = "/bon-entree",
                Order = 1,
                AllowedRoles = [] // Accessible à tous les utilisateurs authentifiés
            },
            new NavMenuItem
            {
                Id = "bem-new",
                Label = "Nouveau Bon",
                Icon = "add_circle",
                Href = "/bon-entree/nouveau",
                Order = 2,
                AllowedRoles = []
            },
            new NavMenuItem
            {
                Id = "bem-my",
                Label = "Mes Demandes",
                Icon = "list_alt",
                Href = "/bon-entree/mes-demandes",
                Order = 3,
                AllowedRoles = []
            },
            new NavMenuItem
            {
                Id = "bem-approve",
                Label = "À Approuver",
                Icon = "approval",
                Href = "/bon-entree/approbations",
                Order = 4,
                AllowedRoles = ["Approbateur", "Superviseur", "Admin"]
            }
        ];
    }
}
