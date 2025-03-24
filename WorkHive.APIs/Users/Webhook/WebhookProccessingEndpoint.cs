using Carter;
using Mapster;
using MediatR;
using Net.payOS.Types;
using WorkHive.Services.Users.Webhook;

namespace WorkHive.APIs.Users.Webhook;

public record ProcessWebhookRequest(WebhookType WebhookData);
public class WebhookProccessingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/webhook", async (ProcessWebhookRequest request, ISender sender) =>
        {
            var command = request.Adapt<ProcessWebhookCommand>();

            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .WithName("Call Webhook")
        .WithSummary("Call Webhook")
        .WithTags("Webhook")
        .WithDescription("Call Webhook");
    }
}
