using Carter;
using Mapster;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Users.UserRating;

namespace WorkHive.APIs.Users.UserRating;

public record UpdateRatingRequest(int UserId, int RatingId, Byte Rate,
    string Comment, List<UpdateRatingImage> Images);
public record UpdateRatingResponse(string Notification);
public record UpdateRatingImage(string Url);

public class UpdateRatingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("users/updaterating", async (UpdateRatingRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateRatingCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateRatingResponse>();

            return Results.Ok(response);
        })
        .WithName("Update Rating")
        .Produces<UpdateRatingResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Update Rating")
        .WithTags("Rating")
        //.RequireAuthorization("Customer")
        .WithDescription("Update Rating");
    }
}
//Customer