using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace;
using WorkHive.Services.Owners.ManageWorkSpace.GetById;
using static WorkHive.APIs.Owner.ManageWorkSpace.Beverage.GetBeverageByIdEndpoint;

namespace WorkHive.APIs.Owner.ManageWorkSpace.WorkSpace
{
    public record GetWorkSpaceByIdResponse(int Id, string Name, string Description, string Address, int? Capacity, string GoogleMapUrl, string Category, string Status, int? CleanTime, int? Area, int OwnerId, List<WorkspacePriceDTO> Prices,
    List<WorkspaceImageDTO> Images);

    public class GetWorkSpaceByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/workspaces/{id}", async (int id, ISender sender) =>
            {
                var query = new GetWorkSpaceByIdQuery(id);
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetWorkSpaceByIdResponse>()); 
                }
                GetWorkSpaceByIdResponse response = result.Adapt<GetWorkSpaceByIdResponse>();

                return Results.Ok(response);
            })
            .WithName("GetWorkSpaceById")
            .Produces<GetWorkSpaceByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Workspace by ID")
            .WithDescription("Retrieve a workspace using its ID.");
        }
    }
}

