namespace DonationHub.Application.Common.Interfaces;

/// <summary>
/// Unit of Work contract coordinating atomic persistence, database transactions, and repository access.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Persists all tracked entity changes to the database in a single atomic transaction.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates an explicit database transaction for multi-step operations.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the active database transaction to make all changes permanent.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the active database transaction to discard uncommitted changes upon failure.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtains a generic repository instance dynamically for the specified entity type.
    /// </summary>
    IGenericRepository<T> Repository<T>() where T : class;
}
