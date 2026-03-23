using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPlusPlus.Models;

[Table("T_Users")]
public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Key]
    [MaxLength(50)]
    [Column("login")]
    public string Login { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Password { get; set; }

    [MaxLength(50)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? Email { get; set; }

    public bool? Activated { get; set; }

    // FK → Rôle (un utilisateur a un seul rôle)
    public int? RoleId { get; set; }

    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }

    public ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();

    // Navigation: User -> UserLocalisations (1:N) - relation many-to-many via T_User_Localisations
    public ICollection<UserLocalisation> UserLocalisations { get; set; } = new List<UserLocalisation>();
}
