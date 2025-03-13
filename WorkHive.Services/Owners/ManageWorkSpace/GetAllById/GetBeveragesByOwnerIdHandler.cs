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
    public record GetBeveragesByOwnerIdQuery(int OwnerId) : IQuery<List<BeverageDTO>>;

    public record BeverageDTO(int Id, string Name, decimal? Price, string ImgUrl, string Description, string Category, string Status, int OwnerId);

    public class GetBeveragesByOwnerIdValidator : AbstractValidator<GetBeveragesByOwnerIdQuery>
    {
        public GetBeveragesByOwnerIdValidator()
        {
            RuleFor(x => x.OwnerId)
                .GreaterThan(0).WithMessage("Owner ID must be greater than 0");
        }
    }
    public class GetBeveragesByOwnerIdHandler(IWorkSpaceManageUnitOfWork OwnerManageUnit)
    : IQueryHandler<GetBeveragesByOwnerIdQuery, List<BeverageDTO>>
    {
        
    public async Task<List<BeverageDTO>> Handle(GetBeveragesByOwnerIdQuery query, CancellationToken cancellationToken)
    {
        var beverages = await OwnerManageUnit.Beverage.GetBeveragesByOwnerIdAsync(query.OwnerId);

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
            b.OwnerId
        )).ToList();
    }
}
    
}
