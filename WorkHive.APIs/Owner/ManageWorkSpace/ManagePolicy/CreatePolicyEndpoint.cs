using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Policy;

namespace WorkHive.APIs.Owner.ManageWorkSpace.ManagePolicy
{
    public record CreatePolicyRequest(string Name);
    public record CreatePolicyResponse(string Notification);
    public class CreatePolicyEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/policies", async (CreatePolicyRequest request, ISender sender) =>
            {
                var query = request.Adapt<CreatePolicyCommand>();
                var result = await sender.Send(query);
                var response = result.Adapt<CreatePolicyResponse>();
                return Results.Created($"/policies", response);
            })
            .WithName("CreatePolicy")
            .Produces<CreatePolicyResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Policy")
            .WithSummary("Create a new policy")
            .RequireAuthorization("Owner")
            .WithDescription("Creates a new policy with the provided details.");
        }
    }
}
