using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.Booking;
using WorkHive.Services.Owners.LoginOwner.Decode_JWT;

namespace WorkHive.APIs.Owner.LoginOwner.Decode_JWT
{
    public record DecodeJwtOwnerRequest(string Token);
    public record DecodeJwtOnwerResponse(Dictionary<string, string> Claims);

    public class DecodeJwtTokenOwnerEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/owners/decodejwttoken", async (DecodeJwtOwnerRequest request, ISender sender) =>
            {
                var command = request.Adapt<DecodeJwtOwnerCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<DecodeJwtOnwerResponse>();

                return Results.Ok(response);
            })
            .WithName("DecodeJwtToken Owner")
            .Produces<DecodeJwtOnwerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Decode Jwt Token Owner")
            .WithTags("DecodeJWTtoken Owner")
            .WithDescription("Decode Jwt Token Owner");
        }
    }
}
