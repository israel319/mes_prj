using ClosedXML.Excel;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.Shared;

/// <summary>
/// Importe Book2.xlsx (15 colonnes) vers T_AllEmployees, puis upserte
/// les AppUser (Login=UserName) depuis AllEmployee (joint sur Matricule == EmployeeCode).
///
/// Colonnes attendues (Sheet1, ligne 1 = en-têtes) :
/// EmployeeCode | FirstName | LastName | DepartementCode | Departement |
/// UserName | Mail | ReportsToEmployeeCode | ReportsToEmployeeDisplay |
/// SuperIntendentEmployeeCode | SuperIntendentEmployeeDisplay |
/// ManagerHodEmployeeCode | ManagerHodEmployeeDisplay |
/// GmEmployeeCode | GmEmployeeDisplay
///
/// Les valeurs littérales "NULL" sont converties en null.
/// </summary>
public sealed class AllEmployeeImportService : IAllEmployeeImportService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly ILogger<AllEmployeeImportService> _logger;

    public AllEmployeeImportService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        ILogger<AllEmployeeImportService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task<GlencoreImportResult> ImportFromXlsxAsync(Stream xlsxStream, CancellationToken cancellationToken = default)
    {
        var warnings = new List<string>();
        using var workbook = new XLWorkbook(xlsxStream);
        var sheet = workbook.Worksheets.FirstOrDefault()
            ?? throw new InvalidOperationException("Aucune feuille trouvée dans Book2.xlsx");

        // Détection des colonnes par en-tête (ligne 1)
        var headerRow = sheet.Row(1);
        var colMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int c = 1; c <= 50; c++)
        {
            var h = headerRow.Cell(c).GetString().Trim();
            if (string.IsNullOrEmpty(h)) break;
            colMap[h] = c;
        }

        int Col(string name) => colMap.TryGetValue(name, out var i) ? i : -1;
        var iCode = Col("EmployeeCode");
        var iFirst = Col("FirstName");
        var iLast = Col("LastName");
        var iDeptCode = Col("DepartementCode");
        var iDept = Col("Departement");
        var iUser = Col("UserName");
        var iMail = Col("Mail");
        var iRtCode = Col("ReportsToEmployeeCode");
        var iRtDisp = Col("ManagerHodEmployeeDisplay");
        var iSiCode = Col("SuperIntendentEmployeeCode");
        var iSiDisp = Col("SuperIntendentEmployeeDisplay");
        var iHodCode = Col("ManagerHodEmployeeCode");
        var iHodDisp = Col("ManagerHodEmployeeDisplay");
        var iGmCode = Col("GmEmployeeCode");
        var iGmDisp = Col("GmEmployeeDisplay");

        if (iCode < 0)
            throw new InvalidOperationException("Colonne 'EmployeeCode' introuvable dans Book2.xlsx");

        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 1;

        await using var ctx = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existing = await ctx.Set<AllEmployee>()
            .ToDictionaryAsync(g => g.EmployeeCode, g => g, StringComparer.OrdinalIgnoreCase, cancellationToken);

        int rowsRead = 0, inserted = 0, updated = 0, skipped = 0;
        var nowImport = DateTime.Now;

        for (int r = 2; r <= lastRow; r++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            rowsRead++;

            var code = Cell(sheet, r, iCode);
            if (string.IsNullOrWhiteSpace(code))
            {
                skipped++;
                continue;
            }

            if (!existing.TryGetValue(code, out var entity))
            {
                entity = new AllEmployee { EmployeeCode = code };
                ctx.Add(entity);
                existing[code] = entity;
                inserted++;
            }
            else
            {
                updated++;
            }

            entity.FirstName = Cell(sheet, r, iFirst);
            entity.LastName = Cell(sheet, r, iLast);
            entity.DepartementCode = Cell(sheet, r, iDeptCode);
            entity.Departement = Cell(sheet, r, iDept);
            entity.UserName = Cell(sheet, r, iUser);
            entity.Mail = Cell(sheet, r, iMail);
            entity.ReportsToEmployeeCode = Cell(sheet, r, iRtCode);
            entity.ManagerHodEmployeeDisplay = Cell(sheet, r, iRtDisp);
            entity.SuperIntendentEmployeeCode = Cell(sheet, r, iSiCode);
            entity.SuperIntendentEmployeeDisplay = Cell(sheet, r, iSiDisp);
            entity.ManagerHodEmployeeCode = Cell(sheet, r, iHodCode);
            entity.ManagerHodEmployeeDisplay = Cell(sheet, r, iHodDisp);
            entity.GmEmployeeCode = Cell(sheet, r, iGmCode);
            entity.GmEmployeeDisplay = Cell(sheet, r, iGmDisp);
            entity.DateImport = nowImport;
        }

        await ctx.SaveChangesAsync(cancellationToken);

        // ── Upsert AppUser depuis AllEmployee.UserName ──────────────────────
        // Joint sur Employee.Matricule == AllEmployee.EmployeeCode
        int appUsersUpserted = 0;
        var glencoreByCode = existing
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value.UserName))
            .ToDictionary(kv => kv.Key, kv => kv.Value.UserName!, StringComparer.OrdinalIgnoreCase);

        var employees = await ctx.Employees
            .Where(e => e.Matricule != null)
            .ToListAsync(cancellationToken);

        var existingAppUsers = await ctx.AppUsers
            .Where(u => u.EmployeeId.HasValue)
            .ToDictionaryAsync(u => u.EmployeeId!.Value, cancellationToken);

        foreach (var emp in employees)
        {
            if (emp.Matricule == null) continue;
            if (!glencoreByCode.TryGetValue(emp.Matricule, out var userName)) continue;

            if (existingAppUsers.TryGetValue(emp.Id, out var appUser))
            {
                if (!string.Equals(appUser.Login, userName, StringComparison.OrdinalIgnoreCase))
                {
                    appUser.Login = userName;
                    appUsersUpserted++;
                }
            }
            else
            {
                ctx.AppUsers.Add(new AppUser
                {
                    Login = userName,
                    EmployeeId = emp.Id,
                    EstActif = true,
                    DateCreation = DateTime.UtcNow
                });
                appUsersUpserted++;
            }
        }

        await ctx.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Glencore import : {Read} lignes lues, {Ins} insertions, {Upd} mises à jour, {Skip} ignorées, {Users} AppUsers upsertés.",
            rowsRead, inserted, updated, skipped, appUsersUpserted);

        return new GlencoreImportResult(rowsRead, inserted, updated, skipped, appUsersUpserted, warnings);
    }

    private static string? Cell(IXLWorksheet sheet, int row, int col)
    {
        if (col < 1) return null;
        var raw = sheet.Cell(row, col).GetString()?.Trim();
        if (string.IsNullOrEmpty(raw)) return null;
        if (string.Equals(raw, "NULL", StringComparison.OrdinalIgnoreCase)) return null;
        return raw;
    }
}
