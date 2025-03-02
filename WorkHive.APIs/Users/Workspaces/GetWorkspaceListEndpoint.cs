using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Data.Models;
using WorkHive.Services.Users.Workspaces;

namespace WorkHive.APIs.Users.Workspaces;

public record GetWorkspaceListResponse(List<Workspace> Workspaces);

public class GetWorkspaceListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/workspaces", async (ISender sender) =>
        {
            var result = await sender.Send(new GetWorkspaceListQuery());

            var response = result.Adapt<GetWorkspaceListResponse>();

            return Results.Ok(response);
        })
        .WithName("GetWorkspaceList")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Workspace List")
        .WithDescription("Get Workspace List");
    }
}
