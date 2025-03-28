using Carter;
using MediatR;
using WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyOwnerWithdrawalRequest
{
    public record GetAllOwnerWithdrawalRequestsByOwnerIdResponse(List<OwnerWithdrawalRequestDTO> Requests);

    public class GetAllOwnerWithdrawalRequestsByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-drawal-requests/{ownerId}", async (ISender sender, int ownerId) =>
            {
                var query = new GetAllOwnerWithdrawalRequestsByOwnerIdQuery(ownerId);
                var result = await sender.Send(query);
                return Results.Ok(new GetAllOwnerWithdrawalRequestsByOwnerIdResponse(result));
            })
            .WithName("GetAllOwnerWithdrawalRequestsByOwnerId")
            .Produces<GetAllOwnerWithdrawalRequestsByOwnerIdResponse>(StatusCodes.Status200OK)
            .WithTags("Manager")
            .WithSummary("Get all owner withdrawal requests by owner ID")
            .WithDescription("Retrieve all withdrawal requests made by a specific owner.");
        }
    }
}
