using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Wallets.UserWallet;

namespace WorkHive.APIs.Wallets.UserWallet;

public record UserDepositRequest(int UserId, int Amount);
public record UserDepositResponse(int CustomerWalletId, string Bin, string AccountNumber, int Amount, string Description,
    long OrderCode, string PaymentLinkId, string Status, string CheckoutUrl, string QRCode);


public class UserDepositEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/wallet/createrequestdeposit", async (UserDepositRequest request, ISender sender) =>
        {
            var command = request.Adapt<UserDepositCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UserDepositResponse>();

            return Results.Ok(response);
        })
        .WithName("Create depoit request")
        .Produces<UserDepositResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create depoit request")
        .WithTags("CustomerWallet")
        .RequireAuthorization("Customer")
        .WithDescription("Create depoit request");
    }
}
