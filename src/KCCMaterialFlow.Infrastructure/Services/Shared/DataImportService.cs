using ClosedXML.Excel;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Entities.Staging;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Implémentation du service d'import depuis DATA.xlsx.
/// Étape 1 : compagnies + contrats → tables staging ; employés → DIRECTEMENT dans T_Employees
///           avec résolution 2-passes des liens ReportTo (self-FK Employee.ReportToEmployeeId).
/// Étape 2 : merge staging → Compagnies / Contrats.
/// </summary>
public sealed class DataImportService : IDataImportService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly ILogger<DataImportService> _logger;

    private const string SheetCompany = "Company";
    private const string SheetContract = "Contract";
    private const string SheetEmployee = "Sheet3";

    public DataImportService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        ILogger<DataImportService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    // ── Étape 1 : import xlsx ─────────────────────────────────────────
    public async Task<DataImportBatchResult> ImportToStagingAsync(Stream xlsxStream, CancellationToken cancellationToken = default)
    {
        var batchId = DateTime.UtcNow;
        var warnings = new List<string>();

        using var workbook = new XLWorkbook(xlsxStream);

        var stgCompanies = ReadCompanies(workbook, batchId, warnings);
        var stgContracts = ReadContracts(workbook, batchId, warnings);
        var employeeRows = ReadEmployeeRows(workbook, warnings);

        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // 1a. Compagnies + Contrats → staging
        ctx.StagingCompanies.AddRange(stgCompanies);
        ctx.StagingContracts.AddRange(stgContracts);
        await ctx.SaveChangesAsync(cancellationToken);

        // 1b. Employés → upsert DIRECT dans T_Employees (passe 1)
        var existingByMatricule = await ctx.Employees
            .Where(e => e.Matricule != null)
            .ToDictionaryAsync(e => e.Matricule!, e => e, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingByNumero = (await ctx.Employees
                .Where(e => e.NumeroEmploye != null)
                .ToListAsync(cancellationToken))
            .GroupBy(e => e.NumeroEmploye!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        int employeesUpserted = 0;
        foreach (var row in employeeRows)
        {
            if (string.IsNullOrWhiteSpace(row.EmployeeId) && string.IsNullOrWhiteSpace(row.EmployeeEntity))
            {
                warnings.Add($"Employé sans EmployeeId ni EmployeeEntity ignoré (ligne)");
                continue;
            }

            Employee? employee = null;
            if (!string.IsNullOrWhiteSpace(row.EmployeeId))
                existingByMatricule.TryGetValue(row.EmployeeId, out employee);
            if (employee == null && !string.IsNullOrWhiteSpace(row.EmployeeEntity))
                existingByNumero.TryGetValue(row.EmployeeEntity, out employee);

            if (employee == null)
            {
                employee = new Employee
                {
                    Matricule = row.EmployeeId,
                    NumeroEmploye = row.EmployeeEntity,
                    NomComplet = ResolveNomComplet(row),
                    DisplayName = ResolveNomComplet(row),
                    Prenom = row.FirstName,
                    Nom = row.LastName,
                    Fonction = row.JobTitle,
                    Email = row.Mail,
                    Telephone = row.TelephoneNumber,
                    DepartementNom = row.Departement,
                    Sources = row.Sources,
                    EstInterne = true,
                    DateCreation = DateTime.Now
                };
                ctx.Employees.Add(employee);
                if (!string.IsNullOrWhiteSpace(row.EmployeeId)) existingByMatricule[row.EmployeeId] = employee;
                if (!string.IsNullOrWhiteSpace(row.EmployeeEntity)) existingByNumero[row.EmployeeEntity] = employee;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(row.EmployeeId)) employee.Matricule = row.EmployeeId;
                if (!string.IsNullOrWhiteSpace(row.EmployeeEntity)) employee.NumeroEmploye = row.EmployeeEntity;
                var nom = ResolveNomComplet(row);
                if (!string.IsNullOrWhiteSpace(nom))
                {
                    employee.NomComplet = nom;
                    // Resync DisplayName seulement s'il n'a jamais été personnalisé (=NomComplet)
                    if (string.IsNullOrWhiteSpace(employee.DisplayName) || employee.DisplayName == employee.NomComplet)
                        employee.DisplayName = nom;
                }
                if (row.FirstName != null) employee.Prenom = row.FirstName;
                if (row.LastName != null) employee.Nom = row.LastName;
                if (row.JobTitle != null) employee.Fonction = row.JobTitle;
                if (row.Mail != null) employee.Email = row.Mail;
                if (row.TelephoneNumber != null) employee.Telephone = row.TelephoneNumber;
                if (row.Departement != null) employee.DepartementNom = row.Departement;
                if (row.Sources != null) employee.Sources = row.Sources;
            }

            employeesUpserted++;
        }

        await ctx.SaveChangesAsync(cancellationToken);

        // 1c. Passe 2 : résolution des liens ReportTo via NumeroEmploye
        // (les Employee.Id sont maintenant disponibles après SaveChanges)
        int reportToResolved = 0;
        int reportToOrphans = 0;

        foreach (var row in employeeRows.Where(r => !string.IsNullOrWhiteSpace(r.ReportToEmployeeId)))
        {
            Employee? employee = null;
            if (!string.IsNullOrWhiteSpace(row.EmployeeId))
                existingByMatricule.TryGetValue(row.EmployeeId, out employee);
            if (employee == null && !string.IsNullOrWhiteSpace(row.EmployeeEntity))
                existingByNumero.TryGetValue(row.EmployeeEntity, out employee);
            if (employee == null) continue;

            if (!existingByNumero.TryGetValue(row.ReportToEmployeeId!, out var manager))
            {
                reportToOrphans++;
                warnings.Add($"ReportTo orphelin : employé {row.EmployeeId ?? row.EmployeeEntity} référence manager {row.ReportToEmployeeId} introuvable");
                continue;
            }

            if (manager.Id == 0)
            {
                // Manager nouvellement inséré → forcer une 2e save d'abord
                await ctx.SaveChangesAsync(cancellationToken);
            }

            if (employee.Id == manager.Id) { reportToOrphans++; continue; }

            employee.ReportToEmployeeId = manager.Id;
            reportToResolved++;
        }

        await ctx.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Import batch={BatchId} : {C} compagnies, {Ct} contrats staging ; {E} employés upsertés ({R} liens ReportTo résolus, {O} orphelins)",
            batchId, stgCompanies.Count, stgContracts.Count, employeesUpserted, reportToResolved, reportToOrphans);

        return new DataImportBatchResult(
            batchId, stgCompanies.Count, stgContracts.Count,
            employeesUpserted, reportToResolved, reportToOrphans, warnings);
    }

    // ── Étape 2 : merge staging → tables réelles (compagnies + contrats uniquement) ─
    public async Task<DataImportMergeResult> MergeStagingAsync(DateTime importBatchId, CancellationToken cancellationToken = default)
    {
        var warnings = new List<string>();
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // ── Compagnies ────────────────────────────────────────────────
        var stgCompanies = await ctx.StagingCompanies
            .Where(s => s.ImportBatchId == importBatchId && !s.EstMerge)
            .ToListAsync(cancellationToken);

        var existingCompagniesByCode = await ctx.Compagnies
            .Where(c => c.Code != null)
            .ToDictionaryAsync(c => c.Code!, c => c, StringComparer.OrdinalIgnoreCase, cancellationToken);

        int companiesUpserted = 0;
        foreach (var stg in stgCompanies)
        {
            if (string.IsNullOrWhiteSpace(stg.CompanyCode) || string.IsNullOrWhiteSpace(stg.CompanyName))
            {
                stg.ErreurMessage = "CompanyCode ou CompanyName manquant";
                continue;
            }

            if (!existingCompagniesByCode.TryGetValue(stg.CompanyCode, out var compagnie))
            {
                compagnie = new Compagnie
                {
                    Code = stg.CompanyCode,
                    Nom = stg.CompanyName,
                    EstActif = stg.Actif ?? true,
                    DateCreation = stg.DateSys ?? DateTime.Now
                };
                ctx.Compagnies.Add(compagnie);
                existingCompagniesByCode[stg.CompanyCode] = compagnie;
            }
            else
            {
                compagnie.Nom = stg.CompanyName;
                compagnie.EstActif = stg.Actif ?? compagnie.EstActif;
                if (stg.DateSys.HasValue) compagnie.DateCreation = stg.DateSys.Value;
            }

            stg.EstMerge = true;
            companiesUpserted++;
        }

        await ctx.SaveChangesAsync(cancellationToken);

        // ── Contrats ──────────────────────────────────────────────────
        var stgContracts = await ctx.StagingContracts
            .Where(s => s.ImportBatchId == importBatchId && !s.EstMerge)
            .ToListAsync(cancellationToken);

        var existingContratsByPo = await ctx.Contrats
            .ToDictionaryAsync(c => c.PoNumber, c => c, StringComparer.OrdinalIgnoreCase, cancellationToken);

        int contractsUpserted = 0;
        foreach (var stg in stgContracts)
        {
            if (string.IsNullOrWhiteSpace(stg.PoNumber) || string.IsNullOrWhiteSpace(stg.CompanyCode))
            {
                stg.ErreurMessage = "PoNumber ou CompanyCode manquant";
                continue;
            }

            if (!existingCompagniesByCode.TryGetValue(stg.CompanyCode, out var compagnie))
            {
                stg.ErreurMessage = $"Compagnie introuvable pour CompanyCode={stg.CompanyCode}";
                warnings.Add(stg.ErreurMessage);
                continue;
            }

            if (!existingContratsByPo.TryGetValue(stg.PoNumber, out var contrat))
            {
                contrat = new Contrat
                {
                    PoNumber = stg.PoNumber,
                    ContratDescription = stg.ContractDescription,
                    Compagnie = compagnie,
                    EstActif = stg.Actif ?? true,
                    DateCreation = stg.DateSys ?? DateTime.Now
                };
                ctx.Contrats.Add(contrat);
                existingContratsByPo[stg.PoNumber] = contrat;
            }
            else
            {
                contrat.ContratDescription = stg.ContractDescription;
                contrat.Compagnie = compagnie;
                contrat.EstActif = stg.Actif ?? contrat.EstActif;
                if (stg.DateSys.HasValue) contrat.DateCreation = stg.DateSys.Value;
            }

            stg.EstMerge = true;
            contractsUpserted++;
        }

        await ctx.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Merge batch={BatchId} : {Comp} compagnies, {Ctr} contrats",
            importBatchId, companiesUpserted, contractsUpserted);

        return new DataImportMergeResult(companiesUpserted, contractsUpserted, warnings);
    }

    public async Task<IReadOnlyList<DataImportBatchSummary>> GetBatchesAsync(CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var companyBatches = await ctx.StagingCompanies
            .GroupBy(s => s.ImportBatchId)
            .Select(g => new { BatchId = g.Key, Pending = g.Count(x => !x.EstMerge), Merged = g.Count(x => x.EstMerge) })
            .ToListAsync(cancellationToken);
        var contractBatches = await ctx.StagingContracts
            .GroupBy(s => s.ImportBatchId)
            .Select(g => new { BatchId = g.Key, Pending = g.Count(x => !x.EstMerge), Merged = g.Count(x => x.EstMerge) })
            .ToListAsync(cancellationToken);

        var allBatchIds = companyBatches.Select(x => x.BatchId)
            .Concat(contractBatches.Select(x => x.BatchId))
            .Distinct()
            .OrderByDescending(b => b)
            .ToList();

        return allBatchIds.Select(id =>
        {
            var c = companyBatches.FirstOrDefault(x => x.BatchId == id);
            var ct = contractBatches.FirstOrDefault(x => x.BatchId == id);
            return new DataImportBatchSummary(
                id,
                c?.Pending ?? 0, ct?.Pending ?? 0,
                c?.Merged ?? 0, ct?.Merged ?? 0);
        }).ToList();
    }

    public async Task PurgeBatchAsync(DateTime importBatchId, CancellationToken cancellationToken = default)
    {
        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await ctx.StagingCompanies.Where(s => s.ImportBatchId == importBatchId).ExecuteDeleteAsync(cancellationToken);
        await ctx.StagingContracts.Where(s => s.ImportBatchId == importBatchId).ExecuteDeleteAsync(cancellationToken);
    }

    // ── Helpers de lecture xlsx ───────────────────────────────────────

    private static List<StagingCompany> ReadCompanies(XLWorkbook wb, DateTime batchId, List<string> warnings)
    {
        var ws = wb.Worksheets.FirstOrDefault(w => string.Equals(w.Name, SheetCompany, StringComparison.OrdinalIgnoreCase));
        if (ws == null) { warnings.Add($"Sheet '{SheetCompany}' introuvable"); return new(); }

        var list = new List<StagingCompany>();
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
        for (int r = 2; r <= lastRow; r++)
        {
            if (ws.Cell(r, 1).IsEmpty() && ws.Cell(r, 2).IsEmpty()) continue;
            list.Add(new StagingCompany
            {
                CompanyName = NullIfEmpty(ws.Cell(r, 1).GetString()),
                CompanyCode = NullIfEmpty(ws.Cell(r, 2).GetString()),
                Actif = ParseBool(ws.Cell(r, 3)),
                DateSys = ParseDate(ws.Cell(r, 4)),
                DateSysRaw = NullIfEmpty(ws.Cell(r, 4).GetString()),
                ImportBatchId = batchId
            });
        }
        return list;
    }

    private static List<StagingContract> ReadContracts(XLWorkbook wb, DateTime batchId, List<string> warnings)
    {
        var ws = wb.Worksheets.FirstOrDefault(w => string.Equals(w.Name, SheetContract, StringComparison.OrdinalIgnoreCase));
        if (ws == null) { warnings.Add($"Sheet '{SheetContract}' introuvable"); return new(); }

        var list = new List<StagingContract>();
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
        for (int r = 2; r <= lastRow; r++)
        {
            if (ws.Cell(r, 1).IsEmpty() && ws.Cell(r, 3).IsEmpty()) continue;
            list.Add(new StagingContract
            {
                PoNumber = NullIfEmpty(ws.Cell(r, 1).GetString()),
                ContractDescription = NullIfEmpty(ws.Cell(r, 2).GetString()),
                CompanyCode = NullIfEmpty(ws.Cell(r, 3).GetString()),
                Actif = ParseBool(ws.Cell(r, 4)),
                DateSys = ParseDate(ws.Cell(r, 5)),
                DateSysRaw = NullIfEmpty(ws.Cell(r, 5).GetString()),
                ImportBatchId = batchId
            });
        }
        return list;
    }

    private static List<EmployeeRow> ReadEmployeeRows(XLWorkbook wb, List<string> warnings)
    {
        var ws = wb.Worksheets.FirstOrDefault(w => string.Equals(w.Name, SheetEmployee, StringComparison.OrdinalIgnoreCase));
        if (ws == null) { warnings.Add($"Sheet '{SheetEmployee}' introuvable"); return new(); }

        var list = new List<EmployeeRow>();
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
        // Colonnes attendues (1-based) :
        // 1=EmployeeEntity 2=EmployeeId 3=FirstName 4=LastName 5=DisplayName 6=EmployeeName
        // 7=Mail 8=Jobtitle 9=TelephoneNumber 10=Departement 11=Sources 12=Actif 13=login 14=ReportToEmployeeID
        for (int r = 2; r <= lastRow; r++)
        {
            if (ws.Cell(r, 1).IsEmpty() && ws.Cell(r, 2).IsEmpty()) continue;
            list.Add(new EmployeeRow(
                EmployeeEntity: NullIfEmpty(ws.Cell(r, 1).GetString()),
                EmployeeId: NullIfEmpty(ws.Cell(r, 2).GetString()),
                FirstName: NullIfEmpty(ws.Cell(r, 3).GetString()),
                LastName: NullIfEmpty(ws.Cell(r, 4).GetString()),
                DisplayName: NullIfEmpty(ws.Cell(r, 5).GetString()),
                EmployeeName: NullIfEmpty(ws.Cell(r, 6).GetString()),
                Mail: NullIfEmpty(ws.Cell(r, 7).GetString()),
                JobTitle: NullIfEmpty(ws.Cell(r, 8).GetString()),
                TelephoneNumber: NullIfEmpty(ws.Cell(r, 9).GetString()),
                Departement: NullIfEmpty(ws.Cell(r, 10).GetString()),
                Sources: NullIfEmpty(ws.Cell(r, 11).GetString()),
                Actif: ParseBool(ws.Cell(r, 12)),
                Login: NullIfEmpty(ws.Cell(r, 13).GetString()),
                ReportToEmployeeId: NullIfEmpty(ws.Cell(r, 14).GetString())));
        }
        return list;
    }

    private static string ResolveNomComplet(EmployeeRow r)
    {
        if (!string.IsNullOrWhiteSpace(r.DisplayName)) return r.DisplayName!;
        if (!string.IsNullOrWhiteSpace(r.EmployeeName)) return r.EmployeeName!;
        return ($"{r.FirstName} {r.LastName}").Trim();
    }

    private static string? NullIfEmpty(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static bool? ParseBool(IXLCell cell)
    {
        if (cell.IsEmpty()) return null;
        if (cell.DataType == XLDataType.Boolean) return cell.GetValue<bool>();
        if (cell.DataType == XLDataType.Number) return cell.GetValue<double>() != 0;
        var s = cell.GetString().Trim();
        if (string.IsNullOrEmpty(s)) return null;
        if (s == "1" || s.Equals("true", StringComparison.OrdinalIgnoreCase) || s.Equals("oui", StringComparison.OrdinalIgnoreCase)) return true;
        if (s == "0" || s.Equals("false", StringComparison.OrdinalIgnoreCase) || s.Equals("non", StringComparison.OrdinalIgnoreCase)) return false;
        return null;
    }

    private static DateTime? ParseDate(IXLCell cell)
    {
        if (cell.IsEmpty()) return null;
        if (cell.TryGetValue<DateTime>(out var dt) && dt > new DateTime(1990, 1, 1)) return dt;
        var s = cell.GetString();
        if (DateTime.TryParse(s, out var parsed)) return parsed;
        return null;
    }

    private sealed record EmployeeRow(
        string? EmployeeEntity,
        string? EmployeeId,
        string? FirstName,
        string? LastName,
        string? DisplayName,
        string? EmployeeName,
        string? Mail,
        string? JobTitle,
        string? TelephoneNumber,
        string? Departement,
        string? Sources,
        bool? Actif,
        string? Login,
        string? ReportToEmployeeId);
}
