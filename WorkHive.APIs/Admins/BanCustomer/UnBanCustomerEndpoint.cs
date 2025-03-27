using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Admins.BanCustomer;

namespace WorkHive.APIs.Admins.BanCustomer;

public record UnBanCusomerResponse(string Notification, int? IsBan);

public class UnBanCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/unbancustomer/{CustomerId}", async (int CustomerId, ISender sender) =>
        {
            var result = await sender.Send(new UnBanCustomerCommand(CustomerId));

            var response = result.Adapt<UnBanCusomerResponse>();

            return Results.Ok(response);
        })
        .WithName("UnBan customer")
        .Produces<UnBanCusomerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("UnBan customer")
        .WithTags("Moderate account")
        .WithDescription("UnBan customer");
    }
}
