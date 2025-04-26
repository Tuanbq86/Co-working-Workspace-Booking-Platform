using Carter;
using MediatR;
using WorkHive.Services.Owners.Notification;
namespace WorkHive.APIs.Owner.OwnerNotification
{
    public record UpdateOwnerNotificationStatusResponse(string Notification, int IsRead);

    public class UpdateOwnerNotificationStatusEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/Owners/updateOwnernotification/{OwnerNotificationId}", async (int OwnerNotificationId, ISender sender) =>
            {
                var result = await sender.Send(new UpdateOwnerNotificationStatusCommand(OwnerNotificationId));

                var response = new UpdateOwnerNotificationStatusResponse(result.Notification, result.IsRead);

                return Results.Ok(response);
            })
            .WithName("UpdateOwnerNotificationStatus")
            .Produces<UpdateOwnerNotificationStatusResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("UpdateOwnerNotificationStatus")
            .WithTags("Owner notification")
            .WithDescription("UpdateOwnerNotificationStatus");
        }
    }

}
//Owner