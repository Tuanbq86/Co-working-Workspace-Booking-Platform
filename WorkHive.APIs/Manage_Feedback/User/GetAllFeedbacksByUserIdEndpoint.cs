using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.User_Feedback;

namespace WorkHive.APIs.Manage_Feedback.User
{
    public record GetAllFeedbacksByUserIdResponse(
         int Id, string Description, string Status, int UserId, int OwnerId, int? BookingId,
         int WorkspaceId, string WorkspaceName, DateTime? CreatedAt, List<string> ImageUrls
     );

    public class GetAllFeedbacksByUserIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/feedbacks/user/{userId}", async (ISender sender, int userId) =>
            {
                var query = new GetAllFeedbacksByUserIdQuery(userId);
                var result = await sender.Send(query);

                return result == null || !result.Any()
                    ? Results.NotFound($"No feedbacks found for user with ID {userId}")
                    : Results.Ok(result.Select(f => new GetAllFeedbacksByUserIdResponse(
                        f.Id,
                        f.Description,
                        f.Status,
                        f.UserId,
                        f.OwnerId,
                        f.BookingId,
                        f.WorkspaceId,
                        f.WorkspaceName,
                        f.CreatedAt,
                        f.ImageUrls
                    )));
            })
            .WithName("GetAllFeedbacksByUserId")
            .Produces<List<GetAllFeedbacksByUserIdResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Feedback")
            .WithSummary("Get all feedbacks by user ID")
            .WithDescription("Retrieves all feedbacks submitted by a specific user.");
        }
    }
}
