using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Users.RegisterUser;

namespace WorkHive.APIs.Users.RegisterUser;

public record CreateUserRequest(string Name, string Email,
    string Phone, string Password, string Sex, int RoleId);

public record CreateUserResponse(string Token, string Notification);

public class CreateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/create", async (CreateUserRequest request, ISender sender) => {
            var command = request.Adapt<CreateUserCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateUserResponse>();

            return Results.Created($"/users/create", new { Token = response.Token, Notification = response.Notification });
        })
        .WithName("Create User")
        .Produces<CreateUserResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create User")
        .WithTags("Register User")
        //.RequireAuthorization("Admin")
        .WithDescription("Create User");
    }
}
//Admin