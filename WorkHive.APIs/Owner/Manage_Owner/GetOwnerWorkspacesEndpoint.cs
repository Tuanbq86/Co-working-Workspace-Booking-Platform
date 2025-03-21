using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Workspace;

namespace WorkHive.APIs.Owner.Manage_Owner
{
    public record GetOwnerWorkspacesResponse(List<GetWorkspaceRevenueResult> Workspaces);

    public class GetOwnerWorkspacesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owners/{ownerId}/workspaces", async (ISender sender, int ownerId) =>
            {
                var query = new GetOwnerWorkspacesQuery(ownerId);
                var result = await sender.Send(query);


                return Results.Ok(new GetOwnerWorkspacesResponse(result ?? new List<GetWorkspaceRevenueResult>()));
            })
            .WithName("GetOwnerWorkspaces")
            .Produces<GetOwnerWorkspacesResponse>(StatusCodes.Status200OK)
            .WithTags("Owner")
            .WithSummary("Get all workspaces of an owner with revenue")
            .WithDescription("Retrieve all workspaces of a specific owner along with their revenue.");

        }
    }
}