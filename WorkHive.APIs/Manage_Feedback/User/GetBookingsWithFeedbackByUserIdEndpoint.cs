using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.User_Feedback;

namespace WorkHive.APIs.Manage_Feedback.User
{
    public class GetBookingsWithFeedbackByUserIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/user-feedback-bookings/{userId}", async (int userId, ISender sender) =>
            {
                var query = new GetBookingsWithFeedbackByUserIdQuery(userId);
                var result = await sender.Send(query);

                return result == null || !result.Any()
                    ? Results.NotFound($"No bookings with feedback found for userId {userId}")
                    : Results.Ok(result);
            })
            .WithName("GetBookingsWithFeedbackByUserId")
            .Produces<List<GetBookingsWithFeedbackByUserIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Feedback")
            .WithSummary("Get all bookings with feedback by UserId")
            .WithDescription("Retrieves all bookings that have received feedback, filtered by UserId.");
        }
    }

}
