using KCCMaterialFlow.Module.Shared.Entities;

namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Service de gestion des notifications de rejet
/// </summary>
public interface INotificationRejetService
{
    /// <summary>
    /// Enregistre une notification de rejet en base de données
    /// </summary>
    Task<NotificationRejet> EnregistrerRejetAsync(
        string bonType,
        string numeroReference,
        string etapeRejet,
        string approbateurNom,
        string? approbateurLogin,
        string motifRejet,
        string? demandeurNom,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les notifications non lues pour l'équipe d'investigation
    /// </summary>
    Task<IEnumerable<NotificationRejet>> GetNotificationsNonLuesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère toutes les notifications avec filtres optionnels
    /// </summary>
    Task<IEnumerable<NotificationRejet>> GetNotificationsAsync(
        string? bonType = null,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        bool? estLue = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marque une notification comme lue
    /// </summary>
    Task MarquerCommeLueAsync(int notificationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marque plusieurs notifications comme lues
    /// </summary>
    Task MarquerCommeLuesAsync(IEnumerable<int> notificationIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compte les notifications non lues
    /// </summary>
    Task<int> CompterNotificationsNonLuesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la dernière notification de rejet pour un bon spécifique
    /// </summary>
    Task<NotificationRejet?> GetLatestByBonAsync(string bonType, int bonId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la dernière notification de rejet par numéro de référence
    /// </summary>
    Task<NotificationRejet?> GetByNumeroReferenceAsync(string numeroReference, CancellationToken cancellationToken = default);
}
