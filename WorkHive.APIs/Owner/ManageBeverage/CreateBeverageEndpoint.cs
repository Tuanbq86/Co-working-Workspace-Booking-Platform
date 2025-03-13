using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Beverage;

namespace WorkHive.APIs.Owner.ManageBeverage
{
    public record CreateBeverageRequest(string Name, decimal Price, string ImgUrl, string Description, string Category, string Status, int OwnerId);
    public record CreateBeverageResponse(string Notification);

    public class CreateBeverageEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/beverages", async (CreateBeverageRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateBeverageCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateBeverageResponse>();
                return Results.Created($"/beverages", response);
            })
            .WithName("CreateBeverage")
            .Produces<CreateBeverageResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Beverage")
            .WithSummary("Create a new beverage")
            .WithDescription("Creates a new beverage with the provided details.");
        }
    }
}