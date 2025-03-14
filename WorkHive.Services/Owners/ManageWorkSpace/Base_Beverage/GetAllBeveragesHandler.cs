using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Owners.ManageWorkSpace.Base_Amenity;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Beverage
{
    public record GetAllBeveragesQuery() : IQuery<List<BeverageDT>>;

    public record BeverageDT(int Id, string Name, decimal? Price, string ImgUrl, string Description, string Category, string Status, int OwnerId);
    public class GetAllBeveragesValidator : AbstractValidator<GetAllBeveragesQuery>
    {
        public GetAllBeveragesValidator()
        {
        }
    }

    class GetAllBeveragesHandler(IWorkSpaceManageUnitOfWork unit) : IQueryHandler<GetAllBeveragesQuery, List<BeverageDT>>
    {
        public async Task<List<BeverageDT>> Handle(GetAllBeveragesQuery query, CancellationToken cancellationToken)
        {
            var beverages = await unit.Beverage.GetAllAsync();

            if (beverages == null || !beverages.Any())
            {
                return new List<BeverageDT>();
            }

            return beverages.Select(be => new BeverageDT(
                be.Id,
                be.Name,
                be.Price,
                be.ImgUrl,
                be.Description,
                be.Category,
                be.Status,
                be.OwnerId
            )).ToList();
        }
    }
}