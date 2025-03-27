using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using System.Text.Json;
using System.Text.RegularExpressions;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;
using WorkHive.Services.Wallets.UserWallet;
using WorkHive.Services.WorkspaceTimes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkHive.Services.Users.Webhook;
public record ProcessWebhookCommand(WebhookType WebhookData) : ICommand<Unit>;

public class WebhookProccessingHandler(IConfiguration configuration, 
    IBookingWorkspaceUnitOfWork bookUnit, IUserUnitOfWork userUnit, ILogger<WebhookProccessingHandler> logger)
    : ICommandHandler<ProcessWebhookCommand, Unit>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["PayOS:CheckSumKey"]!;
    public async Task<Unit> Handle(ProcessWebhookCommand command, 
        CancellationToken cancellationToken)
    {
        try
        {
            var payOS = new PayOS(ClientID, ApiKey, CheckSumKey);

            WebhookData data  = payOS.verifyPaymentWebhookData(command.WebhookData);

            //var description = data.description;
            string depo = "depopayment";
            string book = "bookpayment";

            logger.LogDebug("Thông tin data:" + data.ToString());

            //Xử lý cho api Booking bằng ví PayOS
            if (data.description.EndsWith(book, StringComparison.OrdinalIgnoreCase))
            {
                PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation
                    (data.orderCode);
                var Status = paymentLinkInformation.status.ToString();

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

                    await bookUnit.booking.UpdateAsync(booking);
                    await bookUnit.workspaceTime.UpdateAsync(workspaceTime);

                    //Cộng tiền 90 cho ví owner và ghi lại lịch sử giao dịch cho bên owner
                    var workspace = bookUnit.workspace.GetById(booking.WorkspaceId);
                    var owner = userUnit.Owner.GetById(workspace.OwnerId);

                    var ownerWallet = await bookUnit.ownerWallet.GetOwnerWalletByOwnerIdForBooking(owner.Id);
                    var walletOfOwner = userUnit.Wallet.GetById(ownerWallet.WalletId);
                    walletOfOwner.Balance += (command.WebhookData.data.amount * 90) / 100;

                    await bookUnit.wallet.UpdateAsync(walletOfOwner);

                    //Create Transaction History for owner
                    var transactionHistoryOfOwner = new TransactionHistory
                    {
                        Amount = (data.amount * 90) / 100,
                        Status = "PAID",
                        Description = $"Nhận tiền đơn booking: {bookingId}",
                        CreatedAt = DateTime.Now
                    };
                    await userUnit.TransactionHistory.CreateAsync(transactionHistoryOfOwner);

                    var ownerTransactionHistory = new OwnerTransactionHistory
                    {
                        Status = "PAID",
                        TransactionHistoryId = transactionHistoryOfOwner.Id,
                        OwnerWalletId = ownerWallet.Id
                    };
                    await userUnit.OwnerTransactionHistory.CreateAsync(ownerTransactionHistory);
                }
                //return Unit.Value;
            }

            //Xử lý cho api User deposit
            if (data.description.EndsWith(depo, StringComparison.OrdinalIgnoreCase))
            {
                PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation
                    (data.orderCode);
                var Status = paymentLinkInformation.status.ToString();

                if (Status.Equals(PayOSStatus.PAID.ToString()))
                {
                    var CustomerWalletId = data.orderCode / 1_000_000;
                    //Update amount in wallet
                    var customerWallet = userUnit.CustomerWallet.GetById((int)CustomerWalletId);
                    var wallet = userUnit.Wallet.GetById(customerWallet.WalletId);
                    wallet.Balance += data.amount;
                    await userUnit.Wallet.UpdateAsync(wallet);

                    //Create Transaction History
                    var transactionHistory = new TransactionHistory
                    {
                        Amount = data.amount,
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
                //return Unit.Value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return Unit.Value;
    }
}
