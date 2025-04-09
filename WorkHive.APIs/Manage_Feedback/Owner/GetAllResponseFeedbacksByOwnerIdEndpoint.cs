using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response;

namespace WorkHive.APIs.Manage_Feedback.Owner
{
    public record GetAllResponseFeedbacksByOwnerIdResponse(
        int Id, string Title, string Description, string Status, int UserId, int OwnerId, int? FeedbackId,
        DateTime? CreatedAt, List<string> ImageUrls
    );

    public class GetAllResponseFeedbacksByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-response-feedbacks/owners/{ownerId}", async (ISender sender, int ownerId) =>
            {
                var query = new GetAllResponseFeedbacksByOwnerIdQuery(ownerId);
                var result = await sender.Send(query);

                //return result == null || !result.Any()
                //    ? Results.NotFound($"No owner response feedbacks found for owner with ID {ownerId}")
                //    : Results.Ok(result.Select(r => new GetAllResponseFeedbacksByOwnerIdResponse(
                //        r.Id,
                //        r.Title,
                //        r.Description,
                //        r.Status,
                //        r.UserId,
                //        r.OwnerId,
                //        r.FeedbackId,
                //        r.CreatedAt,
                //        r.ImageUrls
                //    )));
                return Results.Ok(
                result?.Select(r => new GetAllResponseFeedbacksByOwnerIdResponse(
                    r.Id,
                    r.Title,
                    r.Description,
                    r.Status,
                    r.UserId,
                    r.OwnerId,
                    r.FeedbackId,
                    r.CreatedAt,
                    r.ImageUrls
                )) ?? new List<GetAllResponseFeedbacksByOwnerIdResponse>()
            );

            })
            .WithName("GetAllResponseFeedbacksByOwnerId")
            //.Produces<List<GetAllResponseFeedbacksByOwnerIdResponse>>(StatusCodes.Status200OK)
            .Produces<List<GetAllResponseFeedbacksByOwnerIdResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner Response Feedback")
            .WithSummary("Get all owner response feedbacks by owner ID")
            .WithDescription("Retrieves all owner response feedbacks submitted by a specific owner.");
        }
    }
}
