using Carter;
using MediatR;
using WorkHive.Services.Owners.Base_Owner;

namespace WorkHive.APIs.Owner.Manage_Owner
{
    public class UpdateWorkspaceOwnerSocialsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/workspace-owners/{id:int}/socials", async (int id, UpdateWorkspaceOwnerSocialsCommand body, ISender sender) =>
            {
                if (id != body.Id)
                    return Results.BadRequest("ID trong route và trong body không khớp.");

                var result = await sender.Send(body);
                if (!result)
                    return Results.NotFound();

                return Results.Ok("Cập nhật mạng xã hội thành công.");
            })
            .WithName("UpdateWorkspaceOwnerSocials")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Owner")
            .WithSummary("Update owner socials")
            .RequireAuthorization("Owner")
            .WithDescription("Cập nhật các mạng xã hội Facebook, Instagram, Tiktok cho WorkspaceOwner.");
        }
    }
}
