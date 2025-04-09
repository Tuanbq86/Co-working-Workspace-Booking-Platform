using Carter;
using Mapster;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Users.BookingWorkspace.BookingForMobile;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.APIs.Users.Booking.BookingForMobile;

public record BookingForMobileRequest(int UserId, int WorkspaceId, string StartDate, string EndDate,
    List<BookingAmenityDTO> Amenities, List<BookingBeverageDTO> Beverages, string PromotionCode, decimal Price, string WorkspaceTimeCategory);
public record BookingForMobileResponse(int BookingId, string Bin, string AccountNumber, int Amount, string Description,
    long OrderCode, string PaymentLinkId, string Status, string CheckoutUrl, string QRCode);

public class BookingForMobileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/bookingformobile", async (BookingForMobileRequest request, ISender sender) =>
        {
            var command = request.Adapt<BookingForMobileCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<BookingForMobileResponse>();

            return Results.Ok(response);
        })
        .WithName("Booking workspace for mobile")
        .Produces<BookingForMobileResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Booking workspace for mobile")
        .WithTags("Booking")
        .WithDescription("Booking workspace for mobile");
    }
}
