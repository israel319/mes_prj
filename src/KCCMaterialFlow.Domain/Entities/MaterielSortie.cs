using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Matériel associé à un bon de sortie.
/// EF maps Id → IdMateriel column.
/// </summary>
public sealed class MaterielSortie : BaseEntity
{
    public int BonId { get; private set; }

    [MaxLength(100)]
    public string CodeProduitSerial { get; private set; } = string.Empty;

    [MaxLength(300)]
    public string Designation { get; private set; } = string.Empty;

    public decimal Quantite { get; private set; } = 1;

    [MaxLength(500)]
    public string? Remarque { get; private set; }

    // ----- Liaison BEM (Bon d'Entrée Matériel) -----

    /// <summary>FK vers le matériel source du bon d'entrée.</summary>
    public int? MaterielEntreeId { get; private set; }

    /// <summary>FK vers le bon d'entrée source.</summary>
    public int? BonEntreeId { get; private set; }

    /// <summary>Numéro de référence du bon d'entrée source.</summary>
    [MaxLength(20)]
    public string? BonEntreeNumero { get; private set; }

    /// <summary>Quantité initiale issue du BEM.</summary>
    public decimal? QuantiteInitialeBem { get; private set; }

    /// <summary>Quantité encore disponible sur la ligne BEM.</summary>
    public decimal? QuantiteDisponible { get; private set; }

    /// <summary>Observations libres sur la ligne de matériel.</summary>
    [MaxLength(500)]
    public string? Observations { get; private set; }

    private MaterielSortie() { }

    public MaterielSortie(int bonId, string codeProduitSerial, string designation, decimal quantite, string? remarque = null)
    {
        BonId = bonId;
        CodeProduitSerial = codeProduitSerial;
        Designation = designation;
        Quantite = quantite;
        Remarque = remarque;
    }
}
