using FluentValidation;
using WorkHive.APIs;
using WorkHive.Services;

var builder = WebApplication.CreateBuilder(args);
//Get all assemblies from AppDomain
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddServiceServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);
//Scan all assemblies in App Domain to apply validation
builder.Services.AddValidatorsFromAssemblies(assemblies);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiServices();
app.UseServiceServices();

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
