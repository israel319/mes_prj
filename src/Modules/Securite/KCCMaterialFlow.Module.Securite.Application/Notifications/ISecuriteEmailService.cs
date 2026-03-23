namespace KCCMaterialFlow.Module.Securite.Notifications;

/// <summary>
/// SEC-041 à SEC-045: Interface du service d'envoi d'emails pour le module Sécurité
/// </summary>
public interface ISecuriteEmailService
{
    /// <summary>
    /// SEC-042: Envoie une notification d'anomalie à Investigation
    /// SEC-043: Avec copie à Identification
    /// </summary>
    Task SendAnomalieNotificationAsync(AnomalieEmailModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// SEC-044: Envoie une confirmation de passage (scan conforme)
    /// </summary>
    Task SendScanConformeNotificationAsync(ScanConformeEmailModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// SEC-045: Envoie une alerte de prêt expirant (J-7)
    /// </summary>
    Task SendPretExpirantAlertAsync(PretExpirantEmailModel model, CancellationToken cancellationToken = default);
}

#region Email Models

/// <summary>
/// Modèle pour l'email d'anomalie détectée
/// </summary>
public class AnomalieEmailModel
{
    public int AnomalieId { get; set; }
    public string TypeAnomalie { get; set; } = string.Empty;
    public string NiveauGravite { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateSignalement { get; set; }
    
    // Document concerné
    public string? NumeroReference { get; set; }
    public string? TypeBon { get; set; }
    
    // Barrière
    public string? NomBarriere { get; set; }
    public string? LocalisationBarriere { get; set; }
    
    // Agent qui a signalé
    public string? AgentNom { get; set; }
    public string? AgentLogin { get; set; }
    
    // Trajet
    public string? Provenance { get; set; }
    public string? Destination { get; set; }
    
    // Destinataires
    public List<string> ToInvestigation { get; set; } = new();
    public List<string> CcIdentification { get; set; } = new();
    
    // Lien direct
    public string? LinkToAnomalie { get; set; }
}

/// <summary>
/// Modèle pour l'email de scan conforme
/// </summary>
public class ScanConformeEmailModel
{
    public int ScanId { get; set; }
    public string NumeroPreuve { get; set; } = string.Empty;
    public DateTime DateHeureScan { get; set; }
    
    // Document
    public string NumeroReference { get; set; } = string.Empty;
    public string TypeBon { get; set; } = string.Empty;
    
    // Barrière
    public string NomBarriere { get; set; } = string.Empty;
    
    // Trajet
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    
    // Matériels
    public int NombreMateriels { get; set; }
    public List<string> MaterielsDescriptions { get; set; } = new();
    
    // Agent
    public string AgentNom { get; set; } = string.Empty;
    
    // Destinataire
    public string ToEmail { get; set; } = string.Empty;
    public string ToNom { get; set; } = string.Empty;
}

/// <summary>
/// Modèle pour l'email de prêt expirant
/// </summary>
public class PretExpirantEmailModel
{
    public int BonId { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string TypeBon { get; set; } = string.Empty;
    public DateTime DateExpiration { get; set; }
    public int JoursRestants { get; set; }
    
    // Demandeur
    public string DemandeurNom { get; set; } = string.Empty;
    public string DemandeurDepartement { get; set; } = string.Empty;
    
    // Matériels
    public int NombreMateriels { get; set; }
    public List<MaterielPretInfo> Materiels { get; set; } = new();
    
    // Destinataires
    public string ToEmail { get; set; } = string.Empty;
    public List<string> CcEmails { get; set; } = new();
    
    // Lien
    public string? LinkToBon { get; set; }
}

public class MaterielPretInfo
{
    public string Reference { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int Quantite { get; set; }
}

#endregion
