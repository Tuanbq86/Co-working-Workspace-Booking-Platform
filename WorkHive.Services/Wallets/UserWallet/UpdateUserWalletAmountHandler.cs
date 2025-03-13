using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant.Wallet;

namespace WorkHive.Services.Wallets.UserWallet;

public record UpdateUserWalletAmountCommand(int CustomerWalletId, string Status, int Amount)
    : ICommand<UpdateUserWalletAmountResult>;
public record UpdateUserWalletAmountResult(string Notification);

public class UpdateUserWalletAmountHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<UpdateUserWalletAmountCommand, UpdateUserWalletAmountResult>
{
    public async Task<UpdateUserWalletAmountResult> Handle(UpdateUserWalletAmountCommand command, 
        CancellationToken cancellationToken)
    {
        if (command.Status.Equals(DepositStatus.PAID.ToString()))
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
                Status = command.Status.ToString(),
                Description = "Giao dịch thành công",
                CreatedAt = DateTime.Now
            };
            await userUnit.TransactionHistory.CreateAsync(transactionHistory);

            var userTransactionHistory = new UserTransactionHistory
            {
                Status = command.Status.ToString(),
                TransactionHistoryId = transactionHistory.Id,
                CustomerWalletId = command.CustomerWalletId
            };
            await userUnit.UserTransactionHistory.CreateAsync(userTransactionHistory);
            return new UpdateUserWalletAmountResult("Cập nhật thành công");
        }
        else
        {
            //Create Transaction History
            var transactionHistory = new TransactionHistory
            {
                Amount = command.Amount,
                Status = command.Status.ToString(),
                Description = "Giao dịch không thành công",
                CreatedAt = DateTime.Now
            };
            await userUnit.TransactionHistory.CreateAsync(transactionHistory);

            var userTransactionHistory = new UserTransactionHistory
            {
                Status = command.Status.ToString(),
                TransactionHistoryId = transactionHistory.Id,
                CustomerWalletId = command.CustomerWalletId
            };
            await userUnit.UserTransactionHistory.CreateAsync(userTransactionHistory);
            return new UpdateUserWalletAmountResult("Cập nhật không thành công");
        }
    }
}
