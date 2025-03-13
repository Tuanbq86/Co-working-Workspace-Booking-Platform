using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Wallets.UserWallet;

namespace WorkHive.APIs.Wallets.UserWallet;

public record UpdateUserWalletAmountRequest(int CustomerWalletId, long OrderCode, int Amount);
public record UpdateUserWalletAmountResponse(string Notification);

public class UpdateUserWalletAmountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/wallet/updatewalletamount", async(UpdateUserWalletAmountRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateUserWalletAmountCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateUserWalletAmountResponse>();
                
            return Results.Ok(response);
        })
        .WithName("Update user wallet amount")
        .Produces<UpdateUserWalletAmountResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Update user wallet amount")
        .WithTags("CustomerWallet")
        .WithDescription("Update user wallet amount");
    }
}
