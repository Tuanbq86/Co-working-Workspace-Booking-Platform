using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.DTOService;
using WorkHive.Services.Users.BookingWorkspace.GetAllBookingByOwnerId;

namespace WorkHive.APIs.Users.Booking.GetAllBookingByOwnerId;

public record GetAllBookingByOwnerIdResponse(List<BookingByOwnerIdDTO> BookingByOwnerIdDTOs);

public class GetAllBookingByOwnerIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getallbookingbyownerid/{OwnerId}", async(int OwnerId, ISender sender) =>
        {
            var result = await sender.Send(new GetAllBookingByOwnerIdQuery(OwnerId));

            var response = new GetAllBookingByOwnerIdResponse(result.BookingByOwnerIdDTOs);

            return Results.Ok(response);
        })
        .WithName("Get all booking by ownerId")
        .Produces<GetAllBookingByOwnerIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get all booking by ownerId")
        .WithTags("Get all booking by ownerId")
        .WithDescription("Get all booking by ownerId");
    }
}
