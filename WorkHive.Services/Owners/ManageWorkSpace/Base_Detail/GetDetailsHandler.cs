using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Detail
{
    public record GetAllDetailQuery() : IQuery<List<Detail>>;

    public class GetAllDetailValidator : AbstractValidator<GetAllDetailQuery>
    {
        public GetAllDetailValidator() { }
    }

    class GetDetailsHandler(IWorkSpaceManageUnitOfWork unit) : IQueryHandler<GetAllDetailQuery, List<Detail>>
    {
        public async Task<List<Detail>> Handle(GetAllDetailQuery query, CancellationToken cancellationToken)
        {
            var details = await unit.Detail.GetAllAsync();
            return details?.Any() == true ? details : new List<Detail>();
        }
    }
}
