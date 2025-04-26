using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Admins.BanStaff;
using WorkHive.Services.Admins.BanOwner;

namespace WorkHive.APIs.Admins.BanOwner;

public record BanOwnerResponse(string Notification, int? IsBan);

public class BanOwnerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/owners/banowner/{ownerId}", async (int ownerId, ISender sender) =>
        {
            var result = await sender.Send(new BanOwnerCommand(ownerId));

            var response = result.Adapt<BanOwnerResponse>();

            return Results.Ok(response);
        })
        .WithName("Ban owner")
        .Produces<BanOwnerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Ban owner")
        .WithTags("Moderate account")
        .RequireAuthorization("Admin")
        .WithDescription("Ban owner");
    }
}
