using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Owner.RegisterOwner;
using WorkHive.Services.Owners.LoginOwner;

namespace WorkHive.APIs.Owner.LoginOwner
{

    public record LoginWorkspaceOwnerRequest(string Auth, string Password);
    public record LoginWorkspaceOwnerResponse(string Token, string Notification);

    public class LoginWorkspaceOwnerEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/owners/login", async (LoginWorkspaceOwnerRequest request, ISender sender) =>
            {
                var command = request.Adapt<LoginOwnerCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<LoginWorkspaceOwnerResponse>();

                return Results.Ok(response);
            })
            .WithName("OwnerLogin")
            .Produces<LoginWorkspaceOwnerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner")
            .WithSummary("Owner Login")
            .WithDescription("Owner Login");
        }
    }

}
