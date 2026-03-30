namespace KCCMaterialFlow.Domain.Common;

/// <summary>
/// Classe de base pour toutes les entités.
/// Gère l'identité, l'égalité et les domain events.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>
/// Entité avec champs d'audit automatiques.
/// CreatedBy/UpdatedBy sont set par l'interceptor EF Core dans Infrastructure.
/// </summary>
public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Entité auditable avec soft delete.
/// Le filtre global EF Core exclut automatiquement les IsDeleted=true.
/// </summary>
public abstract class BaseSoftDeleteEntity : BaseAuditableEntity, ISoftDelete
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
