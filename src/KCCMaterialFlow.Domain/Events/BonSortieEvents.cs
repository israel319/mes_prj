using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Events;

public sealed record BonSortieCreatedEvent(int BonSortieId, string TypeSortie) : BaseEvent;

public sealed record BonSortieSubmittedEvent(
    int BonSortieId,
    string NumeroReference) : BaseEvent;

public sealed record BonSortieApprovedEvent(
    int BonSortieId,
    string NumeroReference,
    string ApprovedBy,
    int? BonEntreeId) : BaseEvent;

public sealed record BonSortieRejectedEvent(
    int BonSortieId,
    string NumeroReference,
    string RejectedBy,
    string Motif) : BaseEvent;

public sealed record PretRetourneEvent(
    int PretId,
    string NumeroReference,
    DateTime DateRetour) : BaseEvent;

public sealed record PretExpirationProche(
    int PretId,
    string NumeroReference,
    DateTime DateRetourPrevue,
    int JoursRestants) : BaseEvent;
