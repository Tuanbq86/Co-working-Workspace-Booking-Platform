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
    public record GetBeveragesByWorkSpaceIdQuery(int WorkSpaceId) : IQuery<List<BeverageDTO>>;

    public record BeverageDTO(int Id, string Name, decimal? Price, string ImgUrl, string Description, string Category, string Status, int WorkspaceId);

    public class GetBeveragesByWorkSpaceIdValidator : AbstractValidator<GetBeveragesByWorkSpaceIdQuery>
    {
        public GetBeveragesByWorkSpaceIdValidator()
        {
            RuleFor(x => x.WorkSpaceId)
                .GreaterThan(0).WithMessage("WorkSpace ID must be greater than 0");
        }
    }
    public class GetBeveragesByWorkSpaceIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : IQueryHandler<GetBeveragesByWorkSpaceIdQuery, List<BeverageDTO>>
    {
        
    public async Task<List<BeverageDTO>> Handle(GetBeveragesByWorkSpaceIdQuery query, CancellationToken cancellationToken)
    {
        var beverages = await workSpaceManageUnit.Beverage.GetBeveragesByWorkSpaceIdAsync(query.WorkSpaceId);

        if (beverages == null || !beverages.Any())
        {
                return null;
        }

        return beverages.Select(b => new BeverageDTO(
            b.Id,
            b.Name,
            b.Price,
            b.ImgUrl,
            b.Description,
            b.Category,
            b.Status,
            b.WorkspaceId
        )).ToList();
    }
}
    
}
