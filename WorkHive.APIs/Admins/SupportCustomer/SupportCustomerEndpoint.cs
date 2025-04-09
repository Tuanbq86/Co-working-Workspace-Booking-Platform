using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Admins.BanStaff;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Services.Admins.SupportCustomer;

namespace WorkHive.APIs.Admins.SupportCustomer;

public record SupportCustomerRequest(string Name, string Email, string Phone, string Message);
public record SupportCustomerResponse(string Notification);

public class SupportCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/supportuser", async (SupportCustomerRequest request, ISender sender) =>
        {
            var command = request.Adapt<SupportCustomerCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<SupportCustomerResponse>();

            return Results.Ok(response);
        })
        .WithName("Support Customer")
        .Produces<SupportCustomerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Support Customer")
        .WithTags("User")
        .WithDescription("Support Customer");
    }
}
