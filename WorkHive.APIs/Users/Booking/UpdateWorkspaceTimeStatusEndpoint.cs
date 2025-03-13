using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.WorkspaceTimes;

namespace WorkHive.APIs.Users.Booking;

public record UpdateTimeRequest(string Status, int BookingId);
public record UpdateTimeResponse(string Notification);

public class UpdateWorkspaceTimeStatusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/users/booking/updatetimestatus", async (UpdateTimeRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateTimeCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<UpdateTimeResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateWorkspaceTimes")
        .Produces<UpdateTimeResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Update workspace times")
        .WithTags("Booking")
        .WithDescription("Update workspace times");
    }
}
