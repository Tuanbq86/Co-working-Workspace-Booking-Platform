using Carter;
using MediatR;
using WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyCustomerWithdrawalRequest;

public record GetAllCustomerWithdrawalRequestByCustomerIdResponse(List<CustomerWithdrawalRequestDTO> CustomerWithdrawalRequests);

public class GetAllCustomerWithdrawalRequestEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("users/customerwithdrawalrequests/{CustomerId}", async (int CustomerId, ISender sender) =>
        {
            var query = new GetAllCustomerWithdrawalRequestByCustomerIdQuery(CustomerId);

            var result = await sender.Send(query);

            var response = new GetAllCustomerWithdrawalRequestByCustomerIdResponse(result.CustomerWithdrawalRequests);

            return Results.Ok(response);
        })
        .WithName("Get all customer withdrawal requests by customer id")
        .Produces<GetAllCustomerWithdrawalRequestByCustomerIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithTags("Customer Withdraw")
        .WithSummary("Get all customer withdrawal requests by customer id")
        .WithDescription("Get all customer withdrawal requests by customer id");
    }
}
