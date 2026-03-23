namespace KCCMaterialFlow.Module.Shared.Services;

/// <summary>
/// Résumé d'un contexte de workflow pour l'administration.
/// Indique si un motif a une configuration BD spécifique (Personnalisé)
/// ou s'il hérite du workflow générique / du défaut métier.
/// </summary>
public sealed class WorkflowContextSummary
{
    /// <summary>Code du motif. Null = workflow par défaut (sans motif spécifique).</summary>
    public string? RaisonSortieCode { get; init; }

    /// <summary>Nom affiché du motif.</summary>
    public string RaisonSortieNom { get; init; } = string.Empty;

    /// <summary>
    /// True = une configuration explicite existe en BD pour ce contexte (BonType + Motif).
    /// False = le workflow sera résolu depuis le générique ou le défaut métier.
    /// </summary>
    public bool EstPersonnalise { get; init; }

    /// <summary>Nombre d'étapes dans la config BD (0 si non personnalisé).</summary>
    public int NombreEtapesBD { get; init; }

    /// <summary>Nombre d'étapes du workflow résolu (incluant l'héritage).</summary>
    public int NombreEtapesResolu { get; init; }

    /// <summary>Source lisible : "BD spécifique", "BD générique (hérité)", "Défaut métier".</summary>
    public string Source { get; init; } = string.Empty;
}
