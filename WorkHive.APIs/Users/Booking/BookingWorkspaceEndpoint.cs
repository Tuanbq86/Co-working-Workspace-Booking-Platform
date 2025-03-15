using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Data.Models;
using WorkHive.Services.Users.BookingWorkspace;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.APIs.Users.Booking;

public record BookingWorkspaceRequest(int UserId, int WorkspaceId, string StartDate, string EndDate,
    List<BookingAmenityDTO> Amenities, List<BookingBeverageDTO> Beverages, string PromotionCode, decimal Price, string Category);

public record BookingWorkspaceResponse(int BookingId, string Bin, string AccountNumber, int Amount, string Description,
    long OrderCode, string PaymentLinkId, string Status, string CheckoutUrl, string QRCode);

public class BookingWorkspaceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/booking", async (BookingWorkspaceRequest request, ISender sender) =>
        {
            var command = request.Adapt<BookingWorkspaceCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<BookingWorkspaceResponse>();

            return Results.Ok(response);
        })
        .WithName("Booking workspace")
        .Produces<BookingWorkspaceResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Booking workspace")
        .WithTags("Booking")
        .WithDescription("Booking workspace");
    }
}
