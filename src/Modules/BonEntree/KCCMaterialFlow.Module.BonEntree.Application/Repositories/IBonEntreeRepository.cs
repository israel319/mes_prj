using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.BonEntree.Entities;

namespace KCCMaterialFlow.Module.BonEntree.Repositories;

/// <summary>
/// Interface du repository pour les Bons d'Entrée.
/// Simplifié selon le diagramme de classe.
/// </summary>
public interface IBonEntreeRepository
{
    #region CRUD Operations

    Task<Entities.BonEntree?> GetByIdAsync(
        int id,
        bool includeMateriels = true,
        bool includeApprobations = false,
        CancellationToken cancellationToken = default);

    Task<Entities.BonEntree?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default);

    Task<Entities.BonEntree> CreateAsync(Entities.BonEntree bonEntree, CancellationToken cancellationToken = default);

    Task<Entities.BonEntree> UpdateAsync(Entities.BonEntree bonEntree, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, string motif, CancellationToken cancellationToken = default);

    #endregion

    #region Search & Filter

    Task<BonEntreeSearchResult> SearchAsync(BonEntreeFilter filter, CancellationToken cancellationToken = default);

    Task<BonEntreeSearchResult> GetByCreateurAsync(string login, int skip = 0, int take = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les bons en attente d'approbation pour les rôles spécifiés.
    /// </summary>
    /// <param name="userRoles">Les rôles de l'utilisateur (Superviseur, GM, OPJ, Admin)</param>
    Task<IReadOnlyList<Entities.BonEntree>> GetPendingApprovalsAsync(IEnumerable<string> userRoles, CancellationToken cancellationToken = default);

    #endregion

    #region Statistics

    Task<Dictionary<string, int>> GetCountByStatutAsync(CancellationToken cancellationToken = default);

    Task<int> GetTodayCountAsync(CancellationToken cancellationToken = default);

    Task<string> GenerateNextNumeroReferenceAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Related Entities

    Task<Materiel> AddMaterielAsync(int bonId, Materiel materiel, CancellationToken cancellationToken = default);

    Task<Materiel> UpdateMaterielAsync(Materiel materiel, CancellationToken cancellationToken = default);

    Task DeleteMaterielAsync(int materielId, CancellationToken cancellationToken = default);

    Task<Approbation> UpsertApprobationAsync(Approbation approbation, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Approbation>> GetApprobationsAsync(int bonId, CancellationToken cancellationToken = default);

    #endregion

    #region Historique

    /// <summary>
    /// Ajoute une entrée d'historique pour un bon d'entrée
    /// </summary>
    Task AddHistoryAsync(BonEntreeHistory history, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère l'historique d'un bon d'entrée
    /// </summary>
    Task<IReadOnlyList<BonEntreeHistory>> GetHistoryAsync(int bonId, CancellationToken cancellationToken = default);

    #endregion

    #region Liaison Entrée-Sortie (BSM-031)

    /// <summary>
    /// BSM-031: Verrouille un bon d'entrée pour un bon de sortie approuvé
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée à verrouiller</param>
    /// <param name="bonSortieId">ID du bon de sortie associé</param>
    /// <param name="bonSortieNumero">Numéro de référence du bon de sortie</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task LockForSortieAsync(int bonEntreeId, int bonSortieId, string bonSortieNumero, CancellationToken cancellationToken = default);

    /// <summary>
    /// BSM-031: Déverrouille un bon d'entrée (si le bon de sortie est annulé/rejeté)
    /// </summary>
    Task UnlockFromSortieAsync(int bonEntreeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// BSM-031: Vérifie si un bon d'entrée est disponible pour une nouvelle sortie
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée</param>
    /// <returns>True si disponible, False si verrouillé</returns>
    Task<bool> IsAvailableForSortieAsync(int bonEntreeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// INT-006: Archive un bon d'entrée après complétion du bon de sortie associé
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée à archiver</param>
    /// <param name="motif">Motif d'archivage</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task ArchiveAsync(int bonEntreeId, string motif, CancellationToken cancellationToken = default);

    #endregion

    #region Stock & Disponibilité (BSM-031)

    /// <summary>
    /// Décrémente la quantité disponible des matériels lors de l'approbation finale d'un BSM.
    /// </summary>
    Task<StockUpdateResult> DecrementMaterielStockAsync(IEnumerable<MaterielStockDecrement> materielsASortir, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les bons d'entrée disponibles pour sortie (approuvés, non expirés, avec quantité > 0).
    /// Permet un filtre optionnel par département hôte.
    /// </summary>
    Task<IReadOnlyList<BonEntreeForDropdown>> GetAllAvailableForSortieAsync(string? hostDepartmentFilter = null, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// Résultat de recherche paginé pour les bons d'entrée.
/// </summary>
public class BonEntreeSearchResult
{
    public IReadOnlyList<Entities.BonEntree> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public int PageCount => Take > 0 ? (int)Math.Ceiling((double)TotalCount / Take) : 0;
}

/// <summary>
/// Filtre de recherche pour les bons d'entrée.
/// </summary>
public class BonEntreeFilter
{
    public string? SearchTerm { get; set; }
    public string? Statut { get; set; }
    public List<string>? Statuts { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string? Compagnie { get; set; }
    public string? Departement { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public string SortBy { get; set; } = "DateCreation";
    public bool SortDescending { get; set; } = true;
}
