namespace KCCMaterialFlow.Domain.Common;

/// <summary>
/// Classe de base abstraite pour les entités auditables
/// </summary>
public abstract class AuditableEntity : IEntity, IAuditableEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Classe de base abstraite pour les entités auditables avec soft delete
/// </summary>
public abstract class AuditableSoftDeleteEntity : AuditableEntity, ISoftDelete
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
