using System.Security.Principal;

namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// Abstraction pour accéder à l'identité de l'utilisateur courant sans dépendance Web.
/// Permet de découpler la couche Core de Microsoft.AspNetCore.Http.
/// </summary>
public interface IIdentityProvider
{
    /// <summary>
    /// Obtient l'identité Windows de l'utilisateur courant.
    /// </summary>
    /// <returns>WindowsIdentity ou null si non authentifié</returns>
    WindowsIdentity? GetCurrentWindowsIdentity();

    /// <summary>
    /// Obtient le nom (login) de l'utilisateur courant.
    /// </summary>
    /// <returns>Login au format DOMAIN\username ou null</returns>
    string? GetCurrentUserName();

    /// <summary>
    /// Vérifie si l'utilisateur courant est authentifié.
    /// </summary>
    bool IsAuthenticated { get; }
}
