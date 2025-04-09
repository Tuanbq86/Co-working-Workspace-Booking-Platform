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

                // Trả về [] nếu null hoặc không có kết quả
                var safeResult = result ?? new List<GetBookingsWithFeedbackByOwnerIdResult>();

                return Results.Ok(safeResult);
            })
            .WithName("GetBookingsWithFeedbackByOwnerId")
            .Produces<List<GetBookingsWithFeedbackByOwnerIdResult>>(StatusCodes.Status200OK)
            .WithTags("Feedback")
            .WithSummary("Get all bookings with feedback by OwnerId")
            .WithDescription("Retrieves all bookings where is_feedback == 1 for a given OwnerId.");
        }
    }
}