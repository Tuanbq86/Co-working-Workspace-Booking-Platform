using Carter;
using WorkHive.APIs.Users.RegisterUser;
using Mapster;
using WorkHive.Services.Users.UpdateUser;
using MediatR;

namespace WorkHive.APIs.Users.UpdateUser;

public record UpdateUserRequest(int UserId, string Name, string Email, string Location, string Phone,
    DateOnly? DateOfBirth, string Sex, string Avatar);
public record UpdateUserResponse(string Notification);

public class UpdateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/update", async (UpdateUserRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateUserCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateUserResponse>();

            return Results.Ok(response);
        })
        .WithName("Update User")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("UpdateUser")
        .WithDescription("UpdateUser");
    }
}
