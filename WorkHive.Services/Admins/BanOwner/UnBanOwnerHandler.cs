using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Admins.BanStaff;

namespace WorkHive.Services.Admins.BanOwner;

public record UnBanOwnerCommand(int OwnerId) : ICommand<UnBanOwnerResult>;
public record UnBanOwnerResult(string Notification, int? IsBan);

public class UnBanOwnerHandler(IWorkspaceOwnerUnitOfWork ownerUnit, IBookingWorkspaceUnitOfWork bookUnit)
    : ICommandHandler<UnBanOwnerCommand, UnBanOwnerResult>
{
    public async Task<UnBanOwnerResult> Handle(UnBanOwnerCommand command, 
        CancellationToken cancellationToken)
    {
        var owner = ownerUnit.WorkspaceOwner.GetById(command.OwnerId);

        if (owner is null)
        {
            return new UnBanOwnerResult("Không tìm thấy tài khoản owner phù hợp", 0);
        }

        //Ban và gửi thông báo
        owner.IsBan = 0;
        owner.Status = "Active";
        await ownerUnit.WorkspaceOwner.UpdateAsync(owner);

        var workspaces = bookUnit.workspace.GetAll().Where(x => x.OwnerId == owner.Id).ToList();
        if (workspaces.Count > 0)
        {
            foreach (var item in workspaces)
            {
                item.Status = "Active";
                await bookUnit.workspace.UpdateAsync(item);
            }
        }

        var ownerNotification = new OwnerNotification
        {
            OwnerId = owner.Id,
            Description = "Tài khoản đã hoạt động",
            Status = "Active",
            IsRead = 0,
            CreatedAt = DateTime.Now
        };
        await ownerUnit.OwnerNotification.CreateAsync(ownerNotification);

        return new UnBanOwnerResult("Cập nhật trạng thái tài khoản thành công", owner.IsBan);
    }
}
