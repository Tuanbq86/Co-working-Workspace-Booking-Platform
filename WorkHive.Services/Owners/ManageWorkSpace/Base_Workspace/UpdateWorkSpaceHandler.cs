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
    public record UpdateWorkSpaceCommand(int Id, string Name, string Description, int Capacity, string Category, string Status, int CleanTime, int Area, TimeOnly? OpenTime, TimeOnly? CloseTime, int? Is24h, List<PriceDTO> Prices,
    List<ImageDTO> Images, List<FacilityDTO> Facilities, List<PolicyDTO> Policies) : ICommand<UpdateWorkspaceResult>;

    public record UpdateWorkspaceResult(string Notification);

    public class UpdateWorkSpaceValidator : AbstractValidator<UpdateWorkSpaceCommand>
    {
        public UpdateWorkSpaceValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id is required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }

    public class UpdateWorkspaceHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
        : ICommandHandler<UpdateWorkSpaceCommand, UpdateWorkspaceResult>
    {
        public async Task<UpdateWorkspaceResult> Handle(UpdateWorkSpaceCommand command, CancellationToken cancellationToken)
        {
            var workspace = await workSpaceManageUnit.Workspace.GetByIdAsync(command.Id);
            if (workspace == null)
                return new UpdateWorkspaceResult("Workspace not found");

            workspace.Name = command.Name;
            workspace.Description = command.Description;
            workspace.Capacity = command.Capacity;
            workspace.Category = command.Category;
            workspace.Status = command.Status;
            workspace.CleanTime = command.CleanTime;
            workspace.Area = command.Area;
            workspace.OpenTime = command.OpenTime;
            workspace.CloseTime = command.CloseTime;
            workspace.Is24h = command.Is24h;

            await workSpaceManageUnit.SaveAsync();

            return new UpdateWorkspaceResult($"Workspace '{workspace.Name}' updated successfully!");
        }
    }
}
