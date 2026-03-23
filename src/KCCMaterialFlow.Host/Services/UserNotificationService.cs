using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.BonEntree.Services;
using KCCMaterialFlow.Module.BonSortie.Services;

namespace KCCMaterialFlow.Host.Services;

/// <summary>
/// Service centralisé de notifications utilisateur.
/// Agrège les notifications de tous les modules (bons retournés, approbations en attente, etc.)
/// </summary>
public interface IUserNotificationService
{
    /// <summary>
    /// Récupère toutes les notifications de l'utilisateur courant
    /// </summary>
    Task<UserNotifications> GetNotificationsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implémentation du service de notifications
/// </summary>
public class UserNotificationService : IUserNotificationService
{
    private readonly IBonEntreeService _bonEntreeService;
    private readonly IBonSortieService _bonSortieService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UserNotificationService> _logger;

    public UserNotificationService(
        IBonEntreeService bonEntreeService,
        IBonSortieService bonSortieService,
        ICurrentUserService currentUserService,
        ILogger<UserNotificationService> logger)
    {
        _bonEntreeService = bonEntreeService;
        _bonSortieService = bonSortieService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<UserNotifications> GetNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var notifications = new UserNotifications();
        
        try
        {
            // Récupérer les BSM retournés
            var bsmReturned = await _bonSortieService.GetMyReturnedBonsAsync(cancellationToken);
            notifications.ReturnedBons.AddRange(bsmReturned);
            
            // Récupérer les BEM retournés (si disponible)
            var bemReturned = await _bonEntreeService.GetMyReturnedBonsAsync(cancellationToken);
            notifications.ReturnedBons.AddRange(bemReturned.Select(b => new ReturnedBonInfo
            {
                IdBon = b.IdBon,
                NumeroReference = b.NumeroReference,
                TypeBon = "BEM",
                RaisonRetour = b.RaisonRetour,
                DateRetour = b.DateRetour,
                AuteurRetour = b.AuteurRetour
            }));
            
            // Trier par date décroissante
            notifications.ReturnedBons = notifications.ReturnedBons
                .OrderByDescending(r => r.DateRetour)
                .ToList();
            
            // Récupérer les approbations en attente (pour les approbateurs)
            var roles = _currentUserService.GetUserRoles();
            if (roles.Any(r => r is "Superviseur" or "GM" or "OPJ" or "IT" or "Environnement" or "Identification" or "Admin"))
            {
                var pendingBsm = await _bonSortieService.GetPendingApprovalsAsync(cancellationToken);
                notifications.PendingApprovals = pendingBsm.Count;
                
                var pendingBem = await _bonEntreeService.GetPendingApprovalsAsync(cancellationToken);
                notifications.PendingApprovals += pendingBem.Count;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des notifications");
        }
        
        return notifications;
    }
}

/// <summary>
/// DTO contenant toutes les notifications de l'utilisateur
/// </summary>
public class UserNotifications
{
    /// <summary>
    /// Bons retournés pour modification
    /// </summary>
    public List<ReturnedBonInfo> ReturnedBons { get; set; } = [];
    
    /// <summary>
    /// Nombre d'approbations en attente (pour les approbateurs)
    /// </summary>
    public int PendingApprovals { get; set; }
    
    /// <summary>
    /// Nombre total de notifications
    /// </summary>
    public int TotalCount => ReturnedBons.Count + PendingApprovals;
}
