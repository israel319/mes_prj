namespace KCCMaterialFlow.Application.Features.BonSortie.DTOs;

/// <summary>
/// DTO contenant les détails d'un BonEntree pour la création d'un BonSortie
/// </summary>
public class BonEntreeDetailsPourSortieDto
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string NomCompagnie { get; set; } = string.Empty;
    public int? ContratId { get; set; }
    public string? NumeroContrat { get; set; }
    public string SiteManager { get; set; } = string.Empty;
    public string HostDepartment { get; set; } = string.Empty;
    public string ReasonOnSite { get; set; } = string.Empty;
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public DateTime DateExpiration { get; set; }
    public string StatutActuel { get; set; } = string.Empty;

    /// <summary>
    /// Liste des matériels du BonEntree disponibles pour la sortie
    /// </summary>
    public List<MaterielPourSortieDto> Materiels { get; set; } = new();
}

/// <summary>
/// DTO pour un matériel disponible pour la sortie
/// </summary>
public class MaterielPourSortieDto
{
    public int IdMateriel { get; set; }
    public string CodeProduitSerial { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;

    /// <summary>
    /// Quantité initiale entrée dans le BEM
    /// </summary>
    public decimal QuantiteInitiale { get; set; }

    /// <summary>
    /// Quantité déjà sortie (historique)
    /// </summary>
    public decimal QuantiteDejaSortie { get; set; }

    /// <summary>
    /// Quantité disponible pour sortie (QuantiteInitiale - QuantiteDejaSortie)
    /// </summary>
    public decimal QuantiteDisponible => QuantiteInitiale - QuantiteDejaSortie;

    // ===== Champs pour la sélection côté UI =====

    public bool EstSelectionne { get; set; }
    public decimal QuantiteASortir { get; set; }
}
