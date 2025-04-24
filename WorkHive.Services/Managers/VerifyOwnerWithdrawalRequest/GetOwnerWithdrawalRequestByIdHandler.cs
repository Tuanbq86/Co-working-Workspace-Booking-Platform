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
    public record GetOwnerWithdrawalRequestByIdQuery(int Id) : IQuery<OwnerWithdrawalRequestDT?>;


    public record OwnerWithdrawalRequestDT(
    int Id,
    string Title,
    string Description,
    string Status,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    int WorkspaceOwnerId,
    int? UserId,
    string BankName,
    string BankNumber,
    string BankAccountName,
    decimal Balance,
    string ManagerResponse,
    decimal? WalletBalance
);

    class GetOwnerWithdrawalRequestByIdHandler(IWalletUnitOfWork unit) : IQueryHandler<GetOwnerWithdrawalRequestByIdQuery, OwnerWithdrawalRequestDT?>
    {
        public async Task<OwnerWithdrawalRequestDT?> Handle(GetOwnerWithdrawalRequestByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var request = await unit.OwnerWithdrawalRequest.GetWithdrawalRequestByIdAsync(query.Id);
                if (request == null) return null;

                var ownerTransaction = await unit.OwnerTransactionHistory
    .GetLatestTransactionByOwnerIdAsync(request.WorkspaceOwnerId);
                return new OwnerWithdrawalRequestDT(
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
                    request.ManagerResponse ?? "N/A",
                    request.WorkspaceOwner.OwnerWallets
                        .FirstOrDefault(x => x.Id == request.WorkspaceOwnerId)?.Wallet.Balance

                    );

            }
            catch
            {
                return null;
            }
        }
    }
}
