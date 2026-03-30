using ClosedXML.Excel;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Interface pour le service d'export Excel
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// Exporte les Bons d'Entrée en fichier Excel
    /// </summary>
    Task<byte[]> ExportBonsEntreeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Exporte les Bons de Sortie en fichier Excel
    /// </summary>
    Task<byte[]> ExportBonsSortieAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Exporte l'historique complet (BEM + BSM) en fichier Excel
    /// </summary>
    Task<byte[]> ExportHistoriqueAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Service d'export Excel utilisant ClosedXML
/// </summary>
public class ExcelExportService : IExcelExportService
{
    private readonly IBonEntreeService _bonEntreeService;
    private readonly IBonSortieService _bonSortieService;

    public ExcelExportService(IBonEntreeService bonEntreeService, IBonSortieService bonSortieService)
    {
        _bonEntreeService = bonEntreeService;
        _bonSortieService = bonSortieService;
    }

    public async Task<byte[]> ExportBonsEntreeAsync(CancellationToken cancellationToken = default)
    {
        var result = await _bonEntreeService.GetListAsync(new BonEntreeFilter { Take = 5000 }, cancellationToken);
        var bons = result.Items;

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Bons d'Entrée");

        // En-têtes
        var headers = new[] { "N°", "Référence", "Compagnie", "Demandeur", "Département", "Provenance", "Destination", "Date Création", "Date Expiration", "Statut", "Nb Matériels" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1B6EC2");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // Données
        int row = 2;
        foreach (var bon in bons)
        {
            ws.Cell(row, 1).Value = bon.Id;
            ws.Cell(row, 2).Value = bon.NumeroReference;
            ws.Cell(row, 3).Value = bon.NomCompagnie;
            ws.Cell(row, 4).Value = bon.NomDemandeur;
            ws.Cell(row, 5).Value = bon.HostDepartment;
            ws.Cell(row, 6).Value = bon.Provenance;
            ws.Cell(row, 7).Value = bon.Destination;
            ws.Cell(row, 8).Value = bon.DateCreation.ToString("dd/MM/yyyy HH:mm");
            ws.Cell(row, 9).Value = bon.DateExpiration.ToString("dd/MM/yyyy");
            ws.Cell(row, 10).Value = GetStatutLabel(bon.StatutActuel);
            ws.Cell(row, 11).Value = bon.Materiels.Count;

            // Alterner les couleurs des lignes
            if (row % 2 == 0)
            {
                ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F0F4FF");
            }

            row++;
        }

        // Ajuster la largeur des colonnes
        ws.Columns().AdjustToContents();

        // Filtre automatique
        ws.RangeUsed()?.SetAutoFilter();

        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportBonsSortieAsync(CancellationToken cancellationToken = default)
    {
        var result = await _bonSortieService.GetListAsync(new BonSortieFilter { Take = 5000 }, cancellationToken);
        var bons = result.Items;

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Bons de Sortie");

        // En-têtes
        var headers = new[] { "N°", "Référence", "Type", "Demandeur", "Département", "Provenance", "Destination", "Motif", "Date Création", "Date Expiration", "Statut", "Nb Matériels" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#16A34A");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // Données
        int row = 2;
        foreach (var bon in bons)
        {
            ws.Cell(row, 1).Value = bon.Id;
            ws.Cell(row, 2).Value = bon.NumeroReference;
            ws.Cell(row, 3).Value = GetTypeBSM(bon);
            ws.Cell(row, 4).Value = bon.NomDemandeur;
            ws.Cell(row, 5).Value = bon.DepartementDemandeur;
            ws.Cell(row, 6).Value = bon.Provenance;
            ws.Cell(row, 7).Value = bon.Destination;
            ws.Cell(row, 8).Value = bon.MotifSortie;
            ws.Cell(row, 9).Value = bon.DateCreation.ToString("dd/MM/yyyy HH:mm");
            ws.Cell(row, 10).Value = bon.DateExpiration.ToString("dd/MM/yyyy");
            ws.Cell(row, 11).Value = GetStatutLabel(bon.StatutActuel);
            ws.Cell(row, 12).Value = bon.Materiels.Count;

            if (row % 2 == 0)
            {
                ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F0FFF4");
            }

            row++;
        }

        ws.Columns().AdjustToContents();
        ws.RangeUsed()?.SetAutoFilter();

        return WorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportHistoriqueAsync(CancellationToken cancellationToken = default)
    {
        var bemResult = await _bonEntreeService.GetListAsync(new BonEntreeFilter { Take = 5000 }, cancellationToken);
        var bsmResult = await _bonSortieService.GetListAsync(new BonSortieFilter { Take = 5000 }, cancellationToken);

        using var workbook = new XLWorkbook();

        // Feuille Historique BEM
        var wsBEM = workbook.Worksheets.Add("Historique BEM");
        WriteBEMHistoriqueSheet(wsBEM, bemResult.Items);

        // Feuille Historique BSM
        var wsBSM = workbook.Worksheets.Add("Historique BSM");
        WriteBSMHistoriqueSheet(wsBSM, bsmResult.Items);

        // Feuille résumé
        var wsSummary = workbook.Worksheets.Add("Résumé");
        WriteSummarySheet(wsSummary, bemResult.Items, bsmResult.Items);

        return WorkbookToBytes(workbook);
    }

    private static void WriteBEMHistoriqueSheet(IXLWorksheet ws, IReadOnlyList<BonEntree> bons)
    {
        var headers = new[] { "Référence", "Compagnie", "Demandeur", "Action", "Par", "Date Action", "Commentaire", "Statut" };
        WriteHeaders(ws, headers, "#1B6EC2");

        int row = 2;
        foreach (var bon in bons)
        {
            if (bon.Historiques.Any())
            {
                foreach (var h in bon.Historiques.OrderBy(x => x.ActionDate))
                {
                    ws.Cell(row, 1).Value = bon.NumeroReference;
                    ws.Cell(row, 2).Value = bon.NomCompagnie;
                    ws.Cell(row, 3).Value = bon.NomDemandeur;
                    ws.Cell(row, 4).Value = GetActionLabel(h.ActionDescription);
                    ws.Cell(row, 5).Value = h.ActionByNom ?? h.ActionBy;
                    ws.Cell(row, 6).Value = h.ActionDate.ToString("dd/MM/yyyy HH:mm");
                    ws.Cell(row, 7).Value = h.Comment ?? "";
                    ws.Cell(row, 8).Value = GetStatutLabel(h.StatutApres);

                    if (row % 2 == 0)
                        ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F0F4FF");

                    row++;
                }
            }
            else
            {
                // Fallback : bon sans historique, écrire 1 ligne avec les infos du bon
                ws.Cell(row, 1).Value = bon.NumeroReference;
                ws.Cell(row, 2).Value = bon.NomCompagnie;
                ws.Cell(row, 3).Value = bon.NomDemandeur;
                ws.Cell(row, 4).Value = "Nouveau";
                ws.Cell(row, 5).Value = bon.NomDemandeur;
                ws.Cell(row, 6).Value = bon.DateCreation.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(row, 7).Value = "";
                ws.Cell(row, 8).Value = GetStatutLabel(bon.StatutActuel);

                if (row % 2 == 0)
                    ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F0F4FF");

                row++;
            }
        }

        ws.Columns().AdjustToContents();
        ws.RangeUsed()?.SetAutoFilter();
    }

    private static void WriteBSMHistoriqueSheet(IXLWorksheet ws, IReadOnlyList<BonSortie> bons)
    {
        var headers = new[] { "Référence", "Type", "Demandeur", "Action", "Par", "Date Action", "Description", "Statut" };
        WriteHeaders(ws, headers, "#16A34A");

        int row = 2;
        foreach (var bon in bons)
        {
            if (bon.Historiques.Any())
            {
                foreach (var h in bon.Historiques.OrderBy(x => x.ActionDate))
                {
                    ws.Cell(row, 1).Value = bon.NumeroReference;
                    ws.Cell(row, 2).Value = GetTypeBSM(bon);
                    ws.Cell(row, 3).Value = bon.NomDemandeur;
                    ws.Cell(row, 4).Value = GetActionLabel(h.ActionDescription);
                    ws.Cell(row, 5).Value = h.ActionByNom ?? h.ActionBy;
                    ws.Cell(row, 6).Value = h.ActionDate.ToString("dd/MM/yyyy HH:mm");
                    ws.Cell(row, 7).Value = h.Comment ?? "";
                    ws.Cell(row, 8).Value = GetStatutLabel(h.StatutApres);

                    if (row % 2 == 0)
                        ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F0FFF4");

                    row++;
                }
            }
            else
            {
                // Fallback : bon sans historique, écrire 1 ligne avec les infos du bon
                ws.Cell(row, 1).Value = bon.NumeroReference;
                ws.Cell(row, 2).Value = GetTypeBSM(bon);
                ws.Cell(row, 3).Value = bon.NomDemandeur;
                ws.Cell(row, 4).Value = "Nouveau";
                ws.Cell(row, 5).Value = bon.NomDemandeur;
                ws.Cell(row, 6).Value = bon.DateCreation.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(row, 7).Value = "";
                ws.Cell(row, 8).Value = GetStatutLabel(bon.StatutActuel);

                if (row % 2 == 0)
                    ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F0FFF4");

                row++;
            }
        }

        ws.Columns().AdjustToContents();
        ws.RangeUsed()?.SetAutoFilter();
    }

    private static void WriteSummarySheet(IXLWorksheet ws, IReadOnlyList<BonEntree> bonsEntree, IReadOnlyList<BonSortie> bonsSortie)
    {
        ws.Cell(1, 1).Value = "KCC Material Flow - Résumé Export";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;

        ws.Cell(3, 1).Value = "Statistique";
        ws.Cell(3, 2).Value = "Valeur";
        ws.Cell(3, 1).Style.Font.Bold = true;
        ws.Cell(3, 2).Style.Font.Bold = true;
        ws.Range(3, 1, 3, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#374151");
        ws.Range(3, 1, 3, 2).Style.Font.FontColor = XLColor.White;

        ws.Cell(4, 1).Value = "Total BEM";
        ws.Cell(4, 2).Value = bonsEntree.Count;
        ws.Cell(5, 1).Value = "BEM Validés";
        ws.Cell(5, 2).Value = bonsEntree.Count(b => b.StatutActuel == "Approved");
        ws.Cell(6, 1).Value = "BEM En cours";
        ws.Cell(6, 2).Value = bonsEntree.Count(b => b.StatutActuel.StartsWith("Pending", StringComparison.OrdinalIgnoreCase));
        ws.Cell(7, 1).Value = "BEM Refusés";
        ws.Cell(7, 2).Value = bonsEntree.Count(b => b.StatutActuel == "Rejected");
        ws.Cell(8, 1).Value = "BEM Brouillon";
        ws.Cell(8, 2).Value = bonsEntree.Count(b => b.StatutActuel == "Draft");

        ws.Cell(10, 1).Value = "Total BSM";
        ws.Cell(10, 2).Value = bonsSortie.Count;
        ws.Cell(11, 1).Value = "BSM Validés";
        ws.Cell(11, 2).Value = bonsSortie.Count(b => b.StatutActuel == "Approved");
        ws.Cell(12, 1).Value = "BSM En cours";
        ws.Cell(12, 2).Value = bonsSortie.Count(b => b.StatutActuel.StartsWith("Pending", StringComparison.OrdinalIgnoreCase));
        ws.Cell(13, 1).Value = "BSM Refusés";
        ws.Cell(13, 2).Value = bonsSortie.Count(b => b.StatutActuel == "Rejected");
        ws.Cell(14, 1).Value = "BSM Brouillon";
        ws.Cell(14, 2).Value = bonsSortie.Count(b => b.StatutActuel == "Draft");

        ws.Cell(16, 1).Value = $"Exporté le {DateTime.Now:dd/MM/yyyy à HH:mm}";
        ws.Cell(16, 1).Style.Font.Italic = true;

        ws.Columns().AdjustToContents();
    }

    private static void WriteHeaders(IXLWorksheet ws, string[] headers, string colorHex)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(colorHex);
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
    }

    private static byte[] WorkbookToBytes(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string GetTypeBSM(BonSortie bon) => bon switch
    {
        Pret => "Prêt",
        BonSortieExterne => "Externe",
        BonSortieInterne => "Interne",
        _ => "Sortie"
    };

    private static string GetStatutLabel(string? s)
    {
        if (string.IsNullOrEmpty(s)) return "-";
        var upper = s.ToUpperInvariant();
        return upper switch
        {
            "DRAFT" => "Brouillon",
            "APPROVED" => "Validé",
            "REJECTED" => "Refusé",
            "RETURNED" => "Renvoyé",
            "COMPLETED" => "Terminé",
            "CANCELLED" => "Annulé",
            "PENDINGSUP" or "PENDINGSUPERVISEUR" => "Chez Superviseur",
            "PENDINGGM" => "Chez GM",
            "PENDINGOPJ" => "Chez OPJ",
            "PENDINGIDENTIFICATION" => "Chez Identification",
            "PENDINGIT" => "Chez IT",
            "PENDINGENV" => "Chez Environnement",
            _ when upper.StartsWith("PENDING") => $"Chez {s.Replace("Pending", "")}",
            _ => s
        };
    }

    private static string GetActionLabel(string action)
    {
        var lower = action.ToLowerInvariant();
        if (lower.Contains("créat") || lower.Contains("creat") || lower == "nouveau") return "Nouveau";
        if (lower.Contains("soumis") || lower.Contains("submit") || lower.Contains("envoyé")) return "Envoyé";
        if (lower.Contains("appro") || lower.Contains("validé")) return "Validé";
        if (lower.Contains("rejet") || lower.Contains("reject") || lower.Contains("refus")) return "Refusé";
        if (lower.Contains("retour") || lower.Contains("renvoyé")) return "Renvoyé";
        if (lower.Contains("modif")) return "Modifié";
        if (lower.Contains("annul")) return "Annulé";
        if (lower.Contains("qr")) return "QR Code";
        if (lower.Contains("scan")) return "Scanné";
        return action;
    }
}
