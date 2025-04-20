using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Policy;

namespace WorkHive.APIs.Owner.ManageWorkSpace.ManagePolicy
{
    public record DeletePolicyResponse(string Notification);

    public class DeletePolicyEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/policies/{id}", async (int id, ISender sender) =>
            {
                var command = new DeletePolicyCommand(id);
                var result = await sender.Send(command);
                var response = new DeletePolicyResponse(result.Notification);
                return Results.Ok(response);
            })
            .WithName("DeletePolicy")
            .Produces<DeletePolicyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Policy")
            .WithSummary("Delete a policy")
            .WithDescription("Deletes a policy by ID.");
        }
    }
}