using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Notification de rejet.
/// EF maps Id → IdNotificationRejet column.
/// </summary>
public sealed class NotificationRejet : BaseEntity
{
    [MaxLength(20)]
    public string BonType { get; set; } = string.Empty;

    [MaxLength(50)]
    public string NumeroReference { get; set; } = string.Empty;

    [MaxLength(50)]
    public string EtapeRejet { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ApprobateurNom { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ApprobateurLogin { get; set; }

    [MaxLength(1000)]
    public string MotifRejet { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? DemandeurNom { get; set; }

    public DateTime DateRejet { get; set; } = DateTime.Now;
    public bool EstLue { get; set; }
    public DateTime? DateLecture { get; set; }
    public bool EmailEnvoye { get; set; }
    public DateTime? DateEnvoiEmail { get; set; }
}
