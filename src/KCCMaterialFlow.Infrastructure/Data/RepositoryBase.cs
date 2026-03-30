using System.Linq.Expressions;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Data;

/// <summary>
/// Implémentation générique du pattern Repository avec Entity Framework Core.
/// Fournit les opérations CRUD de base pour n'importe quelle entité.
/// </summary>
/// <typeparam name="T">Type de l'entité (doit être une classe)</typeparam>
public class RepositoryBase<T> : IRepository<T> where T : class
{
    private readonly IDbContextFactory<KCCMaterialFlowDbContext> _dbContextFactory;

    /// <summary>
    /// Constructeur avec injection du factory de DbContext
    /// </summary>
    /// <param name="dbContextFactory">Factory pour créer des instances de DbContext</param>
    public RepositoryBase(IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <inheritdoc />
    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<T>().FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> GetByIdAsync(int id, Expression<Func<T, object>>[] includes, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var query = context.Set<T>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<T>().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<T>().Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await context.Set<T>().AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Set<T>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await context.Set<T>().FindAsync([id], cancellationToken);
        if (entity != null)
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public IQueryable<T> GetQueryable()
    {
        // Note: Cette méthode crée un contexte qui doit être géré manuellement.
        // Préférer les autres méthodes async quand possible.
        var context = _dbContextFactory.CreateDbContext();
        return context.Set<T>().AsQueryable();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Set<T>().AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return predicate == null
            ? await context.Set<T>().CountAsync(cancellationToken)
            : await context.Set<T>().CountAsync(predicate, cancellationToken);
    }
}
