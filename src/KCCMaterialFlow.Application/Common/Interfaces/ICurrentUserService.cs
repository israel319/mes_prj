using System.Security.Principal;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Identité de l'utilisateur courant.
/// Implémenté par Infrastructure/Identity/CurrentUserService + Host/DatabaseRoleEnricherService (décorateur).
/// </summary>
public interface ICurrentUserService
{
    CurrentUserInfo? GetCurrentUser();
    string GetUserLogin();
    string GetUserDisplayName();
    string? GetUserEmail();
    string? GetUserDepartment();
    bool IsInRole(string role);
    bool IsInAnyRole(params string[] roles);
    IEnumerable<string> GetUserRoles();
    bool IsAuthenticated();
    bool HasActivite(string codeActivite);
    bool HasAnyActivite(params string[] codeActivites);
    void SetSimulatedUser(string login, string displayName, string? email, string? department, IEnumerable<string> roles);
    void ClearSimulation();
    bool IsSimulationActive { get; }
}

/// <summary>
/// Informations sur l'utilisateur courant.
/// </summary>
public class CurrentUserInfo
{
    public int UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Function { get; set; }
    public string? Department { get; set; }
    public int? DepartmentId { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Abstraction pour accéder à l'identité Windows sans dépendance Web.
/// </summary>
public interface IIdentityProvider
{
    WindowsIdentity? GetCurrentWindowsIdentity();
    string? GetCurrentUserName();
    bool IsAuthenticated { get; }
}
