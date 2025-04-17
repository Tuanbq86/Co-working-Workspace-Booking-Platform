using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.Base_Owner;

namespace WorkHive.APIs.Owner.Manage_Owner
{
    public record GetOwnerVerifyRequestResponse(
            int Id,
            int OwnerId,
            int? UserId,
            string Message,
            string Status,
            string GoogleMapUrl,
            string LicenseName,
            string LicenseNumber,
            string LicenseAddress,
            decimal? CharterCapital,
            string LicenseFile,
            string OwnerName,
            DateOnly? RegistrationDate,
            string Facebook,
            string Instagram,
            string Tiktok,
            DateTime? CreatedAt,
            DateTime? UpdatedAt
        );

    public class GetOwnerVerifyRequestsByOwnerIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/owner-verify-requests/owners/{ownerId}", async (int ownerId, ISender sender) =>
            {
                var query = new GetOwnerVerifyRequestsByOwnerIdQuery(ownerId);
                var result = await sender.Send(query);

                var response = result.Select(r => r.Adapt<GetOwnerVerifyRequestResponse>());
                return Results.Ok(response);
            })
            .WithName("GetOwnerVerifyRequestsByOwnerId")
            .Produces<IEnumerable<GetOwnerVerifyRequestResponse>>(StatusCodes.Status200OK)
            .WithTags("Owner")
            .WithSummary("Get all verify requests by Owner ID")
            .WithDescription("Retrieve all owner verification requests by specific owner ID.");
        }
    }
}
