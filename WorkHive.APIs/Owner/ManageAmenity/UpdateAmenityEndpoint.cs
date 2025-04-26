using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Amenity;

namespace WorkHive.APIs.Owner.ManageAmenity
{
    public record UpdateAmenityRequest(string Name, string Description, string Category, string Status, string ImgUrl, decimal? Price, int? Quantity);
    public record UpdateAmenityResponse(string Notification);

    public class UpdateAmenityEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/amenities/{id}", async (int id, UpdateAmenityRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateAmenityCommand>() with { Id = id };
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateAmenityResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateAmenity")
            .Produces<UpdateAmenityResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Amenity")
            .WithSummary("Update an existing amenity")
            .WithDescription("Updates an amenity by its ID with the provided details.");
        }
    }
}
//Owner