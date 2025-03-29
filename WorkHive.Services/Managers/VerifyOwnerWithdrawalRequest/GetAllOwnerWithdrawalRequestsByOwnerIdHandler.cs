using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest
{
    public record GetAllOwnerWithdrawalRequestsByOwnerIdQuery(int OwnerId) : IQuery<List<OwnerWithdrawalRequestDTO>>;

    public class GetAllOwnerWithdrawalRequestsByOwnerIdHandler(IWalletUnitOfWork unit)
        : IQueryHandler<GetAllOwnerWithdrawalRequestsByOwnerIdQuery, List<OwnerWithdrawalRequestDTO>>
    {
        public async Task<List<OwnerWithdrawalRequestDTO>> Handle(GetAllOwnerWithdrawalRequestsByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var requests = await unit.OwnerWithdrawalRequest.GetByOwnerIdAsync(query.OwnerId);
                var result = new List<OwnerWithdrawalRequestDTO>();

                foreach (var request in requests)
                {
                    var ownerWallet = await unit.OwnerWallet.GetByOwnerIdAsync(request.WorkspaceOwnerId);

                    result.Add(new OwnerWithdrawalRequestDTO(
                        request.Id,
                        request.Title,
                        request.Description,
                        request.Status,
                        request.CreatedAt,
                        request.WorkspaceOwnerId,
                        request.UserId,
                        ownerWallet?.WalletId ?? 0,
                        ownerWallet?.BankName ?? "N/A",
                        ownerWallet?.BankNumber ?? "N/A",
                        ownerWallet?.BankAccountName ?? "N/A",
                        ownerWallet?.Wallet?.Balance ?? 0,
                        request.ManagerResponse ?? "N/A"    
                    ));
                }

                return result;
            }
            catch
            {
                return new List<OwnerWithdrawalRequestDTO>();
            }
        }
    }
}
