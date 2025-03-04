using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.GetById;

namespace WorkHive.APIs.Owner.ManageWorkSpace.Beverage
{
    public class GetBeverageByIdEndpoint : ICarterModule
    {
        public record GetBeverageByIdResponse(int Id, string Name, decimal? Price, string ImgUrl, string Description, string Category, string Status, int WorkspaceId);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/beverages/{id}", async (int id, ISender sender) =>
            {
                var query = new GetBeverageByIdQuery(id);
                var result = await sender.Send(query);
                var response = result.Adapt<GetBeverageByIdResponse>();

                return Results.Ok(response);
            })
            .WithName("GetBeverageById")
            .Produces<GetBeverageByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Beverage by ID")
            .WithDescription("Retrieve a Beverage using its ID.");
        }
    }
}
