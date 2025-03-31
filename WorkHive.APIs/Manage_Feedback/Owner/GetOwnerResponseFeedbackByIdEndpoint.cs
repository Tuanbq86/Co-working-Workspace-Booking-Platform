using Carter;
using MediatR;
using WorkHive.APIs.Manage_Feedback.User;
using WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response;

namespace WorkHive.APIs.Manage_Feedback.Owner
{
    public record GetOwnerResponseFeedbackByIdResponse(int Id, string Title, string Description, string Status, int UserId, int OwnerId, int? FeedbackId, DateTime? CreatedAt, List<string> ImageUrls);
    public class GetOwnerResponseFeedbackByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-response-feedbacks/{id}", async (int id, ISender sender) =>
            {
                var query = new GetOwnerResponseFeedbackByIdQuery(id);
                var result = await sender.Send(query);

                return result == null
                    ? Results.NotFound("Feedback not found")
                    : Results.Ok(new GetOwnerResponseFeedbackByIdResponse(
                        result.Id,
                        result.Title,
                        result.Description,
                        result.Status,
                        result.UserId,
                        result.OwnerId,
                        result.FeedbackId,
                        result.CreatedAt,
                        result.ImageUrls));
            })
            .WithName("GetResponseFeedbackById")
            .Produces<GetOwnerResponseFeedbackByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner Response Feedback")
            .WithSummary("Get response feedback by ID")
            .WithDescription("Retrieves response feedback using its unique identifier.");
        }
    }
}