using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services.BonSortie;

/// <summary>
/// BSM-028: Service de background pour envoyer les alertes email
/// pour les prêts expirant dans les prochains jours (J-7, J-3, J-1, J0, J+N retard).
/// Exécuté une fois par jour à 8h00.
/// </summary>
public class PretExpirationAlertService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PretExpirationAlertService> _logger;

    // Configuration des alertes
    private static readonly int[] AlertDays = [7, 3, 1, 0, -1, -3, -7]; // J-7, J-3, J-1, Jour J, Retard J+1, J+3, J+7
    private static readonly TimeSpan ExecutionTime = new(8, 0, 0); // 8h00 du matin

    public PretExpirationAlertService(
        IServiceProvider serviceProvider,
        ILogger<PretExpirationAlertService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Service d'alerte expiration prêts démarré");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                var nextRun = now.Date.Add(ExecutionTime);
                if (now >= nextRun)
                {
                    nextRun = nextRun.AddDays(1);
                }

                var delay = nextRun - now;
                _logger.LogDebug("Prochaine vérification des prêts: {NextRun} (dans {Delay})", nextRun, delay);

                await Task.Delay(delay, stoppingToken);

                await CheckExpiringLoansAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification des prêts expirant");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        _logger.LogInformation("Service d'alerte expiration prêts arrêté");
    }

    private async Task CheckExpiringLoansAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Démarrage de la vérification des prêts expirant...");

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IBonSortieRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();

        var today = DateTime.Today;
        var alertsSent = 0;

        foreach (var days in AlertDays)
        {
            try
            {
                var targetDate = today.AddDays(days);
                var prets = await GetPretsExpiringOnDateAsync(repository, targetDate, cancellationToken);

                foreach (var pret in prets)
                {
                    try
                    {
                        var joursRestants = (pret.DateRetourPrevue.Date - today).Days;

                        await SendPretAlertAsync(emailService, pret, joursRestants, cancellationToken);
                        alertsSent++;

                        _logger.LogInformation(
                            "Alerte envoyée pour prêt {Numero} - {JoursRestants} jours ({Type})",
                            pret.NumeroReference,
                            joursRestants,
                            joursRestants < 0 ? "RETARD" : joursRestants == 0 ? "JOUR J" : "RAPPEL");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erreur envoi alerte pour prêt {Id}", pret.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des prêts pour J{Days}", days > 0 ? $"-{days}" : $"+{-days}");
            }
        }

        _logger.LogInformation("Vérification terminée. {AlertsSent} alertes envoyées", alertsSent);
    }

    private static async Task<IEnumerable<Domain.Entities.Pret>> GetPretsExpiringOnDateAsync(
        IBonSortieRepository repository,
        DateTime targetDate,
        CancellationToken cancellationToken)
    {
        var activeLoans = await repository.GetActiveLoansAsync(cancellationToken);

        return activeLoans.Where(p =>
            p.DateRetourPrevue.Date == targetDate.Date &&
            p.StatutActuel == "Approved");
    }

    private async Task SendPretAlertAsync(
        IEmailNotificationService emailService,
        Domain.Entities.Pret pret,
        int joursRestants,
        CancellationToken cancellationToken)
    {
        var (alertType, color, icon) = joursRestants switch
        {
            <= -7 => ("RETARD CRITIQUE", "#721c24", "🚨"),
            <= -3 => ("RETARD IMPORTANT", "#856404", "⚠️"),
            < 0 => ("EN RETARD", "#dc3545", "❌"),
            0 => ("JOUR J - RETOUR AUJOURD'HUI", "#fd7e14", "📅"),
            1 => ("RAPPEL - RETOUR DEMAIN", "#ffc107", "⏰"),
            <= 3 => ("RAPPEL", "#17a2b8", "📋"),
            _ => ("INFORMATION", "#6c757d", "ℹ️")
        };

        var materielsHtml = pret.Materiels.Any()
            ? string.Join("", pret.Materiels.Select(m =>
                $"<tr><td style='padding: 8px; border: 1px solid #dee2e6;'>{m.Designation}</td>" +
                $"<td style='padding: 8px; border: 1px solid #dee2e6; text-align: center;'>{m.Quantite}</td></tr>"))
            : "<tr><td colspan='2' style='padding: 8px; color: #6c757d;'>Aucun matériel listé</td></tr>";

        var subject = $"{icon} {alertType}: Prêt {pret.NumeroReference} - Retour {(joursRestants < 0 ? "EN RETARD" : $"prévu le {pret.DateRetourPrevue:dd/MM/yyyy}")}";

        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <div style='background-color: {color}; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0;'>
                    <h2 style='margin: 0;'>{icon} {alertType}</h2>
                </div>

                <div style='padding: 20px; border: 1px solid #dee2e6; border-top: none;'>
                    <h3 style='color: #333; margin-top: 0;'>Détails du Prêt</h3>

                    <table style='width: 100%; margin-bottom: 20px;'>
                        <tr>
                            <td style='padding: 5px 0;'><strong>Numéro:</strong></td>
                            <td>{pret.NumeroReference}</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px 0;'><strong>Demandeur:</strong></td>
                            <td>{pret.NomDemandeur} ({pret.DepartementDemandeur})</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px 0;'><strong>Destinataire:</strong></td>
                            <td>{pret.NomDestinataire ?? "N/A"}</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px 0;'><strong>Date de retour prévue:</strong></td>
                            <td style='color: {color}; font-weight: bold;'>{pret.DateRetourPrevue:dd/MM/yyyy}</td>
                        </tr>
                        <tr>
                            <td style='padding: 5px 0;'><strong>Destination:</strong></td>
                            <td>{pret.Destination}</td>
                        </tr>
                    </table>

                    <h4 style='color: #333;'>Matériels concernés</h4>
                    <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                        <thead>
                            <tr style='background-color: #f8f9fa;'>
                                <th style='padding: 10px; border: 1px solid #dee2e6; text-align: left;'>Description</th>
                                <th style='padding: 10px; border: 1px solid #dee2e6; text-align: center; width: 80px;'>Qté</th>
                            </tr>
                        </thead>
                        <tbody>
                            {materielsHtml}
                        </tbody>
                    </table>

                    <div style='background-color: #e9ecef; padding: 15px; border-radius: 5px; text-align: center;'>
                        <span style='font-size: 20px; color: {color}; font-weight: bold;'>
                            {(joursRestants < 0 ? $"⚠️ EN RETARD DE {-joursRestants} JOUR(S)" : joursRestants == 0 ? "📅 RETOUR PRÉVU AUJOURD'HUI" : $"⏳ {joursRestants} JOUR(S) RESTANT(S)")}
                        </span>
                    </div>

                    {(joursRestants < 0 ? @"
                    <div style='margin-top: 20px; padding: 15px; background-color: #f8d7da; border-left: 4px solid #dc3545; border-radius: 3px;'>
                        <strong>Action requise:</strong><br>
                        Veuillez retourner le matériel immédiatement ou demander une extension via le système KCC Material Flow.
                    </div>" : @"
                    <div style='margin-top: 20px; padding: 15px; background-color: #fff3cd; border-left: 4px solid #ffc107; border-radius: 3px;'>
                        <strong>Rappel:</strong><br>
                        Veuillez préparer le retour du matériel ou demander une extension si nécessaire.
                    </div>")}
                </div>

                <div style='padding: 15px; background-color: #f8f9fa; text-align: center; border-radius: 0 0 5px 5px; border: 1px solid #dee2e6; border-top: none;'>
                    <p style='color: #6c757d; font-size: 12px; margin: 0;'>
                        Ce message a été généré automatiquement par KCC Material Flow.<br>
                        Pour toute question, contactez le service Identification.
                    </p>
                </div>
            </body>
            </html>";

        var demandeurEmail = $"{pret.NomDemandeur.ToLower().Replace(" ", ".")}@kcc.cd";
        var recipients = new List<string> { demandeurEmail };

        string[]? cc = null;
        if (joursRestants <= -3)
        {
            cc = ["identification@kcc.cd"];
        }

        await emailService.SendEmailAsync(recipients[0], subject, body, cc, cancellationToken);
    }
}
