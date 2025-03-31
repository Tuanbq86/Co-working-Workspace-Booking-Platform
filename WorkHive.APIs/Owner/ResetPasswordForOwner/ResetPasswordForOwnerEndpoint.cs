using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ResetPasswordForOwner;

namespace WorkHive.APIs.Owner.ResetPasswordForOwner;

public record ResetPasswordForOwnerRequest(string Token, string NewPassword, string ConfirmPassword);
public record ResetPasswordForOwnerResponse(string Notification);

public class ResetPasswordForOwnerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("owners/resetpassword", async (ResetPasswordForOwnerRequest request, ISender sender) =>
        {
            var command = request.Adapt<ResetPasswordForOwnerCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<ResetPasswordForOwnerResponse>();

            return Results.Ok(response);
        })
        .WithName("Reset password for owner")
        .Produces<ResetPasswordForOwnerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Reset password for owner")
        .WithTags("Forgot password for owner")
        .WithDescription("Reset password for owner");
    }
}
