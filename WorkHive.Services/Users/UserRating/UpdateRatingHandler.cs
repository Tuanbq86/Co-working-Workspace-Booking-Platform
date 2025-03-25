using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.UserRating;

public record UpdateRatingCommand(int UserId, int RatingId, Byte Rate, 
    string Comment, List<UpdateRatingImage> Images)
    : ICommand<UpdateRatingResult>;
public record UpdateRatingResult(string Notification);
public record UpdateRatingImage(string Url);

public class UpdateRatingHandler(IUserRatingUnitOfWork userRating)
    : ICommandHandler<UpdateRatingCommand, UpdateRatingResult>
{
    public async Task<UpdateRatingResult> Handle(UpdateRatingCommand command, 
        CancellationToken cancellationToken)
    {
        var rating = userRating.rating.GetAll()
            .FirstOrDefault(ur => ur.UserId.Equals(command.UserId) 
            && ur.Id.Equals(command.RatingId));

        if (rating is null)
            return new UpdateRatingResult($"Không tìm thấy rating với Id {command.RatingId} để đánh giá");

        rating.Comment = command.Comment;
        rating.Rate = command.Rate;

        await userRating.rating.UpdateAsync(rating);

        // Lấy workspaceRating liên quan
        var workspaceRating = userRating.workspaceRating.GetAll()
            .FirstOrDefault(wr => wr.RatingId == rating.Id);

        if (workspaceRating == null)
            return new UpdateRatingResult("Không tìm thấy thông tin workspace rating");

        // Xử lý ảnh
        //Lấy danh sách ảnh cũ
        var existingImages = userRating.workspaceRatingImage.GetAll()
            .Where(wri => wri.WorkspaceRatingId == workspaceRating.Id)
            .ToList();

        if (existingImages.Count > 0)
        {
            //Xóa các ảnh cũ
            foreach (var existingImage in existingImages)
            {
                var image = await userRating.image.GetByIdAsync(existingImage.ImageId);

                await userRating.workspaceRatingImage.RemoveAsync(existingImage);
                await userRating.image.RemoveAsync(image);
            }
        }
        
        //Thêm các ảnh mới
        foreach (var item in command.Images)
        {
            var image = new Image
            {
                ImgUrl = item.Url,
                Title = "Rating image",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await userRating.image.CreateAsync(image);

            var workspaceRatingImage = new WorkspaceRatingImage
            {
                WorkspaceRatingId = workspaceRating.Id,
                ImageId = image.Id
            };
            await userRating.workspaceRatingImage.CreateAsync(workspaceRatingImage);
        }

        return new UpdateRatingResult("Cập nhật đánh giá thành công");

    }
}
