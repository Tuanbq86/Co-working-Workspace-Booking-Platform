using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Wallets.Base_OwnerWallet
{
    public record UpdateOwnerWalletCommand(int Id, string BankName, string BankNumber, string BankAccountName) : IRequest<bool>;

    public class UpdateOwnerWalletHandler(IWalletUnitOfWork unit) : IRequestHandler<UpdateOwnerWalletCommand, bool>
    {
        public async Task<bool> Handle(UpdateOwnerWalletCommand command, CancellationToken cancellationToken)
        {
            var ownerWallet = await unit.OwnerWallet.GetByIdAsync(command.Id);

            if (ownerWallet == null)
                return false;

            ownerWallet.BankName = command.BankName;
            ownerWallet.BankNumber = command.BankNumber;
            ownerWallet.BankAccountName = command.BankAccountName;

            await unit.SaveAsync();
            return true;
        }
    }
}
