using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.User_Feedback
{
    public record GetBookingsWithFeedbackByOwnerIdQuery(int OwnerId) : IQuery<List<GetBookingsWithFeedbackByOwnerIdResult>>;

    public record GetBookingsWithFeedbackByOwnerIdResult(
        int Id,
        DateTime? StartDate,
        DateTime? EndDate,
        decimal? Price,
        string Status,
        DateTime? CreatedAt,
        int UserId,
        string UserName,
        int WorkspaceId,
        string WorkspaceName,
        List<int> FeedbackIds
    );

    public class GetBookingsWithFeedbackByOwnerIdHandler(IFeedbackManageUnitOfWork unit)
        : IQueryHandler<GetBookingsWithFeedbackByOwnerIdQuery, List<GetBookingsWithFeedbackByOwnerIdResult>>
    {
        public async Task<List<GetBookingsWithFeedbackByOwnerIdResult>> Handle(GetBookingsWithFeedbackByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            var bookings = await unit.Booking.GetBookingsWithFeedbackByOwnerId(query.OwnerId);

            return bookings.Select(b => new GetBookingsWithFeedbackByOwnerIdResult(
                b.Id,
                b.StartDate,
                b.EndDate,
                b.Price,
                b.Status,
                b.CreatedAt,
                b.UserId,
                b.User?.Name ?? "Unknown",
                b.WorkspaceId,
                b.Workspace?.Name ?? "Unknown",
                b.Feedbacks?.Select(f => f.Id).ToList() ?? new List<int>()
            )).ToList();
        }
    }
}
