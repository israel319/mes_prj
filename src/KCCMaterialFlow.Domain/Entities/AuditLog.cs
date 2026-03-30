using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Journal d'audit. EF maps Id → IdAuditLog (long PK mapped separately).
/// Uses long PK — does NOT inherit BaseEntity. Standalone entity.
/// </summary>
public sealed class AuditLog
{
    [Key]
    public long IdAuditLog { get; set; }
    public DateTime DateAction { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string UtilisateurLogin { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? UtilisateurNom { get; set; }

    [MaxLength(50)]
    public string TypeAction { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Categorie { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? EntiteId { get; set; }

    [MaxLength(100)]
    public string? EntiteType { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public string? Details { get; set; }
    public string? AncienneValeur { get; set; }
    public string? NouvelleValeur { get; set; }

    [MaxLength(50)]
    public string? AdresseIP { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [MaxLength(20)]
    public string Resultat { get; set; } = "Succes";

    [MaxLength(2000)]
    public string? MessageErreur { get; set; }

    [MaxLength(20)]
    public string Niveau { get; set; } = "Info";

    public int? DureeMs { get; set; }

    [MaxLength(100)]
    public string? CorrelationId { get; set; }
}
