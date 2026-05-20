using ClosedXML.Excel;

var src = @"c:\Users\ikasa\Downloads\Book2.xlsx";
var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "book2_copy.xlsx");
System.IO.File.Copy(src, path, true);
using var wb = new XLWorkbook(path);
foreach (var ws in wb.Worksheets)
{
    var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;
    var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
    Console.WriteLine($"=== Sheet: {ws.Name} (rows: {lastRow}, cols: {lastCol}) ===");
    var preview = Math.Min(lastRow, 12);
    for (int r = 1; r <= preview; r++)
    {
        var cells = new List<string>();
        for (int c = 1; c <= lastCol; c++)
            cells.Add($"[{c}]'{ws.Cell(r, c).GetString()}'");
        Console.WriteLine($"R{r}: " + string.Join(" | ", cells));
    }
    Console.WriteLine();
    int colsToScan = Math.Min(lastCol, 15);
    for (int c = 1; c <= colsToScan; c++)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int r = 2; r <= lastRow; r++) {
            var v = ws.Cell(r,c).GetString();
            if (!string.IsNullOrWhiteSpace(v)) set.Add(v);
        }
        var hdr = ws.Cell(1,c).GetString();
        Console.WriteLine($"COL {c} '{hdr}' uniques={set.Count}" + (set.Count <= 40 ? " => " + string.Join(" | ", set) : ""));
    }
}
