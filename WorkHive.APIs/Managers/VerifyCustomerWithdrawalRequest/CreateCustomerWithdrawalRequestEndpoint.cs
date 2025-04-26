using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Managers.VerifyOwnerWithdrawalRequest;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyCustomerWithdrawalRequest;

public record CreateCustomerWithdrawalRequestRequest(string Title, string Description, int CustomerId);
public record CreateCustomerWithdrawalRequestResponse(string Notification, int IsLock);
public class CreateCustomerWithdrawalRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/customer-withdrawal-requests", async (CreateCustomerWithdrawalRequestRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateCustomerWithdrawalRequestCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateCustomerWithdrawalRequestResponse>();
            return Results.Created($"/customer-withdrawal-requests", response);
        })
            .WithName("CreateCustomerWithdrawalRequest")
            .Produces<CreateCustomerWithdrawalRequestResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Customer Withdraw")
            .WithSummary("Create a new customer withdrawal request")
            .WithDescription("Creates a new withdrawal request for a customer.");
    }
}

//Customer