using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Gestion CRUD des approbateurs spéciaux (Superintendent / OPJ / Identification)
/// utilisés en fin de chaîne d'approbation après la hiérarchie ReportTo.
/// </summary>
public interface IWorkflowApprobateurSpecialService
{
    Task<IReadOnlyList<WorkflowApprobateurSpecial>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<WorkflowApprobateurSpecial>> GetByTypeAsync(TypeApprobateurSpecial type, bool actifsOnly = true, CancellationToken ct = default);
    Task<WorkflowApprobateurSpecial> AddAsync(TypeApprobateurSpecial type, int employeeId, int ordre = 1, int? siteId = null, CancellationToken ct = default);
    Task UpdateAsync(int id, int ordre, bool estActif, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
