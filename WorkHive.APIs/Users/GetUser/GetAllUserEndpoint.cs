using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using WorkHive.APIs.Users.RegisterUser;
using WorkHive.Services.Users.DTOs;
using WorkHive.Services.Users.GetUser;

namespace WorkHive.APIs.Users.GetUser;

public record GetAllUserResponse(List<UserDTO> Users);

public class GetAllUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllUserQuery());

            var response = result.Adapt<GetAllUserResponse>();

            return Results.Ok(response);
        })
        .WithName("GetAllUser")
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get All User")
        .WithDescription("Get All User");
    }
}
