namespace KCCMaterialFlow.Module.BonEntree.DTOs;

/// <summary>
/// DTO pour l'affichage complet d'un bon d'entrée - Champs selon diagramme UML
/// Base Bon: 8 champs + BonEntree: 8 champs spécifiques
/// </summary>
public class BonEntreeViewDto
{
    #region Base Bon (8 champs)

    /// <summary>
    /// Identifiant unique du bon
    /// </summary>
    public int IdBon { get; set; }

    /// <summary>
    /// Numéro de référence unique (BEM-YYYY-NNNNNN)
    /// </summary>
    public string NumeroReference { get; set; } = string.Empty;

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime DateCreation { get; set; }

    /// <summary>
    /// Date d'expiration du bon
    /// </summary>
    public DateTime DateExpiration { get; set; }

    /// <summary>
    /// Statut actuel (string: Draft, PendingSup, PendingGM, PendingOPJ, Approved, Rejected, InTransit, Completed)
    /// </summary>
    public string StatutActuel { get; set; } = "Draft";

    /// <summary>
    /// Destination du matériel
    /// </summary>
    public string Destination { get; set; } = string.Empty;

    /// <summary>
    /// Provenance du matériel
    /// </summary>
    public string Provenance { get; set; } = string.Empty;

    /// <summary>
    /// Description générale
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Quantité totale
    /// </summary>
    public int Quantite { get; set; }

    #endregion

    #region BonEntree Spécifique (8 champs)

    /// <summary>
    /// Numéro du contrat
    /// </summary>
    public string? NumeroContrat { get; set; }

    /// <summary>
    /// Nom de la compagnie/contractant
    /// </summary>
    public string NomCompagnie { get; set; } = string.Empty;

    /// <summary>
    /// Email du contractant
    /// </summary>
    public string? EmailContractant { get; set; }

    /// <summary>
    /// Nom du Site Manager responsable
    /// </summary>
    public string SiteManager { get; set; } = string.Empty;

    /// <summary>
    /// Département hôte qui reçoit le matériel
    /// </summary>
    public string HostDepartment { get; set; } = string.Empty;

    /// <summary>
    /// Motif de la présence sur site
    /// </summary>
    public string ReasonOnSite { get; set; } = string.Empty;

    /// <summary>
    /// Nom de l'escorteur
    /// </summary>
    public string? NomEscorteur { get; set; }

    /// <summary>
    /// Fonction de l'escorteur
    /// </summary>
    public string? FonctionEscorteur { get; set; }

    #endregion

    #region Collections

    /// <summary>
    /// Liste des matériels
    /// </summary>
    public List<MaterielDto> Materiels { get; set; } = [];

    /// <summary>
    /// Liste des approbations
    /// </summary>
    public List<ApprobationDto> Approbations { get; set; } = [];

    #endregion

    #region Helpers

    /// <summary>
    /// Libellé du statut
    /// </summary>
    public string StatutLibelle => StatutActuel switch
    {
        "Draft" => "Brouillon",
        "PendingSup" => "Att. Superviseur",
        "PendingGM" => "Att. GM",
        "PendingOPJ" => "Att. OPJ",
        "Approved" => "Approuvé",
        "Rejected" => "Rejeté",
        "InTransit" => "En transit",
        "Completed" => "Terminé",
        "Cancelled" => "Annulé",
        _ => StatutActuel
    };

    /// <summary>
    /// Classe CSS pour le badge de statut
    /// </summary>
    public string StatutBadgeClass => StatutActuel switch
    {
        "Draft" => "rz-background-color-info-lighter",
        "PendingSup" or "PendingGM" or "PendingOPJ" => "rz-background-color-warning-lighter",
        "Approved" => "rz-background-color-success-lighter",
        "InTransit" => "rz-background-color-primary-lighter",
        "Completed" => "rz-background-color-success",
        "Rejected" or "Cancelled" => "rz-background-color-danger-lighter",
        _ => "rz-background-color-base-300"
    };

    #endregion
}
