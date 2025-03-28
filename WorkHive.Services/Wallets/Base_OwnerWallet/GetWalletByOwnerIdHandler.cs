using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Wallets.Base_OwnerWallet
{
    public record GetWalletByOwnerIdQuery(int Id) : IQuery<GetWalletByOwnerIdResult>;
    public record GetWalletByOwnerIdResult(int Id, int OwnerWalletId, decimal? Balance, string Status, string BankName, string BankAccountName, string BankNumber, int OwnerId, string OwnerName, string LicenseName);


    public class GetWalletByOwnerIdValidator : AbstractValidator<GetWalletByOwnerIdQuery>
    {
        public GetWalletByOwnerIdValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0");
        }
    }

    public class GetWalletByOwnerIdHandler(IWalletUnitOfWork unit) : IQueryHandler<GetWalletByOwnerIdQuery, GetWalletByOwnerIdResult>
    {
        public async Task<GetWalletByOwnerIdResult> Handle(GetWalletByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            var wallet = await unit.Wallet.GetOwnerWalletByIdAsync(query.Id);
            if (wallet == null)
            {
                return null;
            }

            var ownerWallet = wallet.OwnerWallets.FirstOrDefault();
            var owner = ownerWallet?.Owner; 

            return new GetWalletByOwnerIdResult(
                wallet.Id,
                ownerWallet.Id,
                wallet.Balance,
                wallet.Status,
                ownerWallet.BankName,
                ownerWallet.BankAccountName,
                ownerWallet.BankNumber,
                owner?.Id ?? 0,  
                owner?.IdentityName ?? "Unknown", 
                owner?.LicenseName ?? "N/A" 
            );
        }
    }
}

