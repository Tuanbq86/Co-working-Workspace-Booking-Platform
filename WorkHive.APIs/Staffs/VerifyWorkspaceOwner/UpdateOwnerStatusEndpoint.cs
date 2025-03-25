using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Staff;

namespace WorkHive.APIs.Staff.VerifyWorkspaceOwner
{
    public record UpdateOwnerStatusRequest(string Status);
    public record UpdateOwnerStatusResponse(string Notification);
    public class UpdateOwnerStatusEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/owners/{id}/status", async (int id, UpdateOwnerStatusRequest request, ISender sender) =>
            {
                var command = new UpdateOwnerStatusCommand(id, request.Status);
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateOwnerStatusResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateOwnerStatus")
            .Produces<UpdateOwnerStatusResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Staff")
            .WithSummary("Update the status of an owner")
            .WithDescription("Updates the status of a workspace owner to either 'Fail' or 'Success'.");
        }
    }
}