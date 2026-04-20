using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppPlusPlus.Domain.Entities.Administration;
using AppPlusPlus.Domain.Entities.Approvisionnement;

namespace AppPlusPlus.Domain.Entities.Finance;

[Table("T_Versements")]
public class Versement
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateOnly DateCloture { get; set; }

    [Required]
    public DateTime HeureCloture { get; set; }

    [Required]
    public int LocalisationId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TypeOperation { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Montant { get; set; }

    [Required]
    [MaxLength(50)]
    public string ModeVersement { get; set; } = string.Empty;

    public int NombreOperations { get; set; }

    [MaxLength(500)]
    public string? Observation { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserLogin { get; set; } = string.Empty;

    [Required]
    public DateTime DateSys { get; set; } = DateTime.Now;

    /// <summary>0 = En attente, 1 = Approuvée, 2 = Rejetée</summary>
    public int StatutCloture { get; set; } = 0;

    [MaxLength(500)]
    public string? MotifRejet { get; set; }

    [MaxLength(50)]
    public string? TraitePar { get; set; }

    public DateTime? DateTraitement { get; set; }

    // Navigation
    [ForeignKey(nameof(LocalisationId))]
    public Localisation? Localisation { get; set; }

    /// <summary>Versement de caisse généré automatiquement lors de cette clôture.</summary>
    public ApproExpense? ApproExpense { get; set; }
}
