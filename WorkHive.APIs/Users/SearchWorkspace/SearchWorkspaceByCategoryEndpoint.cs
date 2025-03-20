using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Users.SearchWorkspace;

namespace WorkHive.APIs.Users.SearchWorkspace;

public record SearchWorkspaceByCategoryResponse(List<WorkspaceSearchByCategoryDTO> WorkspaceSearchByCategoryDTOs);

public class SearchWorkspaceByCategoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/searchbycategory{Category}", async (string Category, ISender sender) =>
        {
            var result = await sender.Send(new SearchWorkspaceByCategoryQuery(Category));

            var response = new SearchWorkspaceByCategoryResponse(result.WorkspaceSearchByCategoryDTOs);

            return Results.Ok(response);
        })
        .WithName("Search by category")
        .Produces<SearchWorkspaceByCategoryResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Search by category")
        .WithTags("Search")
        .WithDescription("Search by category");
    }
}
