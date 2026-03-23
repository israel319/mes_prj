using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Raison/motif spécifique de sortie (liée à une catégorie).
/// 
/// INTERNE:
/// - Informatique : transfert équipement IT
/// - Circulaire : documents/courrier interne  
/// - Modification : équipement en réparation/modification
/// - Prêt : sortie temporaire (max 6 mois, retour obligatoire)
/// 
/// EXTERNE:
/// - Fin de chantier : matériel contractant qui repart
/// - Résidu : déchets, matériaux à évacuer
/// - Radio-protection : matériel contrôlé (validation spéciale)
/// </summary>
public class RaisonSortie
{
    /// <summary>
    /// Identifiant unique de la raison de sortie (clé primaire)
    /// </summary>
    [Key]
    public int IdRaisonSortie { get; set; }

    /// <summary>
    /// Id de la catégorie parente
    /// </summary>
    public int CategorieId { get; set; }

    /// <summary>
    /// Nom de la raison (ex: Fin chantier, Informatique, Prêt, etc.)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Code court unique (ex: FIN_CHANTIER, PRET, INFORMATIQUE)
    /// </summary>
    [MaxLength(50)]
    public string? Code { get; set; }

    /// <summary>
    /// Description détaillée
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    // ===== RÈGLES MÉTIER =====

    /// <summary>
    /// Indique si un Bon d'Entrée source est requis (liaison BEM → BSM)
    /// </summary>
    public bool RequiertBonEntree { get; set; } = false;

    /// <summary>
    /// Indique si le passage aux barrières/checkpoints est obligatoire
    /// </summary>
    public bool RequiertBarrieres { get; set; } = false;

    /// <summary>
    /// Indique si c'est une sortie temporaire (prêt) avec date de retour
    /// </summary>
    public bool EstTemporaire { get; set; } = false;

    /// <summary>
    /// Durée maximale en jours pour les sorties temporaires (ex: 180 pour prêt 6 mois)
    /// </summary>
    public int? DureeMaxJours { get; set; }

    /// <summary>
    /// Indique si une validation spéciale est requise (ex: radio-protection)
    /// </summary>
    public bool ValidationSpeciale { get; set; } = false;

    /// <summary>
    /// Type d'approbateur spécial requis (ex: RadioProtection, IT, Security)
    /// </summary>
    [MaxLength(100)]
    public string? TypeApprobateurSpecial { get; set; }

    /// <summary>
    /// Indique si des détails supplémentaires sont requis
    /// </summary>
    public bool RequiertDetails { get; set; } = false;

    // ===== MAPPING VERS WORKFLOW =====

    /// <summary>
    /// Type de matériel par défaut associé à cette raison.
    /// Détermine le workflow d'approbation (IT, Environnement, Standard).
    /// Source unique de vérité pour le routage du workflow.
    /// </summary>
    public KCCMaterialFlow.Domain.Enums.TypeMateriel? TypeMaterielDefaut { get; set; }

    /// <summary>
    /// Indique si la raison est active
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Ordre d'affichage dans la liste
    /// </summary>
    public int OrdreAffichage { get; set; } = 0;

    /// <summary>
    /// Icône ou classe CSS pour affichage (ex: "rzi-truck", "rzi-computer")
    /// </summary>
    [MaxLength(50)]
    public string? Icone { get; set; }

    /// <summary>
    /// Couleur de badge (ex: "#00B193", "#BB8748")
    /// </summary>
    [MaxLength(20)]
    public string? Couleur { get; set; }

    /// <summary>
    /// Navigation vers la catégorie parente
    /// </summary>
    public virtual CategorieSortie? Categorie { get; set; }
}
