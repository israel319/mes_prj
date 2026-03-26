using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppPlusPlus.Domain.Entities.Catalogue;
using AppPlusPlus.Domain.Entities.Administration;

namespace AppPlusPlus.Domain.Entities.Stock;

/// <summary>
/// Stock par article et localisation
/// Remplace l'ancien ArticleLocalisation
/// </summary>
[Table("T_Stock")]
public class Stock
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("Id_Article")]
    public string IdArticle { get; set; } = string.Empty;

    [Column("Id_Localisation")]
    public int IdLocalisation { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Qte { get; set; } = 0;

    public int Seuil { get; set; } = 0;

    [MaxLength(50)]
    public string? CodeBar { get; set; }

    public DateOnly? DateSys { get; set; }

    [MaxLength(50)]
    public string? UserLogin { get; set; }

    // Navigation properties
    [ForeignKey(nameof(IdArticle))]
    public Article? Article { get; set; }

    [ForeignKey(nameof(IdLocalisation))]
    public Localisation? Localisation { get; set; }

    // Proprietes calculees
    [NotMapped]
    public bool EstEnRupture => Qte <= 0;

    [NotMapped]
    public bool EstSousSeuil => Qte <= Seuil && Qte > 0;

    [NotMapped]
    public string EtatStock => Qte <= 0 ? "Rupture" : (Qte <= Seuil ? "Bas" : "OK");
}
