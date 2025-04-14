using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

public record CreateCustomerWithdrawalRequestCommand(string Title, string Description, int CustomerId) 
    : ICommand<CreateCustomerWithdrawalRequestResult>;
public record CreateCustomerWithdrawalRequestResult(string Notification, int IsLock);

public class CreateCustomerWithdrawalRequestHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<CreateCustomerWithdrawalRequestCommand, CreateCustomerWithdrawalRequestResult>
{
    private const string DefaultStatus = "Handling";
    public async Task<CreateCustomerWithdrawalRequestResult> Handle(CreateCustomerWithdrawalRequestCommand commd, CancellationToken cancellationToken)
    {
        var customerWallet = userUnit.CustomerWallet.GetAll().FirstOrDefault(x => x.UserId == commd.CustomerId);
        if (customerWallet is null)
        {
            return new CreateCustomerWithdrawalRequestResult("Không tìm thấy ví người dùng để lấy thông tin tạo yêu cầu", 0);
        }

        var wallet = userUnit.Wallet.GetById(customerWallet.WalletId);

        var newRequest = new CustomerWithdrawalRequest
        {
            Title = commd.Title,
            Description = commd.Description,
            Status = DefaultStatus,
            CreatedAt = DateTime.Now,
            UserId = customerWallet.UserId,
            BankAccountName = customerWallet.BankAccountName,
            BankName = customerWallet.BankName,
            BankNumber = customerWallet.BankNumber,
            Balance = wallet.Balance ?? 0
        };

        //Khóa ví người dùng để không booking bằng ví hệ thống được
        customerWallet.IsLock = 1;
        await userUnit.CustomerWallet.UpdateAsync(customerWallet);

        await userUnit.CustomerWithdrawalRequest.CreateAsync(newRequest);

        return new CreateCustomerWithdrawalRequestResult("Yêu cầu rút tiền của người dùng đã được tạo thành công", (int)customerWallet.IsLock);
    }
}
