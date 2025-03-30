using System;
using System.Collections.Generic;
using System.Linq;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Common;

namespace WorkHive.Services.Wallets.UserWallet;

public record GetAmountWalletByUserIdQuery(int UserId) : IQuery<GetAmountWalletByUserIdResult>;
public record GetAmountWalletByUserIdResult(string? Amount, string Notification);

public class GetAmountWalletByUserIdHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetAmountWalletByUserIdQuery, GetAmountWalletByUserIdResult>
{
    public async Task<GetAmountWalletByUserIdResult> Handle(GetAmountWalletByUserIdQuery query, 
        CancellationToken cancellationToken)
    {
        var customerWallet = userUnit.CustomerWallet.GetAll()
            .Where(x => x.UserId.Equals(query.UserId)).FirstOrDefault();

        if (customerWallet is null)
            return new GetAmountWalletByUserIdResult("", "Không tìm thấy ví của người dùng để lấy số tiền");

        var wallet = userUnit.Wallet.GetById(customerWallet.WalletId);

        return new GetAmountWalletByUserIdResult(wallet.Balance.ToVnd(), "Lấy thành công số dư trong ví của khách hàng");
    }
}
