using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Workspace;

namespace WorkHive.APIs.Owner.ManageWorkSpace.WorkSpace
{ 
public record GetWorkSpacesResponse(List<GetWorkSpacesResult> Workspaces);

public class GetWorkSpacesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/workspaces/", async (ISender sender) =>
        {
            var query = new GetWorkSpacesQuery();
            var result = await sender.Send(query);
            if (result == null)
            {
                return Results.Json(Array.Empty<GetWorkSpacesResponse>());
            }
            var response = new GetWorkSpacesResponse(result);

            return Results.Ok(response);
        })
        .WithName("GetWorkSpaces")
        .Produces<GetWorkSpacesResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithTags("Workspace")
        .WithSummary("Get all workspace ")
        .WithDescription("Retrieve all workspaces.");
    }
}
}
