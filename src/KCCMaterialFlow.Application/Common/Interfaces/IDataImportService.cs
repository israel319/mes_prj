namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Service d'import des données de référence depuis DATA.xlsx.
/// Étape 1 (ImportToStagingAsync) : compagnies + contrats vont en staging,
/// les employés sont upsertés DIRECTEMENT dans T_Employees (avec résolution
/// 2-passes des liens ReportTo via NumeroEmploye).
/// Étape 2 (MergeStagingAsync) : merge compagnies + contrats vers les tables réelles.
/// </summary>
public interface IDataImportService
{
    Task<DataImportBatchResult> ImportToStagingAsync(Stream xlsxStream, CancellationToken cancellationToken = default);
    Task<DataImportMergeResult> MergeStagingAsync(DateTime importBatchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DataImportBatchSummary>> GetBatchesAsync(CancellationToken cancellationToken = default);
    Task PurgeBatchAsync(DateTime importBatchId, CancellationToken cancellationToken = default);
}

public sealed record DataImportBatchResult(
    DateTime BatchId,
    int CompaniesLoaded,
    int ContractsLoaded,
    int EmployeesUpserted,
    int ReportToResolved,
    int ReportToOrphans,
    IReadOnlyList<string> Warnings);

public sealed record DataImportMergeResult(
    int CompaniesUpserted,
    int ContractsUpserted,
    IReadOnlyList<string> Warnings);

public sealed record DataImportBatchSummary(
    DateTime BatchId,
    int CompaniesPending,
    int ContractsPending,
    int CompaniesMerged,
    int ContractsMerged);
