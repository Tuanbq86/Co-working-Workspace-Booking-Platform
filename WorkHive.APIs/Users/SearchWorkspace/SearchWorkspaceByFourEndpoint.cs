using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Users.SearchWorkspace;

namespace WorkHive.APIs.Users.SearchWorkspace;

public record SearchWorkspaceByFourRequest(string? Address, string? Category,
    int? Is24h, int? Capacity);
public record SearchWorkspaceByFourResponse(List<WorkspaceSearch4DTO> Workspaces);

public class SearchWorkspaceByFourEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/searchbyfourcriteria", async ([AsParameters]SearchWorkspaceByFourRequest request, ISender sender) =>
        {
            var query = request.Adapt<SearchWorkspaceByFourQuery>();

            var result = await sender.Send(query);

            var response = result.Adapt<SearchWorkspaceByFourResponse>();

            return Results.Ok(response);
        })
        .WithName("Search by 4 criteria")
        .Produces<SearchWorkspaceByFourResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Search by 4 criteria")
        .WithTags("Search")
        .WithDescription("Search by 4 criteria");
    }
}
