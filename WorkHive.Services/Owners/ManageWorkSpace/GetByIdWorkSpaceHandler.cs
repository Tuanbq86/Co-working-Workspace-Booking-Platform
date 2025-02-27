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
    public record GetByIdWorkSpaceCommand(List<Workspace> Workspaces) : ICommand<GetByIdWorkspaceResult>;
    public record GetByIdWorkspaceResult(string Notification);

    public class GetByIdWorkSpaceValidator : AbstractValidator<GetByIdWorkSpaceCommand>
    {
        public GetByIdWorkSpaceValidator()
        {

        }
    }

    public class GetByIdWorkspaceHandler()
    : ICommandHandler<GetByIdWorkSpaceCommand, GetByIdWorkspaceResult>
    {
        public Task<GetByIdWorkspaceResult> Handle(GetByIdWorkSpaceCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
