using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonEntree.Entities;

/// <summary>
/// Représente l'historique des actions effectuées sur un bon d'entrée.
/// Permet la traçabilité complète de toutes les modifications.
/// </summary>
public class BonEntreeHistory
{
    /// <summary>
    /// Identifiant unique de l'entrée d'historique (clé primaire)
    /// </summary>
    [Key]
    public int IdHistory { get; set; }

    /// <summary>
    /// Identifiant du bon concerné (clé étrangère)
    /// </summary>
    public int BonId { get; set; }

    /// <summary>
    /// Type d'action effectuée
    /// </summary>
    public ActionBonEntree Action { get; set; }

    /// <summary>
    /// Description de l'action effectuée
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string ActionDescription { get; set; } = string.Empty;

    /// <summary>
    /// Login de l'utilisateur qui a effectué l'action
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ActionBy { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet de l'utilisateur (pour affichage)
    /// </summary>
    [MaxLength(200)]
    public string? ActionByNom { get; set; }

    /// <summary>
    /// Date et heure de l'action
    /// </summary>
    public DateTime ActionDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Commentaire ou justification de l'action
    /// </summary>
    [MaxLength(1000)]
    public string? Comment { get; set; }

    /// <summary>
    /// Statut du bon avant l'action
    /// </summary>
    [MaxLength(30)]
    public string? StatutAvant { get; set; }

    /// <summary>
    /// Statut du bon après l'action
    /// </summary>
    [MaxLength(30)]
    public string? StatutApres { get; set; }

    /// <summary>
    /// Données JSON des changements effectués (pour audit détaillé)
    /// </summary>
    public string? ChangementsJson { get; set; }

    /// <summary>
    /// Adresse IP de l'utilisateur (pour audit de sécurité)
    /// </summary>
    [MaxLength(50)]
    public string? AdresseIP { get; set; }

    /// <summary>
    /// Référence vers le bon parent
    /// </summary>
    public virtual Bon? Bon { get; set; }
}

/// <summary>
/// Types d'actions possibles sur un bon d'entrée
/// </summary>
public enum ActionBonEntree
{
    /// <summary>
    /// Création du bon
    /// </summary>
    Creation = 1,

    /// <summary>
    /// Modification des informations
    /// </summary>
    Modification = 2,

    /// <summary>
    /// Soumission pour approbation
    /// </summary>
    Soumission = 3,

    /// <summary>
    /// Approbation par un approbateur
    /// </summary>
    Approbation = 4,

    /// <summary>
    /// Rejet par un approbateur
    /// </summary>
    Rejet = 5,

    /// <summary>
    /// Retour pour modification
    /// </summary>
    RetourModification = 6,

    /// <summary>
    /// Génération du QR Code
    /// </summary>
    GenerationQR = 7,

    /// <summary>
    /// Scan à une barrière
    /// </summary>
    ScanBarriere = 8,

    /// <summary>
    /// Entrée effective sur le site
    /// </summary>
    EntreeEffective = 9,

    /// <summary>
    /// Sortie d'un matériel
    /// </summary>
    SortieMateriel = 10,

    /// <summary>
    /// Clôture du bon (tous matériels ressortis)
    /// </summary>
    Cloture = 11,

    /// <summary>
    /// Annulation du bon
    /// </summary>
    Annulation = 12,

    /// <summary>
    /// Prolongation de la validité
    /// </summary>
    Prolongation = 13,

    /// <summary>
    /// Ajout d'un matériel
    /// </summary>
    AjoutMateriel = 14,

    /// <summary>
    /// Suppression d'un matériel
    /// </summary>
    SuppressionMateriel = 15,

    /// <summary>
    /// Signalement d'une anomalie
    /// </summary>
    Anomalie = 16,

    /// <summary>
    /// Impression du bon ou QR
    /// </summary>
    Impression = 17,

    /// <summary>
    /// Envoi de notification
    /// </summary>
    Notification = 18
}
