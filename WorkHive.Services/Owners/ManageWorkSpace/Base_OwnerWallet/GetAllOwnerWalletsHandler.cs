using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_OwnerWallet
{
    public record GetAllOwnerWalletsQuery() : IQuery<List<GetAllOwnerWalletsResult>>;

    public record GetAllOwnerWalletsResult(int Id, decimal? Balance, string Status, int OwnerId, string OwnerName, string LicenseName);

    public class GetAllOwnerWalletsHandler(IWalletUnitOfWork unit) : IQueryHandler<GetAllOwnerWalletsQuery, List<GetAllOwnerWalletsResult>>
    {
        public async Task<List<GetAllOwnerWalletsResult>> Handle(GetAllOwnerWalletsQuery query, CancellationToken cancellationToken)
        {
            var wallets = await unit.Wallet.GetAllWalletOwnersAsync(); 
            if (wallets == null || !wallets.Any())
            {
                return new List<GetAllOwnerWalletsResult>();
            }

            return wallets.Select(wallet =>
            {
                var ownerWallet = wallet.OwnerWallets.FirstOrDefault();
                var owner = ownerWallet?.Owner;

                return new GetAllOwnerWalletsResult(
                    wallet.Id,
                    wallet.Balance,
                    wallet.Status,
                    owner?.Id ?? 0,
                    owner?.IdentityName ?? "Unknown",
                    owner?.LicenseName ?? "N/A"
                );
            }).ToList();
        }
    }
}
