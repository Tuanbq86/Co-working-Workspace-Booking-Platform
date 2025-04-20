using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Facility
{
    public record GetAllFacilityQuery() : IQuery<List<Facility>>;

    public class GetAllFacilityValidator : AbstractValidator<GetAllFacilityQuery>
    {
        public GetAllFacilityValidator() { }
    }

    class GetFacilitiesHandler(IWorkSpaceManageUnitOfWork unit) : IQueryHandler<GetAllFacilityQuery, List<Facility>>
    {
        public async Task<List<Facility>> Handle(GetAllFacilityQuery query, CancellationToken cancellationToken)
        {
            var facilities = await unit.Facility.GetAllAsync();
            return facilities?.Any() == true ? facilities : new List<Facility>();
        }
    }
}
