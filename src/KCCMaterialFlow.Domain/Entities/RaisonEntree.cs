using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Motif/raison d'entrée structuré (remplace le champ libre ReasonOnSite).
/// Chaque RaisonEntree mappe vers un ou plusieurs RaisonSortie via la table de jonction.
/// </summary>
public sealed class RaisonEntree : BaseEntity
{
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Code { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool EstActif { get; set; } = true;
    public int OrdreAffichage { get; set; }

    [MaxLength(50)]
    public string? Icone { get; set; }

    [MaxLength(20)]
    public string? Couleur { get; set; }

    // Navigation vers les motifs de sortie autorisés
    public ICollection<RaisonEntreeRaisonSortie> RaisonsSortieAutorisees { get; set; } = new List<RaisonEntreeRaisonSortie>();
}
