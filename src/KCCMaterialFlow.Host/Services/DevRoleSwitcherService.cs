namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Service pour simuler différents rôles en mode développement.
/// Permet aux développeurs de tester l'interface avec différents niveaux d'accès.
/// ATTENTION: Ce service ne doit être utilisé qu'en mode DEBUG !
/// </summary>
public class DevRoleSwitcherService
{
#if DEBUG
    // En dev, activer par défaut avec le rôle Admin
    private readonly List<string> _simulatedRoles = new() { "Admin" };
    private bool _isSimulationActive = true;
#else
    private readonly List<string> _simulatedRoles = new();
    private bool _isSimulationActive = false;
#endif
    private string? _simulatedUserLogin;
    private string? _simulatedUserName;

    /// <summary>
    /// Liste des rôles disponibles pour simulation
    /// </summary>
    public static readonly Dictionary<string, RoleInfo> AvailableRoles = new()
    {
        { "Admin", new RoleInfo("Admin", "Administrateur", "settings", "Accès complet au système") },
        { "Superviseur", new RoleInfo("Superviseur", "Superviseur", "supervisor_account", "Supervision et approbation") },
        { "GM", new RoleInfo("GM", "General Manager", "business", "Approbation haute direction") },
        { "IT", new RoleInfo("IT", "Informatique", "computer", "Support technique") },
        { "Investigation", new RoleInfo("Investigation", "Investigation", "search", "Enquêtes et anomalies") },
        { "Security", new RoleInfo("Security", "Sécurité", "security", "Contrôle aux barrières") },
        { "Environnement", new RoleInfo("Environnement", "Environnement", "eco", "Gestion environnementale") },
        { "OPJ", new RoleInfo("OPJ", "OPJ", "gavel", "Officier de police judiciaire") },
        { "Identification", new RoleInfo("Identification", "Identification", "qr_code_2", "Validation finale, QR code et impression") },
        { "Demandeur", new RoleInfo("Demandeur", "Demandeur", "person", "Utilisateur standard") }
    };

    /// <summary>
    /// Indique si la simulation de rôle est active
    /// </summary>
    public bool IsSimulationActive => _isSimulationActive;

    /// <summary>
    /// Rôles actuellement simulés
    /// </summary>
    public IReadOnlyList<string> SimulatedRoles => _simulatedRoles.AsReadOnly();

    /// <summary>
    /// Login de l'utilisateur simulé (si simulation par utilisateur)
    /// </summary>
    public string? SimulatedUserLogin => _simulatedUserLogin;

    /// <summary>
    /// Nom de l'utilisateur simulé (si simulation par utilisateur)
    /// </summary>
    public string? SimulatedUserName => _simulatedUserName;

    /// <summary>
    /// Simule un utilisateur spécifique avec ses rôles
    /// </summary>
    public void SetSimulatedUser(string login, string displayName, IEnumerable<string> roles)
    {
        _simulatedUserLogin = login;
        _simulatedUserName = displayName;
        _simulatedRoles.Clear();
        _simulatedRoles.AddRange(roles);
        _isSimulationActive = true;
    }

    /// <summary>
    /// Active la simulation avec les rôles spécifiés
    /// </summary>
    public void SetSimulatedRoles(IEnumerable<string> roles)
    {
        _simulatedUserLogin = null;
        _simulatedUserName = null;
        _simulatedRoles.Clear();
        _simulatedRoles.AddRange(roles);
        _isSimulationActive = _simulatedRoles.Count > 0;
    }

    /// <summary>
    /// Active un seul rôle (remplace les rôles existants)
    /// </summary>
    public void SetSingleRole(string role)
    {
        _simulatedUserLogin = null;
        _simulatedUserName = null;
        _simulatedRoles.Clear();
        if (!string.IsNullOrEmpty(role))
        {
            _simulatedRoles.Add(role);
            _isSimulationActive = true;
        }
        else
        {
            _isSimulationActive = false;
        }
    }

    /// <summary>
    /// Ajoute un rôle à la simulation
    /// </summary>
    public void AddRole(string role)
    {
        if (!string.IsNullOrEmpty(role) && !_simulatedRoles.Contains(role))
        {
            _simulatedRoles.Add(role);
            _isSimulationActive = true;
        }
    }

    /// <summary>
    /// Retire un rôle de la simulation
    /// </summary>
    public void RemoveRole(string role)
    {
        _simulatedRoles.Remove(role);
        _isSimulationActive = _simulatedRoles.Count > 0;
    }

    /// <summary>
    /// Désactive la simulation (retour aux vrais rôles)
    /// </summary>
    public void ClearSimulation()
    {
        _simulatedRoles.Clear();
        _simulatedUserLogin = null;
        _simulatedUserName = null;
        _isSimulationActive = false;
    }

    /// <summary>
    /// Vérifie si un rôle est simulé
    /// </summary>
    public bool HasRole(string role)
    {
        return _isSimulationActive && _simulatedRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Informations sur un rôle
/// </summary>
public record RoleInfo(string Code, string Name, string Icon, string Description);
