namespace KCCMaterialFlow.Application.Common.Interfaces;

public sealed class WorkflowContextSummary
{
    public string? RaisonSortieCode { get; init; }
    public string RaisonSortieNom { get; init; } = string.Empty;
    public bool EstPersonnalise { get; init; }
    public int NombreEtapesBD { get; init; }
    public int NombreEtapesResolu { get; init; }
    public string Source { get; init; } = string.Empty;
}
