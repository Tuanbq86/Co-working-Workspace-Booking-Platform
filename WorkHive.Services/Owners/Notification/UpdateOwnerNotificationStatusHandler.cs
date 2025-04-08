using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.Notification
{
    public record UpdateOwnerNotificationStatusCommand(int OwnerNotificationId)
    : ICommand<UpdateOwnerNotificationStatusResult>;
    public record UpdateOwnerNotificationStatusResult(string Notification, int IsRead);

    public class UpdateOwnerNotificationStatusHandler(IWorkspaceOwnerUnitOfWork OwnerUnit)
        : ICommandHandler<UpdateOwnerNotificationStatusCommand, UpdateOwnerNotificationStatusResult>
    {
        public async Task<UpdateOwnerNotificationStatusResult> Handle(UpdateOwnerNotificationStatusCommand command,
            CancellationToken cancellationToken)
        {
            var OwnerNotifi = OwnerUnit.OwnerNotification.GetById(command.OwnerNotificationId);

            if (OwnerNotifi is null)
            {
                return new UpdateOwnerNotificationStatusResult($"Không tìm thấy thông báo của người dùng có Id: {command.OwnerNotificationId}", 0);
            }

            OwnerNotifi.IsRead = 1;
            await OwnerUnit.OwnerNotification.UpdateAsync(OwnerNotifi);

            return new UpdateOwnerNotificationStatusResult("Bạn đã đọc thông báo", 1);
        }
    }

}
