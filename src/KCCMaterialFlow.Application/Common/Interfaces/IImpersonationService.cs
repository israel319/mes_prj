namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Indique si l'utilisateur courant est en session d'impersonation
/// (démarrée via /api/impersonation/start, arrêtée via /api/impersonation/stop).
/// L'état est stocké dans un cookie HTTP sécurisé posé par les endpoints API.
/// </summary>
public interface IImpersonationService
{
    /// <summary>True si une impersonation est active pour la requête courante.</summary>
    bool IsImpersonating { get; }

    /// <summary>Matricule / EmployeeCode de l'utilisateur impersonné, ou null.</summary>
    string? ImpersonatedMatricule { get; }

    /// <summary>Login Windows de l'utilisateur impersonné (ex. ANYACCESS\K22233), ou null si non disponible.</summary>
    string? ImpersonatedLogin { get; }

    /// <summary>Login Windows réel (avant impersonation), ou null.</summary>
    string? RealLogin { get; }
}
