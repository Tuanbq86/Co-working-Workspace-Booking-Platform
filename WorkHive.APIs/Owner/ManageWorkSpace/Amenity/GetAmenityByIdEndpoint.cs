using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.GetById;
using static WorkHive.APIs.Owner.ManageWorkSpace.Beverage.GetBeverageByIdEndpoint;

namespace WorkHive.APIs.Owner.ManageWorkSpace.Amenity
{
    public record GetAmenityByIdResponse(int Id, string Name, decimal? Price, int? Quantity, string ImgUrl, string Description, string Category, string Status);

    public class GetAmenityByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/amenities/{id}", async (int id, ISender sender) =>
            {
                var query = new GetAmenityByIdQuery(id);
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetAmenityByIdResponse>()); 
                }
                var response = result.Adapt<GetAmenityByIdResponse>();

                return Results.Ok(response);
            })
            .WithName("GetAmenityById")
            .Produces<GetAmenityByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Amenity by ID")
            .WithDescription("Retrieve a Amenity using its ID.");
        }
    }
}
