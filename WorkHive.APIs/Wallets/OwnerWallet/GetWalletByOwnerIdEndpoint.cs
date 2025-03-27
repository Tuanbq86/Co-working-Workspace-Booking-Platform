using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Wallets.Base_OwnerWallet;

namespace WorkHive.APIs.Wallets.OwnerWallet
{
    public record GetWalletByIdResponse(int Id, decimal? Balance, string Status, string BankName, string BankAccountName, string BankNumber, int OwnerId, string OwnerName, string LicenseName);

    public class GetWalletByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-wallets/{id}", async (int id, ISender sender) =>
            {
                var query = new GetWalletByOwnerIdQuery(id);
                var result = await sender.Send(query);
                if (result == null)
                {
                    return Results.Json(Array.Empty<GetWalletByIdResponse>());
                }
                var response = result.Adapt<GetWalletByIdResponse>();

                return Results.Ok(response);
            })
            .WithName("GetWalletById")
            .Produces<GetWalletByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner Wallet")
            .WithSummary("Get Owner Wallet by OwnerID")
            .WithDescription("Retrieve a wallet including owner name using its ID.");
        }
    }
}