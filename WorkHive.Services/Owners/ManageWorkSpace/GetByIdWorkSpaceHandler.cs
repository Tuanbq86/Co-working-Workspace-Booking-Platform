using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Exceptions;
using WorkHive.Services.Users.LoginUser;

namespace WorkHive.Services.Owners.ManageWorkSpace
{
    public record GetByIdWorkSpaceCommand(int id) : ICommand<GetByIdWorkspaceResult>;
    public record GetByIdWorkspaceResult(Workspace Workspace, string Notification);

    public class GetByIdWorkSpaceValidator : AbstractValidator<GetByIdWorkSpaceCommand>
    {
        public GetByIdWorkSpaceValidator()
        {

        }
    }

    public class GetByIdWorkspaceHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : ICommandHandler<GetByIdWorkSpaceCommand, GetByIdWorkspaceResult>
    {
        public async Task<GetByIdWorkspaceResult> Handle(GetByIdWorkSpaceCommand command, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            Workspace IsExit = workSpaceManageUnit.Workspace.GetById(command.id);
            if (IsExit == null)
            {
                message = "WorkSpace not found! ";
            }
            else
            {               
                message = "Successfully";
            }
            return new GetByIdWorkspaceResult(IsExit, message);
        }
    }
}
