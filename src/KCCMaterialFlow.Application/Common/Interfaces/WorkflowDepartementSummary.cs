namespace KCCMaterialFlow.Application.Common.Interfaces;

public sealed class WorkflowDepartementSummary
{
    public string? DepartementCode { get; init; }
    public string DepartementNom { get; init; } = string.Empty;
    public bool EstPersonnalise { get; init; }
    public int NombreEmployes { get; init; }
    public List<string> EtapesConfigurees { get; init; } = new();
    public string Source { get; init; } = string.Empty;
}

public sealed class DepartementInfo
{
    public string Code { get; init; } = string.Empty;
    public string Nom { get; init; } = string.Empty;
    public int NombreEmployes { get; init; }
}

public sealed class WorkflowRoleDisponible
{
    public string RoleCode { get; init; } = string.Empty;
    public string Nom { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Icone { get; init; } = string.Empty;
    public string Couleur { get; init; } = string.Empty;
    public bool EstAutoResolu { get; init; }

    public static IReadOnlyList<WorkflowRoleDisponible> TousLesRoles => new List<WorkflowRoleDisponible>
    {
        new() { RoleCode = "SUPERINTENDENT", Nom = "Superintendent / Manager", Description = "Résolu automatiquement depuis la hiérarchie Glencore.", Icone = "manage_accounts", Couleur = "#6366f1", EstAutoResolu = true },
        new() { RoleCode = "GM",             Nom = "General Manager",           Description = "Résolu automatiquement depuis la hiérarchie Glencore.", Icone = "supervisor_account", Couleur = "#8b5cf6", EstAutoResolu = true },
        new() { RoleCode = "IT",             Nom = "Département IT",            Description = "Approbateur IT — à configurer dans Approbateurs Spéciaux.", Icone = "computer", Couleur = "#0ea5e9", EstAutoResolu = false },
        new() { RoleCode = "ENVIRONMENT",    Nom = "Environnement",             Description = "Approbateur Environnement — à configurer dans Approbateurs Spéciaux.", Icone = "eco", Couleur = "#22c55e", EstAutoResolu = false },
        new() { RoleCode = "OPJ",            Nom = "OPJ",                       Description = "Officier de Police Judiciaire.", Icone = "gavel", Couleur = "#f59e0b", EstAutoResolu = false },
        new() { RoleCode = "IDENTIFICATION", Nom = "Identification",            Description = "Étape finale d'identification — génération du QR code.", Icone = "qr_code_scanner", Couleur = "#ef4444", EstAutoResolu = false },
    };
}
