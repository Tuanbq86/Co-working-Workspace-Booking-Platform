using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Policy;

namespace WorkHive.APIs.Owner.ManagePolicy
{
    public record GetPolicyByIdResponse(int Id, string Name);

    public class GetPolicyByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/policies/{id}", async (int id, ISender sender) =>
            {
                var query = new GetPolicyByIdQuery(id);
                var result = await sender.Send(query);
                return result == null
                    ? Results.NotFound("Policy not found")
                    : Results.Ok(new GetPolicyByIdResponse(result.Id, result.Name));
            })
            .WithName("GetPolicyById")
            .Produces<GetPolicyByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Policy")
            .WithSummary("Get a policy by ID")
            .WithDescription("Retrieves a policy using its unique identifier.");
        }
    }
}
