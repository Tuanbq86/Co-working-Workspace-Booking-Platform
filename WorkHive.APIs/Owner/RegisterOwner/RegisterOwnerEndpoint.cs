using Carter;
using Mapster;
using MediatR;
using WorkHive.Services.Owner.RegisterOwner;

namespace WorkHive.APIs.Owner.RegisterOwner;

public record RegisterOwnerRequest(string Email, string Phone, string Password);
public record RegisterOwnerResponse(string Notification);
public class RegisterOwnerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/owners/register", async (RegisterOwnerRequest request, ISender sender) =>
        {
            var command = request.Adapt<RegisterOwnerCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<RegisterOwnerResponse>();

            return Results.Created($"/owners/register", response.Notification);
        })
        .WithName("RegisterOwner")
        .Produces<RegisterOwnerResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Register Owner")
        .WithDescription("Register Owner");
    }
}
