using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Common.Interfaces;

/// <summary>
/// Database context contract providing direct read-path access with projection and AsNoTracking queries.
/// </summary>
public interface IAppDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Category> Categories { get; }
    DbSet<Campaign> Campaigns { get; }
    DbSet<Donation> Donations { get; }
    DbSet<PaymentAttempt> PaymentAttempts { get; }
    DbSet<ProcessedStripeEvent> ProcessedStripeEvents { get; }
    DbSet<IdempotencyRecord> IdempotencyRecords { get; }

    /// <summary>
    /// Persists tracked changes directly through the context.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
