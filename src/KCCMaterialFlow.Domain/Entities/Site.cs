using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Site/localisation (FROM/TO).
/// EF maps Id → IdSite column.
/// </summary>
public sealed class Site : BaseEntity
{
    [MaxLength(200)]
    public string Nom { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Code { get; set; }

    [MaxLength(500)]
    public string? Adresse { get; set; }

    [MaxLength(50)]
    public string? TypeSite { get; set; }

    public bool EstInterne { get; set; }
    public bool EstActif { get; set; } = true;
    public int OrdreAffichage { get; set; }
}
