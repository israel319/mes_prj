namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Rôles des utilisateurs dans le système
/// </summary>
public enum RoleUtilisateur
{
    /// <summary>
    /// Demandeur - Peut créer des bons, suivre ses demandes
    /// </summary>
    Demandeur = 1,

    /// <summary>
    /// Superviseur - Première validation hiérarchique
    /// </summary>
    Superviseur = 2,

    /// <summary>
    /// General Manager (GM) - Validation direction
    /// </summary>
    GM = 3,

    /// <summary>
    /// OPJ (Officier de Police Judiciaire) - Validation sécurité
    /// </summary>
    OPJ = 4,

    /// <summary>
    /// Département IT - Approbation matériel informatique
    /// </summary>
    IT = 5,

    /// <summary>
    /// Département Environnement - Approbation résidus, radioprotection, modification
    /// </summary>
    Environnement = 6,

    /// <summary>
    /// Équipe Identification - Vérification finale, génération QR, extension prêts
    /// </summary>
    Identification = 7,

    /// <summary>
    /// Agent Barrière - Scan QR, contrôle physique aux checkpoints
    /// </summary>
    Barriere = 8,

    /// <summary>
    /// Investigation - Traitement des anomalies
    /// </summary>
    Investigation = 9,

    /// <summary>
    /// Administrateur - Gestion complète du système
    /// </summary>
    Admin = 10
}
