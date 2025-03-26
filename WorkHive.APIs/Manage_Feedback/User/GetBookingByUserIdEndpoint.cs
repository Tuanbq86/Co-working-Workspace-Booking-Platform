using Carter;
using MediatR;
using WorkHive.Services.Manage_Feedback.User_Feedback;

namespace WorkHive.APIs.Manage_Feedback.User
{
    public record GetBookingByUserIdResponse(List<BookingDTO> Bookings);

    public class GetBookingByUserIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/users/{userId:int}/bookings", async (int userId, ISender sender) =>
            {
                var query = new GetBookingByUserIdQuery(userId);
                var result = await sender.Send(query);
                return Results.Ok(new GetBookingByUserIdResponse(result));
            })
            .WithName("GetBookingByUserId")
            .Produces<GetBookingByUserIdResponse>(StatusCodes.Status200OK)
            .WithTags("Feedback")
            .WithSummary("Get bookings by user ID")
            .WithDescription("Retrieve all bookings associated with a specific user ID.");
        }
    }
}