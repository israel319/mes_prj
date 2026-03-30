namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Statuts possibles d'un bon d'entrée de matériel dans le workflow.
/// Values match the StatutActuel string column via ToString().
/// </summary>
public enum StatutBonEntree
{
    Draft = 0,
    PendingSup = 10,
    PendingGM = 20,
    PendingIT = 25,
    PendingEnv = 26,
    PendingOPJ = 30,
    Approved = 50,
    InTransit = 60,
    Completed = 70,
    Archived = 80,
    Rejected = 90,
    Returned = 95,
    Investigation = 100
}
