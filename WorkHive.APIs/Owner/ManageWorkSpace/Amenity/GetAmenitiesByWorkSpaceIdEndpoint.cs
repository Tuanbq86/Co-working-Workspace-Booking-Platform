using Carter;
using FluentValidation;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Owners.ManageWorkSpace.GetAllById;

namespace WorkHive.APIs.Owners.ManageWorkSpace.Amenity
{

    public record GetAmenitiesByWorkSpaceIdResponse(List<AmenityDTO> Amenities);

    public class GetAmenitiesByWorkSpaceIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/amenities/workspace/{WorkSpaceId}", async (int WorkSpaceId, ISender sender) =>
            {
                var command = new GetAmenitiesByWorkSpaceIdCommand(WorkSpaceId);
                var result = await sender.Send(command);
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
