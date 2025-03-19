using Carter;
using MediatR;
using WorkHive.Services.Owners.Base_Owner;

namespace WorkHive.APIs.Owner.Manage_Owner
{
    public record GetWorkspaceOwnerByIdResponse(GetWorkspaceOwnerByIdResult Owner);

    public class GetWorkspaceOwnerByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/workspace-owners/{id}", async (int id, ISender sender) =>
            {
                var query = new GetWorkspaceOwnerByIdQuery(id);
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.NotFound($"Workspace owner with ID {id} not found.");
                }
                var response = new GetWorkspaceOwnerByIdResponse(result);
                return Results.Ok(response);
            })
            .WithName("GetWorkspaceOwnerById")
            .Produces<GetWorkspaceOwnerByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner")
            .WithSummary("Get workspace owner by ID")
            .WithDescription("Retrieve a specific workspace owner by their ID.");
        }
    }
}
