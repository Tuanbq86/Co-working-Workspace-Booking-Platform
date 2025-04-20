using Carter;
using MediatR;
using WorkHive.Data.Models;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Facility;

namespace WorkHive.APIs.Owner.ManageWorkSpace.ManageFacility
{
    public record GetFacilitiesResponse(List<Facility> Facilities);

    public class GetFacilitiesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/facilities/", async (ISender sender) =>
            {
                var query = new GetAllFacilityQuery();
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetFacilitiesResponse>());
                }
                var response = new GetFacilitiesResponse(result);
                return Results.Ok(response);
            })
            .WithName("GetFacilities")
            .Produces<GetFacilitiesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Facility")
            .WithSummary("Get all facilities")
            .WithDescription("Retrieve all facilities.");
        }
    }
}
