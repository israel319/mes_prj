namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// Interface pour accéder aux informations de l'utilisateur courant (Windows Authentication)
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Récupère les informations complètes de l'utilisateur courant
    /// </summary>
    /// <returns>Informations de l'utilisateur ou null si non authentifié</returns>
    CurrentUserInfo? GetCurrentUser();

    /// <summary>
    /// Récupère le login de l'utilisateur courant (format: DOMAIN\username ou username)
    /// </summary>
    /// <returns>Login de l'utilisateur</returns>
    string GetUserLogin();

    /// <summary>
    /// Récupère le nom d'affichage de l'utilisateur courant
    /// </summary>
    /// <returns>Nom complet de l'utilisateur</returns>
    string GetUserDisplayName();

    /// <summary>
    /// Récupère l'adresse email de l'utilisateur courant
    /// </summary>
    /// <returns>Email de l'utilisateur</returns>
    string? GetUserEmail();

    /// <summary>
    /// Récupère le département de l'utilisateur courant
    /// </summary>
    /// <returns>Nom du département</returns>
    string? GetUserDepartment();

    /// <summary>
    /// Vérifie si l'utilisateur courant possède un rôle spécifique
    /// </summary>
    /// <param name="role">Code du rôle à vérifier</param>
    /// <returns>True si l'utilisateur possède le rôle</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Vérifie si l'utilisateur courant possède au moins un des rôles spécifiés
    /// </summary>
    /// <param name="roles">Liste des codes de rôles</param>
    /// <returns>True si l'utilisateur possède au moins un rôle</returns>
    bool IsInAnyRole(params string[] roles);

    /// <summary>
    /// Récupère tous les rôles de l'utilisateur courant
    /// </summary>
    /// <returns>Liste des codes de rôles</returns>
    IEnumerable<string> GetUserRoles();

    /// <summary>
    /// Vérifie si l'utilisateur est authentifié
    /// </summary>
    /// <returns>True si authentifié</returns>
    bool IsAuthenticated();

    /// <summary>
    /// Vérifie si l'utilisateur courant possède une activité spécifique
    /// </summary>
    /// <param name="codeActivite">Code de l'activité (ex: BEM_CREER, BSM_APPROUVER)</param>
    /// <returns>True si l'utilisateur possède cette activité</returns>
    bool HasActivite(string codeActivite);

    /// <summary>
    /// Vérifie si l'utilisateur courant possède au moins une des activités spécifiées
    /// </summary>
    /// <param name="codeActivites">Codes des activités à vérifier</param>
    /// <returns>True si l'utilisateur possède au moins une activité</returns>
    bool HasAnyActivite(params string[] codeActivites);

    /// <summary>
    /// Active la simulation d'un utilisateur spécifique (pour les administrateurs)
    /// </summary>
    void SetSimulatedUser(string login, string displayName, string? email, string? department, IEnumerable<string> roles);

    /// <summary>
    /// Désactive la simulation d'utilisateur
    /// </summary>
    void ClearSimulation();

    /// <summary>
    /// Indique si la simulation est active
    /// </summary>
    bool IsSimulationActive { get; }
}

/// <summary>
/// Informations sur l'utilisateur courant
/// </summary>
public class CurrentUserInfo
{
    /// <summary>
    /// Identifiant unique de l'utilisateur
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Login de l'utilisateur (Windows)
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet de l'utilisateur
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Adresse email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Fonction/Titre de l'utilisateur
    /// </summary>
    public string? Function { get; set; }

    /// <summary>
    /// Nom du département
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Identifiant du département
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Liste des rôles de l'utilisateur
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Indique si l'utilisateur est actif
    /// </summary>
    public bool IsActive { get; set; } = true;
}
