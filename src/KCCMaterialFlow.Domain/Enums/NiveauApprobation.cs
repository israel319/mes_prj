namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Niveaux d'approbation dans le workflow des bons d'entrée de matériel.
/// Correspond aux trois signatures sur le formulaire SEC-FM-141(B).
/// </summary>
public enum NiveauApprobation
{
    /// <summary>
    /// Security GATE / Commandant Secteur - Premier niveau (optionnel, pour scan d'entrée).
    /// </summary>
    SecurityGate = 0,

    /// <summary>
    /// Head of KCC Department / Responsable du Département KCC - Validation métier.
    /// </summary>
    HeadOfDepartment = 10,

    /// <summary>
    /// Security Department / Département de Sécurité - Validation sécuritaire finale.
    /// </summary>
    SecurityDepartment = 20
}
