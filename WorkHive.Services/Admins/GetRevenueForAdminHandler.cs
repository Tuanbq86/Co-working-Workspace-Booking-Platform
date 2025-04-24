using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant;

namespace WorkHive.Services.Admins;

public record GetRevenueForAdminQuery(int AdminId) : IQuery<GetRevenueForAdminResult>;
public record GetRevenueForAdminResult(List<BookingInformationForAdmin> BookingInformation, decimal? TotalRevenue);
public record BookingInformationForAdmin(int BookingId, string OwnerName, 
    string WorkspaceName, decimal Price, DateTime DateOfBooking, string Status);

public class GetRevenueForAdminHandler(IBookingWorkspaceUnitOfWork bookUnit, IUserUnitOfWork userUnit)
    : IQueryHandler<GetRevenueForAdminQuery, GetRevenueForAdminResult>
{
    public async Task<GetRevenueForAdminResult> Handle(GetRevenueForAdminQuery query, 
        CancellationToken cancellationToken)
    {
        var admin = userUnit.User.GetById(query.AdminId);

        if(admin is null || admin.RoleId != 1)
        {
            return new GetRevenueForAdminResult([], 0);
        }

        List<BookingInformationForAdmin> result = new List<BookingInformationForAdmin>();

        var bookings = bookUnit.booking.GetAll()
            .Where(b => b.Status.ToLower().Trim().Equals(BookingStatus.Success.ToString().ToLower().Trim()))
            .ToList();

        foreach(var item in bookings)
        {
            var customer = userUnit.User.GetById(item.UserId);
            var workspace = bookUnit.workspace.GetById(item.WorkspaceId);
            var owner = bookUnit.Owner.GetById(workspace.OwnerId);
            result.Add(new BookingInformationForAdmin(item.Id, owner.LicenseName, workspace.Name, (decimal)item.Price!, (DateTime)item.CreatedAt!, item.Status));
        }

        var totalRevenue = bookings.Sum(b => b.Price);

        return new GetRevenueForAdminResult(result, totalRevenue);
    }
}
