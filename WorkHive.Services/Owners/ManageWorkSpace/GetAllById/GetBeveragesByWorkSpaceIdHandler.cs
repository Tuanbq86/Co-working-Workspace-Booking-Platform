using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.BuildingBlocks.Exceptions;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owmers.ManageBeverage.GetAllById
{
    public record GetBeveragesByWorkSpaceIdCommand(int WorkSpaceId) : ICommand<List<BeverageDTO>>;

    public record BeverageDTO(int Id, string Name, decimal? Price, string ImgUrl, string Description, string Category, string Status, DateTime? CreatedAt, DateTime? UpdatedAt, int WorkspaceId);

    public class GetBeveragesByWorkSpaceIdValidator : AbstractValidator<GetBeveragesByWorkSpaceIdCommand>
    {
        public GetBeveragesByWorkSpaceIdValidator()
        {
            RuleFor(x => x.WorkSpaceId)
                .GreaterThan(0).WithMessage("WorkSpace ID must be greater than 0");
        }
    }
    public class GetBeveragesByWorkSpaceIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : ICommandHandler<GetBeveragesByWorkSpaceIdCommand, List<BeverageDTO>>
    {
        
    public async Task<List<BeverageDTO>> Handle(GetBeveragesByWorkSpaceIdCommand command, CancellationToken cancellationToken)
    {
        var beverages = await workSpaceManageUnit.Beverage.GetBeveragesByWorkSpaceIdAsync(command.WorkSpaceId);

        if (beverages == null || !beverages.Any())
        {
            throw new NotFoundException($"No beverages found for WorkSpaceId {command.WorkSpaceId}");
        }

        return beverages.Select(b => new BeverageDTO(
            b.Id,
            b.Name,
            b.Price,
            b.ImgUrl,
            b.Description,
            b.Category,
            b.Status,
            b.CreatedAt,
            b.UpdatedAt,
            b.WorkspaceId
        )).ToList();
    }
}
    
}
