namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Type d'approbateur spécial dans la chaîne de validation des bons.
/// Chaque valeur correspond à un rôle spécial non résolu automatiquement depuis la hiérarchie Glencore.
/// </summary>
public enum TypeApprobateurSpecial
{
    /// <summary>Officier de Police Judiciaire — obligatoire dans tous les workflows sortie.</summary>
    OPJ = 1,

    /// <summary>Agent d'identification finale — génération du QR code de sortie.</summary>
    Identification = 2,

    /// <summary>Département IT — approbation IT (matériels informatiques, etc.).</summary>
    IT = 3,

    /// <summary>Département Environnement — approbation environnementale.</summary>
    Environment = 4
}
