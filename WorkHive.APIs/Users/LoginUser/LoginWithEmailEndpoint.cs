using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Users.LoginUser;

namespace WorkHive.APIs.Users.LoginUser;

public record LoginWithEmailRequest(string Email);
public record LoginWithEmailResponse(string UserName);

public class LoginWithEmailEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/checkuseremail", async (LoginWithEmailRequest request, ISender sender) =>
        {
            var command = request.Adapt<LoginWithEmailCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<LoginWithEmailResponse>();

            return Results.Ok(response.UserName);
        })
        .WithName("checkEmail")
        .Produces<LoginWithEmailResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Check User Email")
        .WithTags("User Login")
        .WithDescription("Check User Email");
    }
}
