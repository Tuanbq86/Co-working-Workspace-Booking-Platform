using Azure.Core;
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
    public record CreateOwnerWithdrawalRequestCommand(string Title, string Description, int WorkspaceOwnerId) : ICommand<CreateOwnerWithdrawalRequestResult>;

    public record CreateOwnerWithdrawalRequestResult(string Notification);

    class CreateOwnerWithdrawalRequestHandler(IWalletUnitOfWork unit) : ICommandHandler<CreateOwnerWithdrawalRequestCommand, CreateOwnerWithdrawalRequestResult>
    {
        private const string DefaultStatus = "Handling";

        public async Task<CreateOwnerWithdrawalRequestResult> Handle(CreateOwnerWithdrawalRequestCommand command, CancellationToken cancellationToken)
        {
            var ownerWallet = await unit.OwnerWallet.GetByOwnerIdAsync(command.WorkspaceOwnerId);

            var newRequest = new OwnerWithdrawalRequest
            {
                Title = command.Title,
                Description = command.Description,
                Status = DefaultStatus,
                CreatedAt = DateTime.Now,
                WorkspaceOwnerId = command.WorkspaceOwnerId,
                BankAccountName = ownerWallet.BankAccountName,
                BankName = ownerWallet.BankName,
                BankNumber = ownerWallet.BankNumber,
                Balance = ownerWallet.Wallet.Balance ?? 0
            };

            await unit.OwnerWithdrawalRequest.CreateAsync(newRequest);
            await unit.SaveAsync();

            return new CreateOwnerWithdrawalRequestResult("Owner withdrawal request created successfully");
        }
    }
}