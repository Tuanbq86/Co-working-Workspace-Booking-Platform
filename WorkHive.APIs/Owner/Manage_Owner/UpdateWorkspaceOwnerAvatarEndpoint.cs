using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.Base_Owner;

namespace WorkHive.APIs.Owner.Manage_Owner
{
    public record UpdateWorkspaceOwnerAvatarRequest(string AvatarUrl);
    public record UpdateWorkspaceOwnerAvatarResponse(string Notification);

    public class UpdateWorkspaceOwnerAvatarEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/workspace-owners/{id}/avatar", async (int id, UpdateWorkspaceOwnerAvatarRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateWorkspaceOwnerAvatarCommand>() with { Id = id };
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateWorkspaceOwnerAvatarResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateWorkspaceOwnerAvatar")
            .Produces<UpdateWorkspaceOwnerAvatarResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Owner")
            .WithSummary("Update avatar for workspace owner")
            .RequireAuthorization("Owner")
            .WithDescription("Updates the avatar URL for a workspace owner.");
        }
    }
}

//Owner