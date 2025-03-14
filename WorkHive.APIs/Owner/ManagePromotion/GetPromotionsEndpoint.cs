using Carter;
using MediatR;
using WorkHive.Data.Models;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion;

namespace WorkHive.APIs.Owner.ManagePromotion
{
    public record GetPromotionsResponse(List<PromotionDT> Promotions);

    public class GetPromotionsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/promotions", async (ISender sender) =>
            {
                var query = new GetAllPromotionsQuery();
                var result = await sender.Send(query);
                if (result == null || !result.Any())
                {
                    return Results.Json(new GetPromotionsResponse(new List<PromotionDT>()));
                }
                var response = new GetPromotionsResponse(result);
                return Results.Ok(response);
            })
            .WithName("GetPromotions")
            .Produces<GetPromotionsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Promotion")
            .WithSummary("Get all promotions")
            .WithDescription("Retrieve all promotions.");
        }
    }
}
