namespace KCCMaterialFlow.Application.Interfaces;

/// <summary>
/// Interface d'abstraction du DbContext pour découpler les modules de l'Infrastructure.
/// Permet aux Services et Repositories d'utiliser le DbContext sans référencer directement KCCMaterialFlowDbContext.
/// Note: Cette interface est volontairement minimaliste pour éviter les dépendances EF Core dans Core.
/// Pour des opérations avancées (queries, includes), les implémentations peuvent caster vers le type concret.
/// </summary>
public interface IAppDbContext : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Sauvegarde les modifications en base de données.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sauvegarde les modifications en base de données (synchrone).
    /// </summary>
    int SaveChanges();
}
