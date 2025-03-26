using Carter;
using MediatR;
using WorkHive.Services.Owners.ManageCustomerBooking;

namespace WorkHive.APIs.Owner.ManageCustomerBooking
{
    public record GetUsersByOwnerIdResponse(List<GetUsersByOwnerIdResult> Users);

    public class GetUsersByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/users/owner/{ownerId}", async (int ownerId, ISender sender) =>
            {
                var query = new GetUsersByOwnerIdQuery(ownerId);
                var result = await sender.Send(query);

                if (result == null || result.Count == 0)
                {
                    return Results.Json(Array.Empty<GetUsersByOwnerIdResponse>());
                }

                return Results.Ok(new GetUsersByOwnerIdResponse(result));
            })
            .WithName("GetUsersByOwnerId")
            .Produces<GetUsersByOwnerIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("User")
            .WithSummary("Get Users by Owner ID")
            .WithDescription("Retrieve a list of users who have booked workspaces owned by the specified owner.");
        }
    }
}