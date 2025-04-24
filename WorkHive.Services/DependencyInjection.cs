using CloudinaryDotNet;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using WorkHive.BuildingBlocks.Behaviors;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Repositories.Repositories;
using WorkHive.Repositories.UnitOfWork;
using WorkHive.Services.EmailServices;
using WorkHive.Services.UploadFiles;
using WorkHive.Services.Users.FirebaseServices;

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
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                RoleClaimType = "RoleId"
            };
        });

        services.AddAuthorization(op =>
        {
            op.AddPolicy("Admin", policy => policy.RequireClaim("RoleId", "1"));
            op.AddPolicy("Manager", policy => policy.RequireClaim("RoleId", "2"));
            op.AddPolicy("Staff", policy => policy.RequireClaim("RoleId", "3"));
            op.AddPolicy("Customer", policy => policy.RequireClaim("RoleId", "4"));
            op.AddPolicy("Owner", policy => policy.RequireClaim("RoleId", "5"));

            //Hoặc có role này hoặc có role kia
            op.AddPolicy("OwnerOrAdmin", policy => policy.RequireClaim("RoleId", "1", "5"));
        });

        //Save session into Ram when I restart all data will delete
        services.AddDistributedMemoryCache();

        //services.AddStackExchangeRedisCache(options =>
        //{
        //    string connection = configuration.GetConnectionString("Redis")!;
        //    options.Configuration = connection;
        //});

        services.AddSession(o =>
        {
            o.IdleTimeout = TimeSpan.FromMinutes(45);//if you send any request in a interval session will auto disabled
            o.Cookie.HttpOnly = true;//avoid accessing from script on browser
        });
        //use to work in outside controller, middleware like services 
        services.AddHttpContextAccessor();

        //Add configure of cloudinary
        services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
        services.AddSingleton<Cloudinary>(provider =>
        {
            var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
            var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
            return new Cloudinary(account);
        });

        //Add EmailService
        services.AddTransient<IEmailService, EmailService>();

        services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IWorkspaceOwnerUnitOfWork, WorkspaceOwnerUnitOfWork>();
        services.AddScoped<IWorkSpaceManageUnitOfWork,WorkSpaceManageUnitOfWork>();
        services.AddScoped<IUserRatingUnitOfWork, UserRatingUnitOfWork>();
        services.AddScoped<IBookingWorkspaceUnitOfWork, BookingWorkspaceUnitOfWork>();
        services.AddScoped<IWorkSpaceManageUnitOfWork, WorkSpaceManageUnitOfWork>();
        services.AddScoped<IFeedbackManageUnitOfWork, FeedbackManageUnitOfWork>();
        services.AddScoped<IWalletUnitOfWork, WalletUnitOfWork>();

        services.AddSingleton<IFirebaseNotificationService, FirebaseNotificationService>();

        return services;
    }

    public static WebApplication UseServiceServices(this WebApplication app)
    {
        app.UseSession();
        return app;
    }
}