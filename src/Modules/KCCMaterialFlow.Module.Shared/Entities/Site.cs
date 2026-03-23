using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Shared.Entities;

/// <summary>
/// Représente un site/localisation (FROM/TO)
/// </summary>
public class Site
{
    /// <summary>
    /// Identifiant unique du site (clé primaire)
    /// </summary>
    [Key]
    public int IdSite { get; set; }

    /// <summary>
    /// Nom du site
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Code court du site
    /// </summary>
    [MaxLength(20)]
    public string? Code { get; set; }

    /// <summary>
    /// Adresse ou localisation
    /// </summary>
    [MaxLength(500)]
    public string? Adresse { get; set; }

    /// <summary>
    /// Type de site (Interne KCC, Externe, Barrière, etc.)
    /// </summary>
    [MaxLength(50)]
    public string? TypeSite { get; set; }

    /// <summary>
    /// Indique si c'est un site interne KCC
    /// </summary>
    public bool EstInterne { get; set; } = false;

    /// <summary>
    /// Indique si le site est actif
    /// </summary>
    public bool EstActif { get; set; } = true;

    /// <summary>
    /// Ordre d'affichage
    /// </summary>
    public int OrdreAffichage { get; set; } = 0;
}
