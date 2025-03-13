using Carter;
using MediatR;
using WorkHive.Data.Models;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Amenity;

namespace WorkHive.APIs.Owner.ManageAmenity
{
    public record GetAmenitiesResponse(List<Amenity> Amenities);

    public class GetAmenitiesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/amenities", async (ISender sender) =>
            {
                var query = new GetAllAmenitiesQuery();
                var result = await sender.Send(query);
                if (result == null || !result.Any())
                {
                    return Results.Json(new GetAmenitiesResponse(new List<Amenity>()));
                }
                var response = new GetAmenitiesResponse(result);
                return Results.Ok(response);
            })
            .WithName("GetAmenities")
            .Produces<GetAmenitiesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Amenity")
            .WithSummary("Get all amenities")
            .WithDescription("Retrieve all amenities.");
        }
    }
}
