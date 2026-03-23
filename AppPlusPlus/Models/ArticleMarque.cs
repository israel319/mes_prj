using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

[Table("T_Art_Marques")]
public class ArticleMarque
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id_Marque")]
    public int IdMarque { get; set; }

    [Column("Description_Marque")]
    public string? DescriptionMarque { get; set; }
}
