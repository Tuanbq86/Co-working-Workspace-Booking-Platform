using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.Base_Owner
{
    public record UpdateWorkspaceOwnerSocialsCommand(
        int Id,
        string Facebook,
        string Instagram,
        string Tiktok
    ) : ICommand<bool>;

    public class UpdateWorkspaceOwnerSocialsValidator : AbstractValidator<UpdateWorkspaceOwnerSocialsCommand>
    {
        public UpdateWorkspaceOwnerSocialsValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            //RuleFor(x => x.Facebook).MaximumLength(255);
            //RuleFor(x => x.Instagram).MaximumLength(255);
            //RuleFor(x => x.Tiktok).MaximumLength(255);
        }
    }

    public class UpdateWorkspaceOwnerSocialsHandler(IWorkSpaceManageUnitOfWork unit)
        : ICommandHandler<UpdateWorkspaceOwnerSocialsCommand, bool>
    {
        public async Task<bool> Handle(UpdateWorkspaceOwnerSocialsCommand command, CancellationToken cancellationToken)
        {
            var owner = await unit.WorkspaceOwner.GetByIdAsync(command.Id);
            if (owner is null)
                return false;

            owner.Facebook = command.Facebook;
            owner.Instagram = command.Instagram;
            owner.Tiktok = command.Tiktok;
            owner.UpdatedAt = DateTime.UtcNow;

            unit.WorkspaceOwner.Update(owner);
            await unit.SaveAsync();

            return true;
        }
    }
}