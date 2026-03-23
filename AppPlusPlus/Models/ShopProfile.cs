using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

/// <summary>
/// Profil du shop utilisé sur les documents imprimés (facture, devis, etc.).
/// </summary>
[Table("T_Profile")]
public class ShopProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string AppNameSettingKey { get; set; } = "AppName";

    public string? PhotoShop { get; set; }

    [MaxLength(250)]
    public string? Adresse1 { get; set; }

    [MaxLength(250)]
    public string? Adresse2 { get; set; }

    [ForeignKey(nameof(AppNameSettingKey))]
    public AppSetting? AppNameSetting { get; set; }
}
