using DonationHub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DonationHub.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Intercepts EF Core save operations to convert hard deletes of ISoftDeletable entities into soft deletes.
/// </summary>
public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts synchronous SaveChanges to mutate deleted entities into soft-deleted state.
    /// </summary>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateSoftDeletableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Intercepts asynchronous SaveChangesAsync to mutate deleted entities into soft-deleted state.
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateSoftDeletableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Updates the entity state from Deleted to Modified and sets deletion audit metadata.
    /// </summary>
    private static void UpdateSoftDeletableEntities(DbContext? context)
    {
        if (context is null)
            return;

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAtUtc = DateTime.UtcNow;
            }
        }
    }
}
