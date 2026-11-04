using DonationHub.Application.Common.Interfaces;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Interceptors;
using DonationHub.Infrastructure.Persistence.Repositories;
using DonationHub.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Infrastructure;

/// <summary>
/// Registers infrastructure persistence services, database interceptors, and data access contracts.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configures AppDbContext with interceptors, connection string, and registers repositories and Unit of Work.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in configuration.");


        services.AddSingleton<SoftDeleteInterceptor>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var softDeleteInterceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();

            options.UseSqlServer(connectionString)
                   .AddInterceptors(softDeleteInterceptor);
        });

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
