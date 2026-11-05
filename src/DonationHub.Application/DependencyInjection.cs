using DonationHub.Application.Features.Auth.Services;
using DonationHub.Application.Features.Categories.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Application;

/// <summary>
/// Registers application layer services and use case handlers.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configures application core services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
