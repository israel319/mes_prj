using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppPlusPlus.Domain.Entities.Catalogue;

namespace AppPlusPlus.Domain.Entities.Commandes;

[Table("T_Livraison_Detail")]
public class LivraisonDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? LivraisonId { get; set; }

    public int? CommandDetailId { get; set; }

    [MaxLength(100)]
    public string? ArticleId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Qte { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MontantPaye { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Taux { get; set; }

    [MaxLength(50)]
    public string? Comment { get; set; }

    public DateTime? Date { get; set; }

    [MaxLength(50)]
    public string? UserLogin { get; set; }

    public DateTime? CreationDate { get; set; }

    [Column("Localisationid")]
    public int? LocalisationId { get; set; }

    // Navigation
    [ForeignKey(nameof(LivraisonId))]
    public Livraison? Livraison { get; set; }

    [ForeignKey(nameof(CommandDetailId))]
    public CommandeDetail? CommandeDetail { get; set; }

    [ForeignKey(nameof(ArticleId))]
    public Article? Article { get; set; }
}
