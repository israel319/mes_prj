using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Catégorie principale de sortie de matériel.
/// INTERNE = transfert interne KCC (Informatique, Circulaire, Modification, Prêt)
/// EXTERNE = sortie définitive hors site (Fin de chantier, Résidu, Radio-protection)
/// </summary>
public class CategorieSortie
{
    /// <summary>
    /// Identifiant unique de la catégorie (clé primaire)
    /// </summary>
    [Key]
    public int IdCategorieSortie { get; set; }

    /// <summary>
    /// Nom de la catégorie (Interne, Externe)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Code court (INT, EXT)
    /// </summary>
    [MaxLength(20)]
    public string? Code { get; set; }

    /// <summary>
    /// Description de la catégorie
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Indique si la validation aux barrières est obligatoire par défaut
    /// </summary>
    public bool RequiertBarrieres { get; set; } = false;

    /// <summary>
    /// Indique si un Bon d'Entrée source est requis
    /// </summary>
    public bool RequiertBonEntree { get; set; } = false;

    /// <summary>
    /// Type d'entité BonSortie correspondant (Interne, Externe, Pret)
    /// </summary>
    [MaxLength(50)]
    public string? TypeEntite { get; set; }

    /// <summary>
    /// Indique si la catégorie est active
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Ordre d'affichage
    /// </summary>
    public int OrdreAffichage { get; set; } = 0;

    /// <summary>
    /// Raisons de sortie possibles pour cette catégorie
    /// </summary>
    public virtual ICollection<RaisonSortie> Raisons { get; set; } = new List<RaisonSortie>();
}
