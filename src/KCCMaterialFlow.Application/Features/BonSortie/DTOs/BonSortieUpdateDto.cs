using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Application.Features.BonSortie.DTOs;

/// <summary>
/// BSM-035: DTO pour la mise à jour d'un bon de sortie.
/// </summary>
public class BonSortieUpdateDto
{
    public int IdBon { get; set; }

    public string? Description { get; set; }
    public string? MotifSortie { get; set; }
    public DateTime? DateExpiration { get; set; }

    // Spécifiques Externe
    public string? NomDestinataire { get; set; }
    public string? AdresseDestination { get; set; }
    public string? NumeroVehicule { get; set; }
    public string? NomChauffeur { get; set; }
    public string? TelephoneChauffeur { get; set; }

    // Spécifiques Interne
    public string? NomReceveur { get; set; }
    public string? FonctionReceveur { get; set; }
    public string? EmailReceveur { get; set; }
    public string? LocalisationDestination { get; set; }
    public DateTime? DateTransfertPrevue { get; set; }

    // Spécifiques Prêt
    public DateTime? DateRetourPrevue { get; set; }

    // Matériels
    public List<MaterielSortieUpdateDto>? Materiels { get; set; }

    // Itinéraires
    public List<int>? BarrieresIds { get; set; }
}

/// <summary>
/// DTO pour mise à jour d'un matériel
/// </summary>
public class MaterielSortieUpdateDto
{
    public int? IdMateriel { get; set; }
    public string CodeProduitSerial { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Quantite { get; set; } = 1;
    public string? Provenance { get; set; }
    public string? Destination { get; set; }
}
