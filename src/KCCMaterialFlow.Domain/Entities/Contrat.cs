using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Contrat lié à une compagnie.
/// EF maps Id → IdContrat column.
/// </summary>
public sealed class Contrat : BaseEntity
{
    [MaxLength(100)]
    public string PoNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ContratDescription { get; set; }

    public int CompagnieId { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(200)]
    public string? SiteManager { get; set; }

    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.Now;

    public Compagnie Compagnie { get; set; } = null!;
}
