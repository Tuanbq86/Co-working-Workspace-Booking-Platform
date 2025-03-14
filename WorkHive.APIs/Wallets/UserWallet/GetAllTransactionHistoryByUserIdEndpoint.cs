using Azure.Core;
using Carter;
using Mapster;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.DTOService;
using WorkHive.Services.Wallets.UserWallet;

namespace WorkHive.APIs.Wallets.UserWallet;

//public record GetAllTransactionHistoryByUserIdRequest(int UserId);
public record GetAllTransactionHistoryByUserIdResponse(List<UserTransactionHistoryDTO> UserTransactionHistoryDTOs);

public class GetAllTransactionHistoryByUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/wallet/getalltransactionhistorybyuserid/{UserId}", async (int UserId, ISender sender) =>
        {
            var result = await sender.Send(new GetAllTransactionHistoryByUserIdQuery(UserId));

            var response = new GetAllTransactionHistoryByUserIdResponse(result.UserTransactionHistoryDTOs);

            return Results.Ok(response);
        })
        .WithName("Get transaction history by userid")
        .Produces<GetAllTransactionHistoryByUserIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get transaction history by userid")
        .WithTags("CustomerWallet")
        .WithDescription("Get transaction history by userid");
    }
}
