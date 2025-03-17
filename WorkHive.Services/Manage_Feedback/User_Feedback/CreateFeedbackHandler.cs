using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkHive.Data.Models;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Manage_Feedback.User_Feedback
{
    public record CreateFeedbackCommand(string Description, int UserId, int OwnerId, List<ImageFeedbackDTO>? Images = null) : ICommand<CreateFeedbackResult>;

    public record ImageFeedbackDTO(string ImgUrl);

    public record CreateFeedbackResult(string Notification);

    class CreateFeedbackHandler(IFeedbackManageUnitOfWork unit) : ICommandHandler<CreateFeedbackCommand, CreateFeedbackResult>
    {
        private const string DefaultImageTitle = "Feedback Image";
        private const string DefaultStatus = "Active";

        public async Task<CreateFeedbackResult> Handle(CreateFeedbackCommand command, CancellationToken cancellationToken)
        {
            List<Image> images = command.Images?.Select(i => new Image
            {
                ImgUrl = i.ImgUrl,
                Title = DefaultImageTitle,
                CreatedAt = DateTime.UtcNow
            }).ToList() ?? new List<Image>();

            var newFeedback = new Feedback
            {
                Description = command.Description,
                UserId = command.UserId,
                OwnerId = command.OwnerId,
                Status = DefaultStatus,
                CreatedAt = DateTime.UtcNow
            };

            if (images.Any())
            {
                await unit.Image.CreateImagesAsync(images);
            }

            await unit.Feedback.CreateAsync(newFeedback);
            await unit.SaveAsync();

            if (images.Any())
            {
                var imageFeedbacks = images.Select(img => new ImageFeedback
                {
                    ImageId = img.Id,
                    FeedbackId = newFeedback.Id,
                    Status = DefaultStatus
                }).ToList();

                await unit.ImageFeedback.CreateImageFeedbackAsync(imageFeedbacks);
                await unit.SaveAsync();
            }

            return new CreateFeedbackResult("Feedback created successfully");
        }
    }
}
