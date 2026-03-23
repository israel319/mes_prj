namespace KCCMaterialFlow.Domain.Common;

/// <summary>
/// Interface pour les entités avec suppression logique (soft delete)
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Indique si l'entité est supprimée
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Date de suppression
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur qui a supprimé l'entité
    /// </summary>
    string? DeletedBy { get; set; }
}
