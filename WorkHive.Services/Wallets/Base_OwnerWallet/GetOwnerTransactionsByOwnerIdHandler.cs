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
    public record GetOwnerTransactionsByOwnerIdQuery(int Id) : IQuery<List<GetOwnerTransactionsByOwnerIdResult>>;

    public record GetOwnerTransactionsByOwnerIdResult(int TransactionId, decimal? Amount, string Status, string Description, DateTime? CreatedAt, decimal? BeforeAmount, decimal? AfterAmount);

    public class GetOwnerTransactionsByOwnerIdValidator : AbstractValidator<GetOwnerTransactionsByOwnerIdQuery>
    {
        public GetOwnerTransactionsByOwnerIdValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Owner ID must be greater than 0");
        }
    }

    public class GetOwnerTransactionsByOwnerIdHandler(IWalletUnitOfWork unit)
        : IQueryHandler<GetOwnerTransactionsByOwnerIdQuery, List<GetOwnerTransactionsByOwnerIdResult>>
    {
        public async Task<List<GetOwnerTransactionsByOwnerIdResult>> Handle(GetOwnerTransactionsByOwnerIdQuery query,
            CancellationToken cancellationToken)
        {
            var transactions = await unit.TransactionHistory.GetTransactionsByOwnerIdAsync(query.Id);

            if (transactions == null || !transactions.Any())
            {
                return null;
            }

            return transactions.Select(tr => new GetOwnerTransactionsByOwnerIdResult(
                tr.Id,
                tr.Amount,
                tr.Status,
                tr.Description,
                tr.CreatedAt,
                tr.BeforeTransactionAmount,
                tr.AfterTransactionAmount
            )).ToList();
        }
    }
}
