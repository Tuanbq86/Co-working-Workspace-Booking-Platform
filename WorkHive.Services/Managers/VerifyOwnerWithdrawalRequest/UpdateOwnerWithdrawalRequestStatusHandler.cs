using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest
{
    public record UpdateOwnerWithdrawalRequestStatusCommand(int Id, int UserId, string ManagerResponse, string Status)
        : ICommand<UpdateOwnerWithdrawalRequestStatusResult>;

    public record UpdateOwnerWithdrawalRequestStatusResult(string Notification);

    class UpdateOwnerWithdrawalRequestStatusHandler(IWalletUnitOfWork unit)
        : ICommandHandler<UpdateOwnerWithdrawalRequestStatusCommand, UpdateOwnerWithdrawalRequestStatusResult>
    {
        public async Task<UpdateOwnerWithdrawalRequestStatusResult?> Handle(UpdateOwnerWithdrawalRequestStatusCommand command, CancellationToken cancellationToken)
        {
            var request = await unit.OwnerWithdrawalRequest.GetByIdAsync(command.Id);
            if (request == null) return null;

            // Nếu trạng thái cập nhật là "Success", thực hiện việc rút tiền và đặt về 0
            if (command.Status == "Success")
            {
                var ownerWallet = await unit.OwnerWallet.GetByOwnerIdAsync(request.WorkspaceOwnerId);
                if (ownerWallet == null)
                    return new UpdateOwnerWithdrawalRequestStatusResult("Không tìm thấy chủ sở hữu.");

                var wallet = await unit.Wallet.GetByIdAsync(ownerWallet.WalletId);
                if (wallet == null)
                    return new UpdateOwnerWithdrawalRequestStatusResult("Không tìm thấy ví.");

                if (wallet.Balance == null || wallet.Balance <= 0)
                    return new UpdateOwnerWithdrawalRequestStatusResult("Ví không có tiền để rút.");

                decimal withdrawAmount = wallet.Balance.Value;
                wallet.Balance = 0;
                                        
                // Ghi lại giao dịch
                var transaction = new TransactionHistory
                {
                    Amount = withdrawAmount,
                    Status = "Withdraw Success",
                    Description = "Withdraw from owner wallet",
                    CreatedAt = DateTime.UtcNow,
                    BankAccountName = ownerWallet.BankAccountName,
                    BankName = ownerWallet.BankName,
                    BankNumber = ownerWallet.BankNumber

                };

                await unit.TransactionHistory.CreateAsync(transaction);
                await unit.SaveAsync();

                var ownerTransactionHistory = new OwnerTransactionHistory
                {
                    OwnerWalletId = ownerWallet.Id,
                    TransactionHistoryId = transaction.Id,
                    Status = "Withdraw Success"
                };

                var ownerNotification = new OwnerNotification
                {
                    Description = $"Yêu cầu rút tiền đã được phê duyệt",
                    Status = "Active",
                    OwnerId = ownerWallet.OwnerId,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = 0,
                    Title = "Yêu cầu rút tiền"
                };

                await unit.OwnerNotification.CreateAsync(ownerNotification);

                await unit.OwnerTransactionHistory.CreateAsync(ownerTransactionHistory);
                await unit.Wallet.UpdateAsync(wallet);
                await unit.SaveAsync();
            }

            // Cập nhật trạng thái yêu cầu rút tiền
            request.Status = command.Status;
            request.UserId = command.UserId;
            request.ManagerResponse = command.ManagerResponse;
            await unit.OwnerWithdrawalRequest.UpdateAsync(request);
            await unit.SaveAsync();

            return new UpdateOwnerWithdrawalRequestStatusResult($"Yêu cầu rút tiền đã được cập nhật thành '{command.Status}'");
        }
    }
}