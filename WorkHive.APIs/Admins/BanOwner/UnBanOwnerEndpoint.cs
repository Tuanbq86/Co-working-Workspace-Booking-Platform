using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Admins.BanOwner;

namespace WorkHive.APIs.Admins.BanOwner;

public record UnBanOwnerResponse(string Notification, int? IsBan);

public class UnBanOwnerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/owners/unbanowner/{ownerId}", async (int ownerId, ISender sender) =>
        {
            var result = await sender.Send(new UnBanOwnerCommand(ownerId));

            var response = result.Adapt<UnBanOwnerResponse>();

            return Results.Ok(response);
        })
        .WithName("UnBan owner")
        .Produces<UnBanOwnerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("UnBan owner")
        .WithTags("Moderate account")
        //.RequireAuthorization("Admin")
        .WithDescription("UnBan owner");
    }
}