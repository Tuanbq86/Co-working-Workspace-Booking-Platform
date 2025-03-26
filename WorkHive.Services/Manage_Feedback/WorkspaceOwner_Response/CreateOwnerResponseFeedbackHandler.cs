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
    public record CreateOwnerResponseFeedbackCommand(string Description, int UserId, int OwnerId, int FeedbackId, List<ImageResponseFeedbackDTO>? Images = null) : ICommand<CreateResponseFeedbackResult>;

    public record ImageResponseFeedbackDTO(string ImgUrl);

    public record CreateResponseFeedbackResult(string Notification);
    class CreateOwnerResponseFeedbackHandler(IFeedbackManageUnitOfWork unit) : ICommandHandler<CreateOwnerResponseFeedbackCommand, CreateResponseFeedbackResult>
    {
        private const string DefaultImageTitle = "Response Feedback Image";
        private const string DefaultStatus = "Active";
        public async Task<CreateResponseFeedbackResult> Handle(CreateOwnerResponseFeedbackCommand command, CancellationToken cancellationToken)
        {
            List<Image> images = command.Images?.Select(i => new Image
            {
                ImgUrl = i.ImgUrl,
                Title = DefaultImageTitle,
                CreatedAt = DateTime.UtcNow
            }).ToList() ?? new List<Image>();

            var newResponseFeedback = new OwnerResponeFeedback
            {
                Description = command.Description,
                UserId = command.UserId,
                OwnerId = command.OwnerId,
                FeedbackId = command.FeedbackId,
                Status = DefaultStatus,
                CreatedAt = DateTime.UtcNow
            };

            if (images.Any())
            {
                await unit.Image.CreateImagesAsync(images);
            }
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
