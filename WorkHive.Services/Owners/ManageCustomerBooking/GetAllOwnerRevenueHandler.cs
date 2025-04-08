using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageCustomerBooking
{
    public record GetAllOwnerRevenueQuery() : IQuery<List<GetAllOwnerRevenueResult>>;

    public record GetAllOwnerRevenueResult(
        int OwnerId,
        string OwnerName,
        decimal TotalRevenue
    );

    public class GetAllOwnerRevenueHandler(IWorkSpaceManageUnitOfWork unitOfWork)
    : IQueryHandler<GetAllOwnerRevenueQuery, List<GetAllOwnerRevenueResult>>
    {
        public async Task<List<GetAllOwnerRevenueResult>> Handle(GetAllOwnerRevenueQuery query, CancellationToken cancellationToken)
        {
            var bookings = await unitOfWork.Booking.GetAllWithWorkspaceAndOwner();

            var revenueByOwner = bookings
                .Where(b =>
                    b.Status == "Success" &&
                    b.Price.HasValue &&
                    b.Workspace != null &&
                    b.Workspace.Owner != null
                )
                .GroupBy(b => new { b.Workspace.Owner.Id, b.Workspace.Owner.LicenseName })
                .Select(g => new GetAllOwnerRevenueResult(
                    g.Key.Id,
                    g.Key.LicenseName,
                    g.Sum(b => b.Price ?? 0)
                ))
                .ToList();

            return revenueByOwner;
        }
    }

}
