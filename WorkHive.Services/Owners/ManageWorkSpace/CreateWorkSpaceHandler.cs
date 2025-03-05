using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace
{
    public record CreateWorkSpaceCommand(string Name, string Description, int Capacity, string Category, string Status, int CleanTime, int Area, int OwnerId ) : ICommand<CreateWorkspaceResult>;
    public record CreateWorkspaceResult(string Notification);

    public class CreateWorkSpaceValidator : AbstractValidator<CreateWorkSpaceCommand>
    {
        public CreateWorkSpaceValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0");

            RuleFor(x => x.Area)
                .GreaterThan(0).WithMessage("Area must be greater than 0");

            RuleFor(x => x.OwnerId)
                .GreaterThan(0).WithMessage("OwnerId is required");
        }
    }

    public class CreateWorkspaceHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : ICommandHandler<CreateWorkSpaceCommand, CreateWorkspaceResult>
    {
        public async Task<CreateWorkspaceResult> Handle(CreateWorkSpaceCommand command, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            var newWorkSpace = new Workspace
            {
                Name = command.Name,
                Description = command.Description,
                Capacity = command.Capacity,
                Category = command.Category,
                Status = command.Status,
                CleanTime = command.CleanTime,
                Area = command.Area,
                OwnerId = command.OwnerId
            };

            workSpaceManageUnit.Workspace.Create(newWorkSpace);

            await workSpaceManageUnit.SaveAsync();

            return new CreateWorkspaceResult("Create successfully");

        }
    }
}
