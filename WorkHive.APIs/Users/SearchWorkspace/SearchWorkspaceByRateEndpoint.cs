using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Users.SearchWorkspace;

namespace WorkHive.APIs.Users.SearchWorkspace;

public record SearchWorkspaceByRateResponse(List<WorkspaceSearchByRateDTO> Workspaces);

public class SearchWorkspaceByRateEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/searchbyrate", async (ISender sender) =>
        {
            var result = await sender.Send(new SearchWorkspaceByRateQuery());

            var response = result.Adapt<SearchWorkspaceByRateResponse>();

            return Results.Ok(response);
        })
        .WithName("Search by rate")
        .Produces<SearchWorkspaceByRateResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Search by rate")
        .WithTags("Search")
        .WithDescription("Search by rate");
    }
}
