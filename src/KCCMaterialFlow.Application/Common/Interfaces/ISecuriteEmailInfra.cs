namespace KCCMaterialFlow.Application.Common.Interfaces;

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
