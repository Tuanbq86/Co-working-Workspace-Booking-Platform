using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Google_Login;

namespace WorkHive.APIs.Google
{
    public record UpdateUserPhoneRequest(string Phone);
    public record UpdateUserPhoneResponse(string Notification);
    public class UpdateUserPhoneEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/users/{id}/phone", async (int id, UpdateUserPhoneRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateUserPhoneCommand>() with { UserId = id };
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateUserPhoneResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateUserPhone")
            .Produces<UpdateUserPhoneResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("User")
            .WithSummary("Update user's phone")
            .WithDescription("Updates a user's phone number by their ID.");
        }
    }
}
