using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Wallets.UserWallet;

public record GetCustomerWalletInformationByCustomerIdQuery(int CustomerId) 
    : IQuery<GetCustomerWalletInformationByCustomerIdResult>;

public record GetCustomerWalletInformationByCustomerIdResult(int WalletId, int CustomerWalletId, decimal? Balance, int? IsLock, string BankName, string BankAccountName, string BankNumber);

public class GetCustomerWalletInformationByCustomerIdHandler(IBookingWorkspaceUnitOfWork bookingUnit)
    : IQueryHandler<GetCustomerWalletInformationByCustomerIdQuery, GetCustomerWalletInformationByCustomerIdResult>
{
    public async Task<GetCustomerWalletInformationByCustomerIdResult> Handle(GetCustomerWalletInformationByCustomerIdQuery query, 
        CancellationToken cancellationToken)
    {
        var customerWallet = bookingUnit.customerWallet.GetAll()
            .Where(cw => cw.UserId == query.CustomerId)
            .FirstOrDefault();

        if (customerWallet == null)
        {
            return new GetCustomerWalletInformationByCustomerIdResult(0, 0, 0, 0, "N/A", "N/A", "N/A");
        }

        var wallet = bookingUnit.wallet.GetById(customerWallet!.WalletId);

        return new GetCustomerWalletInformationByCustomerIdResult(
            wallet.Id,
            customerWallet.Id,
            wallet.Balance,
            customerWallet.IsLock,
            customerWallet.BankName,
            customerWallet.BankAccountName,
            customerWallet.BankNumber
        );
    }
}
