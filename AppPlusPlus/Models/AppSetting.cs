using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

/// <summary>
/// Correspond à la table [dbo].[T_AppSettings] — Paramètres globaux de l'application (clé/valeur).
/// </summary>
[Table("T_AppSettings")]
public class AppSetting
{
    [Key]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Value { get; set; }

    public ICollection<ShopProfile> ShopProfiles { get; set; } = new List<ShopProfile>();
}
