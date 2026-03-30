namespace KCCMaterialFlow.Host.Components.Shared.Workflow;

/// <summary>
/// Types d'actions workflow
/// </summary>
public enum WorkflowActionType
{
    Approve,
    Reject,
    Return,
    RequestInfo,
    Comment
}

/// <summary>
/// Résultat du modal de commentaire
/// </summary>
public class CommentModalResult
{
    public bool Confirmed { get; set; }
    public string Comment { get; set; } = string.Empty;
}
