using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des activités utilisateur.
/// Permet d'assigner, retirer et vérifier les activités autorisées pour chaque utilisateur.
/// </summary>
public interface IActiviteService
{
    // ===== ACTIVITÉS =====

    /// <summary>
    /// Récupère toutes les activités actives du système
    /// </summary>
    Task<IReadOnlyList<Activite>> GetAllActivitesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une activité par son identifiant
    /// </summary>
    Task<Activite?> GetActiviteByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une activité par son code
    /// </summary>
    Task<Activite?> GetActiviteByCodeAsync(string codeActivite, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les activités groupées par module
    /// </summary>
    Task<Dictionary<string, List<Activite>>> GetActivitesGroupedByModuleAsync(CancellationToken cancellationToken = default);

    // ===== ACTIVITÉS UTILISATEUR =====

    /// <summary>
    /// Récupère toutes les activités assignées à un utilisateur
    /// </summary>
    Task<IReadOnlyList<Activite>> GetActivitesForUserAsync(int idUtilisateur, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les IDs des activités assignées à un utilisateur
    /// </summary>
    Task<IReadOnlyList<int>> GetActiviteIdsForUserAsync(int idUtilisateur, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour toutes les activités d'un utilisateur (remplace la liste complète)
    /// </summary>
    Task UpdateUserActivitesAsync(int idUtilisateur, IEnumerable<int> activiteIds, string? attribueParLogin = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigne une activité à un utilisateur
    /// </summary>
    Task<bool> AssignActiviteToUserAsync(int idUtilisateur, int idActivite, string? attribueParLogin = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retire une activité d'un utilisateur
    /// </summary>
    Task<bool> RemoveActiviteFromUserAsync(int idUtilisateur, int idActivite, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un utilisateur a une activité spécifique
    /// </summary>
    Task<bool> UserHasActiviteAsync(int idUtilisateur, string codeActivite, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si un utilisateur a au moins une des activités spécifiées
    /// </summary>
    Task<bool> UserHasAnyActiviteAsync(int idUtilisateur, IEnumerable<string> codeActivites, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalide le cache des activités d'un utilisateur
    /// </summary>
    void InvalidateUserCache(int idUtilisateur);
}
