using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Entité représentant une notification de rejet.
/// Ces notifications peuvent être envoyées par email ou consultées via l'interface.
/// </summary>
public class NotificationRejet
{
    /// <summary>
    /// Identifiant unique de la notification (clé primaire)
    /// </summary>
    [Key]
    public int IdNotificationRejet { get; set; }

    /// <summary>
    /// Type de bon (BEM, BSM, etc.)
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string BonType { get; set; } = string.Empty;

    /// <summary>
    /// Numéro de référence du bon rejeté
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string NumeroReference { get; set; } = string.Empty;

    /// <summary>
    /// Étape où le rejet a eu lieu (Superviseur, GM, OPJ, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string EtapeRejet { get; set; } = string.Empty;

    /// <summary>
    /// Nom de l'approbateur qui a rejeté
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ApprobateurNom { get; set; } = string.Empty;

    /// <summary>
    /// Login de l'approbateur qui a rejeté
    /// </summary>
    [MaxLength(100)]
    public string? ApprobateurLogin { get; set; }

    /// <summary>
    /// Motif du rejet
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string MotifRejet { get; set; } = string.Empty;

    /// <summary>
    /// Nom du demandeur du bon
    /// </summary>
    [MaxLength(200)]
    public string? DemandeurNom { get; set; }

    /// <summary>
    /// Date et heure du rejet
    /// </summary>
    public DateTime DateRejet { get; set; } = DateTime.Now;

    /// <summary>
    /// Indique si la notification a été lue
    /// </summary>
    public bool EstLue { get; set; } = false;

    /// <summary>
    /// Date de lecture
    /// </summary>
    public DateTime? DateLecture { get; set; }

    /// <summary>
    /// Indique si l'email a été envoyé
    /// </summary>
    public bool EmailEnvoye { get; set; } = false;

    /// <summary>
    /// Date d'envoi de l'email
    /// </summary>
    public DateTime? DateEnvoiEmail { get; set; }
}
