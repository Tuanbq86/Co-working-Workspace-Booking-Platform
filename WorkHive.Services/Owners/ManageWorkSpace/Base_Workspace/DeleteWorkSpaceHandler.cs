using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.CRUD_Base_Workspace
{
    public record DeleteWorkSpaceCommand(int id) : ICommand<DeleteWorkspaceResult>;
    public record DeleteWorkspaceResult(string Notification);

    public class DeleteWorkSpaceValidator : AbstractValidator<DeleteWorkSpaceCommand>
    {
        public DeleteWorkSpaceValidator()
        {

        }
    }

    public class DeleteWorkspaceHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : ICommandHandler<DeleteWorkSpaceCommand, DeleteWorkspaceResult>
    {
        public Task<DeleteWorkspaceResult> Handle(DeleteWorkSpaceCommand command, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            var IsExit = workSpaceManageUnit.Workspace.GetById(command.id);
            if (IsExit == null)
            {
                message = "WorkSpace not found! ";
            }
            else
            {
                workSpaceManageUnit.Workspace.Remove(IsExit);
                message = "WorkSpace delete successfully";
            }


            return Task.FromResult(new DeleteWorkspaceResult(message));
        }
    }
}


