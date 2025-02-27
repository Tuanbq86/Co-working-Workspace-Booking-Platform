using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Services.Owners.LoginOwner;
using WorkHive.Services.Users.BookingWorkspace;

namespace WorkHive.Services.Owners.ManageWorkSpace
{
    public record GetAllWorkSpaceCommand(List<Workspace> Workspaces) : ICommand<GetAllWorkspaceResult>;
    public record GetAllWorkspaceResult(string Notification);

    public class GetAllWorkSpaceValidator : AbstractValidator<GetAllWorkSpaceCommand>
    {
        public GetAllWorkSpaceValidator() 
        {
        
        }
    }

    public class GetAllWorkspaceHandler()
    : ICommandHandler<GetAllWorkSpaceCommand, GetAllWorkspaceResult>
    {
        public Task<GetAllWorkspaceResult> Handle(GetAllWorkSpaceCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
