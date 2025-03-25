using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Users.RegisterUser;

namespace WorkHive.APIs.Users.RegisterUser;

public record RegisterUserRequest(string Name, string Email,
    string Phone, string Password, string Sex);
public record RegisterUserResponse(string Token, string Notification);

public class RegisterUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/register", async (RegisterUserRequest request, ISender sender) => {
            var command = request.Adapt<RegisterUserCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<RegisterUserResponse>();

            return Results.Created($"/users/register", new { Token = response.Token, Notification = response.Notification });
        })
        .WithName("Register User")
        .Produces<RegisterUserResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Register successfully")
        .WithTags("Register User")
        .WithDescription("Register successfully");
    }
}
