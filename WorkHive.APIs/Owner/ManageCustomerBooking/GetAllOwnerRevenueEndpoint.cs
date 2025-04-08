using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageCustomerBooking;

namespace WorkHive.APIs.Owner.ManageCustomerBooking
{
    public class GetAllOwnerRevenueEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owners/revenue", async (ISender sender) =>
            {
                var result = await sender.Send(new GetAllOwnerRevenueQuery());
                return Results.Ok(result);
            })
            .WithName("GetAllOwnerRevenue")
            .Produces<List<GetAllOwnerRevenueResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags("Owner")
            .WithSummary("Get revenue of all owners")
            .WithDescription("Retrieve total revenue for each owner based on workspace bookings.");
        }
    }
}
