using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KCCMaterialFlow.Module.Securite.Notifications;

/// <summary>
/// SEC-041 à SEC-045: Implémentation du service d'envoi d'emails pour le module Sécurité
/// </summary>
public class SecuriteEmailService : ISecuriteEmailService
{
    private readonly ILogger<SecuriteEmailService> _logger;
    private readonly SecuriteEmailOptions _options;
    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly IEmailSender _emailSender;

    public SecuriteEmailService(
        ILogger<SecuriteEmailService> logger,
        IOptions<SecuriteEmailOptions> options,
        IEmailTemplateRenderer templateRenderer,
        IEmailSender emailSender)
    {
        _logger = logger;
        _options = options.Value;
        _templateRenderer = templateRenderer;
        _emailSender = emailSender;
    }

    /// <inheritdoc />
    public async Task SendAnomalieNotificationAsync(AnomalieEmailModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("SEC-042: Envoi notification anomalie {AnomalieId} à Investigation", model.AnomalieId);

            // Définir les destinataires Investigation
            var toEmails = model.ToInvestigation.Any() 
                ? model.ToInvestigation 
                : _options.InvestigationEmails;

            // SEC-043: Ajouter CC Identification pour les anomalies critiques/élevées
            var ccEmails = new List<string>();
            if (model.NiveauGravite is "Critique" or "Eleve")
            {
                ccEmails.AddRange(model.CcIdentification.Any() 
                    ? model.CcIdentification 
                    : _options.IdentificationEmails);
                
                _logger.LogInformation("SEC-043: Ajout CC Identification pour anomalie {Gravite}", model.NiveauGravite);
            }

            // Rendre le template
            var htmlBody = await _templateRenderer.RenderAsync("AnomalieDetectee", model, cancellationToken);

            // Construire le sujet
            var subject = BuildAnomalieSubject(model);

            // Envoyer
            await _emailSender.SendAsync(
                to: toEmails,
                cc: ccEmails,
                subject: subject,
                htmlBody: htmlBody,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Notification anomalie {AnomalieId} envoyée à {Count} destinataires", 
                model.AnomalieId, toEmails.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de la notification anomalie {AnomalieId}", model.AnomalieId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendScanConformeNotificationAsync(ScanConformeEmailModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("SEC-044: Envoi confirmation passage {NumeroPreuve}", model.NumeroPreuve);

            var htmlBody = await _templateRenderer.RenderAsync("ScanConforme", model, cancellationToken);

            var subject = $"[KCC Material Flow] Passage confirmé - {model.NumeroReference} - {model.NomBarriere}";

            await _emailSender.SendAsync(
                to: new[] { model.ToEmail },
                cc: Enumerable.Empty<string>(),
                subject: subject,
                htmlBody: htmlBody,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Confirmation passage {NumeroPreuve} envoyée à {Email}", 
                model.NumeroPreuve, model.ToEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de la confirmation de passage {NumeroPreuve}", model.NumeroPreuve);
            // Ne pas propager l'erreur pour les confirmations optionnelles
        }
    }

    /// <inheritdoc />
    public async Task SendPretExpirantAlertAsync(PretExpirantEmailModel model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("SEC-045: Envoi alerte prêt expirant {NumeroReference} (J-{Jours})", 
                model.NumeroReference, model.JoursRestants);

            var htmlBody = await _templateRenderer.RenderAsync("PretExpirant", model, cancellationToken);

            var subject = $"[ALERTE] Prêt expirant J-{model.JoursRestants} - {model.NumeroReference}";

            await _emailSender.SendAsync(
                to: new[] { model.ToEmail },
                cc: model.CcEmails,
                subject: subject,
                htmlBody: htmlBody,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Alerte prêt expirant {NumeroReference} envoyée", model.NumeroReference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de l'alerte prêt expirant {NumeroReference}", model.NumeroReference);
            throw;
        }
    }

    private static string BuildAnomalieSubject(AnomalieEmailModel model)
    {
        var gravitePrefix = model.NiveauGravite switch
        {
            "Critique" => "🚨 [CRITIQUE]",
            "Eleve" => "⚠️ [ÉLEVÉE]",
            "Moyen" => "[MOYENNE]",
            _ => "[INFO]"
        };

        var docInfo = !string.IsNullOrEmpty(model.NumeroReference) 
            ? $" - {model.NumeroReference}" 
            : "";

        return $"{gravitePrefix} Anomalie détectée{docInfo} - {model.NomBarriere}";
    }
}

#region Options et Interfaces

/// <summary>
/// Options de configuration pour les emails du module Sécurité
/// </summary>
public class SecuriteEmailOptions
{
    public const string SectionName = "Securite:Email";

    /// <summary>
    /// Emails de l'équipe Investigation
    /// </summary>
    public List<string> InvestigationEmails { get; set; } = new();

    /// <summary>
    /// Emails de l'équipe Identification
    /// </summary>
    public List<string> IdentificationEmails { get; set; } = new();

    /// <summary>
    /// URL de base de l'application pour les liens
    /// </summary>
    public string BaseUrl { get; set; } = "https://kccmaterialflow.kcc.cd";

    /// <summary>
    /// Activer l'envoi d'emails de confirmation de passage
    /// </summary>
    public bool EnableScanConformeEmails { get; set; } = false;
}

/// <summary>
/// Interface pour le rendu des templates email
/// </summary>
public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync<TModel>(string templateName, TModel model, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface pour l'envoi d'emails
/// </summary>
public interface IEmailSender
{
    Task SendAsync(
        IEnumerable<string> to,
        IEnumerable<string> cc,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}

#endregion
