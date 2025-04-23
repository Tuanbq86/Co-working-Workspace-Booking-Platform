using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Users.NotificationMobile;

namespace WorkHive.APIs.Users.NotificationMobile;

public record SendNotificationForMobileRequest(string FcmToken, string Title, string Body);
public record SendNotificationForMobileResponse(string Notification);

public class SendNotificationForMobileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/sendnotificationformobile", async (SendNotificationForMobileRequest request, ISender sender) =>
        {
            var command = request.Adapt<SendNotificationForMobileCommand>();

            var result = await sender.Send(command);

            var response = new SendNotificationForMobileResponse(result.Notification);

            return Results.Ok(response);
        })
        .WithName("Send notification for mobile app")
        .Produces<SendNotificationForMobileResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Send notification for mobile app")
        .WithTags("User notification")
        .WithDescription("Send notification for mobile app");
    }
}

