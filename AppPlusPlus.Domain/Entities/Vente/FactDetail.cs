using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppPlusPlus.Domain.Entities.Catalogue;

namespace AppPlusPlus.Domain.Entities.Vente;

/// <summary>
/// Correspond a la table [dbo].[T_Fact_Details] -- Lignes de detail d'une facture.
/// </summary>
[Table("T_Fact_Details")]
public class FactDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("Id_Fact")]
    public int? IdFact { get; set; }

    [MaxLength(50)]
    [Column("Id_Article")]
    public string? IdArticle { get; set; }

    public double? Qte { get; set; }

    public double? Pu { get; set; }

    public int? Status { get; set; }

    public int? Localisationid { get; set; }

    // Navigation
    [ForeignKey(nameof(IdFact))]
    public Fact? Fact { get; set; }

    [ForeignKey(nameof(IdArticle))]
    public Article? Article { get; set; }
}
