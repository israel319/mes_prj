namespace KCCMaterialFlow.Application.Features.BonEntree.DTOs;

/// <summary>
/// DTO pour l'affichage complet d'un bon d'entrée - Champs selon diagramme UML
/// </summary>
public class BonEntreeViewDto
{
    #region Base Bon (8 champs)

    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public DateTime DateExpiration { get; set; }
    public string StatutActuel { get; set; } = "Draft";
    public string Destination { get; set; } = string.Empty;
    public string Provenance { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantite { get; set; }

    #endregion

    #region BonEntree Spécifique (8 champs)

    public string? NumeroContrat { get; set; }
    public string NomCompagnie { get; set; } = string.Empty;
    public string? EmailContractant { get; set; }
    public string SiteManager { get; set; } = string.Empty;
    public string HostDepartment { get; set; } = string.Empty;
    public string ReasonOnSite { get; set; } = string.Empty;
    public string? NomEscorteur { get; set; }
    public string? FonctionEscorteur { get; set; }

    #endregion

    #region Collections

    public List<BonEntreeMaterielDto> Materiels { get; set; } = [];
    public List<ApprobationDto> Approbations { get; set; } = [];

    #endregion

    #region Helpers

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
