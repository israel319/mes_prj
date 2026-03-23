namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// BSM-031: Interface de service pour la liaison entre BonEntree et BonSortie.
/// Permet au module BonSortie de verrouiller/déverrouiller un BonEntree sans dépendance directe.
/// Implémenté dans le module BonEntree.
/// </summary>
public interface IBonEntreeLockService
{
    /// <summary>
    /// Vérifie si un bon d'entrée est disponible pour être lié à un bon de sortie.
    /// Un BEM est disponible si: non verrouillé, approuvé, et non expiré.
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat avec disponibilité et message d'erreur si non disponible</returns>
    Task<BonEntreeAvailabilityResult> CheckAvailabilityAsync(int bonEntreeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verrouille un bon d'entrée pour un bon de sortie approuvé.
    /// Empêche le BEM d'être utilisé pour un autre BSM.
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée à verrouiller</param>
    /// <param name="bonSortieId">ID du bon de sortie</param>
    /// <param name="bonSortieNumero">Numéro de référence du bon de sortie</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task LockAsync(int bonEntreeId, int bonSortieId, string bonSortieNumero, CancellationToken cancellationToken = default);

    /// <summary>
    /// Déverrouille un bon d'entrée (si le BSM est annulé ou rejeté).
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée à déverrouiller</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task UnlockAsync(int bonEntreeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les informations de base d'un bon d'entrée pour affichage.
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Informations de base du BEM ou null si non trouvé</returns>
    Task<BonEntreeBasicInfo?> GetBasicInfoAsync(int bonEntreeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les détails complets d'un bon d'entrée incluant ses matériels.
    /// Utilisé pour créer un bon de sortie basé sur un BEM existant.
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Détails complets du BEM avec matériels ou null si non trouvé</returns>
    Task<BonEntreeDetailsForSortie?> GetDetailsForSortieAsync(int bonEntreeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recherche un bon d'entrée par son numéro de référence.
    /// Retourne les détails sans vérifier le statut d'approbation.
    /// </summary>
    /// <param name="numeroReference">Numéro de référence du BEM (ex: BEM-2026-000001)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Détails du BEM avec matériels ou null si non trouvé</returns>
    Task<BonEntreeDetailsForSortie?> SearchByNumeroReferenceAsync(string numeroReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// INT-006: Archive un bon d'entrée lorsque le bon de sortie associé est complété.
    /// Change le statut du BEM à "Archived".
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée à archiver</param>
    /// <param name="bonSortieNumero">Numéro du bon de sortie qui déclenche l'archivage</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    Task ArchiveAfterSortieAsync(int bonEntreeId, string bonSortieNumero, CancellationToken cancellationToken = default);

    /// <summary>
    /// Décrémente la quantité disponible des matériels lors de l'approbation finale d'un BSM.
    /// </summary>
    /// <param name="materielsASortir">Liste des matériels avec ID et quantité à sortir</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>True si le stock a été mis à jour, false en cas d'erreur ou stock insuffisant</returns>
    Task<StockUpdateResult> DecrementStockAsync(IEnumerable<MaterielStockDecrement> materielsASortir, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les bons d'entrée disponibles pour sortie (pour affichage dans dropdown).
    /// Un BEM est disponible si: approuvé, non expiré, et avec quantité disponible > 0.
    /// Possibilité de filtrer par département hôte (ex: IT, Environnement).
    /// </summary>
    /// <param name="hostDepartmentFilter">Filtre optionnel sur HostDepartment</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des BEM disponibles avec informations de base</returns>
    Task<IReadOnlyList<BonEntreeForDropdown>> GetAllAvailableForSortieAsync(string? hostDepartmentFilter = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// BonEntree simplifié pour affichage dans un dropdown
/// </summary>
public class BonEntreeForDropdown
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string NomCompagnie { get; set; } = string.Empty;
    public string HostDepartment { get; set; } = string.Empty;
    public string ReasonOnSite { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DateExpiration { get; set; }
    public int NombreMateriels { get; set; }
    public decimal QuantiteTotaleDisponible { get; set; }
    
    /// <summary>
    /// Texte d'affichage pour le dropdown
    /// </summary>
    public string DisplayText => $"{NumeroReference} - {NomCompagnie} [{HostDepartment}] ({NombreMateriels} article(s), expire {DateExpiration:dd/MM/yyyy})";
}

/// <summary>
/// Résultat de vérification de disponibilité d'un BonEntree
/// </summary>
public class BonEntreeAvailabilityResult
{
    public bool IsAvailable { get; set; }
    public string? ErrorMessage { get; set; }
    public string? NumeroReference { get; set; }
    public string? NomCompagnie { get; set; }

    public static BonEntreeAvailabilityResult Available(string numeroReference, string? nomCompagnie) => new()
    {
        IsAvailable = true,
        NumeroReference = numeroReference,
        NomCompagnie = nomCompagnie
    };

    public static BonEntreeAvailabilityResult NotAvailable(string message) => new()
    {
        IsAvailable = false,
        ErrorMessage = message
    };
}

/// <summary>
/// Informations de base d'un BonEntree pour affichage
/// </summary>
public class BonEntreeBasicInfo
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string NomCompagnie { get; set; } = string.Empty;
    public string StatutActuel { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public DateTime DateExpiration { get; set; }
    public bool EstVerrouille { get; set; }
    public int? BonSortieAssocieId { get; set; }
    public string? BonSortieAssocieNumero { get; set; }
}

/// <summary>
/// Détails complets d'un BonEntree pour créer un BonSortie
/// </summary>
public class BonEntreeDetailsForSortie
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
    /// Liste des matériels du BonEntree
    /// </summary>
    public List<MaterielForSortie> Materiels { get; set; } = new();
}

/// <summary>
/// Matériel d'un BonEntree disponible pour sortie
/// </summary>
public class MaterielForSortie
{
    public int IdMateriel { get; set; }
    public string CodeProduitSerial { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal QuantiteInitiale { get; set; }
    public decimal QuantiteDejaSortie { get; set; }
    public decimal QuantiteDisponible => QuantiteInitiale - QuantiteDejaSortie;
}

/// <summary>
/// Demande de décrémentation de stock pour un matériel
/// </summary>
public class MaterielStockDecrement
{
    /// <summary>
    /// ID du matériel source (dans le BonEntree)
    /// </summary>
    public int MaterielEntreeId { get; set; }
    
    /// <summary>
    /// Quantité à retirer du stock
    /// </summary>
    public decimal QuantiteASortir { get; set; }
}

/// <summary>
/// Résultat de la mise à jour du stock
/// </summary>
public class StockUpdateResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Warnings { get; set; } = new();

    public static StockUpdateResult Ok() => new() { Success = true };
    public static StockUpdateResult Fail(string message) => new() { Success = false, ErrorMessage = message };
}
