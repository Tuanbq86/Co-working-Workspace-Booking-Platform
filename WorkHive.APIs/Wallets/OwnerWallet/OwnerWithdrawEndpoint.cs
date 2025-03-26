using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Wallets.Base_OwnerWallet;

namespace WorkHive.APIs.Wallets.OwnerWallet
{
    public record WithdrawRequest();
    public record WithdrawResponse(string Notification);
    public class OwnerWithdrawEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/owners/{id}/withdraw", async (int id, ISender sender) =>
            {
                var command = new OwnerWithdrawCommand(id);
                var result = await sender.Send(command);
                var response = result.Adapt<WithdrawResponse>();
                return Results.Ok(response);
            })
            .WithName("WithdrawMoney")
            .Produces<WithdrawResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Manager")
            .WithSummary("Withdraw money for an owner")
            .WithDescription("If the owner's balance is greater than 50,000, it will be reset to 0.");
        }
    }
}