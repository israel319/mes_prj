using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.BonSortie.Entities;
using KCCMaterialFlow.Module.BonSortie.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Module.BonSortie.Services;

/// <summary>
/// BSM-065: Service de background pour les prêts expirés nécessitant investigation.
/// Envoie des notifications au service Identification pour les prêts en retard critique.
/// Exécuté une fois par jour à 9h00.
/// </summary>
public class PretInvestigationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PretInvestigationService> _logger;

    private static readonly int[] InvestigationThresholds = [7, 14, 30];
    private static readonly TimeSpan ExecutionTime = new(9, 0, 0);

    public PretInvestigationService(
        IServiceProvider serviceProvider,
        ILogger<PretInvestigationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Service d'investigation des prêts expirés démarré");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                var nextRun = now.Date.Add(ExecutionTime);
                if (now >= nextRun)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;
                _logger.LogDebug("Prochaine vérification investigation: {NextRun}", nextRun);

                await Task.Delay(delay, stoppingToken);
                await CheckOverdueLoansForInvestigationAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification d'investigation");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        _logger.LogInformation("Service d'investigation des prêts expirés arrêté");
    }

    private async Task CheckOverdueLoansForInvestigationAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BSM-065: Vérification des prêts nécessitant investigation...");

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IBonSortieRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();

        var overdueLoans = await repository.GetOverdueLoansAsync(cancellationToken);
        var investigationsSent = 0;

        foreach (var pret in overdueLoans)
        {
            try
            {
                var joursRetard = pret.JoursRetard;

                if (InvestigationThresholds.Contains(joursRetard))
                {
                    await SendInvestigationNotificationAsync(emailService, pret, joursRetard, cancellationToken);
                    investigationsSent++;

                    _logger.LogWarning("BSM-065: Investigation envoyée pour prêt {Numero} - {Jours}j retard",
                        pret.NumeroReference, joursRetard);
                }

                if (joursRetard > 30 && joursRetard % 7 == 0)
                {
                    await SendCriticalAlertAsync(emailService, pret, joursRetard, cancellationToken);
                    investigationsSent++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur investigation prêt {Id}", pret.IdBon);
            }
        }

        var criticalLoans = overdueLoans.Where(p => p.JoursRetard >= 14).ToList();
        if (criticalLoans.Any())
            await SendDailySummaryAsync(emailService, criticalLoans, cancellationToken);

        _logger.LogInformation("BSM-065: {Count} notifications investigation envoyées", investigationsSent);
    }

    private async Task SendInvestigationNotificationAsync(
        IEmailNotificationService emailService, Pret pret, int joursRetard, CancellationToken cancellationToken)
    {
        var severity = joursRetard >= 30 ? "CRITIQUE" : joursRetard >= 14 ? "IMPORTANTE" : "REQUISE";
        var subject = $"🔍 INVESTIGATION {severity}: Prêt {pret.NumeroReference} - Retard {joursRetard} jours";

        var body = $@"
            <h2>Investigation {severity} - Prêt non retourné</h2>
            <p><strong>Numéro:</strong> {pret.NumeroReference}</p>
            <p><strong>Demandeur:</strong> {pret.NomDemandeur} ({pret.DepartementDemandeur})</p>
            <p><strong>Date retour prévue:</strong> {pret.DateRetourPrevue:dd/MM/yyyy}</p>
            <p><strong>Retard:</strong> <span style='color:red;font-size:18px;'>{joursRetard} JOURS</span></p>
            <p><strong>Matériels:</strong> {pret.Materiels.Count} articles</p>
            <hr>
            <p>Action requise: Contacter le demandeur et documenter la situation.</p>";

        string[]? cc = joursRetard >= 14 ? ["superviseur.identification@kcc.cd"] : null;

        await emailService.SendEmailAsync("identification@kcc.cd", subject, body, cc, cancellationToken);
    }

    private async Task SendCriticalAlertAsync(
        IEmailNotificationService emailService, Pret pret, int joursRetard, CancellationToken cancellationToken)
    {
        var subject = $"🚨 ALERTE CRITIQUE: Prêt {pret.NumeroReference} - {joursRetard} jours de retard";
        var body = $"<h2>Situation critique - Suivi hebdomadaire</h2><p>Prêt {pret.NumeroReference} en retard de {joursRetard} jours.</p>";

        await emailService.SendEmailAsync("direction.identification@kcc.cd", subject, body, 
            ["identification@kcc.cd"], cancellationToken);
    }

    private async Task SendDailySummaryAsync(
        IEmailNotificationService emailService, List<Pret> criticalLoans, CancellationToken cancellationToken)
    {
        var subject = $"📊 Rapport Quotidien - {criticalLoans.Count} Prêts Critiques ({DateTime.Now:dd/MM/yyyy})";
        var rows = string.Join("", criticalLoans.OrderByDescending(p => p.JoursRetard)
            .Select(p => $"<tr><td>{p.NumeroReference}</td><td>{p.NomDemandeur}</td><td>{p.JoursRetard}j</td></tr>"));

        var body = $@"
            <h2>Prêts en retard critique</h2>
            <table border='1' cellpadding='5'>
                <tr><th>Référence</th><th>Demandeur</th><th>Retard</th></tr>
                {rows}
            </table>";

        await emailService.SendEmailAsync("identification@kcc.cd", subject, body, null, cancellationToken);
    }
}
