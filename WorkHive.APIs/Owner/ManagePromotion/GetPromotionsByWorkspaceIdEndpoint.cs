using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion;

namespace WorkHive.APIs.Owner.ManagePromotion
{
    public record GetPromotionsByWorkspaceIdResponse(List<PromotionDT> Promotions);

    public class GetPromotionsByWorkspaceIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/workspaces/{workspaceId}/promotions", async (int workspaceId, ISender sender) =>
            {
                var query = new GetPromotionsByWorkspaceIdQuery(workspaceId);
                var result = await sender.Send(query);
                if (result == null || !result.Any())
                {
                    return Results.Json(new GetPromotionsByWorkspaceIdResponse(new List<PromotionDT>()));
                }
                var response = new GetPromotionsByWorkspaceIdResponse(result);
                return Results.Ok(response);
            })
            .WithName("GetPromotionsByWorkspace")
            .Produces<GetPromotionsByWorkspaceIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Promotion")
            .WithSummary("Get promotions by workspace ID")
            .WithDescription("Retrieve promotions that belong to a specific workspace.");
        }
    }
}
