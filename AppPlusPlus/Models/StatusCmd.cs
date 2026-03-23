using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

[Table("T_Status_Cmds")]
public class StatusCmd
{
    [Key]
    [Column("Id_Status")]
    public int IdStatus { get; set; }

    [MaxLength(50)]
    [Column("Descrption_Status")]
    public string? DescriptionStatus { get; set; }
}
