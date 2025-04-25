using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Beverage;

namespace WorkHive.APIs.Owner.ManageBeverage
{
    public record DeleteBeverageResponse(string Notification);

    public class DeleteBeverageEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/beverages/{id}", async (int id, ISender sender) =>
            {
                var command = new DeleteBeverageCommand(id);
                var result = await sender.Send(command);
                var response = result.Adapt<DeleteBeverageResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteBeverage")
            .Produces<DeleteBeverageResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Beverage")
            .WithSummary("Delete a beverage")
            .RequireAuthorization("Owner")
            .WithDescription("Deletes a beverage by its ID.");
        }
    }
}