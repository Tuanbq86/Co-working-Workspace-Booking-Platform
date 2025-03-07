using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Data.Models;
using WorkHive.Services.WorkspaceTimes;

namespace WorkHive.APIs.Users.Booking;

public record WorkspaceTimesRequest(int WorkspaceId);
public record WorkspaceTimesResponse(List<WorkspaceTime> WorkspaceTimes);
public class GetUnavailableWorkspaceTimesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("users/booking/workspacetimes", async ([AsParameters] WorkspaceTimesRequest request, ISender sender) =>
        {
            var query = request.Adapt<WorkspaceTimesQuery>();

            var result = await sender.Send(query);

            var response = result.Adapt<WorkspaceTimesResponse>();

            return Results.Ok(response);
        })
        .WithName("GetWorkspaceTimes")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get workspace times")
        .WithTags("Booking")
        .WithDescription("Get workspace times");
    }
}
