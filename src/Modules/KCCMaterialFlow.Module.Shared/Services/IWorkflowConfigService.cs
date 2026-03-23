using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

public interface IWorkflowConfigService
{
    /// <summary>
    /// Retourne le workflow effectif (BD spécifique → BD générique → défaut métier).
    /// N'écrit JAMAIS en BD. Uniquement pour le moteur d'approbation.
    /// </summary>
    Task<IReadOnlyList<WorkflowEtapeConfig>> GetResolvedWorkflowEtapesAsync(string bonType, string? raisonSortieCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne exactement ce qui est en BD pour ce contexte précis.
    /// Liste vide si aucune personnalisation BD n'existe.
    /// </summary>
    Task<IReadOnlyList<WorkflowEtapeConfig>> GetWorkflowEtapesForAdminAsync(string bonType, string? raisonSortieCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne le résumé de tous les contextes (motifs) pour un type de bon.
    /// Permet à l'UI d'afficher Personnalisé / Hérité pour chaque motif.
    /// </summary>
    Task<IReadOnlyList<WorkflowContextSummary>> GetWorkflowSummaryAsync(string bonType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enregistre (ou remplace) la configuration BD d'un contexte.
    /// </summary>
    Task SaveWorkflowEtapesAsync(string bonType, string? raisonSortieCode, IEnumerable<WorkflowEtapeConfig> etapes, string? modifiedByLogin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime la configuration BD d'un contexte, le faisant revenir à l'héritage.
    /// </summary>
    Task DeleteWorkflowEtapesAsync(string bonType, string? raisonSortieCode, string? modifiedByLogin, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RaisonSortie>> GetRaisonsSortieAsync(CancellationToken cancellationToken = default);
}
