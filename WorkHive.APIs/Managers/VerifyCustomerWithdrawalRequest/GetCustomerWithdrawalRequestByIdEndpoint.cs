using Carter;
using MediatR;
using WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyCustomerWithdrawalRequest;

public record GetCustomerWithdrawalRequestByIdResponse(CustomerWithdrawalRequestByIdDTO CustomerWithdrawalRequestByIdDTOs);

public class GetCustomerWithdrawalRequestByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("getcustomerwithdrawalrequestbyid/{CustomerRequestId}", async (int CustomerRequestId, ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerWithdrawalRequestByIdQuery(CustomerRequestId));

            var response = new GetCustomerWithdrawalRequestByIdResponse(result.CustomerWithdrawalRequestByIdDTOs);

            return Results.Ok(response);
        })
        .WithName("Get all customer withdrawal requests by id")
        .Produces<GetCustomerWithdrawalRequestByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithTags("Customer Withdraw")
        .WithSummary("Get all customer withdrawal requests by id")
        .WithDescription("Get all customer withdrawal requests by id");
    }
}

