using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Application.Features.BonSortie.DTOs;

/// <summary>
/// BSM-036: DTO pour l'affichage complet d'un bon de sortie.
/// </summary>
public class BonSortieViewDto
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string TypeSortie { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public DateTime DateExpiration { get; set; }
    public string StatutActuel { get; set; } = string.Empty;
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantite { get; set; }

    // Demandeur
    public string NomDemandeur { get; set; } = string.Empty;
    public string FonctionDemandeur { get; set; } = string.Empty;
    public string DepartementDemandeur { get; set; } = string.Empty;
    public string MotifSortie { get; set; } = string.Empty;
    public bool EstDefinitif { get; set; }

    // Type de matériel
    public TypeMateriel TypeMateriel { get; set; }
    public string TypeMaterielLibelle => TypeMateriel.ToString();

    // Spécifiques Externe
    public int? BonEntreeAssocieId { get; set; }
    public string? BonEntreeAssocieNumero { get; set; }
    public string? NomDestinataire { get; set; }
    public string? AdresseDestination { get; set; }
    public string? NumeroVehicule { get; set; }
    public string? NomChauffeur { get; set; }
    public string? TelephoneChauffeur { get; set; }

    // Spécifiques Interne
    public string? DepartementOrigine { get; set; }
    public string? DepartementDestination { get; set; }
    public string? NomReceveur { get; set; }
    public string? FonctionReceveur { get; set; }
    public string? EmailReceveur { get; set; }
    public string? LocalisationDestination { get; set; }
    public DateTime? DateTransfertPrevue { get; set; }

    // Spécifiques Prêt
    public DateTime? DateRetourPrevue { get; set; }
    public DateTime? DateRetourEffective { get; set; }
    public string? EtatRetour { get; set; }
    public bool EstRetourne { get; set; }

    // QR Code
    public string? QRCodeBase64 { get; set; }
    public string? QRCodeHash { get; set; }
    public DateTime? DateGenerationQR { get; set; }

    // Collections
    public List<MaterielSortieViewDto> Materiels { get; set; } = [];
    public List<ApprobationSortieViewDto> Approbations { get; set; } = [];
    public List<ItineraireSortieViewDto> Itineraires { get; set; } = [];

    // Métadonnées calculées
    public bool EstExpire => DateExpiration < DateTime.Today;
    public int JoursRestants => (DateExpiration.Date - DateTime.Today).Days;
    public bool PeutEtreModifie => StatutActuel == "Draft" || StatutActuel == "Returned";
    public bool PeutEtreApprouve => StatutActuel.StartsWith("Pending");
    public bool EstApprouve => StatutActuel == "Approved";
}

/// <summary>
/// DTO pour un matériel dans l'affichage
/// </summary>
public class MaterielSortieViewDto
{
    public int IdMateriel { get; set; }
    public string CodeProduitSerial { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Quantite { get; set; }
}

/// <summary>
/// DTO pour une approbation dans l'affichage
/// </summary>
public class ApprobationSortieViewDto
{
    public int IdApprobation { get; set; }
    public int OrdreEtape { get; set; }
    public string NomEtape { get; set; } = string.Empty;
    public string? ApprobateurNom { get; set; }
    public string? ApprobateurFonction { get; set; }
    public DateTime? DateApprobation { get; set; }
    public string Statut { get; set; } = string.Empty;
    public string? Commentaire { get; set; }
    public bool EstApprouve => Statut == "Approved";
    public bool EstRejete => Statut == "Rejected";
    public bool EstEnAttente => Statut == "Pending";
}

/// <summary>
/// DTO pour un itinéraire dans l'affichage
/// </summary>
public class ItineraireSortieViewDto
{
    public int IdItineraire { get; set; }
    public int OrdrePassage { get; set; }
    public string NomBarriere { get; set; } = string.Empty;
    public string? LocalisationBarriere { get; set; }
    public DateTime? DatePassage { get; set; }
    public bool EstPasse { get; set; }
    public string? NomAgent { get; set; }
    public string? Observations { get; set; }
}
