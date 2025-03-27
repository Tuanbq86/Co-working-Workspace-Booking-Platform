using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Staff
{
    public record UpdateOwnerStatusCommand(int Id, string Status) : ICommand<UpdateOwnerStatusResult>;

    public record UpdateOwnerStatusResult(string Notification);

    public class UpdateOwnerStatusHandler(IWalletUnitOfWork unit) : ICommandHandler<UpdateOwnerStatusCommand, UpdateOwnerStatusResult>
    {
        public async Task<UpdateOwnerStatusResult> Handle(UpdateOwnerStatusCommand command, CancellationToken cancellationToken)
        {
            var owner = await unit.WorkspaceOwner.GetByIdAsync(command.Id);
            if (owner == null) return new UpdateOwnerStatusResult("Owner not found");

            if (command.Status != "Fail" && command.Status != "Success")
                return new UpdateOwnerStatusResult("Invalid status value. Use 'Fail' or 'Success'.");

            owner.Status = command.Status;
            owner.UpdatedAt = DateTime.UtcNow;
            if (command.Status == "Success")
            {
                var existingWallet = await unit.OwnerWallet.GetByIdAsync(owner.Id);
                if (existingWallet == null)
                {
                    var newWallet = new Wallet
                    {
                        Balance = 0,
                        Status = "Active"
                    };

                    await unit.Wallet.CreateAsync(newWallet);
                    await unit.SaveAsync();

                    var ownerWallet = new OwnerWallet
                    {
                        OwnerId = owner.Id,
                        WalletId = newWallet.Id,
                        Status = "Active"
                    };

                    await unit.OwnerWallet.CreateAsync(ownerWallet);
                    await unit.SaveAsync();
                }
            }

            await unit.WorkspaceOwner.UpdateAsync(owner);
            await unit.SaveAsync();

            return new UpdateOwnerStatusResult($"Owner status updated to {command.Status}");
        }
    }
}
