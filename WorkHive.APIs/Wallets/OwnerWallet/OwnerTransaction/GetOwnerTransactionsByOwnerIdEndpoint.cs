using Carter;
using MediatR;
using WorkHive.Services.Wallets.Base_OwnerWallet;

namespace WorkHive.APIs.Wallets.OwnerWallet.OwnerTransaction
{
    public record GetOwnerTransactionsByOwnerIdResponse(List<GetOwnerTransactionsByOwnerIdResult> Transactions);

    public class GetOwnerTransactionsByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owners/{Id}/transactions", async (int Id, ISender sender) =>
            {
                var query = new GetOwnerTransactionsByOwnerIdQuery(Id);
                var result = await sender.Send(query);
                if (result == null || !result.Any())
                {
                    return Results.Json(Array.Empty<GetOwnerTransactionsByOwnerIdResponse>());
                }
                var response = new GetOwnerTransactionsByOwnerIdResponse(result);

                return Results.Ok(response);
            })
            .WithName("GetOwnerTransactionsByOwnerId")
            .Produces<GetOwnerTransactionsByOwnerIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner Wallet")
            .WithSummary("Get Owner Transactions by Owner ID")
            .WithDescription("Retrieve all transactions belonging to a specific owner.");
        }
    }
}
