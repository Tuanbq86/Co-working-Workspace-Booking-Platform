using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Admins.BanStaff;

public record BanStaffCommand(int StaffId) : ICommand<BanStaffResult>;
public record BanStaffResult(string Notification, int? IsBan);

public class BanStaffHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<BanStaffCommand, BanStaffResult>
{
    public async Task<BanStaffResult> Handle(BanStaffCommand command, 
        CancellationToken cancellationToken)
    {
        var staff = userUnit.User.GetById(command.StaffId);

        if(staff is null || staff.RoleId != 3)
        {
            return new BanStaffResult("Không tìm thấy tài khoản staff phù hợp", 0);
        }

        //Ban và gửi thông báo
        staff.IsBan = 1;
        staff.Status = "InActive";
        await userUnit.User.UpdateAsync(staff);

        var userNotification = new UserNotification
        {
            UserId = staff.Id,
            Description = "Tài khoản đã bị cấm",
            Status = "Active",
            IsRead = 0,
            CreatedAt = DateTime.Now
        };
        await userUnit.UserNotification.CreateAsync(userNotification);

        return new BanStaffResult("Cập nhật trạng thái tài khoản thành công", staff.IsBan);
    }
}
