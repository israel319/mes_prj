using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using AppPlusPlus.Application.Interfaces;

namespace AppPlusPlus.Infrastructure.ExternalServices;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_config["Smtp:Host"]);

    public async Task SendAsync(string toAddress, string subject, string bodyHtml,
        byte[]? attachmentBytes = null, string? attachmentName = null)
    {
        var smtp = _config.GetSection("Smtp");
        var host = smtp["Host"] ?? throw new InvalidOperationException("SMTP non configuré (Smtp:Host manquant dans appsettings.json).");
        var port = int.Parse(smtp["Port"] ?? "587");
        var user = smtp["User"] ?? "";
        var pass = smtp["Password"] ?? "";
        var from = smtp["From"] ?? user;
        var useSsl = bool.Parse(smtp["UseSsl"] ?? "true");

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = bodyHtml };

        if (attachmentBytes is not null && attachmentName is not null)
        {
            var contentType = attachmentName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
                ? new ContentType("application", "vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                : new ContentType("application", "octet-stream");

            builder.Attachments.Add(attachmentName, attachmentBytes, contentType);
        }

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        var sslOption = port == 465
            ? MailKit.Security.SecureSocketOptions.SslOnConnect
            : (useSsl ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.Auto);
        await client.ConnectAsync(host, port, sslOption);

        if (!string.IsNullOrEmpty(user))
            await client.AuthenticateAsync(user, pass);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
