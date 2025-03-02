using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Data.Models;
using WorkHive.Services.Users.Workspaces;

namespace WorkHive.APIs.Users.Workspaces;

public record GetWorkspaceByIdResponse(Workspace Workspace);

public class GetWorkspaceByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/workspaces/{Id}", async (int Id, ISender sender) =>
        {
            var result = await sender.Send(new GetWorkspaceByIdQuery(Id));

            var response = result.Adapt<GetWorkspaceByIdResponse>();

            return Results.Ok(response);
        })
        .WithName("GetWorkspaceById")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get Workspace By Id")
        .WithDescription("Get Workspace By Id");
    }
}
