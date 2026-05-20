using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Matériel associé à un bon d'entrée.
/// EF maps Id → IdMateriel column.
/// </summary>
public sealed class Materiel : BaseEntity
{
    public int BonId { get; set; }

    [MaxLength(100)]
    public string CodeProduitSerial { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Designation { get; set; } = string.Empty;

    public decimal Quantite { get; set; } = 1;

    public decimal QuantiteDisponible { get; set; } = 1;

    /// <summary>Code département Glencore propriétaire du matériel (utilisé pour filtrer le dropdown matériel).</summary>
    [MaxLength(50)]
    public string? DepartementCode { get; set; }

    private Materiel() { }

    public Materiel(string codeProduitSerial, string designation, decimal quantite)
    {
        CodeProduitSerial = codeProduitSerial;
        Designation = designation;
        Quantite = quantite;
        QuantiteDisponible = quantite;
    }

    public void DeduireQuantite(decimal quantiteSortie)
    {
        QuantiteDisponible -= quantiteSortie;
        if (QuantiteDisponible < 0) QuantiteDisponible = 0;
    }

    public bool AQuantiteDisponible(decimal quantiteDemandee)
        => QuantiteDisponible >= quantiteDemandee;
}
