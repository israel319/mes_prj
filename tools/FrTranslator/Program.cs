using System.Text.RegularExpressions;
using System.Xml.Linq;

string scriptPath = @"c:\Users\ikasa\source\repos\Materiels\src\fr_translate_v2.ps1";
string resxPath = @"c:\Users\ikasa\source\repos\Materiels\src\KCCMaterialFlow.Host\Resources\SharedResource.fr.resx";

// Parse PS hashtable lines: 'Key' = 'Value with ''escaped'' quotes'
var dict = new Dictionary<string, string>(StringComparer.Ordinal);
var rx = new Regex(@"^\s*'([^']+)'\s*=\s*'((?:[^']|'')*)'\s*$");
foreach (var line in File.ReadAllLines(scriptPath))
{
    var m = rx.Match(line);
    if (m.Success)
    {
        var k = m.Groups[1].Value;
        var v = m.Groups[2].Value.Replace("''", "'");
        dict[k] = v;
    }
}
Console.WriteLine($"Parsed {dict.Count} translation entries from script.");

// Load resx
var doc = XDocument.Load(resxPath, LoadOptions.PreserveWhitespace);
int applied = 0, missing = 0;
var seenInDict = new HashSet<string>();

foreach (var data in doc.Root!.Elements("data"))
{
    var name = (string?)data.Attribute("name");
    if (name == null) continue;
    if (dict.TryGetValue(name, out var newVal))
    {
        var valElem = data.Element("value");
        if (valElem == null)
        {
            valElem = new XElement("value");
            data.Add(valElem);
        }
        if (valElem.Value != newVal)
        {
            valElem.Value = newVal;
            applied++;
        }
        seenInDict.Add(name);
    }
}
foreach (var k in dict.Keys)
    if (!seenInDict.Contains(k)) missing++;

// Deduplicate: keep first occurrence per name
var groups = doc.Root!.Elements("data")
    .GroupBy(e => (string?)e.Attribute("name") ?? "")
    .Where(g => g.Count() > 1)
    .ToList();
int duplicatesRemoved = 0;
foreach (var g in groups)
{
    foreach (var dup in g.Skip(1))
    {
        dup.Remove();
        duplicatesRemoved++;
    }
}

doc.Save(resxPath);
Console.WriteLine($"Applied={applied} Missing(not-in-resx)={missing} DuplicatesRemoved={duplicatesRemoved}");
