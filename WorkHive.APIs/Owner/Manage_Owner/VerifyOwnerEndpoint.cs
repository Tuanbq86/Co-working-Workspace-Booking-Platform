using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owners.Base_Owner;

namespace WorkHive.APIs.Owner.Manage_Owner
{
    public record VerifyOwnerRequest(
        string IdentityName,
        string IdentityNumber,
        DateOnly? DateOfBirth,
        string Sex,
        string Nationality,
        string PlaceOfOrigin,
        string PlaceOfResidence,
        DateOnly? IdentityExpiredDate,
        DateOnly? IdentityCreatedDate,
        string IdentityFile,
        string LicenseName,
        string LicenseNumber,
        string LicenseAddress,
        decimal? CharterCapital,
        string LicenseFile,
        string Facebook,
        string Instagram,
        string Tiktok);

    public record VerifyOwnerResponse(string Notification);

    public class VerifyOwnerEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/owners/{id}/verify", async (int id, VerifyOwnerRequest request, ISender sender) =>
            {
                var command = request.Adapt<VerifyOwnerCommand>() with { Id = id };
                var result = await sender.Send(command);
                var response = result.Adapt<VerifyOwnerResponse>();
                return Results.Ok(response);
            })
            .WithName("VerifyOwner")
            .Produces<VerifyOwnerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Owner")
            .WithSummary("Verify an owner")
            .WithDescription("Updates owner verification details by ID.");
        }
    }
}

