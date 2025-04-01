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
    public record GetBookingsWithFeedbackByUserIdQuery(int UserId) : IQuery<List<GetBookingsWithFeedbackByUserIdResult>>;

    public record GetBookingsWithFeedbackByUserIdResult(
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

    public class GetBookingsWithFeedbackByUserIdHandler(IFeedbackManageUnitOfWork unit)
        : IQueryHandler<GetBookingsWithFeedbackByUserIdQuery, List<GetBookingsWithFeedbackByUserIdResult>>
    {
        public async Task<List<GetBookingsWithFeedbackByUserIdResult>> Handle(GetBookingsWithFeedbackByUserIdQuery query, CancellationToken cancellationToken)
        {
            var bookings = await unit.Booking.GetBookingsWithFeedbackByUserId(query.UserId);

            return bookings.Select(b => new GetBookingsWithFeedbackByUserIdResult(
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
