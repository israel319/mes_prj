using System.Linq.Expressions;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Interface générique pour le pattern Repository
/// </summary>
/// <typeparam name="T">Type de l'entité</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Récupère une entité par son identifiant
    /// </summary>
    /// <param name="id">Identifiant de l'entité</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>L'entité ou null si non trouvée</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une entité par son identifiant avec les includes spécifiés
    /// </summary>
    /// <param name="id">Identifiant de l'entité</param>
    /// <param name="includes">Expressions d'inclusion pour le chargement eager</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>L'entité avec ses relations ou null</returns>
    Task<T?> GetByIdAsync(int id, Expression<Func<T, object>>[] includes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère toutes les entités
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste de toutes les entités</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Recherche des entités selon un prédicat
    /// </summary>
    /// <param name="predicate">Condition de filtrage</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des entités correspondantes</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ajoute une nouvelle entité
    /// </summary>
    /// <param name="entity">Entité à ajouter</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>L'entité ajoutée avec son ID généré</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une entité existante
    /// </summary>
    /// <param name="entity">Entité à mettre à jour</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>L'entité mise à jour</returns>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime une entité
    /// </summary>
    /// <param name="entity">Entité à supprimer</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime une entité par son identifiant
    /// </summary>
    /// <param name="id">Identifiant de l'entité à supprimer</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne un IQueryable pour des requêtes personnalisées
    /// </summary>
    /// <returns>IQueryable de l'entité</returns>
    IQueryable<T> GetQueryable();

    /// <summary>
    /// Vérifie si une entité existe selon un prédicat
    /// </summary>
    /// <param name="predicate">Condition de vérification</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>True si au moins une entité correspond</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compte le nombre d'entités selon un prédicat optionnel
    /// </summary>
    /// <param name="predicate">Condition de filtrage (optionnel)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Nombre d'entités</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
