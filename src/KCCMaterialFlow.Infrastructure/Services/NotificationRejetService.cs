using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Service de gestion des notifications de rejet
/// Utilise IDbContextFactory pour éviter les problèmes de concurrence dans Blazor Server.
/// </summary>
public class NotificationRejetService : INotificationRejetService
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;
    private readonly ILogger<NotificationRejetService> _logger;

    public NotificationRejetService(
        IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory,
        ILogger<NotificationRejetService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<NotificationRejet> EnregistrerRejetAsync(
        string bonType,
        string numeroReference,
        string etapeRejet,
        string approbateurNom,
        string? approbateurLogin,
        string motifRejet,
        string? demandeurNom,
        CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        var notification = new NotificationRejet
        {
            BonType = bonType,
            NumeroReference = numeroReference,
            EtapeRejet = etapeRejet,
            ApprobateurNom = approbateurNom,
            ApprobateurLogin = approbateurLogin,
            MotifRejet = motifRejet,
            DemandeurNom = demandeurNom,
            DateRejet = DateTime.Now,
            EstLue = false,
            EmailEnvoye = false
        };

        context.NotificationsRejet.Add(notification);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogWarning("📋 Notification de rejet enregistrée: {BonType} {NumeroRef} - Rejeté par {Approbateur} à l'étape {Etape}. Motif: {Motif}",
            bonType, numeroReference, approbateurNom, etapeRejet, motifRejet);

        return notification;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationRejet>> GetNotificationsNonLuesAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.NotificationsRejet
            .Where(n => !n.EstLue)
            .OrderByDescending(n => n.DateRejet)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<NotificationRejet>> GetNotificationsAsync(
        string? bonType = null,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        bool? estLue = null,
        CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var query = context.NotificationsRejet.AsQueryable();

        if (!string.IsNullOrEmpty(bonType))
            query = query.Where(n => n.BonType == bonType);

        if (dateDebut.HasValue)
            query = query.Where(n => n.DateRejet >= dateDebut.Value);

        if (dateFin.HasValue)
            query = query.Where(n => n.DateRejet <= dateFin.Value);

        if (estLue.HasValue)
            query = query.Where(n => n.EstLue == estLue.Value);

        return await query
            .OrderByDescending(n => n.DateRejet)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task MarquerCommeLueAsync(int notificationId, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var notification = await context.NotificationsRejet
            .FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);

        if (notification != null)
        {
            notification.EstLue = true;
            notification.DateLecture = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task MarquerCommeLuesAsync(IEnumerable<int> notificationIds, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var notifications = await context.NotificationsRejet
            .Where(n => notificationIds.Contains(n.Id))
            .ToListAsync(cancellationToken);

        var now = DateTime.Now;
        foreach (var notification in notifications)
        {
            notification.EstLue = true;
            notification.DateLecture = now;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CompterNotificationsNonLuesAsync(CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.NotificationsRejet
            .CountAsync(n => !n.EstLue, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<NotificationRejet?> GetLatestByBonAsync(string bonType, int bonId, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        // Rechercher par BonType et par le numéro de référence qui contient l'ID
        // Pattern: BSE-2026-000017 pour id=17, ou simplement contient l'ID
        var idFormatted = bonId.ToString("D6"); // 000017
        var idSimple = bonId.ToString(); // 17
        
        return await context.NotificationsRejet
            .Where(n => n.BonType == bonType && 
                        (n.NumeroReference.EndsWith($"-{idFormatted}") || 
                         n.NumeroReference.EndsWith($"-{idSimple}")))
            .OrderByDescending(n => n.DateRejet)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<NotificationRejet?> GetByNumeroReferenceAsync(string numeroReference, CancellationToken cancellationToken = default)
    {
        using var context = _dbContextFactory.CreateDbContext();
        
        return await context.NotificationsRejet
            .Where(n => n.NumeroReference == numeroReference)
            .OrderByDescending(n => n.DateRejet)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
