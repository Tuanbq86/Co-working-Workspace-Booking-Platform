using Carter;
using Mapster;
using MediatR;
using WorkHive.Data.Models;
using WorkHive.Services.Users.BookingWorkspace.BookingByUserWallet;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.APIs.Users.Booking.BookingByUserWallet;

public record BookingByUserWalletRequest(int UserId, int WorkspaceId, string StartDate, string EndDate,
    List<BookingAmenityDTO> Amenities, List<BookingBeverageDTO> Beverages, string PromotionCode, decimal Price, string WorkspaceTimeCategory);
public record BookingByUserWalletResponse(string Notification);

public class BookingByUserWalletEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/bookingbyworkhivewallet", async(BookingByUserWalletRequest request, ISender sender) =>
        {
            var command = request.Adapt<BookingByUserWalletCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<BookingByUserWalletResponse>();

            return Results.Ok(response);
        })
        .WithName("Booking workspace by workhive wallet")
        .Produces<BookingByUserWalletResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Booking workspace by workhive wallet")
        .WithTags("Booking")
        //.RequireAuthorization("Customer")
        .WithDescription("Booking workspace by workhive wallet");
    }
}
