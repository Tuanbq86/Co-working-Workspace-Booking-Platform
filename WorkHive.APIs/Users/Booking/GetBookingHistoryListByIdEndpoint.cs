using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Users.DTOs;
using WorkHive.Services.Users.BookingWorkspace;

namespace WorkHive.APIs.Users.Booking;

public record GetBookingHistoryListByIdRequest(int UserId);
public record GetBookingHistoryListByIdResponse(List<BookingHistory> BookingHistories);

public class GetBookingHistoryListByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("users/booking/historybookings", async([AsParameters] GetBookingHistoryListByIdRequest request, ISender sender) =>
        {
            var query = request.Adapt<GetBookingHistoryListByIdQuery>();

            var result = await sender.Send(query);

            var response = result.Adapt<GetBookingHistoryListByIdResponse>();

            return Results.Ok(response);
        })
        .WithName("HistoryBookingList")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("History Booking List")
        .WithTags("Booking")
        .WithDescription("History Booking List");
    }
}
