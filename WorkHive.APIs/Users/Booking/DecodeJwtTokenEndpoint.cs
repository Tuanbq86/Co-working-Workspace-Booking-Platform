using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Users.BookingWorkspace;

namespace WorkHive.APIs.Users.Booking;

public record DecodeJwtRequest(string Token);
public record DecodeJwtResponse(Dictionary<string, string> Claims, string AvatarUrl);

public class DecodeJwtTokenEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/decodejwttoken", async (DecodeJwtRequest request, ISender sender) =>
        {
            var command = request.Adapt<DecodeJwtCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<DecodeJwtResponse>();

            return Results.Ok(response);
        })
        .WithName("DecodeJwtToken")
        .Produces<DecodeJwtResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Decode Jwt Token")
        .WithTags("DecodeJWTtoken")
        .WithDescription("Decode Jwt Token");
    }
}
