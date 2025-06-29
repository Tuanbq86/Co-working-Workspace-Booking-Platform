﻿using Carter;
using MediatR;
using WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyOwnerWithdrawalRequest
{
    /// <summary>
    /// Represents a response containing all owner withdrawal requests.
    /// </summary>
    /// <param name="Requests"></param>
    public record GetAllOwnerWithdrawalRequestsResponse(List<OwnerWithdrawalRequestDTO> Requests);

    public class GetAllOwnerWithdrawalRequestsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-withdrawal-requests", async (ISender sender) =>
            {
                var query = new GetAllOwnerWithdrawalRequestsQuery();
                var result = await sender.Send(query);
                return Results.Ok(new GetAllOwnerWithdrawalRequestsResponse(result));
            })
            .WithName("GetAllOwnerWithdrawalRequests")
            .Produces<GetAllOwnerWithdrawalRequestsResponse>(StatusCodes.Status200OK)
            .WithTags("Manager")
            .WithSummary("Get all owner withdrawal requests")
            .WithDescription("Retrieve all withdrawal requests made by owners.");
        }
    }
}
