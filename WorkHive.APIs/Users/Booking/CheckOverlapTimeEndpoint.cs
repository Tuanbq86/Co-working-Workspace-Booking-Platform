using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.WorkspaceTimes;

namespace WorkHive.APIs.Users.Booking;

public record CheckTimesRequest(int WorkspaceId, string StartDate, string EndDate);
public record CheckTimesResponse(string Notification);

public class CheckOverlapTimeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("users/booking/checktimesoverlap", async (CheckTimesRequest request, ISender sender) =>
        {
            var command = request.Adapt<CheckTimesCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CheckTimesResponse>();

            return Results.Ok(response.Notification);
        })
        .WithName("Check overlap times")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("CheckOverLapTimes")
        .WithDescription("CheckOverLapTimes");
    }
}
