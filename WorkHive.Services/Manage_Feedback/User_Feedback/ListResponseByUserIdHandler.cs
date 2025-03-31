using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response;

namespace WorkHive.Services.Manage_Feedback.User_Feedback
{
    public record ListResponseByUserIdQuery(int UserId) : IQuery<List<GetAllOwnerResponseFeedbackResult>>;

    public class ListResponseByUserIdHandler(IFeedbackManageUnitOfWork unit)
        : IQueryHandler<ListResponseByUserIdQuery, List<GetAllOwnerResponseFeedbackResult>>
    {
        public async Task<List<GetAllOwnerResponseFeedbackResult>> Handle(ListResponseByUserIdQuery query, CancellationToken cancellationToken)
        {
            var responseFeedbacks = await unit.OwnerResponseFeedback.GetResponseFeedbacksByUserId(query.UserId);

            if (responseFeedbacks == null || !responseFeedbacks.Any())
                return new List<GetAllOwnerResponseFeedbackResult>();

            return responseFeedbacks.Select(responseFeedback => new GetAllOwnerResponseFeedbackResult(
                responseFeedback.Id,
                responseFeedback.Title,
                responseFeedback.Description,
                responseFeedback.Status,
                responseFeedback.Feedback.Booking.User.Id,
                responseFeedback.OwnerId,
                responseFeedback.FeedbackId,
                responseFeedback.CreatedAt,
                responseFeedback.ImageResponseFeedbacks?
                    .Select(imgFeedback => imgFeedback.Image.ImgUrl)
                    .ToList() ?? new List<string>()
            )).ToList();
        }
    }
}
