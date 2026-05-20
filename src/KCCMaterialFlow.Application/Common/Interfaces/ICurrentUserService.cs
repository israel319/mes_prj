using System.Security.Principal;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Identité de l'utilisateur courant (= Employee, identifié par Matricule).
/// Implémenté par Infrastructure/CurrentUserService + Host/EmployeeAdminEnricherService (décorateur).
/// </summary>
public interface ICurrentUserService
{
    // ── API canonique (Employee = Utilisateur) ─────────────────────
    /// <summary>Id de l'Employee correspondant à l'utilisateur connecté, ou null si non identifié.</summary>
    int? EmployeeId { get; }

    /// <summary>Matricule (ex. K26561) extrait du login Windows.</summary>
    string? Matricule { get; }

    /// <summary>Niveau d'administration (Aucun/Admin/SuperAdmin).</summary>
    NiveauAdmin NiveauAdmin { get; }

    /// <summary>True si NiveauAdmin >= Admin.</summary>
    bool IsAdmin { get; }

    /// <summary>True si NiveauAdmin = SuperAdmin.</summary>
    bool IsSuperAdmin { get; }

    /// <summary>True si l'utilisateur peut apparaître dans une chaîne d'approbation : Admin/SuperAdmin, manager ou approbateur spécial actif.</summary>
    bool IsApprover { get; }

    /// <summary>Charge l'Employee complet (avec ReportTo, Departement, etc.) depuis le cache.</summary>
    Employee? GetCurrentEmployee();

    // ── Affichage (DisplayName de Employee est la source unique) ──
    string GetUserDisplayName();
    /// <summary>Prénom de l'utilisateur (Employee.Prenom). Fallback sur le premier mot de DisplayName/NomComplet, sinon Matricule.</summary>
    string GetUserFirstName();
    string? GetUserEmail();
    string? GetUserDepartment();

    // ── Authentification ──
    string GetUserLogin();
    bool IsAuthenticated();

    // ── Profil legacy (pour rétrocompatibilité ; CurrentUserInfo expose Employee.* ) ──
    CurrentUserInfo? GetCurrentUser();

    // ── STUBS LEGACY (à supprimer en Phase 4) ──
    // Conservés pour ne pas casser les ~30 call-sites existants pendant la refonte.
    // Comportement : retournent toujours true / liste vide → la sécurité métier
    // passe désormais par la chaîne d'approbation (ChaineApprobationService).
    bool IsInRole(string role);
    bool IsInAnyRole(params string[] roles);
    IEnumerable<string> GetUserRoles();
    bool HasActivite(string codeActivite);
    bool HasAnyActivite(params string[] codeActivites);
}

/// <summary>
/// Snapshot du profil utilisateur courant, dérivé de Employee.
/// </summary>
public class CurrentUserInfo
{
    /// <summary>Id de l'Employee (ex-UserId).</summary>
    public int UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string? Matricule { get; set; }

    /// <summary>Source unique d'affichage (Employee.DisplayName).</summary>
    public string DisplayName { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? Function { get; set; }
    public string? Department { get; set; }
    public int? DepartmentId { get; set; }

    /// <summary>Niveau d'administration applicatif.</summary>
    public NiveauAdmin NiveauAdmin { get; set; } = NiveauAdmin.Aucun;

    public bool IsActive { get; set; } = true;

    /// <summary>Stub legacy (vide). Conservé pour rétrocompatibilité.</summary>
    public List<string> Roles { get; set; } = new();
}
