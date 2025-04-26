using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Admins.BanCustomer;

namespace WorkHive.APIs.Admins.BanCustomer;

public record BanCusomerResponse(string Notification, int? IsBan);

public class BanCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/bancustomer/{CustomerId}", async (int CustomerId, ISender sender) =>
        {
            var result = await sender.Send(new BanCustomerCommand(CustomerId));

            var response = result.Adapt<BanCusomerResponse>();

            return Results.Ok(response);
        })
        .WithName("Ban customer")
        .Produces<BanCusomerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Ban customer")
        .WithTags("Moderate account")
        .RequireAuthorization("Admin")
        .WithDescription("Ban customer");
    }
}
