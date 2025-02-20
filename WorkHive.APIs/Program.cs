using Carter;
using FluentValidation;
using WorkHive.APIs;
using WorkHive.BuildingBlocks.Behaviors;
using WorkHive.BuildingBlocks.Exceptions.Handler;
using WorkHive.Services;

var builder = WebApplication.CreateBuilder(args);
var program = typeof(Program).Assembly;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddServiceServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
