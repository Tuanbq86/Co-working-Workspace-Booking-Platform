using Carter;
using MediatR;
using WorkHive.Services.DTOService;
using WorkHive.Services.Users.UserRating;

namespace WorkHive.APIs.Users.UserRating;

public record GetAllRatingByWorkspaceIdResponse(List<RatingByWorkspaceIdDTO> RatingByWorkspaceIdDTOs);

public class GetAllRatingByWorkspaceIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("users/rating/getallratingbyworkspaceid/{WorkspaceId}", async (int WorkspaceId, ISender sender) =>
        {
            var result = await sender.Send(new GetAllRatingByWorkspaceIdQuery(WorkspaceId));

            var response = new GetAllRatingByWorkspaceIdResponse(result.RatingByWorkspaceIdDTOs);

            return Results.Ok(response);
        })
        .WithName("Get all rating by workspace Id")
        .Produces<GetAllRatingByWorkspaceIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get all rating by workspace Id")
        .WithTags("Rating")
        .WithDescription("Get all rating by workspace Id");
    }
}
