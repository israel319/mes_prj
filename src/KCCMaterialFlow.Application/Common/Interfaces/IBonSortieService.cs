using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Application.Features.BonSortie.DTOs;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Interface du service métier pour les Bons de Sortie.
/// Encapsule la logique métier, le workflow et les règles de validation.
/// </summary>
public interface IBonSortieService
{
    #region CRUD Operations

    /// <summary>
    /// Crée un nouveau bon de sortie externe
    /// </summary>
    Task<BonSortieResult> CreateExterneAsync(CreateBonSortieExterneRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée un nouveau bon de sortie interne
    /// </summary>
    Task<BonSortieResult> CreateInterneAsync(CreateBonSortieInterneRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée un nouveau prêt
    /// </summary>
    Task<BonSortieResult> CreatePretAsync(CreatePretRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un bon de sortie existant
    /// </summary>
    Task<BonSortieResult> UpdateAsync(UpdateBonSortieRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un bon de sortie par son ID
    /// </summary>
    Task<Domain.Entities.BonSortie?> GetAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un bon de sortie par son numéro de référence
    /// </summary>
    Task<Domain.Entities.BonSortie?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recherche des bons de sortie avec filtres
    /// </summary>
    Task<BonSortieSearchResult> GetListAsync(BonSortieFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Annule un bon de sortie
    /// </summary>
    Task<BonSortieResult> CancelAsync(int id, string motif, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un brouillon de bon de sortie
    /// </summary>
    Task<BonSortieResult> DeleteDraftAsync(int id, CancellationToken cancellationToken = default);

    #endregion

    #region User Specific

    /// <summary>
    /// Récupère les bons créés par l'utilisateur courant
    /// </summary>
    Task<BonSortieSearchResult> GetMyBonsAsync(int skip = 0, int take = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les bons en attente d'approbation par l'utilisateur courant
    /// </summary>
    Task<IReadOnlyList<Domain.Entities.BonSortie>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère l'historique des approbations d'un bon de sortie
    /// </summary>
    Task<IReadOnlyList<ApprobationSortie>> GetApprobationsAsync(int bonId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les bons retournés pour modification à l'utilisateur courant
    /// </summary>
    Task<IReadOnlyList<ReturnedBonInfo>> GetMyReturnedBonsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Workflow

    /// <summary>
    /// Soumet un bon pour approbation
    /// </summary>
    Task<BonSortieResult> SubmitForApprovalAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approuve un bon
    /// </summary>
    Task<BonSortieResult> ApproveAsync(BonSortieApprovalRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejette un bon
    /// </summary>
    Task<BonSortieResult> RejectAsync(RejectRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne un bon au demandeur pour modification
    /// </summary>
    Task<BonSortieResult> ReturnForModificationAsync(BonSortieApprovalRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region Prêts

    /// <summary>
    /// Récupère les prêts en cours
    /// </summary>
    Task<IReadOnlyList<Pret>> GetActiveLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les prêts en retard
    /// </summary>
    Task<IReadOnlyList<Pret>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enregistre le retour d'un prêt
    /// </summary>
    Task<BonSortieResult> ReturnLoanAsync(ReturnLoanRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// BSM-027: Récupère les prêts expirant dans N jours (pour alertes J-7)
    /// </summary>
    Task<IReadOnlyList<Pret>> GetLoansExpiringInDaysAsync(int days, CancellationToken cancellationToken = default);

    /// <summary>
    /// BSM-029: Extension de la date de retour d'un prêt (uniquement par Identification)
    /// </summary>
    Task<BonSortieResult> ExtendLoanAsync(ExtendLoanRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region Alertes Email (BSM-028)

    /// <summary>
    /// BSM-028: Envoie manuellement les alertes pour les prêts expirant dans les N prochains jours
    /// </summary>
    /// <param name="days">Nombre de jours à vérifier (ex: 7 = prêts expirant dans 7 jours)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Nombre d'alertes envoyées</returns>
    Task<int> SendExpirationAlertsAsync(int days = 7, CancellationToken cancellationToken = default);

    // Note: GetOverdueLoansAsync est déjà défini dans la région Prêts

    #endregion

    #region QR Code (BSM-030)

    /// <summary>
    /// BSM-030: Génère le QR Code pour un bon de sortie approuvé (après approbation Identification)
    /// </summary>
    /// <param name="bonId">ID du bon de sortie</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat avec le bon mis à jour contenant le QR Code</returns>
    Task<BonSortieResult> GenerateQRCodeAsync(int bonId, CancellationToken cancellationToken = default);

    /// <summary>
    /// BSM-030: Valide un QR Code scanné et retourne les informations du bon
    /// </summary>
    /// <param name="scannedCode">Code scanné (hash ou données)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de validation avec le bon associé</returns>
    Task<BonSortieQRValidationResult> ValidateQRCodeAsync(string scannedCode, CancellationToken cancellationToken = default);

    #endregion

    #region Liaison Entrée-Sortie (BSM-031)

    /// <summary>
    /// BSM-031: Récupère les informations du bon d'entrée associé à un bon de sortie
    /// </summary>
    /// <param name="bonSortieId">ID du bon de sortie</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Informations du BEM associé ou null si non applicable</returns>
    Task<BonEntreeBasicInfo?> GetBonEntreeAssocieAsync(int bonSortieId, CancellationToken cancellationToken = default);

    /// <summary>
    /// BSM-031: Vérifie si un bon d'entrée peut être utilisé pour un nouveau bon de sortie
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de disponibilité</returns>
    Task<BonEntreeAvailabilityResult> CheckBonEntreeAvailabilityAsync(int bonEntreeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les détails complets d'un BonEntree pour créer un BonSortie
    /// Inclut les matériels avec leurs quantités disponibles
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Détails du BEM avec matériels ou null si non trouvé</returns>
    Task<BonEntreeDetailsPourSortieDto?> GetBonEntreeDetailsForSortieAsync(int bonEntreeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recherche un BonEntree par son numéro de référence.
    /// Retourne les détails sans vérifier le statut d'approbation.
    /// </summary>
    /// <param name="numeroReference">Numéro de référence du BEM (ex: BEM-2026-000001)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Détails du BEM avec matériels ou null si non trouvé</returns>
    Task<BonEntreeDetailsPourSortieDto?> SearchBonEntreeByReferenceAsync(string numeroReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// INT-005: Récupère les bons de sortie liés à un bon d'entrée
    /// </summary>
    /// <param name="bonEntreeId">ID du bon d'entrée</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des bons de sortie liés avec informations de base</returns>
    Task<IReadOnlyList<BonSortieLieInfo>> GetBonsSortieByBonEntreeAsync(int bonEntreeId, CancellationToken cancellationToken = default);

    #endregion

    #region Stats

    /// <summary>
    /// Récupère les statistiques des bons de sortie
    /// </summary>
    Task<BonSortieStats> GetStatsAsync(CancellationToken cancellationToken = default);

    #endregion
}

#region DTOs

/// <summary>
/// Résultat d'une opération sur un bon de sortie
/// </summary>
public class BonSortieResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Domain.Entities.BonSortie? Bon { get; set; }
    public List<string> Errors { get; set; } = [];

    public static BonSortieResult Ok(Domain.Entities.BonSortie bon, string? message = null) =>
        new() { Success = true, Bon = bon, Message = message };

    public static BonSortieResult Fail(string error) =>
        new() { Success = false, Errors = [error] };

    public static BonSortieResult Fail(List<string> errors) =>
        new() { Success = false, Errors = errors };
}

/// <summary>
/// Information sur un bon retourné pour modification
/// </summary>
public class ReturnedBonInfo
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string TypeBon { get; set; } = "BSM"; // BEM ou BSM
    public string RaisonRetour { get; set; } = string.Empty;
    public DateTime DateRetour { get; set; }
    public string AuteurRetour { get; set; } = string.Empty;
}

/// <summary>
/// Requête de création d'un bon de sortie externe
/// </summary>
public class CreateBonSortieExterneRequest
{
    public string NomDemandeur { get; set; } = string.Empty;
    public string FonctionDemandeur { get; set; } = string.Empty;
    public string DepartementDemandeur { get; set; } = string.Empty;
    public string MotifSortie { get; set; } = string.Empty;
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateExpiration { get; set; }

    // Catégorie et Raison (nouveau système)
    public int? CategorieId { get; set; }
    public int? RaisonId { get; set; }

    /// <summary>
    /// Code de la raison de sortie (ex: FIN_CHANTIER, RESIDU, CIRC...) pour auto-dérivation du TypeMateriel
    /// </summary>
    public string? RaisonSortieCode { get; set; }

    // Spécifiques à externe
    public int? BonEntreeAssocieId { get; set; }
    public TypeMateriel TypeMateriel { get; set; }
    public string NomDestinataire { get; set; } = string.Empty;
    public string? AdresseDestination { get; set; }
    public string? NumeroVehicule { get; set; }
    public string? NomChauffeur { get; set; }
    public string? TelephoneChauffeur { get; set; }

    /// <summary>
    /// Date de retour prévue (pour les prêts - max 6 mois)
    /// </summary>
    public DateTime? DateRetourPrevue { get; set; }

    public List<MaterielDto> Materiels { get; set; } = [];
    public List<int> BarrieresIds { get; set; } = [];

    /// <summary>
    /// Liste des checkpoints à traverser avec leur ordre prévu
    /// </summary>
    public List<CheckpointItineraireDto> CheckpointsItineraire { get; set; } = [];
}

/// <summary>
/// DTO pour un checkpoint dans l'itinéraire
/// </summary>
public class CheckpointItineraireDto
{
    public int CheckpointId { get; set; }
    public int OrdrePrevu { get; set; }
}

/// <summary>
/// Requête de création d'un bon de sortie interne
/// </summary>
public class CreateBonSortieInterneRequest
{
    public string NomDemandeur { get; set; } = string.Empty;
    public string FonctionDemandeur { get; set; } = string.Empty;
    public string DepartementDemandeur { get; set; } = string.Empty;
    public string MotifSortie { get; set; } = string.Empty;
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateExpiration { get; set; }

    // BonEntree obligatoire pour traçabilité
    public int? BonEntreeAssocieId { get; set; }

    // Catégorie et Raison (nouveau système)
    public int? CategorieId { get; set; }
    public int? RaisonId { get; set; }

    /// <summary>
    /// Code de la raison de sortie (ex: INFO, CIRC, PRET) pour auto-dérivation du TypeMateriel
    /// </summary>
    public string? RaisonSortieCode { get; set; }

    // Spécifiques à interne
    public TypeMateriel TypeMateriel { get; set; }
    public string? DepartementOrigine { get; set; }
    public string? FonctionReceveur { get; set; }
    public string? EmailReceveur { get; set; }
    public string? LocalisationDestination { get; set; }
    public DateTime? DateTransfertPrevue { get; set; }

    public List<MaterielDto> Materiels { get; set; } = [];

    /// <summary>
    /// Checkpoints pour le motif "Autre" (itinéraire interne)
    /// </summary>
    public List<CheckpointItineraireDto> CheckpointsItineraire { get; set; } = [];
}

/// <summary>
/// Requête de création d'un prêt (utilise DateRetourPrevue de la classe parente)
/// </summary>
public class CreatePretRequest : CreateBonSortieExterneRequest
{
    // DateRetourPrevue est déjà défini dans CreateBonSortieExterneRequest
}

/// <summary>
/// Requête de mise à jour d'un bon de sortie
/// </summary>
public class UpdateBonSortieRequest
{
    public int IdBon { get; set; }
    public string? MotifSortie { get; set; }
    public string? Description { get; set; }
    public string? Provenance { get; set; }
    public string? Destination { get; set; }
    public string? FonctionDemandeur { get; set; }
    public string? DepartementDemandeur { get; set; }
    public DateTime? DateExpiration { get; set; }
    public DateTime? DateRetourPrevue { get; set; }

    // Externe/Prêt
    public string? NomDestinataire { get; set; }
    public string? AdresseDestination { get; set; }
    public string? NumeroVehicule { get; set; }
    public string? NomChauffeur { get; set; }
    public string? TelephoneChauffeur { get; set; }

    public List<MaterielDto>? Materiels { get; set; }
}

/// <summary>
/// DTO pour un matériel
/// </summary>
public class MaterielDto
{
    public int? IdMateriel { get; set; }
    public string CodeProduitSerial { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Quantite { get; set; } = 1;
    public string? Provenance { get; set; }
    public string? Destination { get; set; }

    // === LIAISON BEM (Traçabilité) ===

    /// <summary>
    /// ID du matériel source dans le Bon d'Entrée
    /// </summary>
    public int? MaterielEntreeId { get; set; }

    /// <summary>
    /// ID du Bon d'Entrée source
    /// </summary>
    public int? BonEntreeId { get; set; }

    /// <summary>
    /// Numéro de référence du Bon d'Entrée source
    /// </summary>
    public string? BonEntreeNumero { get; set; }

    /// <summary>
    /// Quantité initiale dans le Bon d'Entrée
    /// </summary>
    public decimal? QuantiteInitialeBem { get; set; }

    /// <summary>
    /// Quantité disponible avant cette sortie
    /// </summary>
    public decimal? QuantiteDisponible { get; set; }

    /// <summary>
    /// Observations spécifiques
    /// </summary>
    public string? Observations { get; set; }
}

/// <summary>
/// Requête d'approbation (BonSortie)
/// </summary>
public class BonSortieApprovalRequest
{
    public int IdBon { get; set; }
    public string? Commentaire { get; set; }
}

/// <summary>
/// Requête de rejet
/// </summary>
public class RejectRequest
{
    public int IdBon { get; set; }
    public string Motif { get; set; } = string.Empty;
}

/// <summary>
/// Requête de retour de prêt
/// </summary>
public class ReturnLoanRequest
{
    public int IdBon { get; set; }
    public string? EtatRetour { get; set; }
    public string? ReceptionnePar { get; set; }
}

/// <summary>
/// BSM-029: Requête d'extension de date de retour d'un prêt
/// </summary>
public class ExtendLoanRequest
{
    public int IdBon { get; set; }
    public DateTime NouvelleDateRetour { get; set; }
    public string Motif { get; set; } = string.Empty;
}

/// <summary>
/// Filtre de recherche pour les bons de sortie
/// </summary>
public class BonSortieFilter
{
    public string? SearchTerm { get; set; }
    public string? Statut { get; set; }
    public string? TypeSortie { get; set; }
    public string? Departement { get; set; }
    public string? Demandeur { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}

/// <summary>
/// Résultat de recherche paginé
/// </summary>
public class BonSortieSearchResult
{
    public IReadOnlyList<Domain.Entities.BonSortie> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
}

/// <summary>
/// Statistiques des bons de sortie
/// </summary>
public class BonSortieStats
{
    public int TotalBons { get; set; }
    public int BonsEnAttente { get; set; }
    public int BonsApprouves { get; set; }
    public int BonsRejetes { get; set; }
    public int PretsEnCours { get; set; }
    public int PretsEnRetard { get; set; }
    public int SortiesExternes { get; set; }
    public int SortiesInternes { get; set; }
}

/// <summary>
/// BSM-030: Résultat de validation d'un QR Code scanné
/// </summary>
public class BonSortieQRValidationResult
{
    /// <summary>
    /// Indique si le QR Code est valide
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Message descriptif (succès ou erreur)
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Bon de sortie associé au QR Code (si valide)
    /// </summary>
    public Domain.Entities.BonSortie? Bon { get; set; }

    /// <summary>
    /// Indique si le bon est encore valide (non expiré)
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// Date de scan/validation
    /// </summary>
    public DateTime ValidationDate { get; set; } = DateTime.Now;

    public static BonSortieQRValidationResult Valid(Domain.Entities.BonSortie bon) => new()
    {
        IsValid = true,
        Message = "QR Code valide",
        Bon = bon,
        IsExpired = bon.DateExpiration < DateTime.Now
    };

    public static BonSortieQRValidationResult Invalid(string message) => new()
    {
        IsValid = false,
        Message = message
    };
}

/// <summary>
/// INT-005: Informations de base d'un bon de sortie lié à un bon d'entrée
/// </summary>
public class BonSortieLieInfo
{
    /// <summary>
    /// ID du bon de sortie
    /// </summary>
    public int IdBonSortie { get; set; }

    /// <summary>
    /// Numéro de référence du bon (BSM-YYYY-XXXXXX)
    /// </summary>
    public string NumeroReference { get; set; } = string.Empty;

    /// <summary>
    /// Type de bon de sortie (Externe, Interne, Prêt)
    /// </summary>
    public string TypeBon { get; set; } = string.Empty;

    /// <summary>
    /// Statut actuel du bon
    /// </summary>
    public string StatutActuel { get; set; } = string.Empty;

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime DateCreation { get; set; }

    /// <summary>
    /// Destination du matériel
    /// </summary>
    public string Destination { get; set; } = string.Empty;

    /// <summary>
    /// Nom du demandeur
    /// </summary>
    public string NomDemandeur { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de matériels dans le bon
    /// </summary>
    public int NombreMateriels { get; set; }

    /// <summary>
    /// URL de navigation vers le détail du bon
    /// </summary>
    public string Url => $"/bon-sortie/{IdBonSortie}";
}

#endregion
