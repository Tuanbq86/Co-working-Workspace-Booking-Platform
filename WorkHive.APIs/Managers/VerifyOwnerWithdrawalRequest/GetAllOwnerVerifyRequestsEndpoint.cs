using Carter;
using MediatR;
using WorkHive.Services.Owners.Base_Owner;

namespace WorkHive.APIs.Managers.VerifyOwnerWithdrawalRequest
{
    public record GetAllOwnerVerifyRequestsResponse(List<GetOwnerVerifyRequestResult> Requests);

    public class GetAllOwnerVerifyRequestsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-verify-requests/", async (ISender sender) =>
            {
                var query = new GetAllOwnerVerifyRequestsQuery();
                var result = await sender.Send(query);

                if (result == null || !result.Any())
                {
                    return Results.Json(Array.Empty<GetAllOwnerVerifyRequestsResponse>());
                }

                var response = new GetAllOwnerVerifyRequestsResponse(result);

                return Results.Ok(response);
            })
            .WithName("GetAllOwnerVerifyRequests")
            .Produces<GetAllOwnerVerifyRequestsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner")
            .WithSummary("Get all owner verify requests")
            .WithDescription("Retrieve all owner verification requests.");
        }
    }
}
