using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Interface pour le service de gestion des barrières de sécurité.
/// </summary>
public interface IBarriereService
{
    /// <summary>
    /// Récupère toutes les barrières actives
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste de toutes les barrières actives</returns>
    Task<IReadOnlyList<Barriere>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une barrière par son identifiant
    /// </summary>
    /// <param name="id">Identifiant de la barrière</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>La barrière trouvée ou null</returns>
    Task<Barriere?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère une barrière par son code
    /// </summary>
    /// <param name="code">Code de la barrière</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>La barrière trouvée ou null</returns>
    Task<Barriere?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les barrières par type (Entrée, Sortie, Mixte)
    /// </summary>
    /// <param name="type">Type de barrière</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des barrières du type spécifié</returns>
    Task<IReadOnlyList<Barriere>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recherche des barrières par localisation
    /// </summary>
    /// <param name="localisation">Terme de recherche pour la localisation</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des barrières correspondantes</returns>
    Task<IReadOnlyList<Barriere>> SearchByLocalisationAsync(string localisation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée une nouvelle barrière
    /// </summary>
    /// <param name="barriere">Données de la barrière</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>La barrière créée</returns>
    Task<Barriere> CreateAsync(Barriere barriere, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour une barrière existante
    /// </summary>
    /// <param name="barriere">Données de la barrière</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>La barrière mise à jour</returns>
    Task<Barriere> UpdateAsync(Barriere barriere, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalide le cache des barrières
    /// </summary>
    void InvalidateCache();
}
