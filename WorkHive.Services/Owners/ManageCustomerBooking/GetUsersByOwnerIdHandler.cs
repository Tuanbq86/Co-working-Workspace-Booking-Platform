using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Owners.ManageCustomerBooking
{
    public record GetUsersByOwnerIdQuery(int OwnerId) : IQuery<List<GetUsersByOwnerIdResult>>;

    public record GetUsersByOwnerIdResult(
        int Id,
        string Name,
        string Email,
        string Phone,
        string Status,
        string Avatar,
        DateTime? CreatedAt,
        string Sex,
        DateOnly? DateOfBirth
    );

    public class GetUsersByOwnerIdHandler(IWorkSpaceManageUnitOfWork workSpaceManageUnit)
    : IQueryHandler<GetUsersByOwnerIdQuery, List<GetUsersByOwnerIdResult>>
    {
        public async Task<List<GetUsersByOwnerIdResult>> Handle(GetUsersByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            // Truy vấn và lọc người dùng có trạng thái Success từ bảng Booking
            var users = await workSpaceManageUnit.User
                .GetUsersByOwnerIdWithBookingStatus(query.OwnerId, "Success");

            // Chuyển đổi các user thành kết quả trả về
            return users.Select(user => new GetUsersByOwnerIdResult(
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.Status,  // Trạng thái user có thể lấy từ bảng User
                user.Avatar,
                user.CreatedAt,
                user.Sex ?? "Unknown",
                user.DateOfBirth
            )).ToList();
        }
    }
}
