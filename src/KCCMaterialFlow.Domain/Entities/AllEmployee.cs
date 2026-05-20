using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Référentiel des employés Glencore importé depuis Book2.xlsx (snapshot).
/// Contient toute la hiérarchie (ReportTo / SuperIntendent / ManagerHod / GM)
/// nécessaire au workflow d'approbation, ainsi que le UserName Windows
/// (ex. ANYACCESS\bkamosi) utilisé pour l'authentification.
///
/// Cette table est REMPLIE/MISE À JOUR par GlencoreEmployeeImportService et
/// ne remplace PAS la table T_Employees (qui reste la source applicative
/// des comptes utilisateur effectifs).
/// </summary>
public sealed class AllEmployee : BaseEntity
{
    /// <summary>Code employé Glencore (ex. K00001). Unique.</summary>
    [MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(50)]
    public string? DepartementCode { get; set; }

    [MaxLength(200)]
    public string? Departement { get; set; }

    /// <summary>Login Windows complet (ex. ANYACCESS\bkamosi). Indexé pour lookup auth.</summary>
    [MaxLength(150)]
    public string? UserName { get; set; }

    [MaxLength(200)]
    public string? Mail { get; set; }

    [MaxLength(50)]
    public string? ReportsToEmployeeCode { get; set; }

    [MaxLength(200)]
    public string? ReportsToEmployeeDisplay { get; set; }

    [MaxLength(50)]
    public string? SuperIntendentEmployeeCode { get; set; }

    [MaxLength(200)]
    public string? SuperIntendentEmployeeDisplay { get; set; }

    [MaxLength(50)]
    public string? ManagerHodEmployeeCode { get; set; }

    [MaxLength(200)]
    public string? ManagerHodEmployeeDisplay { get; set; }

    [MaxLength(50)]
    public string? GmEmployeeCode { get; set; }

    [MaxLength(200)]
    public string? GmEmployeeDisplay { get; set; }

    public DateTime DateImport { get; set; } = DateTime.Now;
}
