using Carter;
using MediatR;
using WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyCustomerWithdrawalRequest;

public record GetAllCustomerRequestResponse(List<CustomerRequestDTO> CustomerRequests);

public class GetAllCustomerRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("getallcustomerwithdrawalrequests", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllCustomerRequestQuery());

            var response = new GetAllCustomerRequestResponse(result.CustomerRequests);

            return Results.Ok(response);
        })
        .WithName("Get all customer withdrawal requests")
        .Produces<GetAllCustomerRequestResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithTags("Customer Withdraw")
        .WithSummary("Get all customer withdrawal requests")
        .WithDescription("Get all customer withdrawal requests");
    }
}
