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
        //Add database connection
        services.AddDbContext<WorkHiveContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //Add for middle ware validate request
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

        services.AddStackExchangeRedisCache(options =>
        {
            string connection = configuration.GetConnectionString("Redis")!;
            options.Configuration = connection;
        });

        services.AddSession(o =>
        {
            o.IdleTimeout = TimeSpan.FromMinutes(45);//if you send any request in a interval session will auto disabled
            o.Cookie.HttpOnly = true;//avoid accessing from script on browser
        });
        //use to work in outside controller, middleware like services 
        services.AddHttpContextAccessor();

        services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IWorkspaceOwnerUnitOfWork, WorkspaceOwnerUnitOfWork>();
                      services.AddScoped<IWorkSpaceManageUnitOfWork,WorkSpaceManageUnitOfWork>();
        
        return services;
    }

    public static WebApplication UseServiceServices(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        return app;
    }
}