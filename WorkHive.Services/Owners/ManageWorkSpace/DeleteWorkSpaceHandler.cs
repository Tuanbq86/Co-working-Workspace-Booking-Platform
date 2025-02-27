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
    public record DeleteWorkSpaceCommand(List<Workspace> Workspaces) : ICommand<DeleteWorkspaceResult>;
    public record DeleteWorkspaceResult(string Notification);

    public class DeleteWorkSpaceValidator : AbstractValidator<DeleteWorkSpaceCommand>
    {
        public DeleteWorkSpaceValidator()
        {

        }
    }

    public class DeleteWorkspaceHandler()
    : ICommandHandler<DeleteWorkSpaceCommand, DeleteWorkspaceResult>
    {
        public Task<DeleteWorkspaceResult> Handle(DeleteWorkSpaceCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
