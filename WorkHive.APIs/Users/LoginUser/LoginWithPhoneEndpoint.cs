using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Users.LoginUser;

namespace WorkHive.APIs.Users.LoginUser;

public record LoginWithPhoneRequest(string Phone);
public record LoginWithPhoneResponse(string UserName);

public class LoginWithPhoneEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/checkuserphone", async (LoginWithPhoneRequest request, ISender sender) =>
        {
            var command = request.Adapt<LoginWithPhoneCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<LoginWithPhoneResponse>();

            return Results.Ok(response.UserName);
        })
        .WithName("checkPhone")
        .Produces<LoginWithPhoneResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Check User Phone")
        .WithDescription("Check User Phone");
    }
}
