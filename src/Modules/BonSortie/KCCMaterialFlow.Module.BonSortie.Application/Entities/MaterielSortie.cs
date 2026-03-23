using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KCCMaterialFlow.Module.BonSortie.Entities;

/// <summary>
/// Représente un matériel associé à un bon de sortie.
/// Lié à un matériel source d'un Bon d'Entrée pour la traçabilité.
/// </summary>
public class MaterielSortie
{
    /// <summary>
    /// Identifiant unique du matériel (clé primaire)
    /// </summary>
    [Key]
    public int IdMateriel { get; set; }

    /// <summary>
    /// Identifiant du bon de sortie parent (clé étrangère)
    /// </summary>
    public int BonSortieId { get; set; }

    // ===== LIAISON BEM → BSM (Traçabilité) =====

    /// <summary>
    /// Identifiant du matériel source (Bon d'Entrée) - requis pour la traçabilité
    /// </summary>
    public int? MaterielEntreeId { get; set; }

    /// <summary>
    /// Identifiant du Bon d'Entrée source (dénormalisé pour requêtes)
    /// </summary>
    public int? BonEntreeId { get; set; }

    /// <summary>
    /// Numéro de référence du Bon d'Entrée source (dénormalisé pour affichage)
    /// </summary>
    [MaxLength(20)]
    public string? BonEntreeNumero { get; set; }

    // ===== DONNÉES MATÉRIEL =====

    /// <summary>
    /// Code produit ou numéro de série
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string CodeProduitSerial { get; set; } = string.Empty;

    /// <summary>
    /// Désignation/description du matériel
    /// </summary>
    [Required]
    [MaxLength(300)]
    public string Designation { get; set; } = string.Empty;

    /// <summary>
    /// Quantité demandée pour sortie
    /// </summary>
    public decimal Quantite { get; set; } = 1;

    /// <summary>
    /// Quantité initiale dans le Bon d'Entrée source (pour affichage historique)
    /// </summary>
    public decimal? QuantiteInitialeBem { get; set; }

    /// <summary>
    /// Quantité disponible au moment de la demande (pour historique)
    /// Après cette sortie, le reliquat = QuantiteDisponible - Quantite
    /// </summary>
    public decimal? QuantiteDisponible { get; set; }

    /// <summary>
    /// Observations spécifiques à ce matériel
    /// </summary>
    [MaxLength(500)]
    public string? Observations { get; set; }

    // ===== NAVIGATION =====

    /// <summary>
    /// Référence vers le bon de sortie parent
    /// </summary>
    public virtual BonSortie? BonSortie { get; set; }
}
