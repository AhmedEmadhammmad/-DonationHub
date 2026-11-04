using DonationHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic repository implementation backed by EF Core for tracking and mutating domain aggregates.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new repository instance for the target entity set.
    /// </summary>
    public GenericRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Retrieves an entity by primary key with tracking enabled.
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object?[] { id }, cancellationToken);
    }

    /// <summary>
    /// Enrolls a new entity into the change tracker in Added state.
    /// </summary>
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Sets the entity state to Modified in the change tracker.
    /// </summary>
    public virtual void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Sets the entity state to Deleted in the change tracker.
    /// </summary>
    public virtual void Delete(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Remove(entity);
    }
}
