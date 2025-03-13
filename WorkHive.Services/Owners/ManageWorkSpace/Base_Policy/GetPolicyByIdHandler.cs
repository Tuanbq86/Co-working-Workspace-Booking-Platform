using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageWorkSpace.Base_Policy
{
    public record GetPolicyByIdQuery(int Id) : IQuery<GetPolicyByIdResult>;
    public record GetPolicyByIdResult(int Id, string Name);

    public class GetPolicyByIdHandler(IWorkSpaceManageUnitOfWork unit)
        : IQueryHandler<GetPolicyByIdQuery, GetPolicyByIdResult>
    {
        public async Task<GetPolicyByIdResult> Handle(GetPolicyByIdQuery query, CancellationToken cancellationToken)
        {
            var policy = await unit.Policy.GetByIdAsync(query.Id);
            return policy == null ? null : new GetPolicyByIdResult(policy.Id, policy.Name);
        }
    }
}
