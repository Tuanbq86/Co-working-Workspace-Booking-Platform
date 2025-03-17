using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.User_Feedback
{
    public record GetFeedbackByIdQuery(int Id) : IQuery<GetFeedbackByIdResult>;

    public record GetFeedbackByIdResult(int Id, string Description, string Status, int UserId, int OwnerId, DateTime? CreatedAt, List<string> ImageUrls);

    public class GetFeedbackByIdHandler(IFeedbackManageUnitOfWork unit)
    : IQueryHandler<GetFeedbackByIdQuery, GetFeedbackByIdResult>
    {
        public async Task<GetFeedbackByIdResult> Handle(GetFeedbackByIdQuery query, CancellationToken cancellationToken)
        {
            var feedback = await unit.Feedback.GetFeedbackById(query.Id);

            if (feedback == null)
                return null;

            // Lấy danh sách đường dẫn ảnh từ ImageFeedback -> Image
            var imageUrls = feedback.ImageFeedbacks?
                .Select(imgFeedback => imgFeedback.Image.ImgUrl) 
                .ToList() ?? new List<string>();

            return new GetFeedbackByIdResult(
                feedback.Id,
                feedback.Description,
                feedback.Status,
                feedback.UserId,
                feedback.OwnerId,
                feedback.CreatedAt,
                imageUrls
                );
        }
    }
}