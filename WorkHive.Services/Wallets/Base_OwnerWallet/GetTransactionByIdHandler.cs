using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Wallets.Base_OwnerWallet
{

    public record GetTransactionByIdQuery(int Id) : IQuery<GetTransactionByIdResult>;

    public record GetTransactionByIdResult(
    int Id,
    decimal? Amount,
    string Status,
    string Description,
    DateTime? CreatedAt
    );

    public class GetTransactionByIdHandler(IWalletUnitOfWork unit)
    : IQueryHandler<GetTransactionByIdQuery, GetTransactionByIdResult>
    {
        public async Task<GetTransactionByIdResult> Handle(GetTransactionByIdQuery query, CancellationToken cancellationToken)
        {
            var transaction = await unit.TransactionHistory.GetByIdAsync(query.Id);

            if (transaction == null)
            {
                return null;
            }

            return new GetTransactionByIdResult(
                transaction.Id,
                transaction.Amount,
                transaction.Status,
                transaction.Description,
                transaction.CreatedAt
            );
        }
    }
}
