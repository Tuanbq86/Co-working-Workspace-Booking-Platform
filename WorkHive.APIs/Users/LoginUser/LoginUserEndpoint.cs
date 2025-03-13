using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Users.LoginUser;

namespace WorkHive.APIs.Users.LoginUser;

public record LoginUserRequest(string Auth, string Password);
public record LoginUserResponse(string Token, string Notification);

public class LoginUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/login", async (LoginUserRequest request, ISender sender) =>
        {
            var command = request.Adapt<LoginUserCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<LoginUserResponse>();

            return Results.Ok(response);
        })
        .WithName("userLogin")
        .Produces<LoginUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("User Login")
        .WithDescription("User Login");
    }
}
