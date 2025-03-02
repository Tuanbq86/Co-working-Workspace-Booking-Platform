using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Data.Models;
using WorkHive.Services.Users.Beverages;

namespace WorkHive.APIs.Users.Beverages;

public record GetBeveragesByWorkspaceIdResponse(List<Beverage> Beverages);

public class GetBeveragesByWorkspaceIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/beverages/workspaces/{Id}", async (int Id, ISender sender) =>
        {
            var result = await sender.Send(new GetBeveragesByWorkspaceIdQuery(Id));

            var response = result.Adapt<GetBeveragesByWorkspaceIdResponse>();

            return Results.Ok(response);
        })
        .WithName("Get Beverages By WorkspaceId")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("GetBeveragesByWorkspaceId")
        .WithDescription("GetBeveragesByWorkspaceId");
    }
}
