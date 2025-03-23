using Carter;
using MediatR;
using WorkHive.Services.Wallets.Base_OwnerWallet;

namespace WorkHive.APIs.Wallets.OwnerWallet.OwnerTransaction
{
    public record GetTransactionByIdResponse(GetTransactionByIdResult Transaction);

    public class GetTransactionByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/transactions/{id}", async (int id, ISender sender) =>
            {
                var query = new GetTransactionByIdQuery(id);
                var result = await sender.Send(query);

                if (result == null)
                {
                    return Results.NotFound(new { Message = "Transaction not found" });
                }

                return Results.Ok(new GetTransactionByIdResponse(result));
            })
            .WithName("GetTransactionById")
            .Produces<GetTransactionByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Transactions")
            .WithSummary("Get Transaction by ID")
            .WithDescription("Retrieve details of a specific transaction by its ID.");
        }
    }
}
