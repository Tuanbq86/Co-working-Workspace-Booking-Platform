using Carter;
using Mapster;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Users.ResetPasswordForUser;

namespace WorkHive.APIs.Users.ResetPasswordForUser;

public record ResetPasswordRequest(string Token, string NewPassword, string ConfirmPassword);
public record ResetPasswordResponse(string Notification);

public class ResetPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("users/resetpassword", async (ResetPasswordRequest request, ISender sender) =>
        {
            var command = request.Adapt<ResetPasswordCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<ResetPasswordResponse>();

            return Results.Ok(response);
        })
        .WithName("Reset password for user")
        .Produces<ResetPasswordResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Reset password for user")
        .WithTags("Forgot password for user")
        .WithDescription("Reset password for user");
    }
}
