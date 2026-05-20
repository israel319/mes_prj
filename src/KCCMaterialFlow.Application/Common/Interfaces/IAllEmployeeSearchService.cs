using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Recherche dans le référentiel T_AllEmployees (autocomplete formulaires).
/// </summary>
public interface IAllEmployeeSearchService
{
    /// <summary>
    /// Recherche par fragment (insensible casse) dans EmployeeCode, FirstName, LastName,
    /// UserName, Departement, Mail. Retourne les actifs en priorité.
    /// </summary>
    Task<IReadOnlyList<AllEmployee>> SearchAsync(
        string? fragment, int maxResults = 25, CancellationToken ct = default);

    /// <summary>Charge une fiche par EmployeeCode (PK). Null si introuvable.</summary>
    Task<AllEmployee?> GetByCodeAsync(string employeeCode, CancellationToken ct = default);

    /// <summary>Recherche le nom d'affichage du Manager HOD (ManagerHodEmployeeCode → ManagerHodEmployeeDisplay).</summary>
    Task<string?> GetSiteManagerForAsync(string requestedForEmployeeCode, CancellationToken ct = default);

    /// <summary>Retourne la liste triée des valeurs distinctes de Departement (non nulles) dans GlencoreEmployees.</summary>
    Task<IReadOnlyList<string>> GetDistinctDepartementsAsync(CancellationToken ct = default);
}
