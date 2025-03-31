using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.User_Feedback
{
    public record GetFeedbackByBookingIdQuery(int BookingId) : IQuery<GetFeedbackByBookingIdResult?>;

    public record GetFeedbackByBookingIdResult(
        int Id,
        string Description,
        string Status,
        int UserId,
        int OwnerId,
        int BookingId,
        int WorkspaceId,
        string WorkspaceName,
        DateTime? CreatedAt,
        List<string> ImageUrls
    );

    public class GetFeedbackByBookingIdHandler(IFeedbackManageUnitOfWork unit)
        : IQueryHandler<GetFeedbackByBookingIdQuery, GetFeedbackByBookingIdResult?>
    {
        public async Task<GetFeedbackByBookingIdResult?> Handle(GetFeedbackByBookingIdQuery query, CancellationToken cancellationToken)
        {
            var feedback = await unit.Feedback.GetFirstFeedbackByBookingId(query.BookingId);
            if (feedback == null) return null;

            return new GetFeedbackByBookingIdResult(
                feedback.Id,
                feedback.Description,
                feedback.Status,
                feedback.UserId,
                feedback.Booking?.Workspace.Owner.Id ?? 0,
                feedback.BookingId,
                feedback.Booking?.Workspace.Id ?? 0,
                feedback.Booking?.Workspace.Name ?? "Unknown",
                feedback.CreatedAt,
                feedback.ImageFeedbacks?.Select(imgFeedback => imgFeedback.Image.ImgUrl).ToList() ?? new List<string>()
            );
        }
    }
}