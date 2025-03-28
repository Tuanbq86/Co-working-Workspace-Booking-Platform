using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.DTOService;
using WorkHive.Services.Managers;

namespace WorkHive.APIs.Managers;
public record GetAllOwnerResponse(List<OwnerDTO> Owners);
public class GetAllOwnerForManagerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/managers/getallowner", async(ISender sender) =>
        {
            var result = await sender.Send(new GetAllOwnerQuery());

            var response = result.Adapt<GetAllOwnerResponse>();

            return Results.Ok(response);
        })
        .WithName("Get all owner for manager, staff and admin")
        .Produces<GetAllOwnerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get all owner for manager, staff and admin")
        .WithTags("Manager")
        .WithDescription("Get all owner for manager, staff and admin");
    }
}
