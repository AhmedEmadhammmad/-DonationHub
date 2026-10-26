using Microsoft.Extensions.DependencyInjection;

namespace DonationHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application layer services registration (e.g. MediatR / Handlers, Validators, Mappers)
        return services;
    }
}
