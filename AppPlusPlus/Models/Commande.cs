using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

[Table("T_Commande")]
public class Commande
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CommandeId { get; set; }

    public int? CustomerId { get; set; }

    [MaxLength(50)]
    public string? CommandeDescription { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MontantTotal { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MontantPaye { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MontantRest { get; set; }

    public int? Status { get; set; }

    [MaxLength(50)]
    public string? UserLogin { get; set; }

    public DateTime? CreationDate { get; set; }

    // Navigation
    [ForeignKey(nameof(CustomerId))]
    public Customer? Customer { get; set; }

    public ICollection<CommandeDetail> Details { get; set; } = new List<CommandeDetail>();

    /// <summary>Livraisons liées à cette commande.</summary>
    public ICollection<Livraison> Livraisons { get; set; } = new List<Livraison>();

    /// <summary>Factures liées (via CommandeId sur T_Facts, pour référence directe).</summary>
    public ICollection<Fact> Factures { get; set; } = new List<Fact>();
}
