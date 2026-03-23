namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// Interface pour le service d'envoi de notifications par email
/// </summary>
public interface IEmailNotificationService
{
    /// <summary>
    /// Envoie un email simple
    /// </summary>
    /// <param name="to">Adresse email du destinataire</param>
    /// <param name="subject">Sujet de l'email</param>
    /// <param name="body">Corps du message (HTML)</param>
    /// <param name="cc">Adresses en copie (optionnel)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task SendEmailAsync(string to, string subject, string body, string[]? cc = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envoie un email à plusieurs destinataires
    /// </summary>
    /// <param name="recipients">Liste des adresses email</param>
    /// <param name="subject">Sujet de l'email</param>
    /// <param name="body">Corps du message (HTML)</param>
    /// <param name="cc">Adresses en copie (optionnel)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task SendEmailAsync(string[] recipients, string subject, string body, string[]? cc = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envoie une notification liée à un bon (création, approbation, rejet, etc.)
    /// </summary>
    /// <typeparam name="T">Type du modèle de données du bon</typeparam>
    /// <param name="templateName">Nom du template email à utiliser</param>
    /// <param name="model">Données du bon pour le template</param>
    /// <param name="recipients">Destinataires de l'email</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task SendBonNotificationAsync<T>(string templateName, T model, string[] recipients, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Envoie une notification de workflow (approbation requise, bon approuvé, etc.)
    /// </summary>
    /// <param name="notificationType">Type de notification (PendingApproval, Approved, Rejected, Returned)</param>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="bonType">Type de bon (BEM, BSM)</param>
    /// <param name="recipientEmail">Email du destinataire</param>
    /// <param name="additionalData">Données supplémentaires pour le template</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task SendWorkflowNotificationAsync(string notificationType, int bonId, string bonType, string recipientEmail, Dictionary<string, object>? additionalData = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envoie une alerte d'anomalie à l'équipe Investigation
    /// </summary>
    /// <param name="anomalieId">Identifiant de l'anomalie</param>
    /// <param name="description">Description de l'anomalie</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task SendAnomalieAlertAsync(int anomalieId, string description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envoie une alerte pour un prêt expirant bientôt
    /// </summary>
    /// <param name="bonId">Identifiant du bon de prêt</param>
    /// <param name="dateRetour">Date de retour prévue</param>
    /// <param name="joursRestants">Nombre de jours restants</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task SendPretExpirationAlertAsync(int bonId, DateTime dateRetour, int joursRestants, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envoie une notification de rejet d'un bon à l'équipe d'investigation
    /// </summary>
    /// <param name="bonType">Type de bon (BEM, BSM)</param>
    /// <param name="numeroReference">Numéro de référence du bon</param>
    /// <param name="etapeRejet">Nom de l'étape qui a rejeté</param>
    /// <param name="approbateurNom">Nom de l'approbateur qui a rejeté</param>
    /// <param name="motifRejet">Motif/raison du rejet</param>
    /// <param name="demandeurNom">Nom du demandeur du bon</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task SendRejectionNotificationAsync(
        string bonType,
        string numeroReference,
        string etapeRejet,
        string approbateurNom,
        string motifRejet,
        string demandeurNom,
        CancellationToken cancellationToken = default);
}
