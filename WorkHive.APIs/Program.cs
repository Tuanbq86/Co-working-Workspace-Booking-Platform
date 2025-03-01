using FluentValidation;
using WorkHive.APIs;
using WorkHive.Services;

var builder = WebApplication.CreateBuilder(args);
//Get all assemblies from AppDomain
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen();

builder.Services.AddServiceServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);
//Scan all assemblies in App Domain to apply validation
builder.Services.AddValidatorsFromAssemblies(assemblies);
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", builder
        => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My WorkHive API");
    });
}

app.UseApiServices();
app.UseServiceServices();

app.UseHttpsRedirection();
app.UseCors("CORSPolicy");

app.Run();
