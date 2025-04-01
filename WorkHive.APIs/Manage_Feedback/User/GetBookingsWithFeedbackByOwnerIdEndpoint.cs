using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.User_Feedback;

namespace WorkHive.APIs.Manage_Feedback.User
{
    public class GetBookingsWithFeedbackByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/bookings/feedback/owner/{ownerId:int}", async (int ownerId, ISender sender) =>
            {
                var query = new GetBookingsWithFeedbackByOwnerIdQuery(ownerId);
                var result = await sender.Send(query);

                return result == null || !result.Any()
                    ? Results.NotFound($"No bookings with feedback found for OwnerId {ownerId}")
                    : Results.Ok(result);
            })
            .WithName("GetBookingsWithFeedbackByOwnerId")
            .Produces<List<GetBookingsWithFeedbackByOwnerIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Feedback")
            .WithSummary("Get all bookings with feedback by OwnerId")
            .WithDescription("Retrieves all bookings where is_feedback == 1 for a given OwnerId.");
        }
    }
}