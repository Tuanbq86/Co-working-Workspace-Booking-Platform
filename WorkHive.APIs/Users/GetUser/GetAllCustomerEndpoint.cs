using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Users.DTOs;
using WorkHive.Services.Users.GetUser;

namespace WorkHive.APIs.Users.GetUser;

public record GetAllCustomerResponse(List<UserDTO> Customers);
public class GetAllCustomerEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/customers", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllCustomerQuery());

            var response = result.Adapt<GetAllCustomerResponse>();

            return Results.Ok(response);
        })
        .WithName("GetAllCustomer")
        .Produces<GetAllCustomerResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get All Customer")
        .WithTags("Get All Customer")
        .RequireAuthorization("Manager")
        .WithDescription("Get All Customer");
    }
}
