using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.Base_Owner
{
    public record UpdateWorkspaceOwnerAvatarCommand(int Id, string AvatarUrl) : ICommand<UpdateWorkspaceOwnerAvatarResult>;

    public record UpdateWorkspaceOwnerAvatarResult(string Notification);

    public class UpdateWorkspaceOwnerAvatarHandler(IWorkSpaceManageUnitOfWork unit) : ICommandHandler<UpdateWorkspaceOwnerAvatarCommand, UpdateWorkspaceOwnerAvatarResult>
    {
        public async Task<UpdateWorkspaceOwnerAvatarResult> Handle(UpdateWorkspaceOwnerAvatarCommand command, CancellationToken cancellationToken)
        {
            var owner = await unit.WorkspaceOwner.GetByIdAsync(command.Id);
            if (owner == null) return new UpdateWorkspaceOwnerAvatarResult("WorkspaceOwner not found");

            if (string.IsNullOrWhiteSpace(command.AvatarUrl))
                return new UpdateWorkspaceOwnerAvatarResult("Invalid Avatar URL");

            owner.Avatar = command.AvatarUrl;
            owner.UpdatedAt = DateTime.Now;

            await unit.WorkspaceOwner.UpdateAsync(owner);
            await unit.SaveAsync();

            return new UpdateWorkspaceOwnerAvatarResult("Avatar updated successfully");
        }
    }
}
