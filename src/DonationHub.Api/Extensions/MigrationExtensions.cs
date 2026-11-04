using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Common.Options;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DonationHub.Api.Extensions;

/// <summary>
/// Extension methods for application initialization, database migrations, and identity seed data.
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Seeds initial platform administrator user account at startup using configured options (Task 7: IDN-11).
    /// </summary>
    public static async Task SeedAdminUserAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedAdminOptions>>().Value;
        var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger(nameof(MigrationExtensions));

        if (string.IsNullOrWhiteSpace(adminOptions.Email) || string.IsNullOrWhiteSpace(adminOptions.Password))
        {
            return;
        }

        var normalizedEmail = adminOptions.Email.Trim().ToLowerInvariant();
        var adminExists = await context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == normalizedEmail);

        if (!adminExists)
        {
            var passwordHash = passwordHasher.HashPassword(adminOptions.Password);
            var adminUser = ApplicationUser.Create(
                id: Guid.NewGuid(),
                email: normalizedEmail,
                userName: normalizedEmail,
                fullName: string.IsNullOrWhiteSpace(adminOptions.FullName) ? "Platform Administrator" : adminOptions.FullName.Trim(),
                passwordHash: passwordHash,
                nowUtc: DateTime.UtcNow,
                role: ApplicationUser.RoleAdmin,
                preferredLanguage: "ar");

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            logger?.LogInformation("Successfully seeded default Admin account: {Email}", normalizedEmail);
        }
    }
}
