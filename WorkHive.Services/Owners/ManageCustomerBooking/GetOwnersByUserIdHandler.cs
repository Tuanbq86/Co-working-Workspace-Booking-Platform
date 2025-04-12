using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageCustomerBooking
{
    public record GetOwnersByUserIdQuery(int UserId) : IQuery<List<GetOwnersByUserIdResult>>;

    public record GetOwnersByUserIdResult(
        int Id,
        string Name,
        string Email,
        string Phone,
        string Status,
        DateTime? CreatedAt
    );

    public class GetOwnersByUserIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
        : IQueryHandler<GetOwnersByUserIdQuery, List<GetOwnersByUserIdResult>>
    {
        public async Task<List<GetOwnersByUserIdResult>> Handle(GetOwnersByUserIdQuery query, CancellationToken cancellationToken)
        {
            var owners = await workSpaceManageUnit.WorkspaceOwner.GetOwnersByUserId(query.UserId);

            return owners.Select(owner => new GetOwnersByUserIdResult(
                owner.Id,
                owner.OwnerName, 
                owner.Email,
                owner.Phone,
                owner.Status,
                owner.CreatedAt
            )).ToList();
        }
    }
}
