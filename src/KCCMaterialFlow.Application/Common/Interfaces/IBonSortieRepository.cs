using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Interface du repository pour les Bons de Sortie.
/// Fournit l'accès aux données avec des méthodes spécialisées.
/// </summary>
public interface IBonSortieRepository
{
    #region CRUD de base

    /// <summary>
    /// Récupère un bon de sortie par son ID
    /// </summary>
    Task<BonSortie?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un bon de sortie par son numéro de référence
    /// </summary>
    Task<BonSortie?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// BSM-030: Récupère un bon de sortie par son hash QR Code
    /// </summary>
    Task<BonSortie?> GetByQRCodeHashAsync(string qrCodeHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ajoute un nouveau bon de sortie
    /// </summary>
    Task<BonSortie> AddAsync(BonSortie bonSortie, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un bon de sortie existant
    /// </summary>
    Task UpdateAsync(BonSortie bonSortie, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un bon de sortie
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    #endregion

    #region Requêtes spécialisées

    /// <summary>
    /// Recherche des bons de sortie avec filtres et pagination
    /// </summary>
    Task<(IReadOnlyList<BonSortie> Items, int TotalCount)> SearchAsync(
        string? searchTerm = null,
        string? statut = null,
        string? typeSortie = null,
        string? departement = null,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        int skip = 0,
        int take = 20,
        string? userLogin = null,
        CancellationToken cancellationToken = default,
        string? demandeur = null);

    /// <summary>
    /// Récupère les bons de sortie créés par un utilisateur
    /// </summary>
    Task<(IReadOnlyList<BonSortie> Items, int TotalCount)> GetByUserAsync(
        string userLogin,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les bons en attente d'approbation pour un rôle
    /// </summary>
    Task<IReadOnlyList<BonSortie>> GetPendingApprovalAsync(
        string role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// v2 — BSM en attente où l'étape COURANTE est assignée à l'employé donné.
    /// </summary>
    Task<IReadOnlyList<BonSortie>> GetPendingApprovalsByEmployeeAsync(int employeeId, bool isAdmin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne les bons approuvés (StatutActuel == "Approved") avec leurs approbations incluses.
    /// Le filtrage par approbateur courant est fait dans la couche service.
    /// </summary>
    Task<IReadOnlyList<BonSortie>> GetApprovedBonsWithApprobationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les prêts en cours (non retournés)
    /// </summary>
    Task<IReadOnlyList<Pret>> GetActiveLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les prêts en retard
    /// </summary>
    Task<IReadOnlyList<Pret>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// BSM-027: Récupère les prêts expirant dans N jours (pour alertes)
    /// </summary>
    Task<IReadOnlyList<Pret>> GetLoansExpiringInDaysAsync(int days, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les bons de sortie externes liés à un bon d'entrée
    /// </summary>
    Task<IReadOnlyList<BonSortieExterne>> GetByBonEntreeAsync(
        int bonEntreeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Compte les bons par statut
    /// </summary>
    Task<Dictionary<string, int>> GetCountByStatutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Génère le prochain numéro de référence
    /// </summary>
    Task<string> GenerateNextNumeroAsync(string prefix, CancellationToken cancellationToken = default);

    #endregion

    #region Historique

    /// <summary>
    /// Ajoute une entrée d'historique
    /// </summary>
    Task AddHistoryAsync(BonSortieHistory history, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère l'historique d'un bon de sortie
    /// </summary>
    Task<IReadOnlyList<BonSortieHistory>> GetHistoryAsync(int bonSortieId, CancellationToken cancellationToken = default);

    #endregion

    #region Approbations

    /// <summary>
    /// Ajoute une étape d'approbation à un bon de sortie
    /// </summary>
    Task AddApprobationAsync(ApprobationSortie approbation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime toutes les approbations existantes pour un bon de sortie (avant recréation lors de la soumission).
    /// </summary>
    Task DeleteApprobationsAsync(int bonSortieId, CancellationToken cancellationToken = default);

    #endregion

    #region Données de référence

    /// <summary>
    /// Récupère le code d'une raison de sortie par son ID.
    /// </summary>
    Task<string?> GetRaisonSortieCodeByIdAsync(int raisonId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime tous les matériels d'un bon de sortie.
    /// </summary>
    Task RemoveMaterielsAsync(int bonSortieId, CancellationToken cancellationToken = default);

    #endregion
}
