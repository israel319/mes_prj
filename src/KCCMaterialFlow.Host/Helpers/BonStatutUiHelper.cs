using KCCMaterialFlow.Domain.Constants;

namespace KCCMaterialFlow.Host.Helpers;

/// <summary>
/// Helper de présentation pour les statuts de bons.
/// Logique CSS/icônes séparée du Domain (Clean Architecture).
/// </summary>
public static class BonStatutUiHelper
{
    public static string GetBadgeClass(string statut) => statut switch
    {
        BonStatuts.Draft => "bg-secondary",
        BonStatuts.PendingSup or BonStatuts.PendingGM or BonStatuts.PendingOPJ => "bg-warning text-dark",
        BonStatuts.Approved => "bg-success",
        BonStatuts.InTransit => "bg-info",
        BonStatuts.Completed => "bg-primary",
        BonStatuts.Rejected or BonStatuts.Cancelled => "bg-danger",
        _ => "bg-secondary"
    };

    public static string GetIcon(string statut) => statut switch
    {
        BonStatuts.Draft => "edit",
        BonStatuts.PendingSup or BonStatuts.PendingGM or BonStatuts.PendingOPJ => "hourglass_empty",
        BonStatuts.Approved => "check_circle",
        BonStatuts.InTransit => "local_shipping",
        BonStatuts.Completed => "done_all",
        BonStatuts.Rejected => "cancel",
        BonStatuts.Cancelled => "block",
        _ => "info"
    };
}
