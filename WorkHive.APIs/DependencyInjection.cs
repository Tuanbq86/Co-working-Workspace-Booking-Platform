using Carter;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WorkHive.BuildingBlocks.Exceptions.Handler;

namespace WorkHive.APIs;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter();

        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.MapCarter();

        app.UseExceptionHandler(options => { });

        return app;
    }
}
