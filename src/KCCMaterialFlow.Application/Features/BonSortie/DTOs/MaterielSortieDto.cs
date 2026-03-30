namespace KCCMaterialFlow.Application.Features.BonSortie.DTOs;

/// <summary>
/// DTO pour les matériels dans un bon de sortie
/// </summary>
public class MaterielSortieDto
{
    public string CodeProduitSerial { get; set; } = "";
    public string Designation { get; set; } = "";
    public decimal Quantite { get; set; } = 1;

    // === LIAISON BEM (Traçabilité) ===

    public int? MaterielEntreeId { get; set; }
    public int? BonEntreeId { get; set; }
    public string? BonEntreeNumero { get; set; }
    public decimal? QuantiteInitialeBem { get; set; }
    public decimal? QuantiteDisponible { get; set; }
    public string? Observations { get; set; }
}
