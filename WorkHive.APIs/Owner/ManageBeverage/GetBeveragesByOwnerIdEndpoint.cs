using Carter;
using MediatR;
using WorkHive.Services.Owmers.ManageBeverage.GetAllById;
using static WorkHive.APIs.Owner.ManageBeverage.GetBeverageByIdEndpoint;

namespace WorkHive.APIs.Owner.ManageBeverage
{
    public record GetBeveragesByOwnerIdResponse(List<BeverageDTO> Beverages);

    public class GetBeveragesByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/beverages/Owner/{OwnerId}", async (int OwnerId, ISender sender) =>
            {
                var query = new GetBeveragesByOwnerIdQuery(OwnerId);
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetBeveragesByOwnerIdResponse>());
                }
                var response = new GetBeveragesByOwnerIdResponse(result);

                return Results.Ok(response);
            })
            .WithName("GetBeveragesByOwnerId")
            .Produces<GetBeveragesByOwnerIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Beverage")
            .WithSummary("Get Beverages by Owner ID")
            .WithDescription("Retrieve all beverages belonging to a specific Owner.");
        }
    }
}
