using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.BuildingBlocks.Exceptions;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.GetAllById
{
    public record GetWorkSpacesByOwnerIdCommand(int OwnerId) : ICommand<List<WorkspaceDTO>>;

    public record WorkspaceDTO(int Id, string Name, string Description, int? Capacity, string Category, string Status, int? CleanTime, int? Area);

    public class GetWorkSpacesByOwnerIdValidator : AbstractValidator<GetWorkSpacesByOwnerIdCommand>
    {
        public GetWorkSpacesByOwnerIdValidator()
        {
            RuleFor(x => x.OwnerId)
                .GreaterThan(0).WithMessage("Owner ID must be greater than 0");
        }
    }
    public class GetWorkSpacesByOwnerIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : ICommandHandler<GetWorkSpacesByOwnerIdCommand, List<WorkspaceDTO>>
    {
        public async Task<List<WorkspaceDTO>> Handle(GetWorkSpacesByOwnerIdCommand command, CancellationToken cancellationToken)
        {
            var workspaces = await workSpaceManageUnit.Workspace.GetAllWorkSpaceByOwnerIdAsync(command.OwnerId);

            if (workspaces == null || !workspaces.Any())
            {
                throw new NotFoundException($"No workspaces found for OwnerId {command.OwnerId}");
            }

            return workspaces.Select(ws => new WorkspaceDTO(
                ws.Id,
                ws.Name,
                ws.Description,
                ws.Capacity,
                ws.Category,
                ws.Status,
                ws.CleanTime,
                ws.Area
            )).ToList();
        }
    }
}
