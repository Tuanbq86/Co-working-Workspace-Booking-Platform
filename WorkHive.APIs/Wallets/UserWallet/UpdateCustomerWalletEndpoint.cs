using Carter;
using Mapster;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Wallets.UserWallet;

namespace WorkHive.APIs.Wallets.UserWallet;
public record UpdateCustomerWalletRequest(int CustomerId, string BankName, string BankNumber, string BankAccountName);
public record UpdateCustomerWalletResponse(string Notification);
public class UpdateCustomerWalletEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("users/wallet/updatecustomerwalletinformation", async (UpdateCustomerWalletRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateCustomerWalletCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateCustomerWalletResponse>();

            return Results.Ok(response);
        })
        .WithName("Update customer wallet information")
        .Produces<UpdateCustomerWalletResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Update customer wallet information")
        .WithTags("Customer Withdraw")
        .WithDescription("Update customer wallet information");
    }
}
//Customer