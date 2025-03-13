using Carter;
using MediatR;
using WorkHive.Data.Models;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Beverage;

namespace WorkHive.APIs.Owner.ManageBeverage
{
    public record GetBeveragesResponse(List<Beverage> Beverages);

    public class GetBeveragesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/beverages", async (ISender sender) =>
            {
                var query = new GetAllBeveragesQuery();
                var result = await sender.Send(query);
                if (result == null || !result.Any())
                {
                    return Results.Json(new GetBeveragesResponse(new List<Beverage>()));
                }
                var response = new GetBeveragesResponse(result);
                return Results.Ok(response);
            })
            .WithName("GetBeverages")
            .Produces<GetBeveragesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Beverage")
            .WithSummary("Get all beverages")
            .WithDescription("Retrieve all beverages.");
        }
    }
}

