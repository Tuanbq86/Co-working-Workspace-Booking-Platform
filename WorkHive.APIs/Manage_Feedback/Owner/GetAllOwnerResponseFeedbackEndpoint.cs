using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response;

namespace WorkHive.APIs.Manage_Feedback.Owner
{
    public record GetAllOwnerResponseFeedbackResponse(int Id, string Description, string Status, int UserId, int OwnerId, int? FeedbackId, DateTime? CreatedAt, List<string> ImageUrls);

    public class GetAllOwnerResponseFeedbackEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-response-feedbacks", async (ISender sender) =>
            {
                var query = new GetAllOwnerResponseFeedbackQuery();
                var result = await sender.Send(query);

                return result == null || !result.Any()
                    ? Results.NotFound("No owner response feedbacks found")
                    : Results.Ok(result.Select(r => new GetAllOwnerResponseFeedbackResponse(
                        r.Id,
                        r.Description,
                        r.Status,
                        r.UserId,
                        r.OwnerId,
                        r.FeedbackId,
                        r.CreatedAt,
                        r.ImageUrls
                    )));
            })
            .WithName("GetAllOwnerResponseFeedbacks")
            .Produces<List<GetAllOwnerResponseFeedbackResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner Response Feedback")
            .WithSummary("Get all owner response feedbacks")
            .WithDescription("Retrieves all owner response feedbacks available in the system.");
        }
    }
}
