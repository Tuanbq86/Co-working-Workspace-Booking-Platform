using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Managers.VerifyOwnerWithdrawalRequest
{
    public record GetAllOwnerWithdrawalRequestsQuery() : IQuery<List<OwnerWithdrawalRequestDTO>>;

    public record OwnerWithdrawalRequestDTO(
        int Id,
        string Title,
        string Description,
        string Status,
        DateTime? CreatedAt,
        int WorkspaceOwnerId,
        int? UserId,
        int WalletId,
        string BankName,
        string BankNumber,
        string BankAccountName,
        decimal Balance
    );


    public class GetAllOwnerWithdrawalRequestsHandler(IWalletUnitOfWork unit) : IQueryHandler<GetAllOwnerWithdrawalRequestsQuery, List<OwnerWithdrawalRequestDTO>>
    {
        public async Task<List<OwnerWithdrawalRequestDTO>> Handle(GetAllOwnerWithdrawalRequestsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var requests = await unit.OwnerWithdrawalRequest.GetAllAsync();

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
                        ownerWallet?.Wallet?.Balance ?? 0
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
