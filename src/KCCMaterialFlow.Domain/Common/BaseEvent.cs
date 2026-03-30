namespace KCCMaterialFlow.Domain.Common;

/// <summary>
/// Marqueur pour les Domain Events.
/// Pas de dépendance sur MediatR — c'est l'Infrastructure
/// qui fait le pont via INotification.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public abstract record BaseEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
