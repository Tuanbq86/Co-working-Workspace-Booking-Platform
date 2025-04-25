using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion;

namespace WorkHive.APIs.Owner.ManagePromotion
{
    public record DeletePromotionResponse(string Notification);

    public class DeletePromotionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/promotions/{id}", async (int id, ISender sender) =>
            {
                var command = new DeletePromotionCommand(id);
                var result = await sender.Send(command);
                var response = result.Adapt<DeletePromotionResponse>();
                return Results.Ok(response);
            })
            .WithName("DeletePromotion")
            .Produces<DeletePromotionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Promotion")
            .WithSummary("Delete a promotion")
            .RequireAuthorization("Owner")
            .WithDescription("Deletes a promotion by its ID.");
        }
    }
}