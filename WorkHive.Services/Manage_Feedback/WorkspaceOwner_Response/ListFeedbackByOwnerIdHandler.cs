using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response
{
    public record ListFeedbackByOwnerIdQuery(int OwnerId) : IQuery<List<ListFeedbackByOwnerIdResult>>;

    public record ListFeedbackByOwnerIdResult(
        int Id, string Description, string Status, int UserId, int OwnerId,
        int? BookingId, int WorkspaceId, string WorkspaceName,
        DateTime? CreatedAt, List<string> ImageUrls
    );

    public class ListFeedbackByOwnerIdHandler(IFeedbackManageUnitOfWork unit)
    : IQueryHandler<ListFeedbackByOwnerIdQuery, List<ListFeedbackByOwnerIdResult>>
    {
        public async Task<List<ListFeedbackByOwnerIdResult>> Handle(ListFeedbackByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            var feedbacks = await unit.Feedback.GetFeedbacksByOwnerId(query.OwnerId);

            if (feedbacks == null || !feedbacks.Any())
                return new List<ListFeedbackByOwnerIdResult>();

            return feedbacks.Select(feedback => new ListFeedbackByOwnerIdResult(
                feedback.Id,
                feedback.Description,
                feedback.Status,
                feedback.UserId,
                feedback.Booking?.Workspace.OwnerId ?? 0,
                feedback.BookingId,
                feedback.Booking?.Workspace.Id ?? 0,
                feedback.Booking?.Workspace.Name ?? "Unknown",
                feedback.CreatedAt,
                feedback.ImageFeedbacks?.Select(imgFeedback => imgFeedback.Image.ImgUrl).ToList() ?? new List<string>()
            )).ToList();
        }
    }
}
