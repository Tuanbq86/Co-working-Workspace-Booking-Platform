using Carter;
using MediatR;
using WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest;

namespace WorkHive.APIs.Managers.VerifyOwnerWithdrawalRequest
{
    public record GetOwnerWithdrawalRequestByIdResponse(OwnerWithdrawalRequestDTO? Request);

public class GetOwnerWithdrawalRequestByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/owner-withdrawal-requests/{id:int}", async (int id, ISender sender) =>
        {
            var query = new GetOwnerWithdrawalRequestByIdQuery(id);
            var result = await sender.Send(query);
            return Results.Ok(new GetOwnerWithdrawalRequestByIdResponse(result));
        })
        .WithName("GetOwnerWithdrawalRequestById")
        .Produces<GetOwnerWithdrawalRequestByIdResponse>(StatusCodes.Status200OK)
        .WithTags("Manager")
        .WithSummary("Get owner withdrawal request by ID")
        .WithDescription("Retrieve a specific withdrawal request using its ID.");
    }
}
}
