using Carter;
using Mapster;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyCustomerWithdrawalRequest;

public record UpdateCustomerWithdrawalRequestStatusRequest(int CustomerWithdrawalRequestId, int ManagerId, string ManagerResponse, string Status);

public record UpdateCustomerWithdrawalRequestStatusResponse(string Notification);

public class UpdateCustomerWithdrawalRequestStatusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/managers/updatecustomerwithdrawalstatusrequest", async (UpdateCustomerWithdrawalRequestStatusRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateCustomerWithdrawalRequestStatusCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateCustomerWithdrawalRequestStatusResponse>();

            return Results.Ok(response);
        })
        .WithName("updatecustomerwithdrawalstatusrequest")
        .Produces<UpdateCustomerWithdrawalRequestStatusResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithTags("Customer Withdraw")
        .WithSummary("Update customer withdrawal request status")
        //.RequireAuthorization("Manager")
        .WithDescription("Update customer withdrawal request status");

    }
}

//Manager