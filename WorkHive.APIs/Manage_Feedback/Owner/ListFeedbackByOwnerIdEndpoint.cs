using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response;

namespace WorkHive.APIs.Manage_Feedback.Owner
{
    public record ListFeedbackByOwnerIdResponse(
       int Id, string Description, string Status, int UserId, int OwnerId,
       int? BookingId, int WorkspaceId, string WorkspaceName,
       DateTime? CreatedAt, List<string> ImageUrls
   );

    public class ListFeedbackByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-feedbacks/{ownerId:int}", async (int ownerId, ISender sender) =>
            {
                var query = new ListFeedbackByOwnerIdQuery(ownerId);
                var result = await sender.Send(query);

                return result == null || !result.Any()
                    ? Results.NotFound($"No feedbacks found for owner with ID {ownerId}")
                    : Results.Ok(result.Select(f => new ListFeedbackByOwnerIdResponse(
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
            .WithName("ListFeedbackByOwnerId")
            .Produces<List<ListFeedbackByOwnerIdResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Feedback")
            .WithSummary("Get all feedbacks by OwnerId")
            .WithDescription("Retrieves all feedbacks for a specific workspace owner.");
        }
    }
}