using System;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Admins.BanCustomer;

public record BanCustomerCommand(int CustomerId) : ICommand<BanCusomerResult>;
public record BanCusomerResult(string Notification, int? IsBan);

public class BanCustomerHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<BanCustomerCommand, BanCusomerResult>
{
    public async Task<BanCusomerResult> Handle(BanCustomerCommand command, 
        CancellationToken cancellationToken)
    {
        var customer = userUnit.User.GetById(command.CustomerId);

        if (customer is null || customer.RoleId != 4)
        {
            return new BanCusomerResult("Không tìm thấy tài khoản khách hàng phù hợp", 0);
        }

        //Ban và gửi thông báo
        customer.IsBan = 1;
        customer.Status = "InActive";
        await userUnit.User.UpdateAsync(customer);

        var userNotification = new UserNotification
        {
            UserId = customer.Id,
            Description = "Tài khoản đã bị cấm",
            Status = "Active",
            IsRead = 0,
            CreatedAt = DateTime.Now
        };
        await userUnit.UserNotification.CreateAsync(userNotification);

        return new BanCusomerResult("Cập nhật trạng thái tài khoản thành công", customer.IsBan);
    }
}
