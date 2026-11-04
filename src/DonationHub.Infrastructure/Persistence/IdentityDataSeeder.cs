using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Common.Options;
using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Persistence;

/// <summary>
/// Seeds initial administrator and required seed records at application startup (IDN-11).
/// </summary>
public static class IdentityDataSeeder
{
    /// <summary>
    /// Checks for and seeds initial platform administrator account using configured SeedAdminOptions.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedAdminOptions>>().Value;
        var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger(nameof(IdentityDataSeeder));

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
