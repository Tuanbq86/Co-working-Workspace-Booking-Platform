using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Wallets.Base_OwnerWallet
{
    public record OwnerWithdrawCommand(int OwnerId) : ICommand<OwnerWithdrawResult>;

    public record OwnerWithdrawResult(string Notification);

    public class OwnerWithdrawHandler(IWalletUnitOfWork unit) : ICommandHandler<OwnerWithdrawCommand, OwnerWithdrawResult>
    {
        public async Task<OwnerWithdrawResult> Handle(OwnerWithdrawCommand command, CancellationToken cancellationToken)
        {
            var ownerWallet = await unit.OwnerWallet.GetByOwnerIdAsync (command.OwnerId);
            if (ownerWallet == null) return new OwnerWithdrawResult("Không tìm thấy chủ sở hữu");

            var wallet = await unit.Wallet.GetByIdAsync(ownerWallet.WalletId);
            if (wallet == null) return new OwnerWithdrawResult("Không tìm thấy ví");

            if (wallet.Balance == null || wallet.Balance <= 50000)
                return new OwnerWithdrawResult("Số dư phải lớn hơn hoặc bằng 50000");

            decimal withdrawAmount = wallet.Balance.Value;
            wallet.Balance = 0; 
            //wallet.Status = "Withdrawn"; 

            // Ghi lại giao dịch
            var transaction = new TransactionHistory
            {
                Amount = withdrawAmount,
                Status = "Withdraw Success",
                Description = "Withdraw from owner wallet",// Chỉnh lại cái này sau
                CreatedAt = DateTime.UtcNow
            };

            await unit.TransactionHistory.CreateAsync(transaction);
            await unit.SaveAsync();

            var ownerTransactionHistory = new OwnerTransactionHistory
            {
                OwnerWalletId = ownerWallet.Id,
                TransactionHistoryId = transaction.Id,
                Status = "Withdraw Success"
            };

            await unit.OwnerTransactionHistory.CreateAsync(ownerTransactionHistory);

            await unit.Wallet.UpdateAsync(wallet);
            await unit.SaveAsync();

            return new OwnerWithdrawResult($"Withdraw successful, balance is now 0. Amount withdrawn: {withdrawAmount}");
        }
    }
}
