using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface IWorkflowConfigService
{
    Task<IReadOnlyList<WorkflowEtapeConfig>> GetResolvedWorkflowEtapesAsync(string bonType, string? raisonSortieCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowEtapeConfig>> GetWorkflowEtapesForAdminAsync(string bonType, string? raisonSortieCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowContextSummary>> GetWorkflowSummaryAsync(string bonType, CancellationToken cancellationToken = default);
    Task SaveWorkflowEtapesAsync(string bonType, string? raisonSortieCode, IEnumerable<WorkflowEtapeConfig> etapes, string? modifiedByLogin, CancellationToken cancellationToken = default);
    Task DeleteWorkflowEtapesAsync(string bonType, string? raisonSortieCode, string? modifiedByLogin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RaisonSortie>> GetRaisonsSortieAsync(CancellationToken cancellationToken = default);
}
