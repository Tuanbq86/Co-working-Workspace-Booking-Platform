using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Users.UserRating;

public record DeleteRatingCommand(int UserId, int RatingId) : ICommand<DeleteRatingResult>;
public record DeleteRatingResult(string Notification);

public class DeleteRatingHandler(IUserRatingUnitOfWork userRating)
    : ICommandHandler<DeleteRatingCommand, DeleteRatingResult>
{
    public async Task<DeleteRatingResult> Handle(DeleteRatingCommand command, 
        CancellationToken cancellationToken)
    {
        var rating = userRating.rating.GetAll()
            .Where(r => r.UserId.Equals(command.UserId) && r.Id.Equals(command.RatingId))
            .FirstOrDefault();

        if (rating is null)
        {
            return new DeleteRatingResult($"Không tìm thấy rating có Id: {command.RatingId} để xóa");
        }

        var workspaceRating = userRating.workspaceRating.GetAll()
            .FirstOrDefault(wr => wr.RatingId.Equals(rating!.Id));

        var workspaceRatingImages = await userRating.workspaceRatingImage.GetAllAsync();

        foreach(var item in  workspaceRatingImages)
        {
            if (item.WorkspaceRatingId.Equals(workspaceRating!.Id))
            {
                var image = userRating.image.GetById(item.ImageId);
                await userRating.workspaceRatingImage.RemoveAsync(item);
                await userRating.image.RemoveAsync(image);
            }
        }

        userRating.workspaceRating.Remove(workspaceRating!);
        userRating.rating.Remove(rating);

        await userRating.SaveAsync();

        return new DeleteRatingResult($"Xóa thành công rating có Id: {command.RatingId}");
    }
}
