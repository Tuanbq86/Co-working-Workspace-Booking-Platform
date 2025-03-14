using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion;

namespace WorkHive.APIs.Owner.ManagePromotion
{
    public class GetPromotionByIdEndpoint : ICarterModule
    {
        public record GetPromotionByIdResponse(int Id, string Code, int? Discount, DateTime? StartDate, DateTime? EndDate, DateTime? CreatedAt, DateTime? UpdatedAt, string Status, int WorkspaceId);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/promotions/{id}", async (int id, ISender sender) =>
            {
                var query = new GetPromotionByIdQuery(id);
                var result = await sender.Send(query);

                if (result == null)
                {
                    return Results.Json(Array.Empty<GetPromotionByIdResponse>());
                }

                var response = result.Adapt<GetPromotionByIdResponse>();
                return Results.Ok(response);
            })
            .WithName("GetPromotionById")
            .Produces<GetPromotionByIdResponse>(StatusCodes.Status200OK)
            .Produces<IEnumerable<GetPromotionByIdResponse>>(StatusCodes.Status200OK)
            .WithTags("Promotion")
            .WithSummary("Get Promotion by ID")
            .WithDescription("Retrieve a Promotion using its ID.");
        }
    }
}
