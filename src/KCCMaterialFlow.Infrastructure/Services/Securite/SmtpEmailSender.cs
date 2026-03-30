using System.Net;
using System.Net.Mail;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KCCMaterialFlow.Infrastructure.Services.Securite;

/// <summary>
/// Implémentation SMTP de l'envoi d'emails
/// </summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly ILogger<SmtpEmailSender> _logger;
    private readonly SmtpOptions _options;

    public SmtpEmailSender(
        ILogger<SmtpEmailSender> logger,
        IOptions<SmtpOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task SendAsync(
        IEnumerable<string> to,
        IEnumerable<string> cc,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogWarning("SMTP désactivé - Email non envoyé: {Subject}", subject);
            return;
        }

        var toList = to.ToList();
        if (toList.Count == 0)
        {
            _logger.LogWarning("Aucun destinataire pour l'email: {Subject}", subject);
            return;
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_options.FromEmail, _options.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            foreach (var email in toList)
            {
                message.To.Add(new MailAddress(email));
            }

            foreach (var email in cc)
            {
                message.CC.Add(new MailAddress(email));
            }

            using var client = CreateSmtpClient();
            await client.SendMailAsync(message, cancellationToken);

            _logger.LogInformation("Email envoyé: {Subject} -> {To}", subject, string.Join(", ", toList));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur d'envoi email: {Subject}", subject);
            throw;
        }
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        if (!string.IsNullOrEmpty(_options.Username))
        {
            client.Credentials = new NetworkCredential(_options.Username, _options.Password);
        }

        return client;
    }
}

/// <summary>
/// Options de configuration SMTP
/// </summary>
public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public bool Enabled { get; set; } = false;
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 25;
    public bool EnableSsl { get; set; } = false;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string FromEmail { get; set; } = "noreply@localhost";
    public string FromName { get; set; } = "KCC Material Flow";
}
