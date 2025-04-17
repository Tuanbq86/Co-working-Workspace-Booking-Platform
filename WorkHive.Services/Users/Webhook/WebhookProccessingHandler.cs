using MediatR;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.Webhook;
public record ProcessWebhookCommand(WebhookType WebhookData) : ICommand<Unit>;
public class WebhookProccessingHandler(IConfiguration configuration, 
    IBookingWorkspaceUnitOfWork bookUnit, IUserUnitOfWork userUnit)
    : ICommandHandler<ProcessWebhookCommand, Unit>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["PayOS:CheckSumKey"]!;
    public async Task<Unit> Handle(ProcessWebhookCommand command, 
        CancellationToken cancellationToken)
    {
        PayOS payOS = new PayOS(ClientID, ApiKey, CheckSumKey);

        var webhookData = payOS.verifyPaymentWebhookData(command.WebhookData);
        var orderCode = webhookData.orderCode;
        var orderCodeString = orderCode.ToString();

        // 1 cho booking, 2 cho deposit
        var typeCode = orderCodeString.Substring(0, 1);
        var timestampPart = orderCodeString.Substring(1, 6);

        if (typeCode == "1")
        {
            //Lấy booking Id
            var bookingIdStr = orderCodeString.Substring(7);
            var bookingId = int.Parse(bookingIdStr);
        }






        return Unit.Value;
    }

    
}