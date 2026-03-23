using Microsoft.Extensions.DependencyInjection;

namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// Interface définissant un module de l'application KCCMaterialFlow.
/// Chaque module (BonEntree, BonSortie, Securite) doit implémenter cette interface.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Identifiant unique du module (ex: "BEM", "BSM", "SEC")
    /// </summary>
    string ModuleId { get; }

    /// <summary>
    /// Nom d'affichage du module (ex: "Bon d'Entrée Matériel")
    /// </summary>
    string ModuleName { get; }

    /// <summary>
    /// Préfixe de route pour les pages du module (ex: "/bon-entree")
    /// </summary>
    string RoutePrefix { get; }

    /// <summary>
    /// Classe d'icône pour le menu de navigation (ex: "fa fa-sign-in")
    /// </summary>
    string IconClass { get; }

    /// <summary>
    /// Ordre d'affichage dans le menu de navigation
    /// </summary>
    int DisplayOrder { get; }

    /// <summary>
    /// Configure les services du module dans le conteneur d'injection de dépendances
    /// </summary>
    /// <param name="services">Collection de services</param>
    void ConfigureServices(IServiceCollection services);

    /// <summary>
    /// Retourne les éléments de menu de navigation du module
    /// </summary>
    /// <returns>Liste des éléments de menu</returns>
    IEnumerable<NavMenuItem> GetNavMenuItems();
}
