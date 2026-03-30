using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Events;

public sealed record ScanProcessedEvent(
    int ScanId,
    int BarriereId,
    string QRCode,
    bool EstValide) : BaseEvent;

public sealed record AnomalieSignaleeEvent(
    int AnomalieId,
    int? ScanId,
    string Description) : BaseEvent;

public sealed record AnomalieTraiteeEvent(
    int AnomalieId,
    string TraiteePar,
    string Action) : BaseEvent;
