using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

public interface INotificationRejetService
{
    Task<NotificationRejet> EnregistrerRejetAsync(
        string bonType,
        string numeroReference,
        string etapeRejet,
        string approbateurNom,
        string? approbateurLogin,
        string motifRejet,
        string? demandeurNom,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<NotificationRejet>> GetNotificationsNonLuesAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<NotificationRejet>> GetNotificationsAsync(
        string? bonType = null,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        bool? estLue = null,
        CancellationToken cancellationToken = default);

    Task MarquerCommeLueAsync(int notificationId, CancellationToken cancellationToken = default);
    Task MarquerCommeLuesAsync(IEnumerable<int> notificationIds, CancellationToken cancellationToken = default);
    Task<int> CompterNotificationsNonLuesAsync(CancellationToken cancellationToken = default);
    Task<NotificationRejet?> GetLatestByBonAsync(string bonType, int bonId, CancellationToken cancellationToken = default);
    Task<NotificationRejet?> GetByNumeroReferenceAsync(string numeroReference, CancellationToken cancellationToken = default);
}
