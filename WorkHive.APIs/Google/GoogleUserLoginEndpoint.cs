using Carter;
using MediatR;
using WorkHive.Services.Google_Login;

namespace WorkHive.APIs.Google
{
    /// <summary>
    /// 
    /// 
    public class GoogleUserLoginEndpoint : ICarterModule
    {
        public record GoogleLoginRequest(string IdToken);
        public record GoogleLoginResponse(string Token, object User);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/user/google-login", async (GoogleLoginRequest request, ISender sender) =>
            {
                var command = new GoogleUserLoginCommand(request.IdToken);
                var result = await sender.Send(command);
                return Results.Ok(new GoogleLoginResponse(result.Token, result.User));
            })
            .WithName("UserGoogleLogin")
            .Produces<GoogleLoginResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Auth")
            .WithSummary("Login user with Google")
            .WithDescription("Authenticate a User using Google OAuth.");
        }
    }
}

