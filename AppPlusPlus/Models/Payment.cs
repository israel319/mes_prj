using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

/// <summary>
/// Correspond à la table [dbo].[T_Payments] — Paiements sur factures.
/// </summary>
[Table("T_Payments")]
public class Payment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("Id_Fact")]
    public int IdFact { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string? Note { get; set; }

    [Required]
    public DateTime DateSys { get; set; } = DateTime.Now;

    [MaxLength(50)]
    [Column("User")]
    public string? User { get; set; }

    // Navigation
    [ForeignKey(nameof(IdFact))]
    public Fact? Fact { get; set; }
}
