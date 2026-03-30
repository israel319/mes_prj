namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Service d'envoi d'emails.
/// </summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
    Task SendRejectionNotificationAsync(string bonNumero, string motif, string rejectedBy, CancellationToken ct = default);
}
