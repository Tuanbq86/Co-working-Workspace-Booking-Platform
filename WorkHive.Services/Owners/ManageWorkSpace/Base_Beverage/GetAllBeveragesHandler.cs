using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Beverage
{
    public record GetAllBeveragesQuery() : IQuery<List<Beverage>>;

    public class GetAllBeveragesValidator : AbstractValidator<GetAllBeveragesQuery>
    {
        public GetAllBeveragesValidator()
        {
        }
    }

    class GetAllBeveragesHandler(IWorkSpaceManageUnitOfWork unit) : IQueryHandler<GetAllBeveragesQuery, List<Beverage>>
    {
        public async Task<List<Beverage>> Handle(GetAllBeveragesQuery query, CancellationToken cancellationToken)
        {
            var beverages = await unit.Beverage.GetAllAsync();
            return beverages?.Any() == true ? beverages : new List<Beverage>();
        }
    }
}