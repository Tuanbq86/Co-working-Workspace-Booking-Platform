using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyOwnerWithdrawalRequest
{
    public record CreateOwnerWithdrawalRequestRequest(string Title, string Description, int WorkspaceOwnerId);

    public record CreateOwnerWithdrawalRequestResponse(string Notification);
    public class CreateOwnerWithdrawalRequestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/owner-withdrawal-requests", async (CreateOwnerWithdrawalRequestRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateOwnerWithdrawalRequestCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateOwnerWithdrawalRequestResponse>();
                return Results.Created($"/owner-withdrawal-requests", response);
            })
            .WithName("CreateOwnerWithdrawalRequest")
            .Produces<CreateOwnerWithdrawalRequestResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Owner")
            .WithSummary("Create a new owner withdrawal request")
            .WithDescription("Creates a new withdrawal request for a workspace owner.");
        }
    }
}