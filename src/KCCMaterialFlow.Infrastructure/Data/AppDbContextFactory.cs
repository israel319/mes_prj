using KCCMaterialFlow.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Data;

/// <summary>
/// Implémentation de IAppDbContextFactory qui wrappe IDbContextFactory&lt;KCCMaterialFlowDbContext&gt;.
/// Permet aux modules d'utiliser l'abstraction IAppDbContext au lieu du type concret.
/// </summary>
public class AppDbContextFactory : IAppDbContextFactory
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _innerFactory;

    public AppDbContextFactory(IDbContextFactory<KCCMaterialFlowDbContext> innerFactory)
    {
        _innerFactory = innerFactory;
    }

    /// <inheritdoc />
    public IAppDbContext CreateDbContext()
    {
        return _innerFactory.CreateDbContext();
    }

    /// <inheritdoc />
    public async Task<IAppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return await _innerFactory.CreateDbContextAsync(cancellationToken);
    }
}
