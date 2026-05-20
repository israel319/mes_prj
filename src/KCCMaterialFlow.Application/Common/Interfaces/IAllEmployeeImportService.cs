namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Import du référentiel RH (Book2.xlsx) vers la table T_AllEmployees.
/// Synchronise également les AppUsers (T_Users) depuis UserName pour permettre l'auth Windows.
/// </summary>
public interface IAllEmployeeImportService
{
    Task<GlencoreImportResult> ImportFromXlsxAsync(Stream xlsxStream, CancellationToken cancellationToken = default);
}

public sealed record GlencoreImportResult(
    int RowsRead,
    int Inserted,
    int Updated,
    int Skipped,
    int AppUsersUpserted,
    IReadOnlyList<string> Warnings);
