namespace KCCMaterialFlow.Module.BonSortie.DTOs;

/// <summary>
/// DTO pour les matériels dans un bon de sortie
/// </summary>
public class MaterielSortieDto
{
    public string CodeProduitSerial { get; set; } = "";
    public string Designation { get; set; } = "";
    public decimal Quantite { get; set; } = 1;
    
    // === LIAISON BEM (Traçabilité) ===
    
    /// <summary>
    /// ID du matériel source dans le Bon d'Entrée
    /// </summary>
    public int? MaterielEntreeId { get; set; }
    
    /// <summary>
    /// ID du Bon d'Entrée source
    /// </summary>
    public int? BonEntreeId { get; set; }
    
    /// <summary>
    /// Numéro de référence du Bon d'Entrée source
    /// </summary>
    public string? BonEntreeNumero { get; set; }
    
    /// <summary>
    /// Quantité initiale dans le Bon d'Entrée
    /// </summary>
    public decimal? QuantiteInitialeBem { get; set; }
    
    /// <summary>
    /// Quantité disponible avant cette sortie
    /// </summary>
    public decimal? QuantiteDisponible { get; set; }
    
    /// <summary>
    /// Observations spécifiques
    /// </summary>
    public string? Observations { get; set; }
}
