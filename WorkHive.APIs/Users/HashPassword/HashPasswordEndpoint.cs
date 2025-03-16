using Carter;
using Mapster;
using MediatR;
using WorkHive.APIs.Users.LoginUser;
using WorkHive.Services.Users.HashPassword;

namespace WorkHive.APIs.Users.HashPassword;

public record HashPasswordRequest(string Password);
public record HashPasswordResponse(string HashPassword);

public class HashPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("users/hashpassword", async(HashPasswordRequest request, ISender sender) =>
        {
            var command = request.Adapt<HashPasswordCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<HashPasswordResponse>();

            return Results.Ok(response);
        })
        .WithName("Hash Password")
        .Produces<HashPasswordResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Hash Password")
        .WithTags("Register User")
        .WithDescription("Hash Password");
    }
}
