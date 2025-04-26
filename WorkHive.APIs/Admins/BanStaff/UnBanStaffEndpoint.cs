using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Admins.BanStaff;

namespace WorkHive.APIs.Admins.BanStaff;

public record UnBanStaffResponse(string Notification, int? IsBan);

public class UnBanStaffEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/unbanstaff/{StaffId}", async (int StaffId, ISender sender) =>
        {
            var result = await sender.Send(new UnBanStaffCommand(StaffId));

            var response = result.Adapt<UnBanStaffResponse>();

            return Results.Ok(response);
        })
        .WithName("UnBan staff")
        .Produces<UnBanStaffResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("UnBan staff")
        .WithTags("Moderate account")
        .RequireAuthorization("Admin")
        .WithDescription("UnBan staff");
    }
}
