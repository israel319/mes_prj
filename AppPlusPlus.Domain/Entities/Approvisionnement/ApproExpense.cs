using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Domain.Entities.Approvisionnement;

/// <summary>
/// T_Appro_Expense -- Versements / approvisionnement de caisse.
/// </summary>
[Table("T_Appro_Expense")]
public class ApproExpense
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int SourceId { get; set; }

    public int? CaisseId { get; set; }

    public int? CurrencyId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AmountUSD { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AmountCDF { get; set; }

    [MaxLength(50)]
    public string? Depositeur { get; set; }

    [MaxLength(50)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Comment { get; set; }

    [MaxLength(50)]
    public string? UserLogin { get; set; }

    public DateTime? CreationDate { get; set; }
}
