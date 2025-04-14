using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Wallets.UserWallet;

public record UpdateCustomerWalletCommand(int CustomerId, string BankName, string BankNumber, string BankAccountName) 
    : ICommand<UpdateCustomerWalletResult>;
public record UpdateCustomerWalletResult(string Notification);

public class UpdateCustomerWalletHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<UpdateCustomerWalletCommand, UpdateCustomerWalletResult>
{
    public async Task<UpdateCustomerWalletResult> Handle(UpdateCustomerWalletCommand command, 
        CancellationToken cancellationToken)
    {
        var customerWallet = userUnit.CustomerWallet.GetAll()
            .FirstOrDefault(x => x.UserId == command.CustomerId);

        if (customerWallet is null) 
            return new UpdateCustomerWalletResult("Không tìm thấy ví người dùng để cập nhật thông tin");

        customerWallet.BankName = command.BankName;
        customerWallet.BankNumber = command.BankNumber;
        customerWallet.BankAccountName = command.BankAccountName;
        await userUnit.CustomerWallet.UpdateAsync(customerWallet);

        return new UpdateCustomerWalletResult("Cập nhật thông tin tài khoản ngân hàng ví người dùng thành công");
    }
}
