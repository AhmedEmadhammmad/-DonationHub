using DonationHub.Infrastructure.Persistence;

namespace DonationHub.Infrastructure;

/// <summary>
/// Extension methods for application bootstrap and persistence lifecycle.
/// </summary>
public static class SeedExtensions
{
    /// <summary>
    /// Seeds default platform administrator and identity records at startup (IDN-11).
    /// </summary>
    public static Task SeedIdentityDataAsync(this IServiceProvider services)
    {
        return IdentityDataSeeder.SeedAsync(services);
    }
}

