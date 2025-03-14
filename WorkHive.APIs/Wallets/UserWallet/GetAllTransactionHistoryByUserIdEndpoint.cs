using Carter;
using Mapster;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.DTOService;
using WorkHive.Services.Wallets.UserWallet;

namespace WorkHive.APIs.Wallets.UserWallet;

public record GetAllTransactionHistoryByUserIdRequest(int UserId);
public record GetAllTransactionHistoryByUserIdResponse(
    List<UserTransactionHistoryDTO> UserTransactionHistoryDTOs = null!)
{
    public List<UserTransactionHistoryDTO> UserTransactionHistoryDTOs { get; init; } = [];
}

public class GetAllTransactionHistoryByUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/wallet/getalltransactionhistorybyuserid", async ([AsParameters] GetAllTransactionHistoryByUserIdRequest request, ISender sender) =>
        {
            var query = request.Adapt<GetAllTransactionHistoryByUserIdQuery>();

            var result = await sender.Send(query);

            var response = result.Adapt<GetAllTransactionHistoryByUserIdResponse>();

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
