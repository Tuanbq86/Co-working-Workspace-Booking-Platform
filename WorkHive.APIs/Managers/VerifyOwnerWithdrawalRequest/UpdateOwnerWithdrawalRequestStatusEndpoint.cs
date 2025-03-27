using Carter;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyOwnerWithdrawalRequest
{
    public record UpdateOwnerWithdrawalRequestStatusRequest(int UserId, string Status);

    public record UpdateOwnerWithdrawalRequestStatusResponse(string Notification);

    public class UpdateOwnerWithdrawalRequestStatusEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/owner-withdrawal-requests/{id}/status", async (int id, UpdateOwnerWithdrawalRequestStatusRequest request, ISender sender) =>
            {
                var command = new UpdateOwnerWithdrawalRequestStatusCommand(id, request.UserId,request.Status);
                var result = await sender.Send(command);
                return result != null ? Results.Ok(new UpdateOwnerWithdrawalRequestStatusResponse(result.Notification)) : Results.NotFound("Request not found.");
            })
            .WithName("UpdateOwnerWithdrawalRequestStatus")
            .Produces<UpdateOwnerWithdrawalRequestStatusResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Manager")
            .WithSummary("Update status of an owner withdrawal request")
            .WithDescription("Updates the status of a withdrawal request to 'Reject' or 'Success'.");
        }
    }
}
