using Carter;
using MediatR;
using WorkHive.Services.Owners.Base_Owner;

namespace WorkHive.APIs.Owner.Manage_Owner
{
    public record GetAllWorkspaceOwnersResponse(List<GetWorkspaceOwnersResult> Owners);

    public class GetAllWorkspaceOwnersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/workspace-owners/", async (ISender sender) =>
            {
                var query = new GetAllWorkspaceOwnersQuery();
                var result = await sender.Send(query);
                if (result == null || !result.Any())
                {
                    return Results.Json(Array.Empty<GetAllWorkspaceOwnersResponse>());
                }
                var response = new GetAllWorkspaceOwnersResponse(result);

                return Results.Ok(response);
            })
            .WithName("GetAllWorkspaceOwners")
            .Produces<GetAllWorkspaceOwnersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner")
            .WithSummary("Get all workspace owners")
            .WithDescription("Retrieve all workspace owners.");
        }
    }
}