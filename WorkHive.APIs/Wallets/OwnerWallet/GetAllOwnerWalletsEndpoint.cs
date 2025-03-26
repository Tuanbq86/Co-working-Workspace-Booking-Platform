using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Wallets.Base_OwnerWallet;

namespace WorkHive.APIs.Wallets.OwnerWallet
{
    public record GetAllOwnerWalletsResponse(int Id, decimal? Balance, string Status, int OwnerId, string OwnerName, string LicenseName);

    public class GetAllOwnerWalletsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/wallets", async (ISender sender) =>
            {
                var query = new GetAllOwnerWalletsQuery();
                var result = await sender.Send(query);

                return Results.Ok(result.Adapt<List<GetAllOwnerWalletsResponse>>());
            })
            .WithName("GetAllOwnerWallets")
            .Produces<List<GetAllOwnerWalletsResponse>>(StatusCodes.Status200OK)
            .WithTags("Owner Wallet")
            .WithSummary("Get All Owner Wallets")
            .WithDescription("Retrieve a list of all owner wallets, including their owner details.");
        }
    }
}

