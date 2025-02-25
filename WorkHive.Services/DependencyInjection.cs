using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.Behaviors;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
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
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();
        services.AddScoped<IOwnerUnitOfWork, OwnerUnitOfWork>();
        
        return services;
    }
}