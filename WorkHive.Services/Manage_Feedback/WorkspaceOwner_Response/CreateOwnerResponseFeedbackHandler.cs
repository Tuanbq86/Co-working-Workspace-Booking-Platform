using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkHive.Data.Models;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.WorkspaceOwner_Response
{
    public record CreateOwnerResponseFeedbackCommand(string Title, string Description, int OwnerId, int FeedbackId, List<ImageResponseFeedbackDTO>? Images = null) : ICommand<CreateResponseFeedbackResult>;

    public record ImageResponseFeedbackDTO(string ImgUrl);

    public record CreateResponseFeedbackResult(string Notification);
    class CreateOwnerResponseFeedbackHandler(IFeedbackManageUnitOfWork unit) : ICommandHandler<CreateOwnerResponseFeedbackCommand, CreateResponseFeedbackResult>
    {
        private const string DefaultImageTitle = "Response Feedback Image";
        private const string DefaultStatus = "Active";
        public async Task<CreateResponseFeedbackResult> Handle(CreateOwnerResponseFeedbackCommand command, CancellationToken cancellationToken)
        {
            var existingResponse = await unit.OwnerResponseFeedback.GetFirstResponseFeedbackByFeedbackId(command.FeedbackId);
            var existingFeedback = await unit.Feedback.GetByIdAsync(command.FeedbackId);
            var existingOwner = await unit.WorkspaceOwner.GetByIdAsync(command.OwnerId);

            if (existingFeedback == null)
            {
                return new CreateResponseFeedbackResult("Không tìm thấy phản hồi.");
            }   
            if (existingResponse != null)
            {
                return new CreateResponseFeedbackResult("Feedback đã được phản hồi trước đó.");
            }
            List<Image> images = command.Images?.Select(i => new Image
            {
                ImgUrl = i.ImgUrl,
                Title = DefaultImageTitle,
                CreatedAt = DateTime.Now
            }).ToList() ?? new List<Image>();

            var newResponseFeedback = new OwnerResponseFeedback
            {
                Title = command.Title,
                Description = command.Description,
                OwnerId = command.OwnerId,
                FeedbackId = command.FeedbackId,
                Status = DefaultStatus,
                CreatedAt = DateTime.Now
            };

            if (images.Any())
            {
                await unit.Image.CreateImagesAsync(images);
            }


            var userNotificaiton = new UserNotification
            {
                Description = $"{newResponseFeedback.Description}",
                Status = "Active",
                UserId = existingFeedback.UserId,
                CreatedAt = DateTime.Now,
                IsRead = 0,
                Title = $"Phản hổi từ {existingOwner.LicenseName}. {newResponseFeedback.Title}"
            };

            await unit.UserNotification.CreateAsync(userNotificaiton);
            await unit.OwnerResponseFeedback.CreateAsync(newResponseFeedback);
            await unit.SaveAsync();

            if (images.Any())
            {
                var imageResponseFeedbacks = images.Select(img => new ImageResponseFeedback
                {
                    ImageId = img.Id,
                    ResponseFeedbackId = newResponseFeedback.Id,
                    Status = DefaultStatus
                }).ToList();
                await unit.ImageResponseFeedback.CreateImageResponseFeedbackAsync(imageResponseFeedbacks);
                await unit.SaveAsync();
            }
            return new CreateResponseFeedbackResult(" Response Feedback created successfully");
        }
    }
}
