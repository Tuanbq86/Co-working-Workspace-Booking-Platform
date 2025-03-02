using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Data.Models;
using WorkHive.Services.Owners.ManageWorkSpace;

namespace WorkHive.APIs.Owner.ManageWorkSpace.WorkSpace
{
    public record CreateWorkspaceRequest(string Name, string Description, int Capacity, string Category, string Status, int CleanTime, int Area, int OwnerId);

    public record CreateWorkspaceResponse(string Notification);

    public class CreateWorkspaceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/workspaces", async (CreateWorkspaceRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateWorkSpaceCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateWorkspaceResponse>();

                return Results.Created($"/workspaces", response);
            })
            .WithName("CreateWorkspace")
            .Produces<CreateWorkspaceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new workspace")
            .WithDescription("Creates a new workspace with the provided details.");

        }
    }

}
