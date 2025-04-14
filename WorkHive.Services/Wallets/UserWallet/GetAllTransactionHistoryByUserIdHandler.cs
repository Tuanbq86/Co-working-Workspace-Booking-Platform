using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.DTOService;

namespace WorkHive.Services.Wallets.UserWallet;

public record GetAllTransactionHistoryByUserIdQuery(int UserId) : IQuery<GetAllTransactionHistoryByUserIdResult>;
public record GetAllTransactionHistoryByUserIdResult(List<UserTransactionHistoryDTO> UserTransactionHistoryDTOs);


public class GetAllTransactionHistoryByUserIdHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetAllTransactionHistoryByUserIdQuery, GetAllTransactionHistoryByUserIdResult>
{
    public async Task<GetAllTransactionHistoryByUserIdResult> Handle(GetAllTransactionHistoryByUserIdQuery query, 
        CancellationToken cancellationToken)
    {
        var customerWallet = userUnit.CustomerWallet.GetAll()
        .FirstOrDefault(x => x.UserId == query.UserId);

        if (customerWallet is null)
            return new GetAllTransactionHistoryByUserIdResult(new List<UserTransactionHistoryDTO>());

        var userTransactionHistories = await userUnit.UserTransactionHistory
            .GetAllUserTransactionHistoryByCustomerWalletId(customerWallet.Id);

        var result = userTransactionHistories.Select(item => new UserTransactionHistoryDTO
        {
            Amount = item.TransactionHistory.Amount,
            Status = item.TransactionHistory.Status,
            Created_At = item.TransactionHistory.CreatedAt,
            Description = item.TransactionHistory.Description,
            BeforeTransactionAmount = item.TransactionHistory.BeforeTransactionAmount,
            AfterTransactionAmount = item.TransactionHistory.AfterTransactionAmount,
        }).ToList();

        return new GetAllTransactionHistoryByUserIdResult(result);
    }
}
