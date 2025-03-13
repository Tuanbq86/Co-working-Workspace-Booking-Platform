using Carter;
using FluentValidation;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Owners.ManageWorkSpace.GetAllById;
using static WorkHive.APIs.Owner.ManageBeverage.GetBeverageByIdEndpoint;

namespace WorkHive.APIs.Owner.ManageAmenity
{

    public record GetAmenitiesByOwnerIdResponse(List<AmenityDTO> Amenities);

    public class GetAmenitiesByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/amenities/Owner/{OwnerId}", async (int OwnerId, ISender sender) =>
            {
                var query = new GetAmenitiesByOwnerIdQuery(OwnerId);
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetAmenitiesByOwnerIdResponse>()); 
                }
                var response = new GetAmenitiesByOwnerIdResponse(result);

                return Results.Ok(response);
            })
            .WithName("GetAmenitiesByOwnerId")
            .Produces<GetAmenitiesByOwnerIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Amenity")
            .WithSummary("Get Amenities by Owner ID")
            .WithDescription("Retrieve all amenities belonging to a specific Owner.");
        }
    }
}
