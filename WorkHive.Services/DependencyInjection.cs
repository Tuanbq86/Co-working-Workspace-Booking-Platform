using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using WorkHive.BuildingBlocks.Behaviors;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Repositories.Repositories;
using WorkHive.Repositories.UnitOfWork;

namespace WorkHive.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<WorkHiveContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //add for middle ware validate request
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            };
        });

        services.AddAuthorization();

        services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IWorkspaceOwnerUnitOfWork, WorkspaceOwnerUnitOfWork>();
        
        return services;
    }

    public static WebApplication UseServiceServices(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}