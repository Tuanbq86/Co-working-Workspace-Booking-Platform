using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Common;
using WorkHive.Services.EmailServices;
using WorkHive.Services.Users.DTOs;

namespace WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

public record UpdateCustomerWithdrawalRequestStatusCommand(int CustomerWithdrawalRequestId, int ManagerId, string ManagerResponse, string Status)
        : ICommand<UpdateCustomerWithdrawalRequestStatusResult>;

public record UpdateCustomerWithdrawalRequestStatusResult(string Notification);

public class UpdateCustomerWithdrawalRequestStatusHandler(IUserUnitOfWork userUnit, IEmailService emailService)
    : ICommandHandler<UpdateCustomerWithdrawalRequestStatusCommand, UpdateCustomerWithdrawalRequestStatusResult>
{
    public async Task<UpdateCustomerWithdrawalRequestStatusResult> Handle(UpdateCustomerWithdrawalRequestStatusCommand command, 
        CancellationToken cancellationToken)
    {
        var request = await userUnit.CustomerWithdrawalRequest.GetByIdAsync(command.CustomerWithdrawalRequestId);
        if (request == null) return new UpdateCustomerWithdrawalRequestStatusResult("Không tìm thấy yêu cầu rút tiền nào của người dùng");

        // Nếu trạng thái cập nhật là "Success", thực hiện việc rút tiền và đặt về 0
        if(command.Status == "Success")
        {
            var customerWallet = userUnit.CustomerWallet.GetAll()
                .FirstOrDefault(x => x.UserId == request.UserId);

            if (customerWallet == null)
                return new UpdateCustomerWithdrawalRequestStatusResult("Không tìm thấy ví người dùng.");

            var wallet = await userUnit.Wallet.GetByIdAsync(customerWallet.WalletId);
            if (wallet == null)
                return new UpdateCustomerWithdrawalRequestStatusResult("Không tìm thấy ví người dùng.");

            if (wallet.Balance == null || wallet.Balance <= 0)
                return new UpdateCustomerWithdrawalRequestStatusResult("Ví không có tiền để rút.");

            decimal withdrawAmount = wallet.Balance.Value;
            wallet.Balance = 0;

            // Ghi lại giao dịch
            var transaction = new TransactionHistory
            {
                Amount = withdrawAmount,
                Status = "Withdraw Success",
                Description = "Rút tiền thành công từ ví người dùng",
                CreatedAt = DateTime.Now,
                BankAccountName = customerWallet.BankAccountName,
                BankName = customerWallet.BankName,
                BankNumber = customerWallet.BankNumber,
                BeforeTransactionAmount = withdrawAmount,
                AfterTransactionAmount = 0,
                Title = "Rút tiền ví người dùng",
            };

            await userUnit.TransactionHistory.CreateAsync(transaction);

            var userTransactionHistory = new UserTransactionHistory
            {
                CustomerWalletId = customerWallet.Id,
                TransactionHistoryId = transaction.Id,
                Status = "Withdraw Success",
            };

            await userUnit.UserTransactionHistory.CreateAsync(userTransactionHistory);

            var userNotification = new UserNotification
            {
                Description = $"Yêu cầu rút tiền của bạn đã được phê duyệt và đang trong trạng thái hoạt động. Bạn có thể kiểm tra lại lịch sử giao dịch.",
                Status = "Active",
                UserId = customerWallet.UserId,
                CreatedAt = DateTime.Now,
                IsRead = 0,
                Title = "Yêu cầu rút tiền đã được duyệt"
            };

            await userUnit.UserNotification.CreateAsync(userNotification);

            await userUnit.Wallet.UpdateAsync(wallet);

            customerWallet.IsLock = 0;
            await userUnit.CustomerWallet.UpdateAsync(customerWallet);

        }
        // Cập nhật trạng thái yêu cầu rút tiền
        request.Status = command.Status;
        request.ManagerId = command.ManagerId;
        request.ManagerResponse = command.ManagerResponse;
        request.UpdatedAt = DateTime.Now;
        await userUnit.CustomerWithdrawalRequest.UpdateAsync(request);

        var walletOfCustomer = userUnit.CustomerWallet.GetAll()
                .FirstOrDefault(x => x.UserId == request.UserId);
        walletOfCustomer!.IsLock = 0;
        await userUnit.CustomerWallet.UpdateAsync(walletOfCustomer);

        // Gửi email thông báo cho người dùng
        var customer = await userUnit.User.GetByIdAsync(request.UserId);
        var emailBody = GenerateWithDrawEmailContent(request);
        await emailService.SendEmailAsync(customer.Email, "Thông tin rút tiền", emailBody);

        return new UpdateCustomerWithdrawalRequestStatusResult($"Yêu cầu rút tiền đã được cập nhật thành '{command.Status}'");
    }

    //Hàm gửi mail
    private string GenerateWithDrawEmailContent(CustomerWithdrawalRequest request)
    {
        var sb = new StringBuilder();
        //Phần nội dung
        sb.AppendLine($@"
    <div style='display: flex; justify-content: center;'>
        <table style='border-collapse: collapse; width: 100%; max-width: 1350px; margin: 0 auto; font-family: Arial, sans-serif; border: 1px solid #ddd;'>
            <tr>
                <th colspan='3' 
                    style='background-color: #f8f3d4; 
                           padding: 15px; 
                           font-size: 22px; 
                           text-align: center;'>
                    THÔNG TIN RÚT TIỀN
                </th>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Trạng thái</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{request.Status}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Ngày tạo</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{request.CreatedAt}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Phản hồi từ hệ thống</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{request.ManagerResponse}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Số tiền rút</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{request.Balance.ToVnd()}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Tên ngân hàng nhận</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{request.BankName}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Tên chủ tài khoản nhận</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{request.BankAccountName}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Số tài khoản nhận</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{request.BankNumber}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-size: 16px; font-weight: bold; border: 1px solid #ddd;'>Mô tả</td>
                <td style='padding: 10px; font-size: 16px; border: 1px solid #ddd;' colspan='2'>{request.Description}</td>
            </tr>");

        // Phần liên hệ (Contact Info)
        sb.AppendLine($@"
    <div style='background-color: #f8d7da; padding: 20px; margin-top: 20px; border-radius: 8px; max-width: 1350px; margin: 0 auto;'>
        <h2 style='text-align: center; font-family: Arial, sans-serif; color: #d63384;'>Liên hệ</h2>
        <p style='text-align: center; font-size: 16px; color: #d63384;'>
            Mail: <a href='mailto:workhive.vn.official@gmail.com' style='color: #d63384;'>workhive.vn.official@gmail.com</a><br>
            Hotline: 0867435157<br>
        </p>
    </div>");

        return sb.ToString();


    }
}
