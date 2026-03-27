using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

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

    // Navigation
    [ForeignKey(nameof(LocalisationId))]
    public Localisation? Localisation { get; set; }
}
