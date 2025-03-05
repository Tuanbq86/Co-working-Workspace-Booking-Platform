using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Users.DTOs;
using WorkHive.Services.Users.GetUser;

namespace WorkHive.APIs.Users.GetUser;

public record GetUserByIdResponse(UserDTO User);

public class GetUserByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{Id}", async(int Id, ISender sender) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(Id));

            var response = result.Adapt<GetUserByIdResponse>();

            return Results.Ok(response);
        })
        .WithName("GetUserById")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get User By Id")
        .WithDescription("Get User By Id");
    }
}
