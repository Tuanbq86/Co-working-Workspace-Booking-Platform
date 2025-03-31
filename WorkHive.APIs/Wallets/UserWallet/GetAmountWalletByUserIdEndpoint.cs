using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Wallets.UserWallet;

namespace WorkHive.APIs.Wallets.UserWallet;

public record GetAmountWalletByUserIdRequest(int UserId);
public record GetAmountWalletByUserIdResponse(string? Amount, string Notification);

public class GetAmountWalletByUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/wallet/getamountwalletbyuserid", async([AsParameters]GetAmountWalletByUserIdRequest request, ISender sender) =>
        {
            var query = request.Adapt<GetAmountWalletByUserIdQuery>();

            var result = await sender.Send(query);

            var response = result.Adapt<GetAmountWalletByUserIdResponse>();

            return Results.Ok(response);
        })
        .WithName("Get amount wallet by userid")
        .Produces<GetAmountWalletByUserIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get amount wallet by userid")
        .WithTags("CustomerWallet")
        .WithDescription("Get amount wallet by userid");
    }
}
