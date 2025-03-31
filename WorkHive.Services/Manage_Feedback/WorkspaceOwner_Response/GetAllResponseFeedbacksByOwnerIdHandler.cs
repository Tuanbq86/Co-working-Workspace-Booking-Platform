using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response
{
    public record GetAllResponseFeedbacksByOwnerIdQuery(int OwnerId) : IQuery<List<GetAllOwnerResponseFeedbackResult>>;

    public class GetAllResponseFeedbacksByOwnerIdHandler(IFeedbackManageUnitOfWork unit)
        : IQueryHandler<GetAllResponseFeedbacksByOwnerIdQuery, List<GetAllOwnerResponseFeedbackResult>>
    {
        public async Task<List<GetAllOwnerResponseFeedbackResult>> Handle(GetAllResponseFeedbacksByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            var responseFeedbacks = await unit.OwnerResponseFeedback.GetResponseFeedbacksByOwnerId(query.OwnerId);

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
