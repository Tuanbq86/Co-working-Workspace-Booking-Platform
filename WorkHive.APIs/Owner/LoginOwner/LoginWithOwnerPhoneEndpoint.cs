using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Owners.LoginOwner;

namespace WorkHive.APIs.Owner.LoginOwner
{
    public record LoginWithOwnerPhoneRequest(string Phone);
    public record LoginWithOwnerPhoneResponse(string UserName);

    public class LoginWithOwnerPhoneEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/owners/checkphone", async (LoginWithOwnerPhoneRequest request, ISender sender) =>
            {
                var command = request.Adapt<LoginWithWorkspaceOwnerPhoneCommand>();

                var result = await sender.Send(command);

                var response = new LoginWithOwnerPhoneResponse(result.OwnerName);

                return Results.Ok(response);
            })
            .WithName("checkOwnerPhone")
            .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner")
            .WithSummary("Check Owner Phone")
            .WithDescription("Check Owner Phone");
        }
    }
}

