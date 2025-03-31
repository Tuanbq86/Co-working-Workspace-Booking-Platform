using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response
{
    public record GetResponseFeedbackByFeedbackIdQuery(int FeedbackId) : IQuery<GetResponseFeedbackByFeedbackIdResult?>;

    public record GetResponseFeedbackByFeedbackIdResult(
        int Id,
        string Description,
        string Status,
        int UserId,
        int OwnerId,
        int FeedbackId,
        DateTime? CreatedAt,
        List<string> ImageUrls
    );

    public class GetResponseFeedbackByFeedbackIdHandler(IFeedbackManageUnitOfWork unit)
        : IQueryHandler<GetResponseFeedbackByFeedbackIdQuery, GetResponseFeedbackByFeedbackIdResult?>
    {
        public async Task<GetResponseFeedbackByFeedbackIdResult?> Handle(GetResponseFeedbackByFeedbackIdQuery query, CancellationToken cancellationToken)
        {
            var responseFeedback = await unit.OwnerResponseFeedback.GetFirstResponseFeedbackByFeedbackId(query.FeedbackId);
            if (responseFeedback == null) return null;

            return new GetResponseFeedbackByFeedbackIdResult(
                responseFeedback.Id,
                responseFeedback.Description,
                responseFeedback.Status,
                responseFeedback.Feedback.Booking.User.Id,
                responseFeedback.OwnerId,
                responseFeedback.FeedbackId,
                responseFeedback.CreatedAt,
                responseFeedback.ImageResponseFeedbacks?
                    .Select(imgFeedback => imgFeedback.Image.ImgUrl)
                    .ToList() ?? new List<string>()
            );
        }
    }
}
