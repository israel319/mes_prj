using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Domain.Events;

public sealed record BonEntreeCreatedEvent(int BonEntreeId) : BaseEvent;

public sealed record BonEntreeSubmittedEvent(
    int BonEntreeId,
    string NumeroReference) : BaseEvent;

public sealed record BonEntreeApprovedEvent(
    int BonEntreeId,
    string NumeroReference,
    string ApprovedBy,
    StatutBonEntree AncienStatut,
    StatutBonEntree NouveauStatut) : BaseEvent;

public sealed record BonEntreeRejectedEvent(
    int BonEntreeId,
    string NumeroReference,
    string RejectedBy,
    string Motif) : BaseEvent;

public sealed record BonEntreeReturnedEvent(
    int BonEntreeId,
    string NumeroReference,
    string ReturnedBy,
    string Motif) : BaseEvent;

public sealed record BonEntreeLockedEvent(
    int BonEntreeId,
    int BonSortieId) : BaseEvent;
