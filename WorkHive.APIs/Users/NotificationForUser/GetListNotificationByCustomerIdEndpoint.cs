using Carter;
using MediatR;
using WorkHive.Services.Users.NotificationForUser;

namespace WorkHive.APIs.Users.NotificationForUser;
public record GetListNotificationByCustomerIdResponse(List<CustomerNotificationDTO> CustomerNotificationDTOs);

public class GetListNotificationByCustomerIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/usernotification/{CustomerId}", async(int CustomerId, ISender sender) =>
        {
            var result = await sender.Send(new GetListNotificationByCustomerIdQuery(CustomerId));

            var response = new GetListNotificationByCustomerIdResponse(result.CustomerNotificationDTOs);

            return Results.Ok(response);
        })
        .WithName("GetListNotificationByCustomerId")
        .Produces<GetListNotificationByCustomerIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("GetListNotificationByCustomerId")
        .WithTags("User notification")
        .WithDescription("GetListNotificationByCustomerId");
    }
}
