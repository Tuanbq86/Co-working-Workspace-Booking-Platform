using Carter;
using MediatR;
using WorkHive.Services.Owners.Notification;
using WorkHive.Services.Users.NotificationForUser;

namespace WorkHive.APIs.Owner.Notification
{
    public record GetListNotificationByOwnerIdResponse(List<OwnerNotificationDTO> OwnerNotificationDTOs);

    public class GetListNotificationByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/Owners/Ownernotification/{OwnerId}", async (int OwnerId, ISender sender) =>
            {
                var result = await sender.Send(new GetListNotificationByOwnerIdQuery(OwnerId));

                var response = new GetListNotificationByOwnerIdResponse(result.OwnerNotificationDTOs);

                return Results.Ok(response);
            })
            .WithName("GetListNotificationByOwnerId")
            .Produces<GetListNotificationByOwnerIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetListNotificationByOwnerId")
            .WithTags("Owner notification")
            .RequireAuthorization("Owner")
            .WithDescription("GetListNotificationByOwnerId");
        }
    }
}
