using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des utilisateurs.
/// Fournit des méthodes pour rechercher et récupérer les utilisateurs du système.
/// </summary>
public interface IUtilisateurService
{
    /// <summary>
    /// Récupère un utilisateur par son login Windows
    /// </summary>
    /// <param name="login">Login Windows (ex: DOMAIN\username)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>L'utilisateur trouvé ou null</returns>
    Task<Utilisateur?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les utilisateurs d'un département
    /// </summary>
    /// <param name="departement">Nom du département</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des utilisateurs du département</returns>
    Task<IReadOnlyList<Utilisateur>> GetByDepartementAsync(string departement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recherche des utilisateurs par nom, login ou fonction
    /// </summary>
    /// <param name="searchTerm">Terme de recherche</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des utilisateurs correspondants</returns>
    Task<IReadOnlyList<Utilisateur>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un utilisateur par son identifiant
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>L'utilisateur trouvé ou null</returns>
    Task<Utilisateur?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les utilisateurs actifs
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des utilisateurs actifs</returns>
    Task<IReadOnlyList<Utilisateur>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les utilisateurs (actifs et inactifs)
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste de tous les utilisateurs</returns>
    Task<IReadOnlyList<Utilisateur>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les utilisateurs ayant un rôle spécifique
    /// </summary>
    /// <param name="role">Rôle recherché</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des utilisateurs avec ce rôle</returns>
    Task<IReadOnlyList<Utilisateur>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée ou met à jour un utilisateur (synchronisation depuis AD)
    /// </summary>
    /// <param name="utilisateur">Données de l'utilisateur</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>L'utilisateur créé ou mis à jour</returns>
    Task<Utilisateur> UpsertAsync(Utilisateur utilisateur, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour la date de dernière connexion d'un utilisateur
    /// </summary>
    /// <param name="login">Login de l'utilisateur</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task UpdateLastLoginAsync(string login, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalide le cache des utilisateurs
    /// </summary>
    void InvalidateCache();
}
