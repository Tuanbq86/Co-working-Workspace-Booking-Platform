using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Admins;

namespace WorkHive.APIs.Users.Booking;

public record GetRevenueForAdminResponse(List<BookingInformationForAdmin> BookingInformation, decimal? TotalRevenue);

public class GetRevenueForAdminEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getrevenueforadmin/{AdminId}", async (int AdminId, ISender sender) =>
        {
            var result = await sender.Send(new GetRevenueForAdminQuery(AdminId));

            var response = result.Adapt<GetRevenueForAdminResponse>();

            return Results.Ok(response);
        })
        .WithName("Get revenue for admin")
        .Produces<GetRevenueForAdminResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get revenue for admin")
        .WithTags("Admin")
        .WithDescription("Get revenue for admin");
    }
}
