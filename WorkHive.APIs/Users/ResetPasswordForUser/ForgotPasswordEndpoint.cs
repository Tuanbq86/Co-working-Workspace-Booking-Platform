using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.SearchWorkspace;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Users.ResetPasswordForUser;

namespace WorkHive.APIs.Users.ResetPasswordForUser;

public record ForgotPasswordRequest(string Email);
public record ForgotPasswordResponse(string Notification);

public class ForgotPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("users/forgotpassword", async(ForgotPasswordRequest request, ISender sender) =>
        {
            var command = request.Adapt<ForgotPasswordCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<ForgotPasswordResponse>();

            return Results.Ok(response);
        })
        .WithName("Forgot password for user")
        .Produces<ForgotPasswordResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Forgot password for user")
        .WithTags("Forgot password for user")
        .WithDescription("Forgot password for user");
    }
}
