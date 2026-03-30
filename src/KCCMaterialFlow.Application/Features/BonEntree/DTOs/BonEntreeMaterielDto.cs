namespace KCCMaterialFlow.Application.Features.BonEntree.DTOs;

/// <summary>
/// DTO pour l'affichage d'un matériel dans un bon d'entrée - 5 champs selon diagramme UML
/// </summary>
public class BonEntreeMaterielDto
{
    public int IdMateriel { get; set; }
    public string CodeProduitSerial { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Quantite { get; set; } = 1;
}
