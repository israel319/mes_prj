using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonEntree.Entities;

/// <summary>
/// Représente un Bon d'Entrée Matériel (BEM) - SEC-FM-141(B).
/// Hérite de Bon et ajoute les propriétés spécifiques selon le diagramme de classe.
/// </summary>
public class BonEntree : Bon
{
    /// <summary>
    /// Nom complet du demandeur (créateur du bon)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string NomDemandeur { get; set; } = string.Empty;

    /// <summary>
    /// ID du contrat sélectionné (FK vers ref.Contrats)
    /// </summary>
    public int? ContratId { get; set; }

    /// <summary>
    /// Numéro du contrat / PO Number (dénormalisé pour affichage)
    /// </summary>
    [MaxLength(50)]
    public string? NumeroContrat { get; set; }

    /// <summary>
    /// Nom de la compagnie (COMPANY NAME)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string NomCompagnie { get; set; } = string.Empty;

    /// <summary>
    /// Email du contractant (CONTRACTOR EMAIL ADDRESS)
    /// </summary>
    [MaxLength(200)]
    public string? EmailContractant { get; set; }

    /// <summary>
    /// Site Manager du contractant (SITE MANAGER CONTRACTOR)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string SiteManager { get; set; } = string.Empty;

    /// <summary>
    /// Département hôte (HOST DEPARTMENT)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string HostDepartment { get; set; } = string.Empty;

    /// <summary>
    /// Motif de la visite (REASON REQUIRED ON SITE)
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string ReasonOnSite { get; set; } = string.Empty;

    /// <summary>
    /// Nom de l'escorteur (DETAIL DE L'ESCORTEUR - Name/Nom)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string NomEscorteur { get; set; } = string.Empty;

    /// <summary>
    /// Fonction de l'escorteur (DETAIL DE L'ESCORTEUR - Function/Fonction)
    /// </summary>
    [MaxLength(150)]
    public string? FonctionEscorteur { get; set; }

    // ===== LIAISON ENTRÉE-SORTIE (BSM-031) =====

    /// <summary>
    /// BSM-031: Indique si ce bon d'entrée est verrouillé car lié à un bon de sortie approuvé.
    /// Un BEM verrouillé ne peut plus être modifié ni utilisé pour un autre BSM.
    /// </summary>
    public bool EstVerrouillePourSortie { get; set; } = false;

    /// <summary>
    /// BSM-031: Date de verrouillage du bon d'entrée
    /// </summary>
    public DateTime? DateVerrouillage { get; set; }

    /// <summary>
    /// BSM-031: Identifiant du bon de sortie qui a verrouillé ce bon d'entrée
    /// </summary>
    public int? BonSortieAssocieId { get; set; }

    /// <summary>
    /// BSM-031: Numéro de référence du bon de sortie associé (dénormalisé pour affichage)
    /// </summary>
    [MaxLength(20)]
    public string? BonSortieAssocieNumero { get; set; }
}
