namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// Factory pour créer des instances de IAppDbContext.
/// Permet aux modules d'obtenir un DbContext sans dépendre directement de l'Infrastructure.
/// Utilisé à la place de IDbContextFactory&lt;KCCMaterialFlowDbContext&gt; dans les modules.
/// </summary>
public interface IAppDbContextFactory
{
    /// <summary>
    /// Crée une nouvelle instance du DbContext.
    /// L'appelant est responsable de disposer l'instance retournée.
    /// </summary>
    IAppDbContext CreateDbContext();

    /// <summary>
    /// Crée une nouvelle instance du DbContext de manière asynchrone.
    /// </summary>
    Task<IAppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}
