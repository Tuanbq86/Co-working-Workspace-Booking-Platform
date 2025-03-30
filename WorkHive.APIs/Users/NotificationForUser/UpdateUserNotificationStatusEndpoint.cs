using Carter;
using MediatR;
using WorkHive.Services.Users.NotificationForUser;

namespace WorkHive.APIs.Users.UserNotification;

public record UpdateUserNotificationStatusResponse(string Notification, int IsRead);

public class UpdateUserNotificationStatusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/updateusernotification/{UserNotificationId}", async (int UserNotificationId, ISender sender) =>
        {
            var result = await sender.Send(new UpdateUserNotificationStatusCommand(UserNotificationId));

            var response = new UpdateUserNotificationStatusResponse(result.Notification, result.IsRead);

            return Results.Ok(response);
        })
        .WithName("UpdateUserNotificationStatus")
        .Produces<UpdateUserNotificationStatusResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("UpdateUserNotificationStatus")
        .WithTags("User notification")
        .WithDescription("UpdateUserNotificationStatus");
    }
}
