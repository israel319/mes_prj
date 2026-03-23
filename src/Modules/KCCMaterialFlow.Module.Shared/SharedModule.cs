using KCCMaterialFlow.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace KCCMaterialFlow.Module.Shared;

/// <summary>
/// Module partagé contenant les entités et services communs à tous les modules.
/// Ce module ne fournit pas de pages ou menus, mais expose les entités partagées
/// (Utilisateur, Departement, Barriere) utilisées par les autres modules.
/// NOTE: Les implémentations des services sont dans Infrastructure/Services/Shared/ (Clean Architecture).
/// L'enregistrement DI se fait dans Program.cs (composition root).
/// </summary>
public class SharedModule : IModule
{
    /// <inheritdoc />
    public string ModuleId => "SHR";

    /// <inheritdoc />
    public string ModuleName => "Module Partagé";

    /// <inheritdoc />
    public string RoutePrefix => "/shared";

    /// <inheritdoc />
    public string IconClass => "person_outline";

    /// <inheritdoc />
    public int DisplayOrder => 0;

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        // Les services partagés sont enregistrés dans Program.cs (composition root)
        // car leurs implémentations résident dans la couche Infrastructure.
    }

    /// <inheritdoc />
    public IEnumerable<NavMenuItem> GetNavMenuItems()
    {
        // Le module partagé n'a pas d'éléments de menu
        // Ses entités sont utilisées par les autres modules
        return [];
    }
}
