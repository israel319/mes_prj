using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente une entrée du journal d'audit.
/// Enregistre toutes les actions significatives dans le système pour traçabilité.
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Identifiant unique de l'entrée (clé primaire)
    /// </summary>
    [Key]
    public long IdAuditLog { get; set; }

    /// <summary>
    /// Date et heure de l'action
    /// </summary>
    public DateTime DateAction { get; set; } = DateTime.Now;

    /// <summary>
    /// Login de l'utilisateur ayant effectué l'action
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UtilisateurLogin { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet de l'utilisateur (pour affichage historique)
    /// </summary>
    [MaxLength(200)]
    public string? UtilisateurNom { get; set; }

    /// <summary>
    /// Type d'action effectuée (Create, Update, Delete, Approve, Reject, Scan, Login, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TypeAction { get; set; } = string.Empty;

    /// <summary>
    /// Catégorie/module concerné (BonEntree, BonSortie, Utilisateur, Parametre, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Categorie { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant de l'entité concernée (ex: ID du bon)
    /// </summary>
    [MaxLength(100)]
    public string? EntiteId { get; set; }

    /// <summary>
    /// Type de l'entité (nom de la classe ou table)
    /// </summary>
    [MaxLength(100)]
    public string? EntiteType { get; set; }

    /// <summary>
    /// Description lisible de l'action
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Détails supplémentaires (format JSON avec anciennes/nouvelles valeurs)
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Valeur avant modification (pour les updates)
    /// </summary>
    public string? AncienneValeur { get; set; }

    /// <summary>
    /// Valeur après modification (pour les updates)
    /// </summary>
    public string? NouvelleValeur { get; set; }

    /// <summary>
    /// Adresse IP de l'utilisateur
    /// </summary>
    [MaxLength(50)]
    public string? AdresseIP { get; set; }

    /// <summary>
    /// Navigateur/agent utilisateur
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Résultat de l'action (Succes, Echec, Avertissement)
    /// </summary>
    [MaxLength(20)]
    public string Resultat { get; set; } = "Succes";

    /// <summary>
    /// Message d'erreur si échec
    /// </summary>
    [MaxLength(2000)]
    public string? MessageErreur { get; set; }

    /// <summary>
    /// Niveau de criticité (Info, Warning, Error, Critical)
    /// </summary>
    [MaxLength(20)]
    public string Niveau { get; set; } = "Info";

    /// <summary>
    /// Durée de l'opération en millisecondes
    /// </summary>
    public int? DureeMs { get; set; }

    /// <summary>
    /// Référence de session ou correlation ID
    /// </summary>
    [MaxLength(100)]
    public string? CorrelationId { get; set; }
}
