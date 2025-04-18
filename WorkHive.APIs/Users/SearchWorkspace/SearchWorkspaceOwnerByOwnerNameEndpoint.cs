using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Users.SearchWorkspace;

namespace WorkHive.APIs.Users.SearchWorkspace;

public record SearchWorkspaceOwnerByOwnerNameRequest(string? OwnerName);
public record SearchWorkspaceOwnerByOwnerNameResponse(List<WorkspaceOwnerByOwnerNameDTO> WorkspaceOwnerByOwnerNameDTOs);

public class SearchWorkspaceOwnerByOwnerNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/searchbyownername", async([AsParameters]SearchWorkspaceOwnerByOwnerNameRequest request, ISender sender) =>
        {
            var query = request.Adapt<SearchWorkspaceOwnerByOwnerNameQuery>();

            var result = await sender.Send(query);

            var response = new SearchWorkspaceOwnerByOwnerNameResponse(result.WorkspaceOwnerByOwnerNameDTOs);

            return Results.Ok(response);
        })
        .WithName("Search by owner name")
        .Produces<SearchWorkspaceOwnerByOwnerNameResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Search by owner name")
        .WithTags("Search")
        .WithDescription("Search by owner name");
    }
}
