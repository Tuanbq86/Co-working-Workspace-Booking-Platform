using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Policy
{
    public record GetAllPolicyQuery() : IQuery<List<Policy>>;

    public class GetAllPolicyValidator : AbstractValidator<GetAllPolicyQuery>
    {
        public GetAllPolicyValidator()
        {
        }
    }
    class GetPoliciesHandler(IWorkSpaceManageUnitOfWork unit) : IQueryHandler<GetAllPolicyQuery, List<Policy>>
    {
        public async Task<List<Policy>> Handle(GetAllPolicyQuery query, CancellationToken cancellationToken)
        {
            var policies = await unit.Policy.GetAllAsync();
            if (policies == null || !policies.Any())
            {
                return new List<Policy>();
            }
            return policies;
        }
    }
}
