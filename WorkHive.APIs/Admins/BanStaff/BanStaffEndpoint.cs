using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Admins.BanCustomer;
using WorkHive.Services.Admins.BanStaff;

namespace WorkHive.APIs.Admins.BanStaff;

public record BanStaffResponse(string Notification, int? IsBan);

public class BanStaffEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/banstaff/{StaffId}", async (int StaffId, ISender sender) =>
        {
            var result = await sender.Send(new BanStaffCommand(StaffId));

            var response = result.Adapt<BanStaffResponse>();

            return Results.Ok(response);
        })
        .WithName("Ban staff")
        .Produces<BanStaffResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Ban staff")
        .WithTags("Moderate account")
        .WithDescription("Ban staff");
    }
}
