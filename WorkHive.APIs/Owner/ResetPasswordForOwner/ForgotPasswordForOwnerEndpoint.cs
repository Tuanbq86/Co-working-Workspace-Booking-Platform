using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ResetPasswordForOwner;
using WorkHive.Services.Users.ResetPasswordForUser;

namespace WorkHive.APIs.Owner.ResetPasswordForOwner;

public record ForgotPasswordForOwnerRequest(string Email);
public record ForgotPasswordForOwnerResponse(string Notification);

public class ForgotPasswordForOwnerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("owners/forgotpassword", async (ForgotPasswordForOwnerRequest request, ISender sender) =>
        {
            var command = request.Adapt<ForgotPasswordForOwnerCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<ForgotPasswordForOwnerResponse>();

            return Results.Ok(response);
        })
        .WithName("Forgot password for owner")
        .Produces<ForgotPasswordForOwnerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Forgot password for owner")
        .WithTags("Forgot password for owner")
        .WithDescription("Forgot password for owner");
    }
}