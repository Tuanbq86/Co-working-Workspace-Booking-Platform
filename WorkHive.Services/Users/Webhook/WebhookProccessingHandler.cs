using MediatR;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Wallets.UserWallet;
using WorkHive.Services.WorkspaceTimes;

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
        var payOS = new PayOS(ClientID, ApiKey, CheckSumKey);

        payOS.verifyPaymentWebhookData(command.WebhookData);
        var description = command.WebhookData.data.description;

        PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation(command.WebhookData.data.orderCode);
        var Status = paymentLinkInformation.status.ToString();

        //Xử lý cho api Booking bằng ví PayOS
        if (description.Trim().Equals("bookingpayment"))
        {
            if (!(Status.Equals(PayOSStatus.PAID.ToString())))
            {
                // Lấy phần đầu của orderCode (bookingId)
                var bookingId = command.WebhookData.data.orderCode / 1_000_000;
                var workspaceTime = bookUnit.workspaceTime.GetAll()
                    .FirstOrDefault(x => x.BookingId.Equals(bookingId));

                bookUnit.workspaceTime.Remove(workspaceTime!);

                var booking = bookUnit.booking.GetById((int)bookingId);

                booking.Status = BookingStatus.Fail.ToString();

                bookUnit.booking.Update(booking);

                await bookUnit.SaveAsync();
            }

            //Pay successfully
            if (Status.Equals(PayOSStatus.PAID.ToString()))
            {
                var bookingId = command.WebhookData.data.orderCode / 1_000_000;
                var workspaceTime = bookUnit.workspaceTime.GetAll()
                    .FirstOrDefault(x => x.BookingId.Equals(bookingId));

                var booking = bookUnit.booking.GetById((int)bookingId);

                booking.Status = BookingStatus.Success.ToString();
                workspaceTime!.Status = WorkspaceTimeStatus.InUse.ToString();

                bookUnit.booking.Update(booking);
                bookUnit.workspaceTime.Update(workspaceTime);

                await bookUnit.SaveAsync();
            }
        }

        //Xử lý cho api User deposit
        if (description.Trim().Equals("userdeposit"))
        {
            if (Status.Equals(PayOSStatus.PAID.ToString()))
            {
                var CustomerWalletId = command.WebhookData.data.orderCode / 1_000_000;
                //Update amount in wallet
                var customerWallet = userUnit.CustomerWallet.GetById((int)CustomerWalletId);
                var wallet = userUnit.Wallet.GetById(customerWallet.WalletId);
                wallet.Balance += command.WebhookData.data.amount;
                await userUnit.Wallet.UpdateAsync(wallet);

                //Create Transaction History
                var transactionHistory = new TransactionHistory
                {
                    Amount = command.WebhookData.data.amount,
                    Status = Status.ToString(),
                    Description = "Nạp tiền",
                    CreatedAt = DateTime.Now
                };
                await userUnit.TransactionHistory.CreateAsync(transactionHistory);

                var userTransactionHistory = new UserTransactionHistory
                {
                    Status = Status.ToString(),
                    TransactionHistoryId = transactionHistory.Id,
                    CustomerWalletId = (int)CustomerWalletId
                };
                await userUnit.UserTransactionHistory.CreateAsync(userTransactionHistory);
            }
        }

        return Unit.Value;
    }
}
