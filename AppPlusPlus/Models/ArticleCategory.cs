using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

[Table("T_Art_Categorys")]
public class ArticleCategory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id_Category")]
    public int IdCategory { get; set; }

    [MaxLength(50)]
    [Column("Description_Category")]
    public string? DescriptionCategory { get; set; }
}
