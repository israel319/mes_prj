using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonEntree.Entities;

/// <summary>
/// Représente un matériel associé à un bon.
/// Selon le diagramme de classe et le formulaire SEC-FM-141(B).
/// </summary>
public class Materiel
{
    /// <summary>
    /// Identifiant unique du matériel (clé primaire)
    /// </summary>
    [Key]
    public int IdMateriel { get; set; }

    /// <summary>
    /// Identifiant du bon parent (clé étrangère)
    /// </summary>
    public int BonId { get; set; }

    /// <summary>
    /// Code produit ou numéro de série (Code produit/N° Serial)
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
    /// Quantité initiale (Qté entrée)
    /// </summary>
    public decimal Quantite { get; set; } = 1;

    /// <summary>
    /// Quantité disponible en stock (diminue après chaque sortie)
    /// Initialisée à Quantite lors de la création du BEM
    /// </summary>
    public decimal QuantiteDisponible { get; set; } = 1;

    /// <summary>
    /// Référence vers le bon parent
    /// </summary>
    public virtual Bon? Bon { get; set; }
}
