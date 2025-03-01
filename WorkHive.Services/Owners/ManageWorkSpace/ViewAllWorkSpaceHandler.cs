using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Owners.LoginOwner;
using WorkHive.Services.Users.BookingWorkspace;

namespace WorkHive.Services.Owners.ManageWorkSpace
{
    public record GetAllWorkSpaceCommand() : ICommand<GetAllWorkspaceResult>;
    public record GetAllWorkspaceResult(List<Workspace> Workspaces);

    public class GetAllWorkSpaceValidator : AbstractValidator<GetAllWorkSpaceCommand>
    {
        public GetAllWorkSpaceValidator() 
        {
        
        }
    }

    public class GetAllWorkspaceHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : ICommandHandler<GetAllWorkSpaceCommand, GetAllWorkspaceResult>
    {
        public Task<GetAllWorkspaceResult> Handle(GetAllWorkSpaceCommand command, CancellationToken cancellationToken)
        {
            List<Workspace> Workspaces = workSpaceManageUnit.Workspace.GetAll();
            return Task.FromResult(new GetAllWorkspaceResult(Workspaces));
        }

    }
}
