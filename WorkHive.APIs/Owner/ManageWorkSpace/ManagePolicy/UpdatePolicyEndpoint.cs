using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Policy;

namespace WorkHive.APIs.Owner.ManageWorkSpace.ManagePolicy
{
    public record UpdatePolicyRequest(string Name);
    public record UpdatePolicyResponse(string Notification);

    public class UpdatePolicyEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/policies/{id}", async (int id, UpdatePolicyRequest request, ISender sender) =>
            {
                var command = new UpdatePolicyCommand(id, request.Name);
                var result = await sender.Send(command);
                var response = new UpdatePolicyResponse(result.Notification);
                return Results.Ok(response);
            })
            .WithName("UpdatePolicy")
            .Produces<UpdatePolicyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Policy")
            .WithSummary("Update an existing policy")
            .WithDescription("Updates an existing policy by ID.");
        }
    }
}