using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KCCMaterialFlow.Infrastructure.Services.Securite;

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

            var toEmails = model.ToInvestigation.Count > 0
                ? model.ToInvestigation
                : _options.InvestigationEmails;

            var ccEmails = new List<string>();
            if (model.NiveauGravite is "Critique" or "Eleve")
            {
                ccEmails.AddRange(model.CcIdentification.Count > 0
                    ? model.CcIdentification
                    : _options.IdentificationEmails);

                _logger.LogInformation("SEC-043: Ajout CC Identification pour anomalie {Gravite}", model.NiveauGravite);
            }

            var htmlBody = await _templateRenderer.RenderAsync("AnomalieDetectee", model, cancellationToken);

            var subject = BuildAnomalieSubject(model);

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
