using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Application.Features.BonSortie.DTOs;

/// <summary>
/// BSM-037: DTO léger pour les listes et recherches de bons de sortie.
/// </summary>
public class BonSortieListDto
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string TypeSortie { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public DateTime DateExpiration { get; set; }
    public string StatutActuel { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;

    // Demandeur
    public string NomDemandeur { get; set; } = string.Empty;
    public string DepartementDemandeur { get; set; } = string.Empty;

    // Compagnie/Destinataire
    public string NomDestinataire { get; set; } = string.Empty;

    // Type de matériel
    public TypeMateriel TypeMateriel { get; set; }
    public string TypeMaterielLibelle => TypeMateriel.ToString();

    // Compteur matériels
    public int NombreMateriels { get; set; }

    // Pour les prêts
    public DateTime? DateRetourPrevue { get; set; }
    public bool? EstRetourne { get; set; }

    // BonEntree associé
    public string? BonEntreeAssocieNumero { get; set; }

    // Métadonnées calculées
    public bool EstExpire => DateExpiration < DateTime.Today;
    public int JoursRestants => (DateExpiration.Date - DateTime.Today).Days;

    public string StatutBadgeClass => StatutActuel switch
    {
        "Draft" => "rz-badge rz-badge-secondary",
        "Approved" => "rz-badge rz-badge-success",
        "Rejected" => "rz-badge rz-badge-danger",
        "Returned" => "rz-badge rz-badge-info",
        "Cancelled" => "rz-badge rz-badge-dark",
        _ when StatutActuel.StartsWith("Pending") => "rz-badge rz-badge-warning",
        _ => "rz-badge rz-badge-light"
    };

    public string TypeSortieIcon => TypeSortie switch
    {
        "Externe" => "logout",
        "Interne" => "swap_horiz",
        "Pret" => "schedule",
        _ => "output"
    };
}
