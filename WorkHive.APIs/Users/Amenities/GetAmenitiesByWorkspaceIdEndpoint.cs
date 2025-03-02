using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Services.Users.Amenities;

namespace WorkHive.APIs.Users.Amenities;

public record GetAmenitiesByWorkspaceIdResponse(List<Amenity> Amenities);

public class GetAmenitiesByWorkspaceIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/amenities/workspaces/{Id}", async (int Id, ISender sender) =>
        {
            var result = await sender.Send(new GetAmenitiesByWorkspaceIdQuery(Id));

            var response = result.Adapt<GetAmenitiesByWorkspaceIdResponse>();

            return Results.Ok(response);
        })
        .WithName("Get Amenities By WorkspaceId")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("GetAmenitiesByWorkspaceId")
        .WithDescription("GetAmenitiesByWorkspaceId");
    }
}
