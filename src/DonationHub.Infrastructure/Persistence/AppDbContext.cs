using DonationHub.Application.Common.Interfaces;
using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Persistence;

/// <summary>
/// Primary Entity Framework Core database context implementing IAppDbContext for read and write operations.
/// </summary>
public class AppDbContext : DbContext, IAppDbContext
{
    /// <summary>
    /// Initializes a new database context instance with configured options.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();
    public DbSet<ProcessedStripeEvent> ProcessedStripeEvents => Set<ProcessedStripeEvent>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    /// <summary>
    /// Applies entity configurations discovered from the persistence assembly.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

