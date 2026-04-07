using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Service pour la gestion des motifs/raisons d'entrée structurés.
/// </summary>
public interface IRaisonEntreeService
{
    /// <summary>
    /// Retourne toutes les raisons d'entrée actives, triées par ordre d'affichage.
    /// </summary>
    Task<IReadOnlyList<RaisonEntree>> GetAllActiveAsync();

    /// <summary>
    /// Retourne une raison d'entrée par son ID.
    /// </summary>
    Task<RaisonEntree?> GetByIdAsync(int id);

    /// <summary>
    /// Retourne une raison d'entrée par son code.
    /// </summary>
    Task<RaisonEntree?> GetByCodeAsync(string code);

    /// <summary>
    /// Retourne les raisons de sortie autorisées pour une raison d'entrée donnée.
    /// C'est la méthode clé utilisée par le formulaire BSM.
    /// </summary>
    Task<IReadOnlyList<RaisonSortie>> GetRaisonsSortieByRaisonEntreeIdAsync(int raisonEntreeId);

    /// <summary>
    /// Retourne les raisons de sortie autorisées pour un code de raison d'entrée.
    /// </summary>
    Task<IReadOnlyList<RaisonSortie>> GetRaisonsSortieByRaisonEntreeCodeAsync(string code);
}
