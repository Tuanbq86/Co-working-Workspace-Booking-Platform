using Carter;
using MediatR;
using WorkHive.Services.Owners.Google_Login;

namespace WorkHive.APIs.Owner.Google
{
    public class GoogleLoginEndpoint : ICarterModule
    {
        public record GoogleLoginRequest(string IdToken);
        public record GoogleLoginResponse(string Token, object User);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/google-login", async (GoogleLoginRequest request, ISender sender) =>
            {
                var command = new GoogleLoginCommand(request.IdToken);
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("GoogleLogin")
            .Produces<GoogleLoginResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Auth")
            .WithSummary("Login with Google")
            .WithDescription("Authenticate a WorkspaceOwner using Google OAuth.");
        }
    }
}
