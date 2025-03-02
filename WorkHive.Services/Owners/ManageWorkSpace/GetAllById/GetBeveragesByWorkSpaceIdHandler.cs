using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.BuildingBlocks.Exceptions;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.WorkSpaces.ManageBeverage.GetAllById
{
    public record GetBeveragesByWorkSpaceIdCommand(int WorkSpaceId) : ICommand<List<BeverageDTO>>;

    public record BeverageDTO(int Id, string Name, string Description, int? Capacity, string Category, string Status, int? CleanTime, int? Area);

    public class GetBeveragesByWorkSpaceIdValidator : AbstractValidator<GetBeveragesByWorkSpaceIdCommand>
    {
        public GetBeveragesByWorkSpaceIdValidator()
        {
            RuleFor(x => x.WorkSpaceId)
                .GreaterThan(0).WithMessage("WorkSpace ID must be greater than 0");
        }
    }
    public class GetBeveragesByWorkSpaceIdHandler(IWorkSpaceManageUnitOfWork BeverageManageUnit)
    : ICommandHandler<GetBeveragesByWorkSpaceIdCommand, List<BeverageDTO>>
    {
        public async Task<List<BeverageDTO>> Handle(GetBeveragesByWorkSpaceIdCommand command, CancellationToken cancellationToken)
        {
            var Beverages = await BeverageManageUnit.Beverage.GetAllBeverageByWorkSpaceIdAsync(command.WorkSpaceId);

            if (Beverages == null || !Beverages.Any())
            {
                throw new NotFoundException($"No Beverages found for WorkSpaceId {command.WorkSpaceId}");
            }

            return Beverages.Select(ws => new BeverageDTO(
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
}