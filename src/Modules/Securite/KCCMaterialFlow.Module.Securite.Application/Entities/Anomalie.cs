using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Securite.Entities;

/// <summary>
/// Représente une anomalie signalée lors d'un scan ou constatée sur un bon.
/// Permet aux agents de sécurité de reporter des problèmes.
/// </summary>
public class Anomalie
{
    /// <summary>
    /// Identifiant unique de l'anomalie (clé primaire)
    /// </summary>
    [Key]
    public int IdAnomalie { get; set; }

    /// <summary>
    /// Type d'anomalie : QRCodeInvalide, BonExpire, MaterielManquant, MaterielExcedentaire, etc.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TypeAnomalie { get; set; } = string.Empty;

    /// <summary>
    /// Niveau de gravité : Faible, Moyen, Eleve, Critique
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string NiveauGravite { get; set; } = "Moyen";

    /// <summary>
    /// Date et heure du signalement
    /// </summary>
    public DateTime DateSignalement { get; set; } = DateTime.Now;

    /// <summary>
    /// Description détaillée de l'anomalie
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant du bon concerné (optionnel)
    /// </summary>
    public int? BonId { get; set; }

    /// <summary>
    /// Type de bon concerné : BEM ou BSM
    /// </summary>
    [MaxLength(10)]
    public string? TypeBon { get; set; }

    /// <summary>
    /// Numéro de référence du bon (pour affichage)
    /// </summary>
    [MaxLength(30)]
    public string? NumeroReferenceBon { get; set; }

    /// <summary>
    /// Identifiant du scan associé (FK vers ScanEvenement, optionnel)
    /// </summary>
    public int? ScanId { get; set; }

    /// <summary>
    /// Identifiant de la barrière où l'anomalie a été constatée
    /// </summary>
    public int? BarriereId { get; set; }

    /// <summary>
    /// Indique si l'anomalie a été traitée/résolue
    /// </summary>
    public bool EstTraitee { get; set; } = false;

    /// <summary>
    /// Date de traitement de l'anomalie
    /// </summary>
    public DateTime? DateTraitement { get; set; }

    /// <summary>
    /// Login de l'agent qui a signalé l'anomalie
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string SignalePar { get; set; } = string.Empty;

    /// <summary>
    /// Nom de l'agent qui a signalé (pour affichage)
    /// </summary>
    [MaxLength(200)]
    public string? SignaleParNom { get; set; }

    /// <summary>
    /// Login de l'utilisateur qui a traité l'anomalie
    /// </summary>
    [MaxLength(100)]
    public string? TraitePar { get; set; }

    /// <summary>
    /// Résolution appliquée
    /// </summary>
    [MaxLength(2000)]
    public string? Resolution { get; set; }

    /// <summary>
    /// Actions correctives prises
    /// </summary>
    [MaxLength(2000)]
    public string? ActionsCorrectives { get; set; }

    /// <summary>
    /// Navigation vers le scan associé
    /// </summary>
    public virtual ScanEvenement? ScanEvenement { get; set; }

    /// <summary>
    /// Navigation vers la barrière
    /// </summary>
    public virtual Shared.Entities.Barriere? Barriere { get; set; }
}

/// <summary>
/// Types d'anomalies possibles
/// </summary>
public static class TypeAnomalieValues
{
    public const string QRCodeInvalide = "QRCodeInvalide";
    public const string QRCodeExpire = "QRCodeExpire";
    public const string QRCodeDejaScan = "QRCodeDejaScan";
    public const string BonNonTrouve = "BonNonTrouve";
    public const string BonExpire = "BonExpire";
    public const string BonAnnule = "BonAnnule";
    public const string MaterielManquant = "MaterielManquant";
    public const string MaterielExcedentaire = "MaterielExcedentaire";
    public const string MaterielNonConforme = "MaterielNonConforme";
    public const string DocumentManquant = "DocumentManquant";
    public const string AutorisationManquante = "AutorisationManquante";
    public const string BarriereNonAutorisee = "BarriereNonAutorisee";
    public const string HorsHoraires = "HorsHoraires";
    public const string Autre = "Autre";
    
    // Constantes additionnelles pour compatibilité
    public const string DocumentExpire = "QRCodeExpire";
    public const string DocumentInexistant = "BonNonTrouve";
    public const string ScanDuplique = "QRCodeDejaScan";
    public const string ItineraireNonRespectee = "BarriereNonAutorisee";
    public const string SignalementManuel = "Autre";
}

/// <summary>
/// Niveaux de gravité des anomalies
/// </summary>
public static class NiveauGraviteValues
{
    public const string Faible = "Faible";
    public const string Moyen = "Moyen";
    public const string Eleve = "Eleve";
    public const string Critique = "Critique";
}
