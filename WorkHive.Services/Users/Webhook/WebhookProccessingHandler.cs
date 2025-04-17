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
    IBookingWorkspaceUnitOfWork bookUnit, IUserUnitOfWork userUnit)
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
            var webHookData = payOS.verifyPaymentWebhookData(command.WebhookData);

            //if (data.code != "00") return Unit.Value;

            // Phân loại nghiệp vụ dựa trên description
            if (command.WebhookData.data.description.Contains("bookpayment"))
            {
                PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation
                (command.WebhookData.data.orderCode);
                var Status = paymentLinkInformation.status.ToString();

                await HandleBookingPayment(command.WebhookData, Status);
            }
            else if (command.WebhookData.data.description.Contains("depopayment"))
            {
                PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation
                (command.WebhookData.data.orderCode);
                var Status = paymentLinkInformation.status.ToString();
                await HandleDepositPayment(command.WebhookData, Status);
            }

            return Unit.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return Unit.Value;
    }

    private async Task HandleBookingPayment(WebhookType data, string status)
    {
        // Tách bookingId từ orderCode (bookingId + 6 chữ số timestamp)
        var orderCodeStr = data.data.orderCode.ToString();
        var bookingIdStr = orderCodeStr.Length <= 6
            ? orderCodeStr
            : orderCodeStr[..^6]; // Bỏ 6 chữ số cuối
        var bookingId = int.Parse(bookingIdStr);

        var booking = await bookUnit.booking.GetByIdAsync(bookingId);
        if (booking == null) return;

        if (status == "PAID")
        {
            // Xử lý booking thành công
            booking.Status = BookingStatus.Success.ToString();
            await bookUnit.booking.UpdateAsync(booking);

            // Cập nhật WorkspaceTime
            var workspaceTime = bookUnit.workspaceTime.GetAll()
                .FirstOrDefault(wt => wt.BookingId == bookingId);
            if (workspaceTime != null)
            {
                workspaceTime.Status = WorkspaceTimeStatus.InUse.ToString();
                await bookUnit.workspaceTime.UpdateAsync(workspaceTime);
            }

            // Xử lý ví owner (90% số tiền)
            var workspace = bookUnit.workspace.GetById(booking.WorkspaceId);
            var ownerWallet = await bookUnit.ownerWallet.GetOwnerWalletByOwnerIdForBooking(workspace.OwnerId);
            var wallet = userUnit.Wallet.GetById(ownerWallet.WalletId);
            wallet.Balance += (int)(data.data.amount * 0.9m);
            await userUnit.Wallet.UpdateAsync(wallet);
        }
        else
        {
            // Xử lý booking thất bại
            booking.Status = BookingStatus.Fail.ToString();
            await bookUnit.booking.UpdateAsync(booking);

            // Rollback amenities (nếu cần)
            var amenities = bookUnit.bookAmenity.GetAll().Where(ba => ba.BookingId == bookingId);
            foreach (var amenity in amenities)
            {
                var amenityEntity = bookUnit.amenity.GetById(amenity.AmenityId);
                amenityEntity.Quantity += amenity.Quantity;
                await bookUnit.amenity.UpdateAsync(amenityEntity);
            }

            // Xóa WorkspaceTime
            var workspaceTime = bookUnit.workspaceTime.GetAll()
                .FirstOrDefault(wt => wt.BookingId == bookingId);
            if (workspaceTime != null)
            {
                bookUnit.workspaceTime.Remove(workspaceTime);
            }
        }

        await bookUnit.SaveAsync();
    }

    private async Task HandleDepositPayment(WebhookType data, string status)
    {
        // Tách customerWalletId từ orderCode (walletId + 6 chữ số timestamp)
        var orderCodeStr = data.data.orderCode.ToString();
        var walletIdStr = orderCodeStr.Length <= 6
            ? orderCodeStr
            : orderCodeStr[..^6]; // Bỏ 6 chữ số cuối
        var customerWalletId = int.Parse(walletIdStr);

        var customerWallet = userUnit.CustomerWallet.GetById(customerWalletId);
        if (customerWallet == null) return;

        if (status == "PAID")
        {
            // Cộng tiền vào ví
            var wallet = userUnit.Wallet.GetById(customerWallet.WalletId);
            wallet.Balance += data.data.amount;
            await userUnit.Wallet.UpdateAsync(wallet);

            // Ghi log lịch sử giao dịch
            var transaction = new TransactionHistory
            {
                Amount = data.data.amount,
                Status = "PAID",
                Description = "Nạp tiền vào ví",
                CreatedAt = DateTime.Now
            };
            await userUnit.TransactionHistory.CreateAsync(transaction);

            var userTransaction = new UserTransactionHistory
            {
                Status = "PAID",
                TransactionHistoryId = transaction.Id,
                CustomerWalletId = customerWalletId
            };
            await userUnit.UserTransactionHistory.CreateAsync(userTransaction);
        }

        await userUnit.SaveAsync();
    }
}