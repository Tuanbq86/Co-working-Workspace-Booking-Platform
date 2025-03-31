using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response
{

    public record GetAllOwnerResponseFeedbackQuery() : IQuery<List<GetAllOwnerResponseFeedbackResult>>;

    public record GetAllOwnerResponseFeedbackResult(
        int Id,
        string Title,
        string Description,
        string Status,
        int UserId,
        int OwnerId,
        int? FeedbackId,
        DateTime? CreatedAt,
        List<string> ImageUrls
    );

    public class GetAllOwnerResponseFeedbackHandler(IFeedbackManageUnitOfWork unit)
    : IQueryHandler<GetAllOwnerResponseFeedbackQuery, List<GetAllOwnerResponseFeedbackResult>>
    {
        public async Task<List<GetAllOwnerResponseFeedbackResult>> Handle(GetAllOwnerResponseFeedbackQuery query, CancellationToken cancellationToken)
        {
            var responseFeedbacks = await unit.OwnerResponseFeedback.GetAllResponseFeedbacks();

            if (responseFeedbacks == null || !responseFeedbacks.Any())
                return new List<GetAllOwnerResponseFeedbackResult>();

            return responseFeedbacks.Select(responeFeedback => new GetAllOwnerResponseFeedbackResult(
                responeFeedback.Id,
                responeFeedback.Title,
                responeFeedback.Description,
                responeFeedback.Status,
                responeFeedback.Feedback.Booking.User.Id,
                responeFeedback.OwnerId,
                responeFeedback.FeedbackId,
                responeFeedback.CreatedAt,
                responeFeedback.ImageResponseFeedbacks?
                    .Select(imgFeedback => imgFeedback.Image.ImgUrl)
                    .ToList() ?? new List<string>()
            )).ToList();
        }
    }
}

