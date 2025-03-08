using Carter;
using FluentValidation;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Owners.ManageWorkSpace.GetAllById;
using static WorkHive.APIs.Owner.ManageWorkSpace.Beverage.GetBeverageByIdEndpoint;

namespace WorkHive.APIs.Owners.ManageWorkSpace.Amenity
{

    public record GetAmenitiesByWorkSpaceIdResponse(List<AmenityDTO> Amenities);

    public class GetAmenitiesByWorkSpaceIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/amenities/workspace/{workSpaceId}", async (int workSpaceId, ISender sender) =>
            {
                var query = new GetAmenitiesByWorkSpaceIdQuery(workSpaceId);
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetAmenitiesByWorkSpaceIdResponse>()); 
                }
                var response = new GetAmenitiesByWorkSpaceIdResponse(result);

                return Results.Ok(response);
            })
            .WithName("GetAmenitiesByWorkSpaceId")
            .Produces<GetAmenitiesByWorkSpaceIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Amenities by WorkSpace ID")
            .WithDescription("Retrieve all amenities belonging to a specific WorkSpace.");
        }
    }
}
