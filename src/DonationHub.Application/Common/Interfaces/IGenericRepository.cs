namespace DonationHub.Application.Common.Interfaces;

/// <summary>
/// Generic repository interface responsible for state mutation and aggregate persistence in the write path.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Retrieves a tracked entity by its unique identifier.
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the persistence context change tracker.
    /// </summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing entity as modified in memory without immediate database roundtrip.
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Marks an entity as deleted in memory for interception or physical removal.
    /// </summary>
    void Delete(T entity);
}
