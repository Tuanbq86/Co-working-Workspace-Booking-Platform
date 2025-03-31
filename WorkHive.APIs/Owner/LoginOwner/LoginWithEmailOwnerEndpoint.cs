using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Owners.LoginOwner;

namespace WorkHive.APIs.Owner.LoginOwner;
public record LoginWithOwnerEmailRequest(string Email);
public record LoginWithOwnerEmailResponse(string UserName);

public class LoginWithOwnerEmailEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/owners/checkemail", async (LoginWithOwnerEmailRequest request, ISender sender) =>
        {
            var command = request.Adapt<LoginWithOwnerEmailCommand>();

            var result = await sender.Send(command);

            var response = new LoginWithOwnerEmailResponse(result.OwnerName);

            return Results.Ok(response);
        })
        .WithName("checkOwnerEmail")
        .Produces<LoginWithOwnerEmailResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithTags("Owner")
        .WithSummary("Check Owner Email")
        .WithDescription("Check Owner Email");
    }
}
