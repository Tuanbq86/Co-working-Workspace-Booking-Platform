using Carter;
using MediatR;
using WorkHive.Services.Wallets.Base_OwnerWallet;

namespace WorkHive.APIs.Wallets.OwnerWallet
{
    public record UpdateOwnerWalletRequest(string BankName, string BankNumber, string BankAccountName);

    public class UpdateOwnerWalletEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/owner-wallets/{id}", async (int id, UpdateOwnerWalletRequest request, ISender sender) =>
            {
                var command = new UpdateOwnerWalletCommand(id, request.BankName, request.BankNumber, request.BankAccountName);
                var result = await sender.Send(command);

                return result
                    ? Results.Ok("Owner wallet updated successfully")
                    : Results.NotFound("Owner wallet not found");
            })
            .WithName("UpdateOwnerWallet")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Owner Wallet")
            .WithSummary("Update owner's bank details")
            .WithDescription("Updates the bank details for the owner's wallet.");
        }
    }
}
