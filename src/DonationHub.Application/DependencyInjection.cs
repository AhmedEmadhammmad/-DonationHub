using DonationHub.Application.Common.Behaviors;
using DonationHub.Application.Features.Auth.Services;
using DonationHub.Application.Features.Categories.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Application;

/// <summary>
/// Registers application layer services, MediatR pipeline behaviors, validators, and feature slices.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configures application core services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        // Keep Sprint 1 services active until feature slices are fully migrated
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
