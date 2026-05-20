using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Utilisateur du système — table d'authentification Windows.
/// Login = SAM account name Windows (ex. "DOMAIN\bkamosi" ou "bkamosi").
/// Lié à un Employee (1:1) pour les données RH et la chaîne d'approbation.
/// </summary>
public sealed class AppUser : BaseEntity
{
    /// <summary>Login Windows (DOMAIN\sam ou sam). Unique, insensible à la casse.</summary>
    [MaxLength(150)]
    public string Login { get; set; } = string.Empty;

    /// <summary>Employé associé (source des données RH : nom, matricule, département…).</summary>
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    /// <summary>Niveau d'administration applicatif (Aucun / Admin / SuperAdmin).</summary>
    public NiveauAdmin NiveauAdmin { get; set; } = NiveauAdmin.Aucun;

    /// <summary>Compte actif. Seuls les comptes actifs peuvent se connecter.</summary>
    public bool EstActif { get; set; } = true;

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}
