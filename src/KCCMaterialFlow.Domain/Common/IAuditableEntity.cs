namespace KCCMaterialFlow.Domain.Common;

/// <summary>
/// Interface pour les entités avec champs d'audit (création/modification)
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Date de création de l'entité
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière modification
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur qui a créé l'entité
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur qui a modifié l'entité
    /// </summary>
    string? UpdatedBy { get; set; }
}
