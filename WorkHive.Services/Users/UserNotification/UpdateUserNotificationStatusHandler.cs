using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.UserNotification;

public record UpdateUserNotificationStatusCommand(int UserNotificationId) 
    : ICommand<UpdateUserNotificationStatusResult>;
public record UpdateUserNotificationStatusResult(string Notification, int IsRead);

public class UpdateUserNotificationStatusHandler(IUserUnitOfWork userUnit)
    : ICommandHandler<UpdateUserNotificationStatusCommand, UpdateUserNotificationStatusResult>
{
    public async Task<UpdateUserNotificationStatusResult> Handle(UpdateUserNotificationStatusCommand command, 
        CancellationToken cancellationToken)
    {
        var userNotifi = userUnit.UserNotification.GetById(command.UserNotificationId);

        if (userNotifi is null)
        {
            return new UpdateUserNotificationStatusResult($"Không tìm thấy thông báo của người dùng có Id: {command.UserNotificationId}", 0);
        }

        userNotifi.IsRead = 1;
        await userUnit.UserNotification.UpdateAsync(userNotifi);

        return new UpdateUserNotificationStatusResult("Bạn đã đọc thông báo", 1);
    }
}
