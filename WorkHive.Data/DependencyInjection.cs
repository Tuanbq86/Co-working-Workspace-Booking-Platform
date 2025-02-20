using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkHive.Data.Base;

namespace WorkHive.Data;
public static class DependencyInjection
{
    public static IServiceCollection AddDataServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        

        return services;
    }
}
