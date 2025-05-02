using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Promotion;

namespace WorkHive.APIs.Owner.ManagePromotion
{
    public record CreatePromotionRequest(string Code, int Discount, DateTime StartDate, DateTime EndDate, string Status, int WorkspaceId, string Description);
    public record CreatePromotionResponse(string Notification);

    public class CreatePromotionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/promotions", async (CreatePromotionRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreatePromotionCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreatePromotionResponse>();
                return Results.Created($"/promotions", response);
            })
            .WithName("CreatePromotion")
            .Produces<CreatePromotionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Promotion")
            .WithSummary("Create a new promotion")
            .RequireAuthorization("Owner")
            .WithDescription("Creates a new promotion with the provided details.");
        }
    }
}
//Owner