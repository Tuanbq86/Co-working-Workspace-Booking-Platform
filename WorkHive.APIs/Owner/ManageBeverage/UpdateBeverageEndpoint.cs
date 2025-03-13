using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Beverage;

namespace WorkHive.APIs.Owner.ManageBeverage
{
    public record UpdateBeverageRequest(string Name, decimal Price, string ImgUrl, string Description, string Category, string Status);
    public record UpdateBeverageResponse(string Notification);

    public class UpdateBeverageEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/beverages/{id}", async (int id, UpdateBeverageRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateBeverageCommand>() with { Id = id };
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateBeverageResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateBeverage")
            .Produces<UpdateBeverageResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Beverage")
            .WithSummary("Update an existing beverage")
            .WithDescription("Updates a beverage by its ID with the provided details.");
        }
    }
}
