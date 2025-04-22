using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Workspace;

namespace WorkHive.APIs.Owner.ManageWorkSpace.WorkSpace
{
    public record GetNearbyWorkspacesResponse(List<WorkspaceNearbyDT> Workspaces);

    public class GetNearbyWorkspacesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/workspaces/nearby", async (double? lat, double? lng, double? radiusKm, ISender sender) =>
            {
                if (lat is null || lng is null)
                    return Results.Ok(null);

                var query = new GetNearbyWorkspacesQuery(lat.Value, lng.Value, radiusKm ?? 200);
                var result = await sender.Send(query);

                return Results.Ok(new GetNearbyWorkspacesResponse(result));
            })
            .WithName("GetNearbyWorkspaces")
            .Produces<GetNearbyWorkspacesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Workspace")
            .WithSummary("Get nearby workspaces")
            .WithDescription("Retrieve workspaces located within a certain radius from a given location.");
        }
    }       
}
