using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;

namespace WorkHive.Services.Owners.ManageWorkSpace
{
    public record UpdateWorkSpaceCommand(List<Workspace> Workspaces) : ICommand<UpdateWorkspaceResult>;
    public record UpdateWorkspaceResult(string Notification);

    public class UpdateWorkSpaceValidator : AbstractValidator<UpdateWorkSpaceCommand>
    {
        public UpdateWorkSpaceValidator()
        {

        }
    }

    public class UpdateWorkspaceHandler()
    : ICommandHandler<UpdateWorkSpaceCommand, UpdateWorkspaceResult>
    {
        public Task<UpdateWorkspaceResult> Handle(UpdateWorkSpaceCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
