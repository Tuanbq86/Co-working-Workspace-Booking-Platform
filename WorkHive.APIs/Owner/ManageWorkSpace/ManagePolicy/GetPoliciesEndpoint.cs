using Carter;
using MediatR;
using WorkHive.Data.Models;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Policy;

namespace WorkHive.APIs.Owner.ManageWorkSpace.ManagePolicy
{
    public record GetPoliciesResponse(List<Policy> Policies);
    public class GetPoliciesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/policies/", async (ISender sender) =>
            {
                var query = new GetAllPolicyQuery();
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetPoliciesResponse>());
                }
                var response = new GetPoliciesResponse(result);
                return Results.Ok(response);
            })
            .WithName("GetPolicies")
            .Produces<GetPoliciesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Policy")
            .WithSummary("Get all policies ")
            .WithDescription("Retrieve all policies.");
        }
    }
}
