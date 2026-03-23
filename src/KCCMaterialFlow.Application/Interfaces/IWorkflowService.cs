namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// Interface pour le service de gestion des workflows d'approbation
/// </summary>
public interface IWorkflowService
{
    /// <summary>
    /// Approuve un bon et le fait passer à l'étape suivante du workflow
    /// </summary>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="bonType">Type de bon (BEM, BSM)</param>
    /// <param name="comment">Commentaire d'approbation (optionnel)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération avec le nouveau statut</returns>
    Task<WorkflowResult> ApproveAsync(int bonId, string bonType, string? comment = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejette un bon et termine le workflow
    /// </summary>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="bonType">Type de bon (BEM, BSM)</param>
    /// <param name="comment">Commentaire de rejet (obligatoire)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération</returns>
    Task<WorkflowResult> RejectAsync(int bonId, string bonType, string comment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retourne un bon au demandeur pour modification
    /// </summary>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="bonType">Type de bon (BEM, BSM)</param>
    /// <param name="comment">Commentaire expliquant les modifications demandées</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération</returns>
    Task<WorkflowResult> ReturnAsync(int bonId, string bonType, string comment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Détermine le prochain approbateur dans le workflow
    /// </summary>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="bonType">Type de bon (BEM, BSM)</param>
    /// <param name="typeMateriel">Type de matériel (pour workflow dynamique BSM)</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Information sur le prochain approbateur</returns>
    Task<NextApproverInfo> GetNextApproverAsync(int bonId, string bonType, string? typeMateriel = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si l'utilisateur courant peut approuver le bon
    /// </summary>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="bonType">Type de bon (BEM, BSM)</param>
    /// <param name="userLogin">Login de l'utilisateur</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>True si l'utilisateur peut approuver</returns>
    Task<bool> CanApproveAsync(int bonId, string bonType, string userLogin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère la chaîne d'approbation pour un type de matériel
    /// </summary>
    /// <param name="typeMateriel">Type de matériel</param>
    /// <returns>Liste ordonnée des étapes d'approbation</returns>
    IEnumerable<WorkflowStep> GetApprovalChain(string typeMateriel);
}

/// <summary>
/// Résultat d'une opération de workflow
/// </summary>
public class WorkflowResult
{
    /// <summary>
    /// Indique si l'opération a réussi
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message de résultat ou d'erreur
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Nouveau statut du bon après l'opération
    /// </summary>
    public string? NewStatus { get; set; }

    /// <summary>
    /// Prochain approbateur (si applicable)
    /// </summary>
    public string? NextApprover { get; set; }

    /// <summary>
    /// Indique si le workflow est terminé
    /// </summary>
    public bool IsCompleted { get; set; }
}

/// <summary>
/// Information sur le prochain approbateur
/// </summary>
public class NextApproverInfo
{
    /// <summary>
    /// Login de l'approbateur
    /// </summary>
    public string? ApproverLogin { get; set; }

    /// <summary>
    /// Nom complet de l'approbateur
    /// </summary>
    public string? ApproverName { get; set; }

    /// <summary>
    /// Rôle de l'approbateur dans le workflow
    /// </summary>
    public string? ApproverRole { get; set; }

    /// <summary>
    /// Numéro de l'étape dans le workflow
    /// </summary>
    public int StepNumber { get; set; }

    /// <summary>
    /// Indique si c'est la dernière étape
    /// </summary>
    public bool IsFinalStep { get; set; }
}

/// <summary>
/// Représente une étape du workflow d'approbation
/// </summary>
public class WorkflowStep
{
    /// <summary>
    /// Ordre de l'étape dans le workflow
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Code du rôle approbateur (ex: "SUPERVISEUR", "DG", "OPJ")
    /// </summary>
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// Nom d'affichage de l'étape
    /// </summary>
    public string StepName { get; set; } = string.Empty;

    /// <summary>
    /// Indique si l'étape est optionnelle
    /// </summary>
    public bool IsOptional { get; set; }
}
