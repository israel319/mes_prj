namespace KCCMaterialFlow.Module.BonEntree.DTOs;

/// <summary>
/// DTO pour l'affichage d'un matériel - 5 champs selon diagramme UML
/// </summary>
public class MaterielDto
{
    /// <summary>
    /// Identifiant du matériel
    /// </summary>
    public int IdMateriel { get; set; }

    /// <summary>
    /// Code produit ou numéro de série
    /// </summary>
    /// 
    
    public string CodeProduitSerial { get; set; } = string.Empty;

    /// <summary>
    /// Désignation du matériel
    /// </summary>
    public string Designation { get; set; } = string.Empty;

    /// <summary>
    /// Quantité (décimal pour poids/volumes)
    /// </summary>
    public decimal Quantite { get; set; } = 1;
}
