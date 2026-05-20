using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IWorkflowConfigService
{
    // ─── Résolution runtime ──────────────────────────────────────────────────
    /// <summary>
    /// Résout le workflow effectif pour un bon.
    /// Priorité : (raison+dept) > (raison seule) > (générique) > défaut métier.
    /// </summary>
    Task<IReadOnlyList<WorkflowEtapeConfig>> GetResolvedWorkflowEtapesAsync(string bonType, string? raisonSortieCode, string? agentDepartementCode = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowEtapeConfig>> GetResolvedWorkflowEtapesForDepartementAsync(string bonType, string? departementCode, CancellationToken cancellationToken = default);

    // ─── Lecture admin ───────────────────────────────────────────────────────
    Task<IReadOnlyList<WorkflowEtapeConfig>> GetWorkflowEtapesForAdminAsync(string bonType, string? raisonSortieCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowEtapeConfig>> GetWorkflowEtapesForDepartementAsync(string bonType, string? departementCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowContextSummary>> GetWorkflowSummaryAsync(string bonType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowDepartementSummary>> GetDepartementWorkflowSummaryAsync(string bonType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DepartementInfo>> GetDepartementsAsync(CancellationToken cancellationToken = default);

    // ─── Sauvegarde / suppression ────────────────────────────────────────────
    Task SaveWorkflowEtapesAsync(string bonType, string? raisonSortieCode, IEnumerable<WorkflowEtapeConfig> etapes, string? modifiedByLogin, CancellationToken cancellationToken = default);
    Task SaveWorkflowEtapesForDepartementAsync(string bonType, string? departementCode, IEnumerable<WorkflowEtapeConfig> etapes, string? modifiedByLogin, CancellationToken cancellationToken = default);
    Task DeleteWorkflowEtapesAsync(string bonType, string? raisonSortieCode, string? modifiedByLogin, CancellationToken cancellationToken = default);
    Task DeleteWorkflowEtapesForDepartementAsync(string bonType, string? departementCode, string? modifiedByLogin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RaisonSortie>> GetRaisonsSortieAsync(CancellationToken cancellationToken = default);

    // ─── Exception cross-département (RaisonSortie + DepartementCode) ────────
    /// <summary>Retourne les étapes configurées pour la combinaison raison × département (admin).</summary>
    Task<IReadOnlyList<WorkflowEtapeConfig>> GetWorkflowEtapesCrossAsync(string bonType, string? raisonSortieCode, string departementCode, CancellationToken cancellationToken = default);

    /// <summary>Liste les départements qui ont une exception configurée pour cette raison.</summary>
    Task<IReadOnlyList<DepartementInfo>> GetDepartementOverridesForRaisonAsync(string bonType, string? raisonSortieCode, CancellationToken cancellationToken = default);

    /// <summary>Sauvegarde les étapes pour la combinaison raison × département.</summary>
    Task SaveWorkflowEtapesCrossAsync(string bonType, string? raisonSortieCode, string departementCode, IEnumerable<WorkflowEtapeConfig> etapes, string? modifiedByLogin, CancellationToken cancellationToken = default);

    /// <summary>Supprime les étapes pour la combinaison raison × département.</summary>
    Task DeleteWorkflowEtapesCrossAsync(string bonType, string? raisonSortieCode, string departementCode, string? modifiedByLogin, CancellationToken cancellationToken = default);
}
