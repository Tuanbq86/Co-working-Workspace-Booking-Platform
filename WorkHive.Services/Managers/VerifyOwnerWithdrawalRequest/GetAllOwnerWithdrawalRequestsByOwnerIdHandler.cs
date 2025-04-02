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
                    // Lấy thông tin giao dịch gần nhất của Owner
                    var ownerTransaction = await unit.OwnerTransactionHistory
                        .GetLatestTransactionByOwnerIdAsync(request.WorkspaceOwnerId);

                    result.Add(new OwnerWithdrawalRequestDTO(
                        request.Id,
                        request.Title,
                        request.Description,
                        request.Status,
                        request.CreatedAt,
                        request.WorkspaceOwnerId,
                        request.UserId,
                        ownerTransaction?.TransactionHistoryId ?? 0,
                        ownerTransaction?.TransactionHistory?.BankName ?? "N/A",
                        ownerTransaction?.TransactionHistory?.BankNumber ?? "N/A",
                        ownerTransaction?.TransactionHistory?.BankAccountName ?? "N/A",
                        ownerTransaction?.TransactionHistory?.Amount ?? 0, 
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