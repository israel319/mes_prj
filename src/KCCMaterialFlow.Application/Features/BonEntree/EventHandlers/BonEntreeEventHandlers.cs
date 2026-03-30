using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Application.Features.BonEntree.EventHandlers;

/// <summary>
/// MediatR notification wrapper for domain events.
/// The Domain layer has no MediatR dependency, so we wrap IDomainEvent
/// in a MediatR INotification for dispatching through the pipeline.
/// Infrastructure is responsible for publishing DomainEventNotification&lt;T&gt;
/// after SaveChanges via an interceptor.
/// </summary>
public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent)
    : INotification where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}

public sealed class BonEntreeCreatedEventHandler(
    ILogger<BonEntreeCreatedEventHandler> logger)
    : INotificationHandler<DomainEventNotification<BonEntreeCreatedEvent>>
{
    public Task Handle(DomainEventNotification<BonEntreeCreatedEvent> notification, CancellationToken ct)
    {
        logger.LogInformation(
            "BonEntree {BonEntreeId} created.", notification.DomainEvent.BonEntreeId);

        return Task.CompletedTask;
    }
}

public sealed class BonEntreeApprovedEventHandler(
    ILogger<BonEntreeApprovedEventHandler> logger)
    : INotificationHandler<DomainEventNotification<BonEntreeApprovedEvent>>
{
    public Task Handle(DomainEventNotification<BonEntreeApprovedEvent> notification, CancellationToken ct)
    {
        var evt = notification.DomainEvent;
        logger.LogInformation(
            "BonEntree {NumeroReference} approved by {ApprovedBy}. {AncienStatut} -> {NouveauStatut}.",
            evt.NumeroReference,
            evt.ApprovedBy,
            evt.AncienStatut,
            evt.NouveauStatut);

        return Task.CompletedTask;
    }
}

public sealed class BonEntreeRejectedEventHandler(
    ILogger<BonEntreeRejectedEventHandler> logger,
    IEmailService emailService)
    : INotificationHandler<DomainEventNotification<BonEntreeRejectedEvent>>
{
    public async Task Handle(DomainEventNotification<BonEntreeRejectedEvent> notification, CancellationToken ct)
    {
        var evt = notification.DomainEvent;
        logger.LogInformation(
            "BonEntree {NumeroReference} rejected by {RejectedBy}. Motif: {Motif}.",
            evt.NumeroReference,
            evt.RejectedBy,
            evt.Motif);

        await emailService.SendRejectionNotificationAsync(
            evt.NumeroReference,
            evt.Motif,
            evt.RejectedBy,
            ct);
    }
}
