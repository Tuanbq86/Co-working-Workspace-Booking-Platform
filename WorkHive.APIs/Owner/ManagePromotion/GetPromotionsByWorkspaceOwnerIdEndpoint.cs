using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion;

namespace WorkHive.APIs.Owner.ManagePromotion
{ 
    public record GetPromotionsByWorkspaceOwnerIdResponse(List<PromotionsByOwnerIdDT> Promotions);

    public class GetPromotionsByWorkspaceOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/workspace-owners/{workspaceOwnerId}/promotions", async (int workspaceOwnerId, ISender sender) =>
            {
                var query = new GetPromotionsByWorkspaceOwnerIdQuery(workspaceOwnerId);
                var result = await sender.Send(query);
                if (result == null || !result.Any())
                {
                    return Results.Json(new GetPromotionsByWorkspaceOwnerIdResponse(new List<PromotionsByOwnerIdDT>()));
                }
                var response = new GetPromotionsByWorkspaceOwnerIdResponse(result);
                return Results.Ok(response);
            })
            .WithName("GetPromotionsByWorkspaceOwnerId")
            .Produces<GetPromotionsByWorkspaceOwnerIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Promotion")
            .WithSummary("Get promotions by workspace owner ID")
            .WithDescription("Retrieve promotions that belong to a specific workspace owner.");
        }
    }
}
