using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Service d'envoi d'emails via SMTP
/// </summary>
public class EmailNotificationService : IEmailNotificationService, IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly bool _enableSsl;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _isEnabled;
    private readonly string[] _investigationEmails;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(
        IConfiguration configuration,
        ILogger<EmailNotificationService> logger)
    {
        _smtpHost = configuration["Smtp:Host"] ?? "smtp.office365.com";
        _smtpPort = configuration.GetValue<int>("Smtp:Port", 587);
        _enableSsl = configuration.GetValue<bool>("Smtp:EnableSsl", true);
        _smtpUsername = configuration["Smtp:Username"] ?? "";
        _smtpPassword = configuration["Smtp:Password"] ?? "";
        _fromEmail = configuration["Smtp:FromEmail"] ?? "noreply@kcc.cd";
        _fromName = configuration["Smtp:FromName"] ?? "KCC Material Flow";
        _isEnabled = configuration.GetValue<bool>("Smtp:Enabled", true);
        _investigationEmails = configuration.GetSection("Securite:Email:InvestigationEmails").Get<string[]>() 
            ?? new[] { "israel.kasa@kamotocopper.com" };
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendEmailAsync(string to, string subject, string body, string[]? cc = null, CancellationToken cancellationToken = default)
    {
        if (!_isEnabled)
        {
            _logger.LogWarning("Email désactivé. Email non envoyé à {To}", to);
            return;
        }

        _logger.LogInformation("Tentative d'envoi d'email à {To} via SMTP {Host}:{Port}", to, _smtpHost, _smtpPort);

        try
        {
            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = _enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000 // 30 secondes timeout
            };

            // Utiliser les credentials seulement si configurés (relay interne = pas de credentials)
            if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            }
            else
            {
                smtpClient.UseDefaultCredentials = false;
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            // Ajouter les destinataires (séparés par ;)
            foreach (var recipient in to.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.To.Add(recipient.Trim());
            }

            // Ajouter les CC
            if (cc is { Length: > 0 })
            {
                foreach (var ccRecipient in cc)
                {
                    mailMessage.CC.Add(ccRecipient.Trim());
                }
            }

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            _logger.LogInformation("✅ Email envoyé avec succès à {To}. Sujet: {Subject}", to, subject);
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(smtpEx, "❌ Erreur SMTP lors de l'envoi à {To}. Code: {StatusCode}, Message: {Message}", 
                to, smtpEx.StatusCode, smtpEx.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur générale lors de l'envoi de l'email à {To}", to);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendEmailAsync(string[] recipients, string subject, string body, string[]? cc = null, CancellationToken cancellationToken = default)
    {
        var to = string.Join(";", recipients);
        await SendEmailAsync(to, subject, body, cc, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SendBonNotificationAsync<T>(
        string templateName, 
        T model, 
        string[] recipients, 
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var body = GenerateEmailBody(model, templateName);
            var subject = GetSubjectForTemplate(templateName, model);

            await SendEmailAsync(recipients, subject, body, null, cancellationToken);

            _logger.LogInformation("Notification {Template} envoyée à {Count} destinataires", 
                templateName, recipients.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de la notification {Template}", templateName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendWorkflowNotificationAsync(
        string notificationType, 
        int bonId, 
        string bonType, 
        string recipientEmail, 
        Dictionary<string, object>? additionalData = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = GetWorkflowSubject(notificationType, bonId, bonType);
            var body = GenerateWorkflowBody(notificationType, bonId, bonType, additionalData);

            await SendEmailAsync(recipientEmail, subject, body, null, cancellationToken);

            _logger.LogInformation("Notification workflow {Type} envoyée pour bon {BonId} ({BonType})", 
                notificationType, bonId, bonType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de la notification workflow {Type} pour bon {BonId}", 
                notificationType, bonId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendAnomalieAlertAsync(
        int anomalieId, 
        string description, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Récupérer les destinataires de l'équipe Investigation depuis la configuration
            var recipients = new[] { "israel.kasa@kamotocopper.com" };

            var subject = $"🚨 ALERTE: Anomalie #{anomalieId} détectée";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #dc3545;'>⚠️ Anomalie Détectée</h2>
                    <p><strong>ID Anomalie:</strong> #{anomalieId}</p>
                    <p><strong>Description:</strong></p>
                    <div style='background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107;'>
                        {description}
                    </div>
                    <p style='margin-top: 20px;'>
                        Veuillez vérifier cette anomalie dès que possible.
                    </p>
                    <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                    <p style='color: #6c757d; font-size: 12px;'>
                        Ce message a été généré automatiquement par KCC Material Flow.
                    </p>
                </body>
                </html>";

            await SendEmailAsync(recipients, subject, body, null, cancellationToken);

            _logger.LogWarning("Alerte anomalie #{AnomalieId} envoyée", anomalieId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de l'alerte anomalie #{AnomalieId}", anomalieId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendPretExpirationAlertAsync(
        int bonId, 
        DateTime dateRetour, 
        int joursRestants, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Récupérer l'email du demandeur depuis le bon
            var recipientEmail = "demandeur@kcc.cd";

            var urgencyColor = joursRestants <= 1 ? "#dc3545" : joursRestants <= 3 ? "#ffc107" : "#17a2b8";
            var urgencyText = joursRestants <= 0 ? "RETARD" : joursRestants == 1 ? "DEMAIN" : $"{joursRestants} jours";

            var subject = $"⏰ Rappel: Prêt #{bonId} - Retour prévu {(joursRestants <= 0 ? "EN RETARD" : $"dans {urgencyText}")}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: {urgencyColor};'>⏰ Rappel de Retour de Prêt</h2>
                    <p><strong>Bon:</strong> #{bonId}</p>
                    <p><strong>Date de retour prévue:</strong> {dateRetour:dd/MM/yyyy}</p>
                    <div style='background-color: #e9ecef; padding: 15px; border-radius: 5px; text-align: center;'>
                        <span style='font-size: 24px; color: {urgencyColor}; font-weight: bold;'>
                            {urgencyText}
                        </span>
                    </div>
                    <p style='margin-top: 20px;'>
                        Veuillez préparer le retour du matériel ou demander une extension si nécessaire.
                    </p>
                    <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                    <p style='color: #6c757d; font-size: 12px;'>
                        Ce message a été généré automatiquement par KCC Material Flow.
                    </p>
                </body>
                </html>";

            await SendEmailAsync(recipientEmail, subject, body, null, cancellationToken);

            _logger.LogInformation("Rappel expiration prêt #{BonId} envoyé. Retour prévu: {Date}, Jours restants: {Days}", 
                bonId, dateRetour, joursRestants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi du rappel pour prêt #{BonId}", bonId);
            throw;
        }
    }

    private static string GetWorkflowSubject(string notificationType, int bonId, string bonType)
    {
        return notificationType.ToLower() switch
        {
            "pendingapproval" => $"⏳ Bon {bonType} #{bonId} - En attente de votre approbation",
            "approved" => $"✅ Bon {bonType} #{bonId} - Approuvé",
            "rejected" => $"❌ Bon {bonType} #{bonId} - Rejeté",
            "returned" => $"↩️ Bon {bonType} #{bonId} - Retourné pour modification",
            "created" => $"📝 Nouveau Bon {bonType} #{bonId} créé",
            "completed" => $"🎉 Bon {bonType} #{bonId} - Complété",
            _ => $"📋 Bon {bonType} #{bonId} - {notificationType}"
        };
    }

    private static string GenerateWorkflowBody(string notificationType, int bonId, string bonType, Dictionary<string, object>? additionalData)
    {
        var statusEmoji = notificationType.ToLower() switch
        {
            "pendingapproval" => "⏳",
            "approved" => "✅",
            "rejected" => "❌",
            "returned" => "↩️",
            "created" => "📝",
            "completed" => "🎉",
            _ => "📋"
        };

        var additionalInfo = string.Empty;
        if (additionalData != null && additionalData.Count > 0)
        {
            additionalInfo = "<h3>Détails:</h3><ul>" + string.Join("", 
                additionalData.Select(kv => $"<li><strong>{kv.Key}:</strong> {kv.Value}</li>")) + "</ul>";
        }

        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>{statusEmoji} {notificationType}</h2>
                <p><strong>Type:</strong> {bonType}</p>
                <p><strong>Numéro:</strong> #{bonId}</p>
                {additionalInfo}
                <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                <p style='color: #6c757d; font-size: 12px;'>
                    Ce message a été généré automatiquement par KCC Material Flow.
                </p>
            </body>
            </html>";
    }

    private static string GenerateEmailBody<T>(T model, string templateName) where T : class
    {
        // TODO: Implémenter un système de templates plus sophistiqué (Razor, Scriban, etc.)
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Notification - {templateName}</h2>
                <p>Une action a été effectuée sur un bon de matériel.</p>
                <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                <p style='color: #6c757d; font-size: 12px;'>
                    Ce message a été généré automatiquement par KCC Material Flow.
                </p>
            </body>
            </html>";
    }

    private static string GetSubjectForTemplate<T>(string templateName, T model) where T : class
    {
        return $"KCC Material Flow - {templateName}";
    }

    /// <inheritdoc />
    public async Task SendRejectionNotificationAsync(
        string bonType,
        string numeroReference,
        string etapeRejet,
        string approbateurNom,
        string motifRejet,
        string demandeurNom,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("📧 Envoi notification de rejet pour {BonType} {NumeroRef} à {Emails}",
            bonType, numeroReference, string.Join(", ", _investigationEmails));

        try
        {
            var subject = $"❌ REJET: Bon {bonType} {numeroReference} - Rejeté par {etapeRejet}";
            
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #dc3545; color: white; padding: 20px; text-align: center;'>
                        <h1 style='margin: 0;'>❌ BON REJETÉ</h1>
                    </div>
                    
                    <div style='padding: 20px; background-color: #f8f9fa;'>
                        <h2 style='color: #333; border-bottom: 2px solid #dc3545; padding-bottom: 10px;'>
                            Détails du Rejet
                        </h2>
                        
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd; font-weight: bold; width: 40%;'>Type de Bon:</td>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{bonType}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd; font-weight: bold;'>Numéro de Référence:</td>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd; font-weight: bold; color: #dc3545;'>{numeroReference}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd; font-weight: bold;'>Demandeur:</td>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{demandeurNom}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd; font-weight: bold;'>Étape de Rejet:</td>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{etapeRejet}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd; font-weight: bold;'>Rejeté par:</td>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{approbateurNom}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd; font-weight: bold;'>Date du Rejet:</td>
                                <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{DateTime.Now:dd/MM/yyyy HH:mm}</td>
                            </tr>
                        </table>
                        
                        <div style='margin-top: 20px; padding: 15px; background-color: #fff3cd; border-left: 4px solid #ffc107; border-radius: 4px;'>
                            <h3 style='margin-top: 0; color: #856404;'>📝 Motif du Rejet:</h3>
                            <p style='margin-bottom: 0; color: #856404; font-size: 14px;'>{motifRejet}</p>
                        </div>
                    </div>
                    
                    <div style='padding: 15px; background-color: #e9ecef; text-align: center;'>
                        <p style='color: #6c757d; font-size: 12px; margin: 0;'>
                            Ce message a été généré automatiquement par KCC Material Flow.<br/>
                            Équipe Investigation - Sécurité
                        </p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(_investigationEmails, subject, body, null, cancellationToken);

            _logger.LogWarning("✅ Notification de rejet envoyée pour {BonType} {NumeroRef}. Motif: {Motif}",
                bonType, numeroReference, motifRejet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ ÉCHEC envoi notification de rejet pour {BonType} {NumeroRef}. Erreur: {Error}",
                bonType, numeroReference, ex.Message);
            // Ne pas propager l'exception pour ne pas bloquer le workflow de rejet
        }
    }

    // --- IEmailService implementation ---

    async Task IEmailService.SendAsync(string to, string subject, string body, CancellationToken ct)
    {
        await SendEmailAsync(to, subject, body, null, ct);
    }

    async Task IEmailService.SendRejectionNotificationAsync(string bonNumero, string motif, string rejectedBy, CancellationToken ct)
    {
        await SendRejectionNotificationAsync(
            bonType: "BEM",
            numeroReference: bonNumero,
            etapeRejet: "Approbation",
            approbateurNom: rejectedBy,
            motifRejet: motif,
            demandeurNom: string.Empty,
            cancellationToken: ct);
    }
}
