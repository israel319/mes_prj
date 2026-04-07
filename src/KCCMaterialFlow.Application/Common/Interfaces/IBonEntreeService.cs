using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Interface du service métier pour les Bons d'Entrée.
/// Encapsule la logique métier, le workflow et les règles de validation.
/// </summary>
public interface IBonEntreeService
{
    #region CRUD Operations

    /// <summary>
    /// Crée un nouveau bon d'entrée avec validation et génération du numéro
    /// </summary>
    /// <param name="request">Données de création</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de création avec le bon créé</returns>
    Task<BonEntreeResult> CreateAsync(CreateBonEntreeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un bon d'entrée existant
    /// </summary>
    /// <param name="request">Données de mise à jour</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de mise à jour</returns>
    Task<BonEntreeResult> UpdateAsync(UpdateBonEntreeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un bon d'entrée par son ID
    /// </summary>
    /// <param name="id">Identifiant du bon</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Le bon d'entrée ou null</returns>
    Task<BonEntree?> GetAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un bon d'entrée par son numéro de référence
    /// </summary>
    /// <param name="numeroReference">Numéro de référence</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Le bon d'entrée ou null</returns>
    Task<BonEntree?> GetByNumeroAsync(string numeroReference, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recherche des bons d'entrée avec filtres
    /// </summary>
    /// <param name="filter">Critères de recherche</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat paginé</returns>
    Task<BonEntreeSearchResult> GetListAsync(BonEntreeFilter filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Annule un bon d'entrée
    /// </summary>
    /// <param name="id">Identifiant du bon</param>
    /// <param name="motif">Motif d'annulation</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'annulation</returns>
    Task<BonEntreeResult> CancelAsync(int id, string motif, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un brouillon de bon d'entrée
    /// </summary>
    /// <param name="id">Identifiant du bon</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de la suppression</returns>
    Task<BonEntreeResult> DeleteDraftAsync(int id, CancellationToken cancellationToken = default);

    #endregion

    #region User Specific

    /// <summary>
    /// Récupère les bons créés par l'utilisateur courant
    /// </summary>
    /// <param name="skip">Pagination - skip</param>
    /// <param name="take">Pagination - take</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste paginée des bons</returns>
    Task<BonEntreeSearchResult> GetMyBonsAsync(int skip = 0, int take = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les bons en attente d'approbation par l'utilisateur courant
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des bons à approuver</returns>
    Task<IReadOnlyList<BonEntree>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère les bons retournés pour modification à l'utilisateur courant
    /// </summary>
    Task<IReadOnlyList<ReturnedBemInfo>> GetMyReturnedBonsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Workflow

    /// <summary>
    /// Soumet un bon pour approbation (passage de Draft à PendingSup)
    /// </summary>
    /// <param name="id">Identifiant du bon</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de la soumission</returns>
    Task<BonEntreeResult> SubmitForApprovalAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approuve un bon (passage à l'étape suivante)
    /// </summary>
    /// <param name="request">Données d'approbation</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'approbation</returns>
    Task<BonEntreeResult> ApproveAsync(ApprovalRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejette un bon
    /// </summary>
    /// <param name="request">Données de rejet</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat du rejet</returns>
    Task<BonEntreeResult> RejectAsync(ApprovalRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne un bon pour modification
    /// </summary>
    /// <param name="request">Données de retour</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat du retour</returns>
    Task<BonEntreeResult> ReturnForModificationAsync(ApprovalRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region QR Code & Itineraire

    /// <summary>
    /// Génère le QR Code pour un bon approuvé
    /// </summary>
    /// <param name="id">Identifiant du bon</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat avec chemin du QR Code</returns>
    Task<BonEntreeQRCodeResult> GenerateQRCodeAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcule l'itinéraire pour un bon selon provenance/destination
    /// </summary>
    /// <param name="id">Identifiant du bon</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des barrières de l'itinéraire</returns>
    Task<IReadOnlyList<ItinerairePrevu>> CalculateItineraireAsync(int id, CancellationToken cancellationToken = default);

    #endregion

    #region Materiels

    /// <summary>
    /// Ajoute un matériel à un bon
    /// </summary>
    Task<BonEntreeResult> AddMaterielAsync(int bonId, Materiel materiel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un matériel
    /// </summary>
    Task<BonEntreeResult> UpdateMaterielAsync(Materiel materiel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un matériel
    /// </summary>
    Task<BonEntreeResult> DeleteMaterielAsync(int materielId, CancellationToken cancellationToken = default);

    #endregion

    #region Statistics

    /// <summary>
    /// Récupère les statistiques des bons
    /// </summary>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Statistiques</returns>
    Task<BonEntreeStats> GetStatsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Approbations

    /// <summary>
    /// Récupère les approbations d'un bon
    /// </summary>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste des approbations</returns>
    Task<IReadOnlyList<Approbation>> GetApprobationsAsync(int bonId, CancellationToken cancellationToken = default);

    #endregion
}

#region Request/Response Models

/// <summary>
/// Requête de création d'un bon d'entrée selon le formulaire SEC-FM-141(B)
/// </summary>
public class CreateBonEntreeRequest
{
    // === Informations Compagnie / Contrat ===
    public int? ContratId { get; set; }
    public string? NumeroContrat { get; set; }

    [Required(ErrorMessage = "Le nom de la compagnie est obligatoire")]
    public string NomCompagnie { get; set; } = string.Empty;

    public string? EmailContractant { get; set; }

    [Required(ErrorMessage = "Le site manager est obligatoire")]
    public string SiteManager { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le département hôte est obligatoire")]
    public string HostDepartment { get; set; } = string.Empty;

    public int? DepartementId { get; set; }

    public int? RaisonEntreeId { get; set; }

    [Required(ErrorMessage = "Le motif de la visite est obligatoire")]
    public string ReasonOnSite { get; set; } = string.Empty;

    // === Détail de l'Escorteur ===
    [Required(ErrorMessage = "Le nom de l'escorteur est obligatoire")]
    public string NomEscorteur { get; set; } = string.Empty;

    public string? FonctionEscorteur { get; set; }

    // === Provenance / Destination (FROM / TO) ===
    [Required(ErrorMessage = "La provenance est obligatoire")]
    public string Provenance { get; set; } = string.Empty;

    [Required(ErrorMessage = "La destination est obligatoire")]
    public string Destination { get; set; } = string.Empty;

    // === Dates ===
    [Required(ErrorMessage = "La date d'expiration est obligatoire")]
    public DateTime DateExpiration { get; set; }

    // === Description / Observations ===
    public string? Description { get; set; }

    // === Liste des matériels ===
    public List<MaterielRequest> Materiels { get; set; } = [];
}

/// <summary>
/// Requête de mise à jour d'un bon d'entrée
/// </summary>
public class UpdateBonEntreeRequest
{
    public int IdBon { get; set; }
    public int? ContratId { get; set; }
    public string? NumeroContrat { get; set; }
    public string NomCompagnie { get; set; } = string.Empty;
    public string? EmailContractant { get; set; }
    public string SiteManager { get; set; } = string.Empty;
    public string HostDepartment { get; set; } = string.Empty;
    public int? DepartementId { get; set; }
    public int? RaisonEntreeId { get; set; }
    public string ReasonOnSite { get; set; } = string.Empty;
    public string NomEscorteur { get; set; } = string.Empty;
    public string? FonctionEscorteur { get; set; }
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DateExpiration { get; set; }
    public string? Description { get; set; }

    /// <summary>
    /// Liste des matériels à mettre à jour (uniquement en brouillon)
    /// </summary>
    public List<MaterielUpdateRequest> Materiels { get; set; } = [];
}

/// <summary>
/// Requête de mise à jour d'un matériel
/// </summary>
public class MaterielUpdateRequest
{
    public int IdMateriel { get; set; }
    public string CodeProduitSerial { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Quantite { get; set; } = 1;
}

/// <summary>
/// Requête pour un matériel selon le formulaire SEC-FM-141(B)
/// Champs: Code produit/N Serial | Designation | Qte | FROM | TO
/// </summary>
public class MaterielRequest
{
    public string CodeProduitSerial { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Quantite { get; set; } = 1;

    /// <summary>
    /// ID du site de provenance (FROM)
    /// </summary>
    public int? ProvenanceId { get; set; }

    /// <summary>
    /// ID du site de destination (TO)
    /// </summary>
    public int? DestinationId { get; set; }

    /// <summary>
    /// Nom du site de provenance (rempli automatiquement)
    /// </summary>
    public string? Provenance { get; set; }

    /// <summary>
    /// Nom du site de destination (rempli automatiquement)
    /// </summary>
    public string? Destination { get; set; }
}

/// <summary>
/// Requête d'approbation/rejet
/// </summary>
public class ApprovalRequest
{
    public int BonId { get; set; }
    public string? ReservesEventuelles { get; set; }
}

/// <summary>
/// Résultat d'opération sur un bon d'entrée
/// </summary>
public class BonEntreeResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public BonEntree? BonEntree { get; set; }
    public List<string> Errors { get; set; } = [];

    public static BonEntreeResult Ok(BonEntree bon, string? message = null) =>
        new() { Success = true, BonEntree = bon, Message = message };

    public static BonEntreeResult Fail(string error) =>
        new() { Success = false, Errors = [error] };

    public static BonEntreeResult Fail(List<string> errors) =>
        new() { Success = false, Errors = errors };
}

/// <summary>
/// Information sur un bon d'entrée retourné pour modification
/// </summary>
public class ReturnedBemInfo
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public string RaisonRetour { get; set; } = string.Empty;
    public DateTime DateRetour { get; set; }
    public string AuteurRetour { get; set; } = string.Empty;
}

/// <summary>
/// Résultat de génération QR Code pour un bon d'entrée
/// </summary>
public class BonEntreeQRCodeResult
{
    public bool Success { get; set; }
    public string? QRCodePath { get; set; }
    public string? QRCodeData { get; set; }
    public byte[]? QRCodeBytes { get; set; }
    public string? ErrorMessage { get; set; }

    public static BonEntreeQRCodeResult Ok(string path, string data, byte[]? bytes = null) =>
        new() { Success = true, QRCodePath = path, QRCodeData = data, QRCodeBytes = bytes };

    public static BonEntreeQRCodeResult Fail(string error) =>
        new() { Success = false, ErrorMessage = error };
}

/// <summary>
/// Statistiques des bons d'entrée
/// </summary>
public class BonEntreeStats
{
    public int TotalBons { get; set; }
    public int BonsAujourdHui { get; set; }
    public int BonsEnAttente { get; set; }
    public int BonsApprouves { get; set; }
    public int BonsEnTransit { get; set; }
    public int BonsCompletes { get; set; }
    public int MaterielsNonSortis { get; set; }
    public Dictionary<string, int> ParStatut { get; set; } = [];
}

#endregion
