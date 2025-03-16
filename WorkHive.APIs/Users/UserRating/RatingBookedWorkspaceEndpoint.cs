using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Users.UserRating;

namespace WorkHive.APIs.Users.UserRating;

public record RatingBookedWorkspaceRequest
    (int BookingId, Byte Rate, string Comment, List<RatingImage> Images);
public record RatingBookedWorkspaceResponse(string Notification, int BookingIsReview);
public record RatingImage(string Url);

public class RatingBookedWorkspaceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("users/booking/rating", async(RatingBookedWorkspaceRequest request, ISender sender) =>
        {
            var command = request.Adapt<RatingBookedWorkspaceCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<RatingBookedWorkspaceResponse>();

            return Results.Ok(response);
        })
        .WithName("Rating booking workspace")
        .Produces<RatingBookedWorkspaceResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Rating booking workspace")
        .WithTags("Rating")
        .WithDescription("Rating booking workspace");
    }
}
