using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Users.BookingWorkspace.CancelBooking;

namespace WorkHive.APIs.Users.Booking.CancelBooking;

public record CancelBookingRequest(int BookingId);
public record CancelBookingResponse(string Notification, int IsLock);

public class CancelBookingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/cancelbooking", async(CancelBookingRequest request, ISender sender) =>
        {
            var command = request.Adapt<CancelBookingCommand>();

            var result = await sender.Send(command);

            var response = new CancelBookingResponse(result.Notificationn, result.IsLock);

            return Results.Ok(response);
        })
        .WithName("Cancel booking workspace")
        .Produces<CancelBookingResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Cancel booking workspace")
        .WithTags("Booking")
        .WithDescription("Cancel booking workspace");
    }
}
