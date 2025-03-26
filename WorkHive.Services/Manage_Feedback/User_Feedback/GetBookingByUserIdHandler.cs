using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.User_Feedback
{
    public record GetBookingByUserIdQuery(int UserId) : IQuery<List<BookingDTO>>;

    public record BookingDTO(int Id, DateTime? StartDate, DateTime? EndDate, decimal? Price, string Status, DateTime? CreatedAt, int PaymentId, int WorkspaceId, int? PromotionId, int UserId);

    class GetBookingByUserIdHandler(IFeedbackManageUnitOfWork unit) : IQueryHandler<GetBookingByUserIdQuery, List<BookingDTO>>
    {
        public async Task<List<BookingDTO>> Handle(GetBookingByUserIdQuery query, CancellationToken cancellationToken)
        {
            var bookings = await unit.Booking.GetAllBookingByUserId( query.UserId);
            if (bookings == null || !bookings.Any())
                return new List<BookingDTO>(); // Trả về [] nếu không có dữ liệu

            return bookings.Select(b => new BookingDTO(
                b.Id,
                b.StartDate,
                b.EndDate,
                b.Price,
                b.Status,
                b.CreatedAt,
                b.PaymentId,
                b.WorkspaceId,
                b.PromotionId,
                b.UserId
            )).ToList();
        }
    }
}

