using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion;

namespace WorkHive.APIs.Owner.ManagePromotion
{
    public record UpdatePromotionRequest(string Code, decimal Discount, DateTime StartDate, DateTime EndDate, string Status, string Description);
    public record UpdatePromotionResponse(string Notification);

    public class UpdatePromotionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/promotions/{id}", async (int id, UpdatePromotionRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdatePromotionCommand>() with { Id = id };
                var result = await sender.Send(command);
                var response = result.Adapt<UpdatePromotionResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdatePromotion")
            .Produces<UpdatePromotionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Promotion")
            .WithSummary("Update an existing promotion")
            .WithDescription("Updates a promotion by its ID with the provided details.");
        }
    }
}