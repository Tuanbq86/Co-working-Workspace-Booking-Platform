using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.User_Feedback
{
    public record GetAllFeedbackQuery() : IQuery<List<GetAllFeedbackResult>>;

    public record GetAllFeedbackResult(int Id, string Description, string Status, int UserId, int OwnerId, int? BookingId, int WorkspaceId, string WorkspaceName, DateTime? CreatedAt, List<string> ImageUrls);

    public class GetAllFeedbackHandler(IFeedbackManageUnitOfWork unit)
    : IQueryHandler<GetAllFeedbackQuery, List<GetAllFeedbackResult>>
    {
        public async Task<List<GetAllFeedbackResult>> Handle(GetAllFeedbackQuery query, CancellationToken cancellationToken)
        {
            var feedbacks = await unit.Feedback.GetAllFeedbacks();

            if (feedbacks == null || !feedbacks.Any())
                return new List<GetAllFeedbackResult>();

            return feedbacks.Select(feedback => new GetAllFeedbackResult(
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
            )).ToList();
        }
    }
}