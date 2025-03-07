using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Users.UpdateUser;

namespace WorkHive.APIs.Users.UpdateUser;

public record UpdateUserPasswordRequest
    (int UserId, string OldPassword, string NewPassword, string ConfirmPassword);
public record UpdateUserPasswordResponse(string Notification);

public class UpdateUserPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/updatepassword", async (UpdateUserPasswordRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateUserPasswordCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateUserPasswordResponse>();

            return Results.Ok(response);
        })
        .WithName("Update User Password")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("UpdateUserPassword")
        .WithDescription("UpdateUserPassword");
    }
}
