using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

[Table("T_Commande_Details")]
public class CommandeDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? CommandeId { get; set; }

    [MaxLength(50)]
    public string? ArticleId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Qte { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? QtePrise { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? QteRest { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Pu { get; set; }

    public double? Taux { get; set; }

    public int? Status { get; set; }

    // Navigation
    [ForeignKey(nameof(CommandeId))]
    public Commande? Commande { get; set; }

    [ForeignKey(nameof(ArticleId))]
    public Article? Article { get; set; }
}
