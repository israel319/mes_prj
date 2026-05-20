using System.Text.RegularExpressions;
using System.Xml.Linq;

string scriptPath = @"c:\Users\ikasa\source\repos\Materiels\src\fr_translate_v2.ps1";
string frResx = @"c:\Users\ikasa\source\repos\Materiels\src\KCCMaterialFlow.Host\Resources\SharedResource.fr.resx";
string enResx = @"c:\Users\ikasa\source\repos\Materiels\src\KCCMaterialFlow.Host\Resources\SharedResource.resx";

var frTr = new Dictionary<string, string>(StringComparer.Ordinal);
var rx = new Regex(@"^\s*'([^']+)'\s*=\s*'((?:[^']|'')*)'\s*$");
foreach (var line in File.ReadAllLines(scriptPath))
{
    var m = rx.Match(line);
    if (m.Success) frTr[m.Groups[1].Value] = m.Groups[2].Value.Replace("''", "'");
}
Console.WriteLine($"Parsed {frTr.Count} FR translations from script.");

var newKeys = new Dictionary<string, (string en, string fr)>(StringComparer.Ordinal)
{
    ["Dash_LivePortal"] = ("Live Portal", "Portail en direct"),
    ["ListeBons_TabFallback"] = ("Vouchers", "Bons"),
    ["Wfl_GenericTitle"] = ("Generic workflow", "Workflow générique"),
    ["Wfl_NotifSavedTitle"] = ("Saved", "Enregistré"),
    ["Wfl_NotifSavedMsg"] = ("Workflow updated successfully.", "Workflow mis à jour avec succès."),
    ["Wfl_NotifResetTitle"] = ("Reset", "Réinitialisé"),
    ["Wfl_NotifResetMsg"] = ("Workflow for {0} removed.", "Workflow de {0} supprimé."),
    ["Wfl_ConfirmResetTitle"] = ("Reset workflow", "Réinitialiser le workflow"),
    ["Wfl_ConfirmResetMsg"] = ("Remove the specific configuration for <strong>{0}</strong>? The department will inherit the generic workflow.", "Supprimer la configuration spécifique de <strong>{0}</strong> ? Le département héritera du workflow générique."),
};

void Process(string path, Dictionary<string, string>? translations, Dictionary<string, string> addIfMissing, string label)
{
    var doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
    int translated = 0, dupRemoved = 0, added = 0;

    if (translations != null)
    {
        foreach (var data in doc.Root!.Elements("data"))
        {
            var name = (string?)data.Attribute("name");
            if (name == null) continue;
            if (translations.TryGetValue(name, out var newVal))
            {
                var v = data.Element("value");
                if (v == null) { v = new XElement("value"); data.Add(v); }
                if (v.Value != newVal) { v.Value = newVal; translated++; }
            }
        }
    }

    var groups = doc.Root!.Elements("data")
        .GroupBy(e => (string?)e.Attribute("name") ?? "")
        .Where(g => g.Count() > 1).ToList();
    foreach (var g in groups)
        foreach (var dup in g.Skip(1)) { dup.Remove(); dupRemoved++; }

    var existing = new HashSet<string>(doc.Root!.Elements("data")
        .Select(e => (string?)e.Attribute("name") ?? ""), StringComparer.Ordinal);
    foreach (var kv in addIfMissing)
    {
        if (!existing.Contains(kv.Key))
        {
            doc.Root!.Add(new XElement("data",
                new XAttribute("name", kv.Key),
                new XAttribute(XNamespace.Xml + "space", "preserve"),
                new XElement("value", kv.Value)));
            added++;
        }
    }

    doc.Save(path);
    Console.WriteLine($"[{label}] translated={translated} dupRemoved={dupRemoved} added={added}");
}

Process(frResx, frTr,
    addIfMissing: newKeys.ToDictionary(k => k.Key, k => k.Value.fr, StringComparer.Ordinal),
    label: "FR");
Process(enResx, translations: null,
    addIfMissing: newKeys.ToDictionary(k => k.Key, k => k.Value.en, StringComparer.Ordinal),
    label: "EN");

var frDoc = XDocument.Load(frResx);
var enDoc = XDocument.Load(enResx);
var frKeys = new HashSet<string>(frDoc.Root!.Elements("data").Select(e => (string?)e.Attribute("name") ?? ""));
var enKeys = new HashSet<string>(enDoc.Root!.Elements("data").Select(e => (string?)e.Attribute("name") ?? ""));
var inFrNotEn = frKeys.Except(enKeys).OrderBy(s => s).ToList();
var inEnNotFr = enKeys.Except(frKeys).OrderBy(s => s).ToList();
Console.WriteLine($"\n=== PARITY ===");
Console.WriteLine($"FR keys: {frKeys.Count}, EN keys: {enKeys.Count}");
Console.WriteLine($"In FR not EN ({inFrNotEn.Count})");
Console.WriteLine($"In EN not FR ({inEnNotFr.Count})");

File.WriteAllLines(@"c:\Users\ikasa\source\repos\Materiels\src\parity_fr_only.txt", inFrNotEn);
File.WriteAllLines(@"c:\Users\ikasa\source\repos\Materiels\src\parity_en_only.txt", inEnNotFr);
