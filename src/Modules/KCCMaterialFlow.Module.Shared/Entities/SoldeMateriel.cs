using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente le solde disponible d'un matériel issu d'un Bon d'Entrée.
/// Permet le suivi des quantités : initiale, sorties, et restante.
/// Utilisé pour la liaison BEM ↔ BSM avec gestion du reliquat.
/// </summary>
public class SoldeMateriel
{
    /// <summary>
    /// Identifiant unique (clé primaire)
    /// </summary>
    [Key]
    public int IdSoldeMateriel { get; set; }

    /// <summary>
    /// Identifiant du matériel source (Bon d'Entrée)
    /// </summary>
    public int MaterielEntreeId { get; set; }

    /// <summary>
    /// Identifiant du Bon d'Entrée parent
    /// </summary>
    public int BonEntreeId { get; set; }

    /// <summary>
    /// Code produit / numéro de série (dénormalisé pour affichage)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CodeProduitSerial { get; set; } = string.Empty;

    /// <summary>
    /// Désignation du matériel (dénormalisé pour affichage)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Designation { get; set; } = string.Empty;

    /// <summary>
    /// Quantité initiale entrée via le BEM
    /// </summary>
    public decimal QuantiteInitiale { get; set; }

    /// <summary>
    /// Quantité totale déjà sortie via des BSM
    /// </summary>
    public decimal QuantiteSortie { get; set; } = 0;

    /// <summary>
    /// Quantité restante disponible pour sortie
    /// Calculée automatiquement : QuantiteInitiale - QuantiteSortie
    /// </summary>
    public decimal QuantiteRestante => QuantiteInitiale - QuantiteSortie;

    /// <summary>
    /// Indique si tout le stock a été sorti (solde épuisé)
    /// </summary>
    public bool EstEpuise => QuantiteRestante <= 0;

    /// <summary>
    /// Indique si ce matériel est partiellement sorti
    /// </summary>
    public bool EstPartiel => QuantiteSortie > 0 && QuantiteSortie < QuantiteInitiale;

    /// <summary>
    /// Date de dernière mise à jour du solde
    /// </summary>
    public DateTime DateDerniereMaj { get; set; } = DateTime.Now;

    /// <summary>
    /// Numéro de référence du dernier BSM ayant impacté ce solde
    /// </summary>
    [MaxLength(50)]
    public string? DernierBsmNumero { get; set; }
}
