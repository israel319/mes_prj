namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Statut de passage à un checkpoint
/// </summary>
public enum StatutPassage
{
    Prevu = 0,
    Passe = 1,
    Saute = 2,
    Anomalie = 3,
    Annule = 4
}
