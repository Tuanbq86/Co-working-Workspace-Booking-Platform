using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Admins.BanStaff;

public record UnBanStaffCommand(int StaffId) : ICommand<UnBanStaffResult>;
public record UnBanStaffResult(string Notification, int? IsBan);

public class UnBanStaffHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<UnBanStaffCommand, UnBanStaffResult>
{
    public async Task<UnBanStaffResult> Handle(UnBanStaffCommand command, 
        CancellationToken cancellationToken)
    {
        var staff = userUnit.User.GetById(command.StaffId);

        if (staff is null || staff.RoleId != 3)
        {
            return new UnBanStaffResult("Không tìm thấy tài khoản staff phù hợp", 0);
        }

        //Ban và gửi thông báo
        staff.IsBan = 0;
        staff.Status = "Active";
        await userUnit.User.UpdateAsync(staff);

        var userNotification = new UserNotification
        {
            UserId = staff.Id,
            Description = "Tài khoản đã hoạt động",
            Status = "Active",
            IsRead = 0,
            CreatedAt = DateTime.Now
        };
        await userUnit.UserNotification.CreateAsync(userNotification);

        return new UnBanStaffResult("Cập nhật trạng thái tài khoản thành công", staff.IsBan);
    }
}
