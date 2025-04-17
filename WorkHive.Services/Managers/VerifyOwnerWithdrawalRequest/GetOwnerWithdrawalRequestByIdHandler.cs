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
    public record GetOwnerWithdrawalRequestByIdQuery(int Id) : IQuery<OwnerWithdrawalRequestDTO?>;

    class GetOwnerWithdrawalRequestByIdHandler(IWalletUnitOfWork unit) : IQueryHandler<GetOwnerWithdrawalRequestByIdQuery, OwnerWithdrawalRequestDTO?>
    {
        public async Task<OwnerWithdrawalRequestDTO?> Handle(GetOwnerWithdrawalRequestByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var request = await unit.OwnerWithdrawalRequest.GetByIdAsync(query.Id);
                if (request == null) return null;

                var ownerTransaction = await unit.OwnerTransactionHistory
    .GetLatestTransactionByOwnerIdAsync(request.WorkspaceOwnerId);
                return new OwnerWithdrawalRequestDTO(
                    request.Id, 
                    request.Title,
                    request.Description,
                    request.Status,
                    request.CreatedAt,
                    request.UpdatedAt,
                    request.WorkspaceOwnerId,
                    request.UserId,
                    request.BankName,
                    request.BankNumber,
                    request.BankAccountName,
                    request.Balance ?? 0,
                    request.ManagerResponse ?? "N/A");

            }
            catch
            {
                return null;
            }
        }
    }
}
