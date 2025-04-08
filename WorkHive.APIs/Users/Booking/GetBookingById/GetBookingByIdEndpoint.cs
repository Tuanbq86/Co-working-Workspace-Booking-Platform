using Carter;
using MediatR;
using WorkHive.APIs.Users.Booking.GetAllBookingByOwnerId;
using WorkHive.Services.DTOService;
using WorkHive.Services.Users.BookingWorkspace.GetBookingById;

namespace WorkHive.APIs.Users.Booking.GetBookingById;

public record GetBookingByBookingIdResponse(BookingByBookingIdDTO BookingByBookingIdDTO);

public class GetBookingByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getbookingbyid/{BookingId}", async (int BookingId, ISender sender) =>
        {
            var result = await sender.Send(new GetBookingByBookingIdQuery(BookingId));

            var response = new GetBookingByBookingIdResponse(result.BookingByBookingIdDTO);

            return Results.Ok(response);
        })
        .WithName("Get booking by Id")
        .Produces<GetBookingByBookingIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get booking by Id")
        .WithTags("Booking")
        .WithDescription("Get booking by Id");
    }
}
