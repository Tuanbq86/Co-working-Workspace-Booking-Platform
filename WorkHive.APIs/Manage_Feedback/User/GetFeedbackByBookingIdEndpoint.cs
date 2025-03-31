using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.User_Feedback;

namespace WorkHive.APIs.Manage_Feedback.User
{
    public class GetFeedbackByBookingIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/feedbacks/booking/{bookingId:int}", async (int bookingId, ISender sender) =>
            {
                var query = new GetFeedbackByBookingIdQuery(bookingId);
                var result = await sender.Send(query);

                return result == null
                    ? Results.NotFound($"No feedback found for BookingId {bookingId}")
                    : Results.Ok(result);
            })
            .WithName("GetFeedbackByBookingId")
            .Produces<GetFeedbackByBookingIdResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Feedback")
            .WithSummary("Get the first feedback by BookingId")
            .WithDescription("Retrieves the first feedback associated with a given BookingId.");
        }
    }

}
