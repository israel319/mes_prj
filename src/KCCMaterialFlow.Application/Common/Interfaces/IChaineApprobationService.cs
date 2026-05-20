using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Service qui construit la chaîne d'approbateurs ordonnée pour un demandeur donné.
///
/// Règle métier v2 (basée sur le référentiel Glencore + WorkflowApprobateurSpecial) :
///   1. SuperIntendent si renseigné, sinon Manager (ManagerHod) si renseigné, sinon ignoré.
///      → Un seul des deux au maximum dans la chaîne.
///   2. GM du demandeur (Glencore.GmEmployeeCode)
///   3. OPJ (WorkflowApprobateurSpecial filtré par site, fallback sur ligne globale SiteId=NULL)
///   4. Identification (WorkflowApprobateurSpecial — étape OR : 1er qui approuve valide)
///
/// Doublons consécutifs supprimés (skip si même Employee 2× de suite).
/// </summary>
public interface IChaineApprobationService
{
    /// <summary>
    /// Construit la chaîne ordonnée. Lève <see cref="InvalidOperationException"/> si :
    /// - le demandeur n'a pas de ligne Glencore correspondante (par Login Windows)
    /// - le demandeur n'a pas de ReportsTo Glencore configuré
    /// - aucun OPJ / Identification configuré (global ou par site)
    /// </summary>
    /// <param name="demandeurEmployeeId">Id local de l'Employee créateur du bon.</param>
    /// <param name="typeMateriel">OBSOLÈTE. Utiliser le département du demandeur à la place. null = pas de routage spécial.</param>
    /// <param name="siteId">Site du bon (pour résoudre l'OPJ par site). null = OPJ global uniquement.</param>
    Task<ChaineApprobationResult> ConstruireChaineAsync(
        int demandeurEmployeeId,
        string? descriptionMateriel,
        int? siteId,
        WorkflowRoutage routage = WorkflowRoutage.Standard,
        CancellationToken ct = default);

    /// <summary>
    /// Validation légère utilisable depuis l'UI/l'API : retourne true si la chaîne peut être construite
    /// (sinon expose les raisons via <see cref="ChaineApprobationValidation.Erreurs"/>).
    /// OBSOLÈTE : À remplacer par une version basée sur le département du demandeur.
    /// </summary>
    Task<ChaineApprobationValidation> ValiderAsync(
        int demandeurEmployeeId,
        string? descriptionMateriel,
        int? siteId,
        WorkflowRoutage routage = WorkflowRoutage.Standard,
        CancellationToken ct = default);
}

/// <summary>Une étape de la chaîne d'approbation.</summary>
public sealed record ChaineApprobationEtape(
    int Ordre,
    EtapeApprobationKind Kind,
    int EmployeeId,
    string EmployeeNomComplet,
    string? EmployeeMatricule,
    string? EmployeeLogin,
    /// <summary>Pour les étapes de type OR (Identification) — IDs des co-approbateurs au même niveau.</summary>
    IReadOnlyList<int> CoApprobateurIds);

public enum EtapeApprobationKind
{
    /// <summary>Superintendent du demandeur (Glencore.SuperIntendentEmployeeCode). Prioritaire sur Manager.</summary>
    SuperIntendent,
    /// <summary>Manager direct du demandeur (Glencore.ManagerHodEmployeeCode). Utilisé si Superintendent absent.</summary>
    ReportsTo,
    /// <summary>General Manager du demandeur (Glencore.GmEmployeeCode).</summary>
    GM,
    /// <summary>Département IT — applicable si TypeMateriel = Informatique.</summary>
    ITDepartment,
    /// <summary>Département Environnement — applicable si TypeMateriel ∈ {Residu, Radioprotection, Modification}.</summary>
    EnvironmentDepartment,
    /// <summary>Approbateur spécial OPJ (paramétrable par site).</summary>
    OPJ,
    /// <summary>Approbateur spécial Identification (étape OR — finale, impression).</summary>
    Identification
}

public sealed record ChaineApprobationResult(
    int DemandeurEmployeeId,
    IReadOnlyList<ChaineApprobationEtape> Etapes);

public sealed record ChaineApprobationValidation(
    bool EstValide,
    IReadOnlyList<string> Erreurs);
