using Microsoft.Extensions.Configuration;
using Net.payOS.Types;
using Net.payOS;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant.Wallet;
using WorkHive.Services.Constant;

namespace WorkHive.Services.Wallets.UserWallet;

public record UpdateUserWalletAmountCommand(int CustomerWalletId, long OrderCode, int Amount)
    : ICommand<UpdateUserWalletAmountResult>;
public record UpdateUserWalletAmountResult(string Notification);

public class UpdateUserWalletAmountHandler(IUserUnitOfWork userUnit, IConfiguration configuration)
    : ICommandHandler<UpdateUserWalletAmountCommand, UpdateUserWalletAmountResult>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["PayOS:CheckSumKey"]!;
    public async Task<UpdateUserWalletAmountResult> Handle(UpdateUserWalletAmountCommand command, 
        CancellationToken cancellationToken)
    {
        PayOS payOS = new PayOS(ClientID, ApiKey, CheckSumKey);

        PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation(command.OrderCode);

        var Status = paymentLinkInformation.status.ToString();

        if (Status.Equals(PayOSStatus.PAID.ToString()))
        {
            //Update amount in wallet
            var customerWallet = userUnit.CustomerWallet.GetById(command.CustomerWalletId);
            var wallet = userUnit.Wallet.GetById(customerWallet.WalletId);
            wallet.Balance += command.Amount;
            await userUnit.Wallet.UpdateAsync(wallet);

            //Create Transaction History
            var transactionHistory = new TransactionHistory
            {
                Amount = command.Amount,
                Status = Status.ToString(),
                Description = "Nạp tiền",
                CreatedAt = DateTime.Now
            };
            await userUnit.TransactionHistory.CreateAsync(transactionHistory);

            var userTransactionHistory = new UserTransactionHistory
            {
                Status = Status.ToString(),
                TransactionHistoryId = transactionHistory.Id,
                CustomerWalletId = command.CustomerWalletId
            };
            await userUnit.UserTransactionHistory.CreateAsync(userTransactionHistory);

            //Thông báo
            var userNotifi = new UserNotification
            {
                UserId = customerWallet.UserId,
                IsRead = 0,
                CreatedAt = DateTime.Now,
                Description = $"Nạp thành công số tiền: {command.Amount}₫ vào ví người dùng",
                Status = "PAID"
            };
            await userUnit.UserNotification.CreateAsync(userNotifi);

            return new UpdateUserWalletAmountResult("Cập nhật thành công");
        }
        return new UpdateUserWalletAmountResult("Cập nhật thành công");
        //else
        //{
        //    //Create Transaction History
        //    var transactionHistory = new TransactionHistory
        //    {
        //        Amount = command.Amount,
        //        Status = Status.ToString(),
        //        Description = "Giao dịch thất bại",
        //        CreatedAt = DateTime.Now
        //    };
        //    await userUnit.TransactionHistory.CreateAsync(transactionHistory);

        //    var userTransactionHistory = new UserTransactionHistory
        //    {
        //        Status = Status.ToString(),
        //        TransactionHistoryId = transactionHistory.Id,
        //        CustomerWalletId = command.CustomerWalletId
        //    };
        //    await userUnit.UserTransactionHistory.CreateAsync(userTransactionHistory);
        //    return new UpdateUserWalletAmountResult("Cập nhật không thành công");
        //}
    }
}
