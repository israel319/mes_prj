using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Module.Securite.Notifications;

/// <summary>
/// SEC-045: Service de fond pour envoyer les alertes de prêts expirants (J-7)
/// Exécute une fois par jour à 8h00
/// </summary>
public class PretExpirantAlertService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PretExpirantAlertService> _logger;
    private readonly TimeSpan _executionTime = TimeSpan.FromHours(8); // 8h00 du matin

    public PretExpirantAlertService(
        IServiceScopeFactory scopeFactory,
        ILogger<PretExpirantAlertService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SEC-045: Service d'alerte prêts expirants démarré");

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = GetNextRunTime(now);
            var delay = nextRun - now;

            _logger.LogInformation("Prochaine exécution des alertes prêts: {NextRun}", nextRun);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                await SendPretsExpirantsAlertsAsync(stoppingToken);
            }
        }

        _logger.LogInformation("Service d'alerte prêts expirants arrêté");
    }

    private DateTime GetNextRunTime(DateTime now)
    {
        var todayExecution = now.Date.Add(_executionTime);
        
        // Si l'heure est déjà passée aujourd'hui, planifier pour demain
        return now > todayExecution 
            ? todayExecution.AddDays(1) 
            : todayExecution;
    }

    private async Task SendPretsExpirantsAlertsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SEC-045: Début de l'envoi des alertes prêts expirants");

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<ISecuriteEmailService>();
            var pretExpirantProvider = scope.ServiceProvider.GetService<IPretExpirantProvider>();

            if (pretExpirantProvider == null)
            {
                _logger.LogWarning("IPretExpirantProvider non enregistré - alertes prêts désactivées");
                return;
            }

            // Récupérer les prêts expirant dans 7 jours
            var pretsExpirants = await pretExpirantProvider.GetPretsExpirantsAsync(7, cancellationToken);

            var alertsSent = 0;
            foreach (var pret in pretsExpirants)
            {
                try
                {
                    var model = new PretExpirantEmailModel
                    {
                        BonId = pret.BonId,
                        NumeroReference = pret.NumeroReference,
                        TypeBon = pret.TypeBon,
                        DateExpiration = pret.DateExpiration,
                        JoursRestants = (pret.DateExpiration - DateTime.Today).Days,
                        DemandeurNom = pret.DemandeurNom,
                        DemandeurDepartement = pret.DemandeurDepartement,
                        NombreMateriels = pret.Materiels.Count,
                        Materiels = pret.Materiels,
                        ToEmail = pret.EmailDemandeur,
                        CcEmails = pret.CcEmails,
                        LinkToBon = pret.LinkToBon
                    };

                    await emailService.SendPretExpirantAlertAsync(model, cancellationToken);
                    alertsSent++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur envoi alerte prêt {NumeroReference}", pret.NumeroReference);
                }
            }

            _logger.LogInformation("SEC-045: {Count} alertes prêts expirants envoyées", alertsSent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du traitement des alertes prêts expirants");
        }
    }
}

/// <summary>
/// Interface pour fournir les prêts expirants depuis le module concerné (Prêt/Sortie)
/// </summary>
public interface IPretExpirantProvider
{
    Task<IEnumerable<PretExpirantInfo>> GetPretsExpirantsAsync(int joursAvantExpiration, CancellationToken cancellationToken = default);
}

/// <summary>
/// Informations sur un prêt expirant
/// </summary>
public class PretExpirantInfo
{
    public int BonId { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string TypeBon { get; set; } = string.Empty;
    public DateTime DateExpiration { get; set; }
    public string DemandeurNom { get; set; } = string.Empty;
    public string DemandeurDepartement { get; set; } = string.Empty;
    public string EmailDemandeur { get; set; } = string.Empty;
    public List<string> CcEmails { get; set; } = new();
    public List<MaterielPretInfo> Materiels { get; set; } = new();
    public string? LinkToBon { get; set; }
}
