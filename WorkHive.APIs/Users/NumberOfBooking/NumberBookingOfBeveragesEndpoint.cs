using Carter;
using MediatR;
using WorkHive.Services.DTOService;
using WorkHive.Services.Users.NumberOfBooking;

namespace WorkHive.APIs.Users.NumberOfBooking;

public record NumberBookingOfBeveragesResponse(List<NumberOfBookingBeveragesDTO> NumberOfBookingBeveragesDTOs);

public class NumberBookingOfBeveragesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/owners/bookings/beverages/{OwnerId}", async (int OwnerId, ISender sender) =>
        {
            var result = await sender.Send(new NumberBookingOfBeveragesQuery(OwnerId));

            var response = new NumberBookingOfBeveragesResponse(result.NumberOfBookingBeveragesDTOs);

            return Results.Ok(response);
        })
        .WithName("Number Of beverage booking")
        .Produces<NumberBookingOfBeveragesResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Number Of beverage booking")
        .WithTags("Owner Dashboard")
        .WithDescription("Number Of beverage booking");
    }
}
