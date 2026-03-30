namespace KCCMaterialFlow.Application.Features.BonEntree.DTOs;

/// <summary>
/// DTO pour la liste des bons d'entrée - Version allégée
/// </summary>
public class BonEntreeListDto
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public string StatutActuel { get; set; } = "Draft";
    public string NomCompagnie { get; set; } = string.Empty;
    public string NomDemandeur { get; set; } = string.Empty;
    public string SiteManager { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public int NombreMateriels { get; set; }

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
}

/// <summary>
/// Résultat de recherche paginé
/// </summary>
public class BonEntreeListResultDto
{
    public List<BonEntreeListDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Critères de recherche
/// </summary>
public class BonEntreeSearchCriteriaDto
{
    public string? SearchTerm { get; set; }
    public string? Statut { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int Skip { get; set; }
    public int PageSize { get; set; } = 20;
}
