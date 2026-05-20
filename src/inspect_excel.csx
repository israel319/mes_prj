#r "KCCMaterialFlow.Host/bin/Debug/net10.0/ClosedXML.dll"
#r "KCCMaterialFlow.Host/bin/Debug/net10.0/DocumentFormat.OpenXml.dll"

using ClosedXML.Excel;

var path = @"c:\Users\ikasa\Downloads\DATA.xlsx";
using var wb = new XLWorkbook(path);
foreach (var ws in wb.Worksheets)
{
    Console.WriteLine($"=== Sheet: {ws.Name} (rows: {ws.LastRowUsed()?.RowNumber()}, cols: {ws.LastColumnUsed()?.ColumnNumber()}) ===");
    var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
    var lastRow = Math.Min(ws.LastRowUsed()?.RowNumber() ?? 0, 5);
    for (int r = 1; r <= lastRow; r++)
    {
        var cells = new List<string>();
        for (int c = 1; c <= lastCol; c++)
            cells.Add($"[{c}]'{ws.Cell(r, c).GetString()}'");
        Console.WriteLine($"R{r}: " + string.Join(" | ", cells));
    }
    Console.WriteLine();
}
