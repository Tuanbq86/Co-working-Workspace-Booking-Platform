using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.User_Feedback;

namespace WorkHive.APIs.Manage_Feedback.User
{
    public record ListResponseByUserIdResponse(
        int Id, string Title, string Description, string Status, int UserId, int OwnerId, int? FeedbackId,
        DateTime? CreatedAt, List<string> ImageUrls
    );

    public class ListResponseByUserIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-response-feedbacks/user/{userId}", async (ISender sender, int userId) =>
            {
                var query = new ListResponseByUserIdQuery(userId);
                var result = await sender.Send(query);

                return result == null || !result.Any()
                    ? Results.NotFound($"No owner response feedbacks found for user with ID {userId}")
                    : Results.Ok(result.Select(r => new ListResponseByUserIdResponse(
                        r.Id,
                        r.Title,
                        r.Description,
                        r.Status,
                        r.UserId,
                        r.OwnerId,
                        r.FeedbackId,
                        r.CreatedAt,
                        r.ImageUrls
                    )));
            })
            .WithName("ListResponseByUserId")
            .Produces<List<ListResponseByUserIdResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner Response Feedback")
            .WithSummary("Get all owner response feedbacks by user ID")
            .WithDescription("Retrieves all owner response feedbacks submitted for a specific user.");
        }
    }
}
