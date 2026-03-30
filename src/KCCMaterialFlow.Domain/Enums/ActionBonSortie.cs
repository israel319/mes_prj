namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Types d'actions possibles sur un bon de sortie.
/// </summary>
public enum ActionBonSortie
{
    Creation = 1,
    Modification = 2,
    Soumission = 3,
    Approbation = 4,
    Rejet = 5,
    RetourModification = 6,
    GenerationQR = 7,
    ScanBarriere = 8,
    SortieEffective = 9,
    RetourPret = 10,
    Cloture = 11,
    Annulation = 12,
    Prolongation = 13,
    AjoutMateriel = 14,
    SuppressionMateriel = 15,
    Anomalie = 16,
    Impression = 17,
    Notification = 18,
    ExtensionPret = 19,
    MiseEnInvestigation = 20
}
