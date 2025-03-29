using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Admins.BanCustomer;

public record UnBanCustomerCommand(int CustomerId) : ICommand<UnBanCusomerResult>;
public record UnBanCusomerResult(string Notification, int? IsBan);

public class UnBanCustomerHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<UnBanCustomerCommand, UnBanCusomerResult>
{
    public async Task<UnBanCusomerResult> Handle(UnBanCustomerCommand command, 
        CancellationToken cancellationToken)
    {
        var customer = userUnit.User.GetById(command.CustomerId);

        if (customer is null || customer.RoleId != 4)
        {
            return new UnBanCusomerResult("Không tìm thấy tài khoản khách hàng phù hợp", 0);
        }

        //Mở Ban và gửi thông báo
        customer.IsBan = 0;
        customer.Status = "Active";
        await userUnit.User.UpdateAsync(customer);

        var userNotification = new UserNotification
        {
            UserId = customer.Id,
            Description = "Tài khoản đã hoạt động",
            Status = "Active",
            IsRead = 0,
            CreatedAt = DateTime.Now
        };
        await userUnit.UserNotification.CreateAsync(userNotification);

        return new UnBanCusomerResult("Cập nhật trạng thái tài khoản thành công", customer.IsBan);
    }
}
