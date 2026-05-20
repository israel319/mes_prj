using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Employé = Utilisateur du système (1:1, identifié par Matricule).
/// Source : DATA.xlsx (sheet Sheet3) importée via DataImportService.
/// Tout employé peut soumettre une demande ; la validation suit la chaîne ReportTo.
/// EF maps Id → IdEmployee column.
/// </summary>
public sealed class Employee : BaseEntity
{
    /// <summary>Matricule officiel (ex. K26561). Clé d'identification + lien vers login Windows.</summary>
    [MaxLength(50)]
    public string? Matricule { get; set; }

    /// <summary>
    /// Numéro interne legacy (Excel "EmployeeEntity", ex. "9359").
    /// Utilisé pour résoudre les liens ReportTo entre employés.
    /// </summary>
    [MaxLength(50)]
    public string? NumeroEmploye { get; set; }

    /// <summary>Nom complet brut (depuis l'import Excel).</summary>
    [MaxLength(200)]
    public string NomComplet { get; set; } = string.Empty;

    /// <summary>
    /// Nom à afficher dans l'UI (en-tête, listes, traçabilité).
    /// Source unique pour tout affichage utilisateur.
    /// Initialisé depuis NomComplet à l'import, modifiable via admin.
    /// </summary>
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Prenom { get; set; }

    [MaxLength(100)]
    public string? Nom { get; set; }

    [MaxLength(200)]
    public string? Fonction { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Telephone { get; set; }

    /// <summary>Département tel que reçu de l'Excel (chaîne libre, indépendant de T_Departements).</summary>
    [MaxLength(200)]
    public string? DepartementNom { get; set; }

    /// <summary>Source/origine de l'employé dans le système RH.</summary>
    [MaxLength(100)]
    public string? Sources { get; set; }

    /// <summary>Texte libre du manager direct (pour les utilisateurs non liés au référentiel RH).</summary>
    [MaxLength(200)]
    public string? ReportToText { get; set; }

    public bool PeutEtreEscorteur { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;

    // ── Hiérarchie (self-FK) ─────────────────────────────────────────
    /// <summary>Manager direct de l'employé. Première étape de la chaîne d'approbation.</summary>
    public int? ReportToEmployeeId { get; set; }
    public Employee? ReportTo { get; set; }
    public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

    // ── Compagnie (legacy, conservé pour compatibilité) ─────────────
    public int? CompagnieId { get; set; }
    public Compagnie? Compagnie { get; set; }

    public bool EstInterne { get; set; } = true;
}
