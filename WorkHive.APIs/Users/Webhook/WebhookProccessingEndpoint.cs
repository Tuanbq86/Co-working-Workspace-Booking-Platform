using System.Text.Json;
using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using WorkHive.Services.Users.Webhook;

namespace WorkHive.APIs.Users.Webhook;
//public record ProcessWebhookRequest(WebhookType WebhookData);
public class WebhookProccessingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/webhook", async ([FromBody]WebhookType request, ISender sender) =>
        {
            try
            {
                await sender.Send(new ProcessWebhookCommand(request));
                return Results.Ok();
            }
            catch
            {
                // Log error here if needed
                return Results.Ok();
            }
        })
        .WithName("Call Webhook")
        .WithSummary("Call Webhook")
        .WithTags("Webhook")
        .WithDescription("Call Webhook");
    }
}
