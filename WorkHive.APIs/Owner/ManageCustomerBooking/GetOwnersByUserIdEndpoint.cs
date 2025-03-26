using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageCustomerBooking;

namespace WorkHive.APIs.Owner.ManageCustomerBooking
{
    public record GetOwnersByUserIdResponse(List<GetOwnersByUserIdResult> Owners);

    public class GetOwnersByUserIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owners/user/{userId}", async (int userId, ISender sender) =>
            {
                var query = new GetOwnersByUserIdQuery(userId);
                var result = await sender.Send(query);

                if (result == null || result.Count == 0)
                {
                    return Results.Json(Array.Empty<GetOwnersByUserIdResponse>());
                }

                return Results.Ok(new GetOwnersByUserIdResponse(result));
            })
            .WithName("GetOwnersByUserId")
            .Produces<GetOwnersByUserIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Owner")
            .WithSummary("Get Owners by User ID")
            .WithDescription("Retrieve a list of workspace owners that the specified user has booked.");
        }
    }
}