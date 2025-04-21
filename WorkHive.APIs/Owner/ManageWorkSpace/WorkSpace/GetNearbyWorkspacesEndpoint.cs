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
            app.MapGet("/workspaces/nearby", async (double lat, double lng, double? radiusKm, ISender sender) =>
            {
                var query = new GetNearbyWorkspacesQuery(lat, lng, radiusKm ?? 5);  // Sử dụng GetNearbyWorkspacesQuery
                var result = await sender.Send(query);  // Gửi yêu cầu thông qua MediatR

                return Results.Ok(new GetNearbyWorkspacesResponse(result));  // Trả về kết quả
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
