using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.User_Feedback;

namespace WorkHive.APIs.Manage_Feedback.User
{
    public record GetAllFeedbackResponse(int Id, string Description, string Status, int UserId, int OwnerId, int? BookingId, int WorkspaceId, string WorkspaceName, DateTime? CreatedAt, List<string> ImageUrls);

    public class GetAllFeedbackEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/feedbacks", async (ISender sender) =>
            {
                var query = new GetAllFeedbackQuery();
                var result = await sender.Send(query);

                return result == null || !result.Any()
                    ? Results.NotFound("No feedbacks found")
                    : Results.Ok(result.Select(f => new GetAllFeedbackResponse(
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
            .WithName("GetAllFeedback")
            .Produces<List<GetAllFeedbackResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Feedback")
            .WithSummary("Get all feedbacks")
            .WithDescription("Retrieves all feedbacks available in the system.");
        }
    }
}
