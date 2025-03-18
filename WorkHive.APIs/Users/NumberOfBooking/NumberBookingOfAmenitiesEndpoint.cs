using Carter;
using MediatR;
using WorkHive.APIs.Users.SearchWorkspace;
using WorkHive.Services.DTOService;
using WorkHive.Services.Users.NumberOfBooking;

namespace WorkHive.APIs.Users.NumberOfBooking;

public record NumberBookingOfAmenitiesResponse(List<NumberOfBookingAmenitiesDTO> NumberOfBookingAmenitiesDTOs);

public class NumberBookingOfAmenitiesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/owners/bookings/amenities/{OwnerId}", async(int OwnerId, ISender sender) =>
        {
            var result = await sender.Send(new NumberBookingOfAmenitiesQuery(OwnerId));

            var response = new NumberBookingOfAmenitiesResponse(result.NumberOfBookingAmenitiesDTOs);

            return Results.Ok(response);
        })
        .WithName("Number Of amenity booking")
        .Produces<NumberBookingOfAmenitiesResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Number Of amenity booking")
        .WithTags("Owner Dashboard")
        .WithDescription("Number Of amenity booking");
    }
}
