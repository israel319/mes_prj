namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Niveau d'administration de l'utilisateur dans le système.
/// Stocké directement sur Employee (Employee = Utilisateur).
/// </summary>
public enum NiveauAdmin
{
    /// <summary>Utilisateur standard (Employee normal sans privilège admin).</summary>
    Aucun = 0,

    /// <summary>
    /// Admin : gestion des Employees standards + accès métier complet.
    /// NE PEUT PAS gérer les SuperAdmin, importer des données critiques, ni utiliser l'impersonation.
    /// </summary>
    Admin = 1,

    /// <summary>
    /// SuperAdmin : accès total sans restriction.
    /// Peut gérer Admin + SuperAdmin, importer données critiques (matricules), utiliser impersonation.
    /// </summary>
    SuperAdmin = 2
}
