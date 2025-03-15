using Carter;
using MediatR;
using WorkHive.Services.DTOService;
using WorkHive.Services.Users.UserRating;

namespace WorkHive.APIs.Users.UserRating;

public record GetAllRatingByUserIdResponse(List<RatingByUserIdDTO> RatingByUserIdDTOs);

public class GetAllRatingByUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("users/rating/getallratingbyuserid/{UserId}", async(int UserId, ISender sender) =>
        {
            var result = await sender.Send(new GetAllRatingByUserIdQuery(UserId));

            var response = new GetAllRatingByUserIdResponse(result.RatingByUserIdDTOs);

            return Results.Ok(response);
        })
        .WithName("Get All rating by userId")
        .Produces<GetAllRatingByUserIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get All rating by userId")
        .WithTags("Rating")
        .WithDescription("Get All rating by userId");
    }
}
