using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Manage_Feedback.User_Feedback;

namespace WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response
{

    public record GetOwnerResponseFeedbackByIdQuery(int Id) : IQuery<GetOwnerResponseFeedbackByIdResult>;

    public record GetOwnerResponseFeedbackByIdResult(int Id, string Title, string Description, string Status, int UserId, int OwnerId, int? FeedbackId, DateTime? CreatedAt, List<string> ImageUrls);
    class GetOwnerResponseFeedbackByIdHandler(IFeedbackManageUnitOfWork unit)
    : IQueryHandler<GetOwnerResponseFeedbackByIdQuery, GetOwnerResponseFeedbackByIdResult>
    {
        public async Task<GetOwnerResponseFeedbackByIdResult> Handle(GetOwnerResponseFeedbackByIdQuery query, CancellationToken cancellationToken)
        {
            var responeFeedback = await unit.OwnerResponseFeedback.GetResponseFeedbackById(query.Id);

            if (responeFeedback == null)
                return null;

            var imageUrls = responeFeedback.ImageResponseFeedbacks?
                .Select(imgFeedback => imgFeedback.Image.ImgUrl)
                .ToList() ?? new List<string>();

            return new GetOwnerResponseFeedbackByIdResult(
                responeFeedback.Id,
                responeFeedback.Title,
                responeFeedback.Description,
                responeFeedback.Status,
                responeFeedback.Feedback.Booking.User.Id,
                responeFeedback.OwnerId,
                responeFeedback.FeedbackId,
                responeFeedback.CreatedAt,
                imageUrls
                );
        }
    }
}