using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Wallets.UserWallet;

namespace WorkHive.APIs.Wallets.UserWallet;

public record GetCustomerWalletInformationByCustomerIdResponse(int WalletId, int CustomerWalletId, decimal? Balance, int? IsLock, string BankName, string BankAccountName, string BankNumber);

public class GetCustomerWalletInformationByCustomerIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/wallet/getcustomerwalletinformation/{CustomerId}", async (int CustomerId, ISender sender) =>
        {
            var result = await sender.Send(new GetCustomerWalletInformationByCustomerIdQuery(CustomerId));

            var response = result.Adapt<GetCustomerWalletInformationByCustomerIdResponse>();

            return Results.Ok(response);
        })
        .WithName("Get customer wallet information by customer Id")
        .Produces<GetCustomerWalletInformationByCustomerIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get customer wallet information by customer Id")
        .WithTags("CustomerWallet")
        .WithDescription("Get customer wallet information by customer Id");
    }
}
