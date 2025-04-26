using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Amenity;

namespace WorkHive.APIs.Owner.ManageAmenity
{
    public record CreateAmenityRequest(string Name, decimal? Price, int? Quantity, string ImgUrl, string Description, string Category, string Status, int OwnerId);
    public record CreateAmenityResponse(string Notification);

    public class CreateAmenityEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/amenities", async (CreateAmenityRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateAmenityCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateAmenityResponse>();
                return Results.Created($"/amenities", response);
            })
            .WithName("CreateAmenity")
            .Produces<CreateAmenityResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Amenity")
            .WithSummary("Create a new amenity")
            .WithDescription("Creates a new amenity with the provided details.");
        }
    }
}
//Owner