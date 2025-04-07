using FluentValidation;
using Microsoft.OpenApi.Models;
using WorkHive.APIs;
using WorkHive.Services;

var builder = WebApplication.CreateBuilder(args);
//Get all assemblies from AppDomain
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
//Add for authorization
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My WorkHive API",
        Version = "v1"
    });

    // Thêm cấu hình để Swagger hỗ trợ JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token vào đây. VD: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddServiceServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);
//Scan all assemblies in App Domain to apply validation
builder.Services.AddValidatorsFromAssemblies(assemblies);
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", builder
        => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment() /*|| app.Environment.IsProduction()*/)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My WorkHive API");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.UseApiServices();
app.UseServiceServices();

app.UseHttpsRedirection();
app.UseCors("CORSPolicy");

app.Run();
