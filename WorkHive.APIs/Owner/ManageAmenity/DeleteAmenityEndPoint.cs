using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Amenity;

namespace WorkHive.APIs.Owner.ManageAmenity
{
    public record DeleteAmenityResponse(string Notification);

    public class DeleteAmenityEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/amenities/{id}", async (int id, ISender sender) =>
            {
                var command = new DeleteAmenityCommand(id);
                var result = await sender.Send(command);
                var response = result.Adapt<DeleteAmenityResponse>();
                return Results.Ok(response);
            })
            .WithName("DeleteAmenity")
            .Produces<DeleteAmenityResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Amenity")
            .WithSummary("Delete an amenity")
            .WithDescription("Deletes an amenity by its ID.");
        }
    }
}
//Owner