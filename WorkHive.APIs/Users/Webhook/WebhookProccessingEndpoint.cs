using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using WorkHive.Services.Users.Webhook;

namespace WorkHive.APIs.Users.Webhook;

public record ProcessWebhookResponse(string Notification);

public class WebhookProccessingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/payos/webhook", async ([FromBody]WebhookType WebhookData, ISender sender) =>
        {
            try
            {
                var result = await sender.Send(new ProcessWebhookCommand(WebhookData));
                var response = new ProcessWebhookResponse(result.Notification);
                return Results.Ok(response);
            }
            catch
            {
                return Results.Ok();
            }
        })
        .WithName("Call Webhook")
        .WithSummary("Call Webhook")
        .WithTags("Webhook")
        .WithDescription("Call Webhook");
    }
}
