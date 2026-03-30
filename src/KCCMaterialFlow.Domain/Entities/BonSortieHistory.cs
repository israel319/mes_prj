using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Historique des actions sur un bon de sortie.
/// EF maps Id → IdHistory column.
/// </summary>
public sealed class BonSortieHistory : BaseEntity
{
    public int BonId { get; private set; }
    public ActionBonSortie Action { get; private set; }

    [MaxLength(500)]
    public string ActionDescription { get; private set; } = string.Empty;

    [MaxLength(100)]
    public string ActionBy { get; private set; } = string.Empty;

    [MaxLength(200)]
    public string? ActionByNom { get; private set; }

    public DateTime ActionDate { get; private set; } = DateTime.Now;

    [MaxLength(1000)]
    public string? Comment { get; private set; }

    [MaxLength(30)]
    public string? StatutAvant { get; private set; }

    [MaxLength(30)]
    public string? StatutApres { get; private set; }

    public string? ChangementsJson { get; private set; }

    [MaxLength(50)]
    public string? AdresseIP { get; private set; }

    private BonSortieHistory() { }

    public BonSortieHistory(
        int bonId,
        ActionBonSortie action,
        string actionDescription,
        string actionBy,
        string? actionByNom = null,
        string? comment = null,
        string? statutAvant = null,
        string? statutApres = null,
        string? adresseIP = null)
    {
        BonId = bonId;
        Action = action;
        ActionDescription = actionDescription;
        ActionBy = actionBy;
        ActionByNom = actionByNom;
        Comment = comment;
        StatutAvant = statutAvant;
        StatutApres = statutApres;
        AdresseIP = adresseIP;
    }
}
