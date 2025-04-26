using Carter;
using Mapster;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Wallets.UserWallet.UserDepositForMobile;

namespace WorkHive.APIs.Wallets.UserWallet.UserDepositForMobile;

public record UserDepositForMobileRequest(int UserId, int Amount);
public record UserDepositForMobileResponse(int CustomerWalletId, string Bin, string AccountNumber, int Amount, string Description,
    long OrderCode, string PaymentLinkId, string Status, string CheckoutUrl, string QRCode);

public class UserDepositForMobileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/wallet/userdepositformobile", async (UserDepositForMobileRequest request, ISender sender) =>
        {
            var command = request.Adapt<UserDepositForMobileCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UserDepositForMobileResponse>();

            return Results.Ok(response);
        })
        .WithName("Create depoit for mobile")
        .Produces<UserDepositForMobileResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create depoit for mobile")
        .WithTags("CustomerWallet")
        .WithDescription("Create depoit for mobile");
    }
}
//Customer
